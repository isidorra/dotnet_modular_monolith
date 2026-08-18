using FluentValidation;
using FluentValidation.Results;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using ModularMonolith.Shared.Infrastructure.Authentication;
using ModularMonolith.Shared.Infrastructure.Http;

using Npgsql;

namespace ModularMonolith.Shared.Tests.Http;

public sealed class ProblemDetailsExceptionHandlerTests
{
    [Fact]
    public async Task Maps_a_failed_authentication_to_401()
    {
        var (handled, context, httpContext) = await HandleAsync(new AuthenticationFailedException());

        handled.ShouldBeTrue();
        httpContext.Response.StatusCode.ShouldBe(StatusCodes.Status401Unauthorized);
        context.ProblemDetails.Title.ShouldBe("Invalid credentials");
    }

    [Fact]
    public async Task Maps_a_validation_failure_to_400_with_errors_grouped_by_property()
    {
        var exception = new ValidationException([
            new ValidationFailure("Email", "'Email' must not be empty"),
            new ValidationFailure("Email", "'Email' is not a valid email address"),
            new ValidationFailure("Password", "'Password' must not be empty")
        ]);

        var (handled, context, httpContext) = await HandleAsync(exception);

        handled.ShouldBeTrue();
        httpContext.Response.StatusCode.ShouldBe(StatusCodes.Status400BadRequest);

        var problemDetails = context.ProblemDetails.ShouldBeOfType<ValidationProblemDetails>();

        problemDetails.Errors["Email"].Length.ShouldBe(2);
        problemDetails.Errors["Password"].ShouldBe(["'Password' must not be empty"]);
    }

    [Fact]
    public async Task Maps_a_unique_violation_to_409()
    {
        var (handled, _, httpContext) = await HandleAsync(UniqueViolation());

        handled.ShouldBeTrue();
        httpContext.Response.StatusCode.ShouldBe(StatusCodes.Status409Conflict);
    }

    [Fact]
    public async Task Maps_a_nested_unique_violation_to_409()
    {
        var wrapped = new InvalidOperationException("save failed", new Exception("inner", UniqueViolation()));

        var (handled, _, httpContext) = await HandleAsync(wrapped);

        handled.ShouldBeTrue();
        httpContext.Response.StatusCode.ShouldBe(StatusCodes.Status409Conflict);
    }

    [Fact]
    public async Task Leaves_an_unrecognised_exception_to_the_pipeline()
    {
        var (handled, context, httpContext) = await HandleAsync(new InvalidOperationException("boom"));

        handled.ShouldBeFalse();
        context.ShouldBeNull();
        httpContext.Response.StatusCode.ShouldBe(StatusCodes.Status200OK);
    }

    private static async Task<(bool Handled, ProblemDetailsContext Context, HttpContext HttpContext)> HandleAsync(
        Exception exception)
    {
        var problemDetailsService = new CapturingProblemDetailsService();
        var handler = new ProblemDetailsExceptionHandler(problemDetailsService);
        var httpContext = new DefaultHttpContext();

        var handled = await handler.TryHandleAsync(httpContext, exception, TestContext.Current.CancellationToken);

        return (handled, problemDetailsService.Captured, httpContext);
    }

    private static PostgresException UniqueViolation()
    {
        return new PostgresException(
            "duplicate key value violates unique constraint",
            "ERROR",
            "ERROR",
            PostgresErrorCodes.UniqueViolation);
    }

    private sealed class CapturingProblemDetailsService : IProblemDetailsService
    {
        public ProblemDetailsContext Captured { get; private set; }

        public ValueTask<bool> TryWriteAsync(ProblemDetailsContext context)
        {
            Captured = context;

            return ValueTask.FromResult(true);
        }

        public ValueTask WriteAsync(ProblemDetailsContext context)
        {
            Captured = context;

            return ValueTask.CompletedTask;
        }
    }
}