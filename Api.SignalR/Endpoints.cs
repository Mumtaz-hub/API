using Api.SignalR.Hubs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Api.SignalR
{
    public static class Endpoints
    {
        public static void MapProjectHub(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapHub<ProjectedCreatedClientHub>("/project-created");
        }
    }
}