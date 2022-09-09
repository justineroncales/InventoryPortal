using InvoiceManagement.Models;

namespace InvoiceManagement.BL
{
    public interface IBusnessLayer
    {
        Task<List<Invoice>> GetInvoices();
        Task<CurrentInvoice> SaveInvoices();
    }
}