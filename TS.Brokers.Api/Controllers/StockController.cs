using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Concurrent;
using TS.Brokers.States;

namespace TS.Brokers.Api.Controllers
{
    [ApiController, Route("api/[controller]")]
    public class StockController : ControllerBase
    {
        ConcurrentDictionary<string, StockState> Stocks { get; } = new ConcurrentDictionary<string, StockState>();

        public StockController(ConcurrentDictionary<string, StockState> stocks)
        {
            Stocks = stocks;
        }

        [HttpGet]
        public IActionResult Get() => Ok(Stocks);
    }
}