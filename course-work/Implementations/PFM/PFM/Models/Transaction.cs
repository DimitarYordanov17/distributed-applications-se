using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using PFM.Models;

namespace PFM.Models
{
    [Authorize]
    public class Transaction
    {
        public int Id { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }

        public int CategoryId { get; set; }

        [ValidateNever]
        public Category Category { get; set; } = null!;

        [MaxLength(200)]
        public string Description { get; set; }

        public DateTime Date { get; set; } = DateTime.UtcNow;

        public int AccountId { get; set; }

        [ValidateNever]
        public Account Account { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

    }
}