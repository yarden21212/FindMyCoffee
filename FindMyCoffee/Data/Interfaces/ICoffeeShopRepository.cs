using FindMyCoffee.Dtos;
using FindMyCoffee.Models;

namespace FindMyCoffee.Data.Interfaces
{
    /*
     * ICoffeeShopRepository, acts as the Repository for CoffeeShop entities.
     * It hides the database layer and provides simple operations for creating, retrieving, updating, and deleting shops.
     */
    public interface ICoffeeShopRepository
    {
        Task<bool> AsyncSaveChanges();
        IEnumerable<CoffeeShopEntity> GetWebShopInfo();
        CoffeeShopEntity GetShopById(int id);
        CoffeeShopEntity? GetShopByName(string name);
        void CreateCoffeeShop(CoffeeShopEntity shop);
        void UpdateCoffeeShop(CoffeeShopEntity shop);
        void DeleteCoffeeShop(CoffeeShopEntity shop);
    }
}