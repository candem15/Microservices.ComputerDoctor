using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CatalogService.Api.Infrastructure.Contexts
{
    public class CatalogContextDesignFactory : IDesignTimeDbContextFactory<CatalogContext>
    {
        private readonly IConfiguration _configuration;
        public CatalogContextDesignFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public CatalogContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<CatalogContext>()
                .UseSqlServer(_configuration.GetConnectionString("CatalogServiceSqlServer"));

            return new CatalogContext(optionsBuilder.Options);
        }
    }
}