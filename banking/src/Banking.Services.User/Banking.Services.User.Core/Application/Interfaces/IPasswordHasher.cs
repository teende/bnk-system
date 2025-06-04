using System.Threading.Tasks;

namespace Banking.Services.User.Core.Application.Interfaces
{
    public interface IPasswordHasher
    {
        Task<string> HashPasswordAsync(string password);
        Task<bool> VerifyPasswordAsync(string password, string hash);
    }
} 