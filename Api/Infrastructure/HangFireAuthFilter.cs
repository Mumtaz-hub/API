using Hangfire.Dashboard;

namespace Api.Infrastructure
{
    public class HangFireAuthFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            return true;
        }
    }
}