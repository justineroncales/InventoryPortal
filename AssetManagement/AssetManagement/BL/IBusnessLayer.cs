using AssetsManagement.Models;

namespace AssetsManagement.BL
{
    public interface IBusnessLayer
    {
        Task<bool> DeleteAssetAsync(int id);
        Task<Asset> GetAsset(int id);
        Task<List<Asset>> GetAssets();
        Task<List<Invoice>> GetInvoices();
        bool IsAssetExisting(int id);
        Task<Asset> SaveAsset(Asset asset);
        Task<CurrentInvoice> SaveInvoices();
        Task<bool> UpdateAsset(Asset asset);
    }
}