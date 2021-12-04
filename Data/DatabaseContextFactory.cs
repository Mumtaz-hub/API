using System;
using System.Collections.Generic;
using System.Text;
using Common.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Data
{
    public class DatabaseContextFactory : IDesignTimeDbContextFactory<DatabaseContext>
    {
        public DatabaseContextFactory()
        {
        }

        public DatabaseContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<DatabaseContext>();
            builder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=config;");
            return new DatabaseContext(builder.Options);
        }
    }
}
