using Xunit;
using Moq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Banking.Services.Account.Api.Controllers;
using Banking.Services.Account.Core.Application;
using Banking.Services.Account.Api.Dtos;

namespace Banking.Services.Main.Tests
{
    public class AccountControllerTests
    {
        [Fact]
        public async Task TransferMoney_ReturnsOk_WhenSuccess()
        {
            // Arrange
            var mockService = new Mock<IAccountService>();
            var dto = new TransferMoneyDto { FromAccountId = Guid.NewGuid(), ToAccountId = Guid.NewGuid(), Amount = 100, Currency = "RUB" };
            var result = new TransferResultDto { Success = true };
            mockService.Setup(s => s.TransferMoneyAsync(dto)).ReturnsAsync(result);
            var controller = new AccountController(mockService.Object);

            // Act
            var response = await controller.TransferMoney(dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(response);
            Assert.Equal(result, okResult.Value);
        }

        [Fact]
        public async Task TransferMoney_ReturnsBadRequest_WhenFail()
        {
            // Arrange
            var mockService = new Mock<IAccountService>();
            var dto = new TransferMoneyDto { FromAccountId = Guid.NewGuid(), ToAccountId = Guid.NewGuid(), Amount = 100, Currency = "RUB" };
            var result = new TransferResultDto { Success = false, ErrorMessage = "Ошибка" };
            mockService.Setup(s => s.TransferMoneyAsync(dto)).ReturnsAsync(result);
            var controller = new AccountController(mockService.Object);

            // Act
            var response = await controller.TransferMoney(dto);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(response);
            Assert.Equal(result, badRequest.Value);
        }
    }
} 