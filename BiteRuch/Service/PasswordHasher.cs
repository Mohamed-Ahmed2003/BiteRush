namespace BiteRush.Service;

public class PasswordHasher
{
    public static string HashPassword(string password)
    {
        var hashed = BCrypt.Net.BCrypt.HashPassword(password);
        return hashed;
    }

    public static bool VerifyPassword(string password, string hashedPassword)
    {
        return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
    }

}
