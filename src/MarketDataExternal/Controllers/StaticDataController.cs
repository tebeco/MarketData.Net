using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarketDataCommon.Dto;
using MarketDataExternal.Services;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace MarketDataExternal.Controllers
{
    [Route("api/[controller]")]
    public class StaticDataController : Controller
    {
        private readonly IStockService _stockService;

        public StaticDataController(IStockService stockService)
        {
            _stockService = stockService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_stockService.GetAllStock());
        }

        [HttpGet("{code}")]
        public IActionResult Get(string code)
        {
            if (_stockService.TryGetFromCode(code, out var stock))
                return Ok(stock);

            return BadRequest(new { Error = $"Code : `{code}` is not available" });
        }
    }
}
