//using FindMyCoffee.Data.Interfaces;
//using FindMyCoffee.Models;

//namespace FindMyCoffee.Data
//{
//    public class MockShopRepo : IFindMyCoffeeRepo
//    {
//        public void CreateCoffeeShop(CoffeeShopEntity cafe)
//        {
//            throw new NotImplementedException();
//        }

//        public void DeleteCoffeeShop(CoffeeShopEntity shop)
//        {
//            throw new NotImplementedException();
//        }

//        public CoffeeShopEntity GetShopById(int id)
//        {
//            //id(int), Name, Type, title, rating, latitude, longitude, IsOpen
//            return new CoffeeShopEntity { Id = 1, BusinessName = "LaKitty", Type = "Cat coffee shop", Title = "Pet a cat and get fat!", Latitude = 2.1, Longitude = 3.2, IsOpen = true };
//        }

//        public CoffeeShopEntity? GetShopByName(string name)
//        {
//            throw new NotImplementedException();
//        }

//        public IEnumerable<CoffeeShopEntity> GetWebShopInfo()
//        {
//            List<CoffeeShopEntity> coffeeShops = new List<CoffeeShopEntity>
//            {
//                new CoffeeShopEntity { Id = 1, BusinessName = "LaKitty", Type = "Cat coffee shop", Title = "Pet a cat and get fat!", Latitude = 2.1, Longitude = 3.2, IsOpen = true },
//                new CoffeeShopEntity { Id = 2, BusinessName = "Larc-En-Ciel", Type = "Bakery", Title = "Baking your cake!", Latitude = 2.3, Longitude = 3.4, IsOpen = false }
//            };

//            return coffeeShops;
//        }

//        public Task<bool> AsyncSaveChanges()
//        {
//            throw new NotImplementedException();
//        }

//        public void UpdateCoffeeShop(CoffeeShopEntity shop)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}
