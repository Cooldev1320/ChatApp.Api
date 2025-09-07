using ChatApp.Api.Models;

namespace ChatApp.Api.Services
{
    public interface IJwtService
    {
        string GenerateToken(User user);
    }
}