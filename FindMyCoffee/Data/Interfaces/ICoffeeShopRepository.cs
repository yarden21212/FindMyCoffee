using FindMyCoffee.Dtos;
using FindMyCoffee.Models;

namespace FindMyCoffee.Data.Interfaces
{
    public interface ICoffeeShopRepository
    {
        /* --------------------------------- Shop --------------------------------- */
        Task<bool> AsyncSaveChanges();
        IEnumerable<CoffeeShopEntity> GetWebShopInfo();
        CoffeeShopEntity GetShopById(int id);
        CoffeeShopEntity? GetShopByName(string name);
        void CreateCoffeeShop(CoffeeShopEntity shop);
        void UpdateCoffeeShop(CoffeeShopEntity shop);

        void DeleteCoffeeShop(CoffeeShopEntity shop);



    }
}