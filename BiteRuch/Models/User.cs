using System.ComponentModel.DataAnnotations;

namespace BiteRush.Models;

public class User
{
    [Key]
    public Guid Id { get; set; }
    [Required]
    [StringLength(50)]
    public string? Name { get; set; }
    [StringLength(50)]
    [EmailAddress]
    public string? Email { get; set; }
    [Required]
    [StringLength(1000)]
    public string? Password { get; set; }
    [Required]
    [StringLength(10)]
    public string? Gender { get; set; }
    [Range(1, 4)]
    public int Level { get; set; }

    public User() { }
    public User(Guid id, string name, string email, string password, string gender, int level)
    {
        Id = id;
        Name = name;
        Email = email;
        Password = password;
        Gender = gender;
        Level = level;
    }

}
