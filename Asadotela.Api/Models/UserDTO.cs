using System.ComponentModel.DataAnnotations;

namespace Asadotela.Api.Models;



public class LoginDTO
{
    [Required]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; }
    [Required]
    [StringLength(15, ErrorMessage = "Your password is limmited to {2} to {1} charachters", MinimumLength = 6)]
    public string Password { get; set; }
}
public class UserDTO:LoginDTO
{

    public string FirstName { get; set; }
    public string LastName { get; set; }
    [DataType(DataType.PhoneNumber)] public string PhoneNumber { get; set; }

    public ICollection<string> Roles { get; set; }

}
