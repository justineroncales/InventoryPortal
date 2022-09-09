using AssetsManagement.Data;
using System.ComponentModel.DataAnnotations;

namespace AssetsManagement.Models
{

    public class Asset 
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public decimal Price { get; set; }
        public string? ValidFrom { get; set; }
        public string? ValidTo { get; set; }
    }
}
