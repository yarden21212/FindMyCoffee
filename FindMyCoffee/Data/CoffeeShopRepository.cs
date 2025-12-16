using FindMyCoffee.Data.Interfaces;
using FindMyCoffee.Models;
using Microsoft.AspNetCore.Mvc;
using FindMyCoffee.Domain.Enums;
using FindMyCoffee.Dtos;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace FindMyCoffee.Data
{
    public class CoffeeShopRepository : ICoffeeShopRepository
    {
        private readonly FindMyCoffeeContext _context;
        private readonly IMapper _mapper;

        public CoffeeShopRepository(FindMyCoffeeContext context)
        {
            _context = context;
        }
        /* Retrieves a coffee shop by its ID. Throws an exception if it does not exist. */
        public CoffeeShopEntity GetShopById(int id)
        {
            CoffeeShopEntity? shop = _context.CoffeeShops.FirstOrDefault(cafe => cafe.Id == id);

            if (shop == null) throw new ArgumentException($"Shop with ID {id} not found");
            else { return shop; }
        }

        /* Returns all coffee shop records from the database as a list. */
        public IEnumerable<CoffeeShopEntity> GetWebShopInfo()
        {
            return _context.CoffeeShops.ToList();
        }

        /* Get a specific coffee shop by its name. Used for "FindByName" method in the website*/
        public CoffeeShopEntity GetShopByName(string name)
        {
            CoffeeShopEntity? shop = _context.CoffeeShops.FirstOrDefault(cafe => cafe.BusinessName == name);

            if(shop == null) throw new ArgumentException($"Could not find {name}");
            else { return shop; }
        }
        /* Get coffee shops by type. Used for "FindByType" method in the website*/
        public CoffeeShopEntity GetShopByType(string type)
        {
            CoffeeShopEntity? shop = _context.CoffeeShops.FirstOrDefault(c => c.Type == type);

            if (shop == null) throw new ArgumentException($"Could not find {type}");
            else { return shop; }
        }
        /* Call this method whenever you want to save changes to the database. */
        public async Task<bool> AsyncSaveChanges()
        {
            return await _context.SaveChangesAsync() >= 0;
        }

        /* Adds a new coffee shop entity to the DbContext for creation. */
        public void CreateCoffeeShop(CoffeeShopEntity shop)
        {
            if (shop == null) throw new ArgumentNullException(nameof(shop));
            else
            {
                _context.CoffeeShops.Add(shop);
            }
        }

        public void UpdateCoffeeShop(CoffeeShopEntity shop)
        {
            // Do nothing. We want to keep the interface consistent (CRUD architecture).
        }

        /* Marks the coffee shop entity for deletion. The actual removal occurs when AsyncSaveChanges() is called. */
        public void DeleteCoffeeShop(CoffeeShopEntity shop)
        {
            if (shop == null) { throw new ArgumentException(nameof(shop)); }
            else
            {
                _context.CoffeeShops.Remove(shop);
            }
        }
    }
}
