using InvoiceManagement.BL;
using InvoiceManagement.Data;
using InvoiceManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Polly;
using System;
using System.Collections.Generic;
using System.Globalization;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace InvoiceManagement.Controllers
{
    [Route("api/[controller]")]
    public class InvoiceController : ControllerBase
    {
        private readonly IBusnessLayer _busnessLayer;
        private readonly IRabitMQProducer _rabitMQProducer;
        public InvoiceController(IBusnessLayer busnessLayer, IRabitMQProducer rabitMQProducer)
        {
            _busnessLayer = busnessLayer;
            _rabitMQProducer = rabitMQProducer;
        }
        [HttpPost]
        public async Task<IActionResult> Post()
        {
            var result =  await _busnessLayer.SaveInvoices();
            _rabitMQProducer.SendProductMessage(result);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetInvoices()
        {
            var result = await _busnessLayer.GetInvoices();
            return Ok(result);
        }
    }
}
