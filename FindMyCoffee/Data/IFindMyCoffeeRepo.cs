using FindMyCoffee.Models;

namespace FindMyCoffee.Data
{
    public interface IFindMyCoffeeRepo
    {
        bool SaveChanges();
        IEnumerable<CoffeeShopEntity> GetWebShopInfo();
        CoffeeShopEntity GetShopById(int id);
        CoffeeShopEntity? GetShopByName(string name);
        void CreateCoffeeShop(CoffeeShopEntity shop);
        void UpdateCoffeeShop(CoffeeShopEntity shop);

        void DeleteCoffeeShop(CoffeeShopEntity shop);
    }
}