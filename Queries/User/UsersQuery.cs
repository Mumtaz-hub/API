using System;
using Api.Caching.Abstractions;
using System.Collections.Generic;
using Common.Constants;
using MediatR;
using ViewModel.Users;

namespace Queries.User
{
    public class UsersQuery : Query<IEnumerable<UsersViewModel>>, ICacheableQuery
    {
        public string Key { get; }
        public ExpirationOptions Options { get; }

        public UsersQuery()
        {
            Key = ApiRouteConstants.Users.GetUsersRoute;
            Options = new ExpirationOptions(TimeSpan.FromMinutes(15));
        }
    }

    //public class UsersQuery : IRequest<IEnumerable<UsersViewModel>>
    //{
    //    public string Key { get; }
    //    public ExpirationOptions Options { get; }

    //    public UsersQuery()
    //    {
    //        Key = ApiRouteConstants.Users.GetUsersRoute;
    //        Options = new ExpirationOptions(TimeSpan.FromMinutes(15));
    //    }
    //}
}