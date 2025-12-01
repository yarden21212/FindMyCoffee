using FindMyCoffee.Data.Interfaces;
using FindMyCoffee.Models;
using Microsoft.AspNetCore.Mvc;
using FindMyCoffee.Domain.Enums;
using FindMyCoffee.Dtos;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace FindMyCoffee.Data
{
    public class SqlFindMyCoffeeRepo : ICoffeeShopRepository
    {
        private readonly FindMyCoffeeContext _context;
        private readonly IMapper _mapper;

        public SqlFindMyCoffeeRepo(FindMyCoffeeContext context)
        {
            _context = context;
        }
        public CoffeeShopEntity GetShopById(int id)
        {
            CoffeeShopEntity? shop = _context.CoffeeShops.FirstOrDefault(cafe => cafe.Id == id);

            if (shop == null) throw new ArgumentException($"Shop with ID {id} not found");
            else { return shop; }
        }

        public IEnumerable<CoffeeShopEntity> GetWebShopInfo()
        {
            return _context.CoffeeShops.ToList();
        }

        //More methods
        public CoffeeShopEntity GetShopByName(string name)
        {
            CoffeeShopEntity? shop = _context.CoffeeShops.FirstOrDefault(cafe => cafe.BusinessName == name);

            if(shop == null) throw new ArgumentException($"Could not find {name}");
            else { return shop; }
        }
        public CoffeeShopEntity GetShopByType(string type)
        {
            CoffeeShopEntity? shop = _context.CoffeeShops.FirstOrDefault(c => c.Type == type);

            if (shop == null) throw new ArgumentException($"Could not find {type}");
            else { return shop; }
        }

        public async Task<bool> AsyncSaveChanges()
        {
            return await _context.SaveChangesAsync() >= 0;
        }

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
            //Do nothing. We want to keep the interface consistent.
        }

        public void DeleteCoffeeShop(CoffeeShopEntity shop)
        {
            if (shop == null) { throw new ArgumentException(nameof(shop)); }
            else
            {
                _context.CoffeeShops.Remove(shop);
            }
        }
         //---------------------------------                   User interface implementation:                   ------------------------------------------// 


    }
}
