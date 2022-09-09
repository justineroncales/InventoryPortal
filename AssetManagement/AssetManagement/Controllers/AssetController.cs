using AssetsManagement.BL;
using AssetsManagement.Data;
using AssetsManagement.Models;
using InvoiceManagement.Data;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AssetsManagement.Controllers
{
    //
    [ApiController]
    [Route("api/[controller]")]
    public class AssetController : Controller
    {
        private readonly IBusnessLayer _busnessLayer;
        //private readonly IRabitMQProducer _rabitMQProducer;
        public AssetController(IBusnessLayer busnessLayer)
        {
            _busnessLayer = busnessLayer;
        }
        [HttpGet]
        [EnableCors("MyPolicy")]
        public async Task<IActionResult> GetAssets()
        {
            var _assets = await _busnessLayer.GetAssets();
            if (_assets.Count() == 0)
            {
                return NotFound("Asset not found");
            }
            //for multipleservices 
            //_rabitMQProducer.SendProductMessage(_assets.ToList());
            return Ok(_assets);
        }
        [HttpGet]
        [Route("{id}")]
        [ActionName("GetAsset")]
        [EnableCors("MyPolicy")]
        public async Task<IActionResult> GetAsset([FromRoute]int id)
        {
            var _assets = await _busnessLayer.GetAsset(id);
            if (_assets == null )
            {
                return NotFound("Asset not found");
            }
            return Ok(_assets);
        }
        [HttpPost]
        [EnableCors("MyPolicy")]
        public async Task<IActionResult> Post([FromBody] Asset asset)
        {
           var _asset =  await _busnessLayer.SaveAsset(asset);
            if (_asset == null)
            {
                return NotFound();
            }
            return Ok(_asset);
        }
        [HttpPut]
        [EnableCors("MyPolicy")]
        public async Task<IActionResult> Put([FromBody]Asset asset)
        {
            var _asset = await _busnessLayer.UpdateAsset(asset);
            if (!_asset)
            {
                return NotFound("Asset not found");
            }
            return Ok(_asset);
        }
        [HttpDelete]
        [Route("{id}")]
        [EnableCors("MyPolicy")]
        public async Task<IActionResult> Delete([FromRoute]int id)
        {
            var _asset = await _busnessLayer.DeleteAssetAsync(id);
            if (!_asset)
            {
                return NotFound("Asset not found");
            }
            return Ok(_asset);
        }
    }
}
