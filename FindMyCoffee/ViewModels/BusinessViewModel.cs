using FindMyCoffee.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace FindMyCoffee.ViewModels
{

    public class BusinessViewModel
    {
        [Required]
        [Phone]
        public string Phone { get; set; } = default!;

        [Required]
        [EmailAddress]
        public string? ContactEmail { get; set; }

        //TODO: [Required]
        public bool AcceptBusinessTerms { get; set; }
    }
}

public enum BusinessStatus { Pending, Approved, Rejected };
