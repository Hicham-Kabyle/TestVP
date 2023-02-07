using Microsoft.EntityFrameworkCore;
using SocleRHHBESSAIH.CustomExceptions;
using SocleRHHBESSAIH.Data;
using SocleRHHBESSAIH.Models;
using SocleRHHBESSAIH.Repositories.Interfaces;

namespace SocleRHHBESSAIH.Repositories
{
    public class ExpenseRepository : IExpenseRepository
    {

        private readonly SocleRHHBContext _dbContext;

        public ExpenseRepository(SocleRHHBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<Expense> GetExpensesByUser(Guid userId)
        {
            return _dbContext.Expenses
                .Include(d => d.User)
                .Where(d => d.UserId == userId);
        }

        public void AddExpense(Expense expense)
        {
            if (!IsExpenseValid(expense, out var message))
                throw new InvalidExpenseException(message);

            _dbContext.Expenses.Add(expense);
            _dbContext.SaveChanges();
        }

        public bool IsExpenseValid(Expense expense, out string message)
        {
            message = "";

            if (expense.Date > DateTime.Now)
            {
                message = "Expense date must not be in the future";
                return false;
            }
            if (expense.Date < DateTime.Now.AddMonths(-3))
            {
                message = "Expense date must be within the past 3 months";
                return false;
            }
            if (string.IsNullOrEmpty(expense.Comment))
            {
                message = "Expense comment must not be empty";
                return false;
            }
            if (expense.User.Expenses.Any(d => d.Date == expense.Date && d.Amount == expense.Amount))
            {
                message = "Expense must not have the same date and amount as another expense for the same user";
                return false;
            }
            if (expense.ExpenseCurrency != expense.User.UserCurrency)
            {
                message = "Expense currency must match user currency";
                return false;
            }

            return true;
        }
    }

}
