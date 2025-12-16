using FindMyCoffee.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace FindMyCoffee.Dtos
{
    public class UserReadDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public Role Role { get; set; }
        public DateOnly? DOB { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; } = default!;                                                   
        public Gender Gender { get; set; } = default!;

    }
}
