using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Moq;
using SocleRHHBESSAIH.Controllers;
using SocleRHHBESSAIH.CustomExceptions;
using SocleRHHBESSAIH.Models;
using SocleRHHBESSAIH.Repositories;
using SocleRHHBESSAIH.Repositories.Interfaces;
using SocleRHHBESSAIH.ViewModels.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace TestSocleRHHB
{
    public class ExpenseControllerTest
    {

        [Fact]
        public void GetByUser_ReturnsExpensesByUser_WhenCalled()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var expenses = new[] {
                new Expense { Id = Guid.NewGuid(), User = new User { Id = userId }, Amount = 100, Date = DateTime.Now },
                new Expense { Id = Guid.NewGuid(), User = new User { Id = userId }, Amount = 200, Date = DateTime.Now },
                new Expense { Id = Guid.NewGuid(), User = new User { Id = Guid.NewGuid() }, Amount = 50, Date = DateTime.Now }
            };
            var mockRepo = new Mock<IExpenseRepository>();
            mockRepo.Setup(repo => repo.GetExpensesByUser(userId))
                    .Returns(expenses.Where(e => e.User.Id == userId));
            var controller = new ExpenseController(mockRepo.Object);

            // Act
            var result = controller.GetByUser(userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnExpenses = Assert.IsAssignableFrom<IEnumerable<ExpenseDTO>>(okResult.Value);
            Assert.Equal(2, returnExpenses.Count());
            Assert.Equal(expenses.Where(e => e.User.Id == userId).Sum(e => e.Amount), returnExpenses.Sum(e => e.Amount));
        }

        [Fact]
        public void Create_AddsExpense_WhenCalled()
        {
            // Arrange
            var mockExpenseRepository = new Mock<IExpenseRepository>();
            var expenseController = new ExpenseController(mockExpenseRepository.Object);
            var expense = new Expense { Id = Guid.NewGuid(), 
                                        User = new User { Id = Guid.NewGuid(),UserCurrency = "Euro" }, 
                                        Amount = 100,
                                        ExpenseCurrency = "Euro",
                                        Date = DateTime.Now,
                                        Comment ="comment "};

            // Act
            var result = expenseController.Create(expense);

            // Assert 
            mockExpenseRepository.Verify(r => r.AddExpense(expense), Times.Once);
            var createdAtRouteResult = Assert.IsType<CreatedAtRouteResult>(result);
            Assert.Equal("GetExpense", createdAtRouteResult.RouteName);
            Assert.Same(expense, createdAtRouteResult.Value);
        }

        [Fact]
        public void Create_AddsExpense_WhenModelStateIsValid()
        {
            // Arrange
            var mockExpenseRepository = new Mock<IExpenseRepository>();
            var controller = new ExpenseController(mockExpenseRepository.Object);
            var expense = new Expense();

            // Act
            var result = controller.Create(expense);

            // Assert
            mockExpenseRepository.Verify(repo => repo.AddExpense(expense), Times.Once);
            var createdAtRouteResult = Assert.IsType<CreatedAtRouteResult>(result);
            Assert.Equal("GetExpense", createdAtRouteResult.RouteName);
            Assert.Equal(expense.Id, createdAtRouteResult.RouteValues["id"]);
            Assert.Equal(expense, createdAtRouteResult.Value);
        }




        [Fact]
        public void Create_ReturnsBadRequest_WhenDateIsInTheFuture()
        {
            // Arrange
            var mockExpenseRepository = new Mock<IExpenseRepository>();
            var expenseController = new ExpenseController(mockExpenseRepository.Object);

            var expense = new Expense
            {
                Comment = " Comment ",
                Date = DateTime.Now.AddMonths(1),
                Amount = 100,
                User = new User { UserCurrency = "USD" },
                ExpenseCurrency = "USD"
            };

            mockExpenseRepository
                .Setup(repo => repo.AddExpense(expense))
                .Throws(new InvalidExpenseException("Expense date must not be in the future"));

            // Act
            var result = expenseController.Create(expense);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Expense date must not be in the future", badRequestResult.Value);
        }


        [Fact]
        public void Create_ReturnsBadRequest_WhenDateIsMoreThanThreeMonthsInThePast()
        {
            // Arrange
            var mockExpenseRepository = new Mock<IExpenseRepository>();
            var expenseController = new ExpenseController(mockExpenseRepository.Object);

            var expense = new Expense
            {
                Comment = " Comment ",
                Date = DateTime.Now.AddMonths(-4),
                Amount = 100,
                User = new User { UserCurrency = "USD" },
                ExpenseCurrency = "USD"
            };

            mockExpenseRepository
                .Setup(repo => repo.AddExpense(expense))
                .Throws(new InvalidExpenseException("Expense date must be within the past 3 months"));

            // Act
            var result = expenseController.Create(expense);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Expense date must be within the past 3 months", badRequestResult.Value);
        }


        [Fact]
        public void Create_ReturnsBadRequest_WhenCommentIsEmpty()
        {
            // Arrange
            var mockExpenseRepository = new Mock<IExpenseRepository>();
            var expenseController = new ExpenseController(mockExpenseRepository.Object);

            var expense = new Expense
            {
                Comment = "",
                Date = DateTime.Now,
                Amount = 100,
                User = new User { UserCurrency = "USD" },
                ExpenseCurrency = "USD"
            };
            
            mockExpenseRepository
                .Setup(repo => repo.AddExpense(expense))
                .Throws(new InvalidExpenseException("Expense comment must not be empty"));
            
            // Act
            var result = expenseController.Create(expense);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Expense comment must not be empty", badRequestResult.Value);
        }



        [Fact]
        public void Create_ReturnsBadRequest_WhenExpenseAlreadyExistsForUser()
        {
            // Arrange
            var expense = new Expense
            {
                Date = DateTime.Now,
                Amount = 100,
                Comment = "",
                ExpenseCurrency = "USD",
                User = new User { UserCurrency = "USD" }
            };
            var existingExpense = new Expense
            {
                Date = expense.Date,
                Amount = expense.Amount,
                Comment = "Existing expense",
                ExpenseCurrency = "USD",
                User = expense.User
            };
            var mockRepository = new Mock<IExpenseRepository>();

            mockRepository.Setup(repo => repo.AddExpense(expense))
                .Throws(new InvalidExpenseException("Expense must not have the same date and amount as another expense for the same user"));
            
            var controller = new ExpenseController(mockRepository.Object);

            // Act
            var result = controller.Create(expense);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Expense must not have the same date and amount as another expense for the same user", badRequestResult.Value);
        }        

    }
}