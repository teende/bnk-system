using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using FluentValidation;
using AutoMapper;
using Banking.Services.Card.Core.Domain.Entities;
using Banking.Services.Card.Core.Application.Interfaces;
using Banking.Services.Card.Core.Application.Exceptions;
using Banking.Services.Card.Core.Application.DTOs;
using Microsoft.Extensions.Logging;

namespace Banking.Services.Card.Core.Application.UseCases.CreateCard
{
    public readonly record struct CreateCardCommand : IRequest<CreateCardResponse>
    {
        public Guid UserId { get; init; }
        public string CardHolderName { get; init; }
        public CardType Type { get; init; }
    }

    public sealed class CreateCardCommandValidator : AbstractValidator<CreateCardCommand>
    {
        private static readonly Regex CardHolderNameRegex = new(@"^[A-Za-zА-Яа-я\s]+$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public CreateCardCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty()
                .WithMessage("UserId is required.");

            RuleFor(x => x.CardHolderName)
                .NotEmpty().WithMessage("Cardholder name is required.")
                .Length(3, 50).WithMessage("Cardholder name must be between 3 and 50 characters long.")
                .Matches(CardHolderNameRegex).WithMessage("Cardholder name must contain only letters and spaces.")
                .Must(BeValidCardHolderName).WithMessage("Cardholder name must be in 'FIRSTNAME LASTNAME' format.");

            RuleFor(x => x.Type)
                .IsInEnum().WithMessage("Invalid card type.");
        }

        private static bool BeValidCardHolderName(string? cardHolderName)
        {
            if (string.IsNullOrWhiteSpace(cardHolderName))
                return false;

            var parts = cardHolderName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return parts.Length >= 2 && parts.All(part => part.Length >= 2);
        }
    }

    public sealed record CreateCardResponse
    {
        public Guid Id { get; init; }
        public string CardNumber { get; init; } = string.Empty;
        public string CardHolderName { get; init; } = string.Empty;
        public DateTime ExpiryDate { get; init; }
        public string CVV { get; init; } = string.Empty;
        public CardType Type { get; init; }
        public decimal CreditLimit { get; init; }
        public decimal DailyLimit { get; init; }
        public string TariffName { get; init; } = string.Empty;
        public decimal AnnualFee { get; init; }
    }

    public sealed class CreateCardCommandHandler : IRequestHandler<CreateCardCommand, CreateCardResponse>
    {
        private readonly ICardRepository _cardRepository;
        private readonly ICreditHistoryService _creditHistoryService;
        private readonly IMapper _mapper;
        private readonly ILogger<CreateCardCommandHandler> _logger;
        private const int MaxCardsPerUser = 5;
        private const string MaskedCvv = "***";

        public CreateCardCommandHandler(
            ICardRepository cardRepository,
            ICreditHistoryService creditHistoryService,
            IMapper mapper,
            ILogger<CreateCardCommandHandler> logger)
        {
            _cardRepository = cardRepository ?? throw new ArgumentNullException(nameof(cardRepository));
            _creditHistoryService = creditHistoryService ?? throw new ArgumentNullException(nameof(creditHistoryService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<CreateCardResponse> Handle(CreateCardCommand request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using var activity = _logger.BeginScope(new Dictionary<string, object>
            {
                ["UserId"] = request.UserId,
                ["CardType"] = request.Type,
                ["Operation"] = "CreateCard"
            });

            _logger.LogInformation("Starting card creation for user {UserId}, type: {CardType}", 
                request.UserId, request.Type);

            try
            {
                var validationTasks = new List<Task>
                {
                    CheckUserCardLimitAsync(request.UserId, cancellationToken)
                };

                if (request.Type == CardType.Credit)
                {
                    validationTasks.Add(CheckCreditHistoryAsync(request.UserId, cancellationToken));
                }

                await Task.WhenAll(validationTasks).ConfigureAwait(false);

                var card = MapToCardEntity(request);
                await _cardRepository.AddAsync(card, cancellationToken).ConfigureAwait(false);

                _logger.LogInformation("Card successfully created for user {UserId}, card ID: {CardId}", 
                    request.UserId, card.Id);

                var response = _mapper.Map<CreateCardResponse>(card);
                MaskSensitiveData(response);

                return response;
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("Card creation cancelled for user {UserId}", request.UserId);
                throw;
            }
            catch (BusinessRuleException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error creating card for user {UserId}", request.UserId);
                throw new ApplicationException("An unexpected error occurred while creating the card.", ex);
            }
        }

        private async Task CheckUserCardLimitAsync(Guid userId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            _logger.LogDebug("Checking card limit for user {UserId}", userId);

            var cardCount = await _cardRepository.GetCardCountByUserIdAsync(userId, cancellationToken).ConfigureAwait(false);

            _logger.LogDebug("User {UserId} has {CardCount} cards", userId, cardCount);

            if (cardCount >= MaxCardsPerUser)
            {
                _logger.LogWarning("User {UserId} has reached card limit ({MaxCards})", userId, MaxCardsPerUser);
                throw new BusinessRuleException($"User has reached the maximum number of allowed cards ({MaxCardsPerUser}).");
            }
        }

        private async Task CheckCreditHistoryAsync(Guid userId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            _logger.LogDebug("Checking credit history for user {UserId}", userId);

            var hasGoodCreditHistory = await _creditHistoryService.HasGoodCreditHistoryAsync(userId, cancellationToken).ConfigureAwait(false);

            _logger.LogDebug("Credit history check result for user {UserId}: {HasGoodCreditHistory}", userId, hasGoodCreditHistory);

            if (!hasGoodCreditHistory)
            {
                _logger.LogWarning("Insufficient credit history for user {UserId}", userId);
                throw new BusinessRuleException("Insufficient credit history for a credit card.");
            }
        }

        private Card MapToCardEntity(CreateCardCommand command)
        {
            _logger.LogDebug("Mapping CreateCardCommand to Card entity for user {UserId}", command.UserId);
            
            try
            {
                var card = _mapper.Map<Card>(command);
                _logger.LogDebug("Successfully mapped CreateCardCommand to Card entity");
                return card;
            }
            catch (AutoMapperMappingException ex)
            {
                _logger.LogError(ex, "AutoMapper mapping failed for user {UserId}", command.UserId);
                throw new ApplicationException("Failed to map card data", ex);
            }
        }

        private void MaskSensitiveData(CreateCardResponse response)
        {
            _logger.LogDebug("Masking sensitive data for card {CardId}", response.Id);

            if (!string.IsNullOrEmpty(response.CardNumber))
            {
                var originalNumber = response.CardNumber;
                response = response with { CardNumber = MaskCardNumber(response.CardNumber) };
                _logger.LogDebug("Masked card number: {OriginalNumber} -> {MaskedNumber}", 
                    originalNumber, response.CardNumber);
            }

            if (!string.IsNullOrEmpty(response.CVV))
            {
                response = response with { CVV = MaskedCvv };
                _logger.LogDebug("Masked CVV for card {CardId}", response.Id);
            }
        }

        private static string MaskCardNumber(string cardNumber)
        {
            if (string.IsNullOrEmpty(cardNumber) || cardNumber.Length < 4)
                return cardNumber;

            return string.Create(cardNumber.Length, cardNumber, (span, original) =>
            {
                original.AsSpan().CopyTo(span);
                span[..^4].Fill('*');
            });
        }
    }

    public sealed class BusinessRuleException : Exception
    {
        public BusinessRuleException(string message) : base(message) { }
        public BusinessRuleException(string message, Exception innerException) : base(message, innerException) { }
    }
} 