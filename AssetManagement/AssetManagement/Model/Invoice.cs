using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace AssetsManagement.Models
{
    public class CurrentInvoice
    {
        public Invoice? Invoice { get; set; }
    }
    public class Invoice
    {
        public int Id { get; set; }
        public string? InvoiceName { get; set; }
        public DateTime IssuedDate { get; set; } = DateTime.Now;
        public string? CycleMonth { get; set; }
        public string? CycleYear { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }
        public ICollection<Asset_Fact>? Asset_Facts { get; set; }
    }
    public class Asset_Fact
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public decimal Price { get; set; }
        public Nullable<DateTime> ValidFrom { get; set; }
        public Nullable<DateTime> ValidTo { get; set; }
        public int InvoiceId { get; set; }
    }
}
