using Microsoft.AspNetCore.Identity;
using System.Reflection.Metadata.Ecma335;

namespace Asadotela.Api.Data;

public class ApiUser : IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
}
