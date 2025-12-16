namespace FindMyCoffee.Models
{
    public class UserCoffeeShopsEntity
    {
        public CoffeeShopEntity CoffeeShop { get; set; }
        public int CoffeeShopId { get; set; }

        public UserEntity User { get; set; }
        public int UserId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }
}
