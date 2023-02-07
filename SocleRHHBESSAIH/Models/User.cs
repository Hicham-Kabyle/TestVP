using System.ComponentModel.DataAnnotations;

namespace SocleRHHBESSAIH.Models
{
    public class User : EntityBase
    {
        [MaxLength(800)]
        public string FirstName { get; set; }
        [MaxLength(800)]
        public string LastName { get; set; }
        [MaxLength(800)]
        public string UserCurrency { get; set; }
        public string FullName()
        {
            return $"{FirstName} {LastName}";
        }

        public ICollection<Expense> Expenses { get; set; } = new List<Expense>();


    }
}
