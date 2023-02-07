using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SocleRHHBESSAIH.Models;
using SocleRHHBESSAIH.Repositories.Interfaces;
using SocleRHHBESSAIH.ViewModels.DTO;

namespace SocleRHHBESSAIH.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExpenseController : ControllerBase
    {
        private readonly IExpenseRepository _expenseRepository;

        public ExpenseController( IExpenseRepository expenseRepository)
        {
            _expenseRepository = expenseRepository;
        }
        [HttpGet("{userId}")]
        public IActionResult GetByUser(Guid userId, [FromQuery] string sortBy = nameof(Expense.Date))
        {
            var expenses = _expenseRepository.GetExpensesByUser(userId);

            if (sortBy.ToLower() == nameof(Expense.Amount).ToLower())
            {
                expenses = expenses.OrderByDescending(d => d.Amount);
            }
            else
            {
                expenses = expenses.OrderByDescending(d => d.Date);
            }

            var expensesDto = expenses.Select(d => new ExpenseDTO
            {
                Id = d.Id.ToString(),
                User = $"{d.User.FirstName} {d.User.LastName}",
                Date = d.Date,
                ExpenseOrigin = d.ExpenseOrigin.ToString(),
                Amount = d.Amount,
                ExpenseCurrency = d.ExpenseCurrency,
                Comment = d.Comment
            });
            return Ok(expensesDto);
        }

        [HttpPost]
        public IActionResult Create([FromBody] Expense expense)
        {
            try
            {

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                _expenseRepository.AddExpense(expense);

                return CreatedAtRoute("GetExpense", new { id = expense.Id }, expense);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

    }
}
