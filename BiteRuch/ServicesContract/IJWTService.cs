using BiteRush.Models;

namespace BiteRush.ServicesContract;

public interface IJWTService
{
    public string GenrateJWT(User user);
}
