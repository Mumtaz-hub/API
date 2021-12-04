using System;
using Common.Interface;
using Data;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace Test.UnitTests
{
    public abstract class MockDbContextBuilder
    {
        protected DatabaseContext GetDbContext()
        {
            //var user = Substitute.For<ILoggedOnUserProvider>();
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new DatabaseContext(options);
        }
    }
}