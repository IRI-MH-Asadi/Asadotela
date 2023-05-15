using Asadotela.Api.IRepository;
using Asadotela.Api.Models;
using Asadotela.Api.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Asadotela.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CountryController : ControllerBase
{

    private readonly IUnitOfWork _db;
    private readonly ILogger<CountryController> _logger;
    private readonly IMapper _mapper;
    public CountryController(IUnitOfWork db, ILogger<CountryController> logger,IMapper mapper) 
    {
        _logger = logger;
        _db = db;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetCountries()
    {
        try
        {
            var countries =await  _db.Countries.GetAllAsync();
            var result = _mapper.Map<IList<CountryDTO>>(countries);
            return Ok(countries);
        }
        catch (Exception e)
        {
            _logger.LogError(e,$"Something went wrong in the {nameof(GetCountries)}");
            return StatusCode(500, "Internal Server Error. Please Try Again later.");
        }
    }
    
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetCountry(int id)
    {
        try
        {
            var country =await  _db.Countries.GetAsync(q => q.Id==id, new List<string> { "Hotels" });
            var result = _mapper.Map<CountryDTO>(country);
            return Ok(country);
        }
        catch (Exception e)
        {
            _logger.LogError(e,$"Something went wrong in the {nameof(GetCountry)}");
            return StatusCode(500, "Internal Server Error. Please Try Again later.");
        }
    }
}
