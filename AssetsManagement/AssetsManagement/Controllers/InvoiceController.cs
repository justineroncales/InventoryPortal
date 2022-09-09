using AssetsManagement.BL;
using InvoiceManagement.Data;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace AssetsManagement.Controllers
{
    [Route("api/[controller]")]
    public class InvoiceController : ControllerBase
    {
        private readonly IBusnessLayer _busnessLayer;
        public InvoiceController(IBusnessLayer busnessLayer)
        {
            _busnessLayer = busnessLayer;
        }
        [HttpPost]
        [EnableCors("MyPolicy")]
        public async Task<IActionResult> Post()
        {
            var result = await _busnessLayer.SaveInvoices();
            return Ok(result);
        }

        [HttpGet]
        [EnableCors("MyPolicy")]
        public async Task<IActionResult> GetInvoices()
        {
            var result = await _busnessLayer.GetInvoices();
            return Ok(result);
        }
    }
}
