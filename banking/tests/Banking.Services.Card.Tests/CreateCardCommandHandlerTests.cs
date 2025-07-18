using Xunit;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Banking.Services.Card.Core.Application.UseCases.CreateCard;
using Banking.Services.Card.Core.Application.Interfaces;
using Banking.Services.Card.Core.Domain.Entities;

namespace Banking.Services.Card.Tests
{
    public class CreateCardCommandHandlerTests
    {
        private readonly Mock<ICardRepository> _cardRepo = new();
        private readonly Mock<ICreditHistoryService> _creditService = new();
        private readonly Mock<IMapper> _mapper = new();
        private readonly Mock<ILogger<CreateCardCommandHandler>> _logger = new();

        [Fact]
        public async Task Handle_SuccessfulCreditCardCreation_ReturnsResponse()
        {
            var command = new CreateCardCommand { UserId = Guid.NewGuid(), CardHolderName = "IVAN IVANOV", Type = CardType.Credit };
            _cardRepo.Setup(r => r.GetCardCountByUserIdAsync(command.UserId, It.IsAny<CancellationToken>())).ReturnsAsync(0);
            _creditService.Setup(s => s.HasGoodCreditHistoryAsync(command.UserId, It.IsAny<CancellationToken>())).ReturnsAsync(true);
            var card = new Card { Id = Guid.NewGuid(), CardHolderName = command.CardHolderName, Type = command.Type };
            _mapper.Setup(m => m.Map<Card>(command)).Returns(card);
            _mapper.Setup(m => m.Map<CreateCardResponse>(card)).Returns(new CreateCardResponse { Id = card.Id, CardHolderName = card.CardHolderName, Type = card.Type });
            var handler = new CreateCardCommandHandler(_cardRepo.Object, _creditService.Object, _mapper.Object, _logger.Object);

            var response = await handler.Handle(command, CancellationToken.None);

            Assert.Equal(card.Id, response.Id);
            Assert.Equal(command.CardHolderName, response.CardHolderName);
            Assert.Equal(command.Type, response.Type);
        }

        [Fact]
        public async Task Handle_ExceedsCardLimit_ThrowsBusinessRuleException()
        {
            var command = new CreateCardCommand { UserId = Guid.NewGuid(), CardHolderName = "IVAN IVANOV", Type = CardType.Debit };
            _cardRepo.Setup(r => r.GetCardCountByUserIdAsync(command.UserId, It.IsAny<CancellationToken>())).ReturnsAsync(5);
            var handler = new CreateCardCommandHandler(_cardRepo.Object, _creditService.Object, _mapper.Object, _logger.Object);

            await Assert.ThrowsAsync<BusinessRuleException>(() => handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_BadCreditHistory_ThrowsBusinessRuleException()
        {
            var command = new CreateCardCommand { UserId = Guid.NewGuid(), CardHolderName = "IVAN IVANOV", Type = CardType.Credit };
            _cardRepo.Setup(r => r.GetCardCountByUserIdAsync(command.UserId, It.IsAny<CancellationToken>())).ReturnsAsync(0);
            _creditService.Setup(s => s.HasGoodCreditHistoryAsync(command.UserId, It.IsAny<CancellationToken>())).ReturnsAsync(false);
            var handler = new CreateCardCommandHandler(_cardRepo.Object, _creditService.Object, _mapper.Object, _logger.Object);

            await Assert.ThrowsAsync<BusinessRuleException>(() => handler.Handle(command, CancellationToken.None));
        }
    }
} 