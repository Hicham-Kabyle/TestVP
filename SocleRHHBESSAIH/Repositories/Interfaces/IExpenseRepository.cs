using SocleRHHBESSAIH.Models;

namespace SocleRHHBESSAIH.Repositories.Interfaces
{
    public interface IExpenseRepository
    {
        IEnumerable<Expense> GetExpensesByUser(Guid userId);
        void AddExpense(Expense expense);
        bool IsExpenseValid(Expense expense, out string message);
    }
}
