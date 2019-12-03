using System.Threading.Tasks;

namespace DTShop.OrderService.Data.Repositories
{
    public interface IWarehouseRepository
    {
        Task SupplyItemsAsync(int itemId, int amount, string name, decimal price);
        Task<bool> SaveChangesAsync();
    }
}
