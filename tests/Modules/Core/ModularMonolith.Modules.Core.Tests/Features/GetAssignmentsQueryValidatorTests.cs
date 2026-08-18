using FluentValidation.TestHelper;

using ModularMonolith.Modules.Core.Features;
using ModularMonolith.SharedKernel.Pagination;

namespace ModularMonolith.Modules.Core.Tests.Features;

public sealed class GetAssignmentsQueryValidatorTests
{
    private readonly GetAssignmentsQueryValidator _validator = new();

    [Fact]
    public void Accepts_the_defaults_the_endpoint_falls_back_to()
    {
        _validator.TestValidate(new GetAssignmentsQuery(PageDefaults.Number, PageDefaults.Size))
            .ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Rejects_a_page_below_one(int page)
    {
        _validator.TestValidate(new GetAssignmentsQuery(page, PageDefaults.Size))
            .ShouldHaveValidationErrorFor(x => x.Page);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(PageDefaults.MaxSize + 1)]
    public void Rejects_a_page_size_outside_the_allowed_range(int pageSize)
    {
        _validator.TestValidate(new GetAssignmentsQuery(PageDefaults.Number, pageSize))
            .ShouldHaveValidationErrorFor(x => x.PageSize);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(PageDefaults.MaxSize)]
    public void Accepts_a_page_size_on_the_boundary(int pageSize)
    {
        _validator.TestValidate(new GetAssignmentsQuery(PageDefaults.Number, pageSize))
            .ShouldNotHaveAnyValidationErrors();
    }
}