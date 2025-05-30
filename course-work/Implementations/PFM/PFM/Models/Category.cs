using System;
using System.ComponentModel.DataAnnotations;

namespace PFM.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(250)]
        public string? Description { get; set; }

        [Required]
        public bool IsIncome { get; set; } // true za dohod, false za razhd

        [Required]
        public DateTime CreatedDate { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;


    }
}
