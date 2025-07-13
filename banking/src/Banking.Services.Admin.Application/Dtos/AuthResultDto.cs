using System;

namespace Banking.Services.Admin.Application.Dtos
{
  public class AuthResultDto
  {
      public bool Success { get; set; }
      public string Token { get; set; }
      public string UserName { get; set; }
      public string Role { get; set; }
      public string ErrorMessage { get; set; }
      public DateTime? ExpiresAt { get; set; }
  }
}