using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Concurrent;
using TS.Brokers.States;

namespace TS.Brokers.Api.Controllers
{
    [ApiController, Route("api/[controller]")]
    public class StockController : ControllerBase
    {
        ConcurrentDictionary<Guid, StockState> Stocks { get; } = new ConcurrentDictionary<Guid, StockState>();

        public StockController(ConcurrentDictionary<Guid, StockState> stocks)
        {
            Stocks = stocks;
        }

        [HttpGet]
        public IActionResult Get() => Ok(Stocks);
    }
}