﻿using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using MongoDB.EntityFrameworkCore.Extensions;
using Skymey_main_lib.Models.Dividends.Polygon;
using Skymey_main_lib.Models.Tickers.Polygon;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Skymey_stock_polygon_dividends.Data
{
    public class ApplicationContext : DbContext
    {
        public DbSet<DividendsPolygon> DividendsPolygon { get; init; }
        public DbSet<TickerList> TickerList { get; init; }
        public static ApplicationContext Create(IMongoDatabase database) =>
            new(new DbContextOptionsBuilder<ApplicationContext>()
                .UseMongoDB(database.Client, database.DatabaseNamespace.DatabaseName)
                .Options);
        public ApplicationContext(DbContextOptions options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<DividendsPolygon>().ToCollection("stock_dividends");
            modelBuilder.Entity<TickerList>().ToCollection("stock_tickerlist");
        }
    }
}
