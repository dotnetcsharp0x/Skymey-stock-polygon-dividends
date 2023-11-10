using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using Nancy.Json;
using RestSharp;
using Skymey_main_lib.Models.Dividends.Polygon;
using Skymey_main_lib.TickerDetails.Polygon;
using Skymey_stock_polygon_dividends.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skymey_stock_polygon_dividends.Actions.GetDividends
{
    public class GetDividends
    {
        private RestClient _client;
        private RestRequest _request;
        private MongoClient _mongoClient;
        private ApplicationContext _db;
        private string _apiKey;
        public GetDividends()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false);

            IConfiguration config = builder.Build();

            _apiKey = config.GetSection("ApiKeys:Polygon").Value;
            _mongoClient = new MongoClient("mongodb://127.0.0.1:27017");
            _db = ApplicationContext.Create(_mongoClient.GetDatabase("skymey"));
        }
        public void GetDividendsFromPolygon()
        {
            var all_tickers = (from i in _db.TickerList select i).AsNoTracking();
            foreach (var item in all_tickers)
            {
                _client = new RestClient("https://api.polygon.io/v3/reference/dividends?ticker="+item.ticker+"&limit=1000&apiKey=" + _apiKey);
                _request = new RestRequest("https://api.polygon.io/v3/reference/dividends?ticker="+item.ticker+"&limit=1000&apiKey=" + _apiKey, Method.Get);
                _request.AddHeader("Content-Type", "application/json");
                var r = _client.Execute(_request).Content;
                DividendsPolygonQuery tp = new JavaScriptSerializer().Deserialize<DividendsPolygonQuery>(r);
                if (tp.results.Count > 0)
                {
                    foreach (var dividend in tp.results) {
                        Console.WriteLine(dividend.ticker + " " + dividend.declaration_date);
                        DividendsPolygon? ticker_find = (from i in _db.DividendsPolygon where i.ticker == dividend.ticker && i.declaration_date == dividend.declaration_date select i).FirstOrDefault();
                        if (ticker_find == null)
                        {
                            dividend._id = ObjectId.GenerateNewId();
                            dividend.Update = DateTime.UtcNow;
                            _db.DividendsPolygon.Add(dividend);
                        }
                    }
                    _db.SaveChanges();
                }
            }
        }
    }
}
