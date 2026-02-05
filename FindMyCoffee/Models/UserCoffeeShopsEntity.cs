namespace FindMyCoffee.Models
{

    /*
    * Join (link) entity that implements the many-to-many relationship between UserEntity and CoffeeShopEntity.
    *
    * Responsibilities:
    *   Connects a user to a specific coffee shop record (UserId <-> CoffeeShopId).
    *   Allows the relationship to store additional metadata (CreatedAt timestamp).
    */

    public class UserCoffeeShopsEntity
    {
        public CoffeeShopEntity CoffeeShop { get; set; }
        public int CoffeeShopId { get; set; }

        public UserEntity User { get; set; }
        public int UserId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }
}
