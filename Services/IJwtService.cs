using Backend_APIs.Models;

namespace Backend_APIs.Services
{
    public interface IJwtService
    {
        string GenerateToken(User user);
    }
}
