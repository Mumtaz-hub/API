using System;
using Api.Caching.Abstractions;
using Common.Constants;
using ViewModel.Users;

namespace Queries.User
{
    public class UserQuery : Query<UserViewModel>, ICacheableQuery
    {
        public long Id { get; set; }

        public string Key { get; }
        public ExpirationOptions Options { get; }

        public UserQuery(long id)
        {
            Id = id;
            Key = ApiRouteConstants.Users.GetUserByIdRoute.Replace("{id}", id.ToString());
            Options = new ExpirationOptions(TimeSpan.FromMinutes(15));
        }
    }
}