using InvoiceManagement.Data;
using InvoiceManagement.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace InvoiceManagement.BL
{
    public class BusnessLayer : IBusnessLayer
    {
        private readonly DataContext _dataContext;
        private readonly AssetClient _assetClient;
        public BusnessLayer(DataContext dataContext, AssetClient assetClient)
        {
            _dataContext = dataContext;
            _assetClient = assetClient;
        }
        public async Task<CurrentInvoice> SaveInvoices()
        {
            var InvNos = "Inv00001";
            DateTime dt = DateTime.Now.Date;

            List<Asset> response = await _assetClient.GetAssetsAsync();
            var assets = from asset in response
                         where (asset.ValidFrom == null || asset.ValidFrom <= dt || asset.ValidFrom.ToString() =="") && (asset.ValidTo == null || asset.ValidTo.ToString()=="" || asset.ValidTo >= dt)
                         select new
                         {
                             ValidFrom = asset.ValidFrom,
                             ValidTo = asset.ValidTo,
                             Name = asset.Name,
                             Price = asset.Price,
                         };
            var lengthSum = from asset in response
                            where (asset.ValidFrom == null || asset.ValidFrom <= dt) && (asset.ValidTo == null || asset.ValidTo >= dt)
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
                Asset _ass = new Asset
                {
                    InvoiceId = InvoiceId,
                    Name = item.Name,
                    Price = item.Price
                };
                _dataContext.Assets.Add(_ass);
                await _dataContext.SaveChangesAsync();

            }
            var currentInvoice = new CurrentInvoice();


            currentInvoice.Invoice = _dataContext.Invoices.Where(x => x.Id == InvoiceId).FirstOrDefault();
            currentInvoice.Invoice.Assets = _dataContext.Assets.Where(x => x.InvoiceId == InvoiceId).ToList();

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
