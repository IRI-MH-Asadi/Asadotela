using Asadotela.Api.Data;
using Asadotela.Api.Models;
using Asadotela.Api.Services;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Asadotela.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly UserManager<ApiUser> _userManager;

    private readonly ILogger<AccountController> _logger;
    private readonly IMapper _mapper;
    private readonly IAuthManager _authManager;
    public AccountController( UserManager<ApiUser> userManager, ILogger<AccountController> logger, IAuthManager authManager, IMapper mapper )
    {
        _logger = logger;
        _userManager = userManager;
        _authManager = authManager;
        _mapper = mapper;
    }



    [HttpPost]
    [Route("Register")]
    public async Task<IActionResult> Register([FromBody] UserDTO userDTO)
    {
        _logger.LogInformation($"Registration Attempt for {userDTO.Email} ");
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var user = _mapper.Map<ApiUser>(userDTO);
            user.UserName = userDTO.Email;
            var result = await _userManager.CreateAsync(user, userDTO.Password);

            if (!result.Succeeded)
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError(item.Code, item.Description);

                }
                return BadRequest(ModelState);
            }

            await _userManager.AddToRolesAsync(user, userDTO.Roles);
            return Accepted();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Something Went Wrong in the {nameof(Register)}");
            return Problem($"Something Went Wrong in the {nameof(Register)}", statusCode: 500);
        }

    }


    [HttpPost]
    [Route("Login")]
    public async Task<IActionResult> Login([FromBody] LoginDTO userDTO)
    {
        _logger.LogInformation($"Login Attempt for {userDTO.Email} ");
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }


        try
        {

            
            if (!await _authManager.ValidateUser(userDTO))
            {
                return Unauthorized(userDTO);
            }

            return Accepted(new { Token = await _authManager.CreatrToken()});
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Something Went Wrong in the {nameof(Login)}");
            return Problem($"Something Went Wrong in the {nameof(Login)}", statusCode: 500);
        }

    }

}

