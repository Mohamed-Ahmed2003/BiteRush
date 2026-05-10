using System.ComponentModel.DataAnnotations;

namespace BiteRush.DTO;

public class UserDTO
{
    [Required(ErrorMessage = " the name cannot be blank")]
    public string UserName { get; set; }
    [Required(ErrorMessage = "the email cannot be blank")]
    [EmailAddress]
    public string EmailAddress { get; set; }

    [Required(ErrorMessage = " the name cannot be blank")]
    public string Password { get; set; }

    [Required(ErrorMessage = "the gender cannot be blank")]
    public string Gender { get; set; }
    [Range(1, 4)]
    public int Level { get; set; }

}
