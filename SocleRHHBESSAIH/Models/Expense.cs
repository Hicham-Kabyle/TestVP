using SocleRHHBESSAIH.Data.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocleRHHBESSAIH.Models
{
    public class Expense : EntityBase
    {
        public User User { get; set; }
        public Guid UserId { get; set; }

        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        public ExpenseOrigin ExpenseOrigin { get; set; }
        public decimal Amount { get; set; }

        [MaxLength(800)]
        public string ExpenseCurrency { get; set; }

        [Required]
        public string Comment { get; set; }

        [NotMapped]
        public string AmountAndCurrency { get { return $"{Amount} {ExpenseCurrency}";  } }

    }

}
