using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace PFM.Models
{
    public class Account
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Type { get; set; } = string.Empty;

        [Required]
        public double Balance { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; }

        [MaxLength(250)]
        public string? Description { get; set; }

        [BindNever]
        public string UserId { get; set; } = string.Empty;
    }
}
