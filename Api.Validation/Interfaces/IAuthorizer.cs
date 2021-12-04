using System.Threading;
using System.Threading.Tasks;
using Api.Behaviors.Authorization.Models;

namespace Api.Behaviors.Authorization.Interfaces
{
    public interface IAuthorizer<in T>
    {
        Task<AuthorizationResult> AuthorizeAsync(T instance, CancellationToken cancellation = default);
    }
}
