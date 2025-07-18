using System;

namespace Banking.Services.Card.Core.Application.Exceptions
{
    public class BusinessRuleException : Exception
    {
        public BusinessRuleException(string message) : base(message) { }
        public BusinessRuleException(string message, Exception innerException) : base(message, innerException) { }
    }
} 