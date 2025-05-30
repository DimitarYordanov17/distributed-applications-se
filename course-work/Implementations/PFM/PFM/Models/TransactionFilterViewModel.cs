using System.Collections.Generic;
using PFM.Models;

namespace PFM.ViewModels
{
    public class TransactionFilterViewModel
    {
        public IEnumerable<Transaction> Transactions { get; set; }
        public int? CategoryId { get; set; }
        public string SearchUserId { get; set; }

        public List<Category> Categories { get; set; }
    }
}
