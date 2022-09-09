using AssetsManagement.Data;
using AssetsManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;

namespace AssetsManagement.BL
{
    public class BusnessLayer : IBusnessLayer
    {
        private readonly DataContext _dataContext;
        public BusnessLayer(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        public bool IsAssetExisting(int id)
        {
            return (_dataContext.Assets?.Any(e => e.Id == id)).GetValueOrDefault();
        }
        public async Task<List<Asset>> GetAssets()
        {
            var _assets = (from e in _dataContext.Assets
                           select e).ToList();
            return _assets;
        }
        public async Task<Asset> GetAsset(int id)
        {
            var _asset = (from e in _dataContext.Assets
                          where e.Id == id
                          select e).FirstOrDefault();
            return _asset;
        }
        public async Task<Asset> SaveAsset(Asset asset)
        {
            await _dataContext.Assets.AddAsync(asset);
            _dataContext.SaveChanges();

            var _asset = (from e in _dataContext.Assets
                          where e.Id == asset.Id
                          select e).FirstOrDefault();
            return _asset;
        }
        public async Task<bool> UpdateAsset(Asset asset)
        {
            _dataContext.Entry(asset).State = EntityState.Modified;
            try
            {
                await _dataContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!IsAssetExisting(asset.Id))
                {
                    return false;
                }
                throw ex;
            }
            return true;
        }
        public async Task<bool> DeleteAssetAsync(int id)
        {
            var _asset = await _dataContext.Assets.FindAsync(id);
            if (_dataContext.Assets == null || _asset == null)
            {
                return false;
            }
            try
            {
                _dataContext.Assets.Remove(_asset);
                await _dataContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return true;
        }
        public async Task<CurrentInvoice> SaveInvoices()
        {
            var InvNos = "Inv00001";
            DateTime dt = DateTime.Now.Date;

            List<Asset> response = await GetAssets();
            var assets = from asset in response
                         where (asset.ValidFrom == null || asset.ValidFrom.ToString() == "" || Convert.ToDateTime(asset.ValidFrom).Date <= dt) && (asset.ValidTo == null || asset.ValidTo.ToString() == "" || Convert.ToDateTime(asset.ValidTo).Date >= dt)
                         select new
                         {
                             ValidFrom = asset.ValidFrom,
                             ValidTo = asset.ValidTo,
                             Name = asset.Name,
                             Price = asset.Price,
                         };
            var lengthSum = from asset in response
                            where (asset.ValidFrom == null || asset.ValidFrom.ToString() == "" || Convert.ToDateTime(asset.ValidFrom).Date <= dt) && (asset.ValidTo == null || asset.ValidTo.ToString() == "" || Convert.ToDateTime(asset.ValidTo).Date >= dt)
                            group asset by asset.Name into assetName
                            select new
                            {
                                Price = assetName.Sum(x => x.Price)
                            };
            decimal sum = lengthSum.Sum(x => x.Price);

            var query = _dataContext.Invoices
                   .OrderByDescending(x => x.Id)
                   .Take(1)
                   .SingleOrDefault();

            if (query != null)
            {
                var Clenght = query.Id.ToString().Length;
                InvNos = InvNos.Remove(InvNos.Length - Clenght) + (query.Id + Clenght).ToString();
            }

            Invoice invoice = new Invoice
            {
                InvoiceName = InvNos,
                TotalAmount = sum,
                CycleMonth = dt.Month.ToString(),
                CycleYear = dt.Year.ToString()
            };

            _dataContext.Add(invoice);
            await _dataContext.SaveChangesAsync();

            int InvoiceId = invoice.Id;

            foreach (var item in response)
            {
                Asset_Fact _ass = new Asset_Fact
                {
                    InvoiceId = InvoiceId,
                    Name = item.Name,
                    Price = item.Price
                };
                _dataContext.Asset_Facts.Add(_ass);
                await _dataContext.SaveChangesAsync();

            }
            var currentInvoice = new CurrentInvoice();


            currentInvoice.Invoice = _dataContext.Invoices.Where(x => x.Id == InvoiceId).FirstOrDefault();
            currentInvoice.Invoice.Asset_Facts = _dataContext.Asset_Facts.Where(x => x.InvoiceId == InvoiceId).ToList();

            return currentInvoice;
        }
        public async Task<List<Invoice>> GetInvoices()
        {
            var returnValues = new List<Invoice>();
            returnValues = await (from e in _dataContext.Invoices
                                  select e).ToListAsync();

            return returnValues;
        }
    }
}
