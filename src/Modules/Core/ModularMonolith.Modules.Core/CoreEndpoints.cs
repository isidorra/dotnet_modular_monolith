using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

using ModularMonolith.Modules.Core.Features;
using ModularMonolith.SharedKernel.Pagination;

using Wolverine;

namespace ModularMonolith.Modules.Core;

internal static class CoreEndpoints
{
    public static void Map(IEndpointRouteBuilder endpoints)
    {
        var assignments = endpoints.MapGroup("/assignments").RequireAuthorization();

        assignments.MapPost("", (CreateAssignmentCommand command, IMessageBus bus, CancellationToken cancellationToken) =>
        {
            return bus.InvokeAsync<AssignmentResponse>(command, cancellationToken);
        });

        assignments.MapGet("", (int? page, int? pageSize, IMessageBus bus, CancellationToken cancellationToken) =>
        {
            var query = new GetAssignmentsQuery(page ?? PageDefaults.Number, pageSize ?? PageDefaults.Size);
            return bus.InvokeAsync<PagedResult<AssignmentResponse>>(query, cancellationToken);
        });
    }
}