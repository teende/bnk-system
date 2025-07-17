using Microsoft.AspNetCore.Mvc;
using Banking.Services.Account.Core.Application;
using Banking.Services.Account.Api.Dtos;

namespace Banking.Services.Account.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        
        public AccountController(IAccountService accountService)
        {
          _accountService = accountService;
        }


    [HttpPost("transfer")]
    public async Task<IActionResult> TransferMoney([FromBody] TransferMoneyDto dto)
    {
      var result = await _accountService.TransferMoneyAsync(dto);
      if (!result.Success)
          return BadRequest(result);
      return Ok(result);
    }
    }
} 