using Asadotela.Api.Data;
using Asadotela.Api.IRepository;
using Asadotela.Api.Models;
using Asadotela.Api.Repository;
using AutoMapper;
using Marvin.Cache.Headers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Asadotela.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CountryController : ControllerBase
{

    private readonly IUnitOfWork _db;
    private readonly ILogger<CountryController> _logger;
    private readonly IMapper _mapper;
    public CountryController(IUnitOfWork db, ILogger<CountryController> logger, IMapper mapper)
    {
        _logger = logger;
        _db = db;
        _mapper = mapper;
    }

    [HttpGet]
    //[ResponseCache(CacheProfileName = "120SecondsDuration")]
    //[HttpCacheExpiration(CacheLocation = CacheLocation.Public, MaxAge = 1000)]
    //[HttpCacheValidation(MustRevalidate = false)]
    public async Task<IActionResult> GetCountries([FromQuery]RequestParams requestParams)
    {

        var countries = await _db.Countries.GetAllAsync(requestParams,includes: q=>q.Include(i=>i.Hotels));
        var result = _mapper.Map<IList<CountryDTO>>(countries);
        return Ok(countries);



    }

    [HttpGet("{id:int}", Name = "GetCountry")]
    public async Task<IActionResult> GetCountry(int id)
    {

        var country = await _db.Countries.GetAsync(q => q.Id == id, q => q.Include(i => i.Hotels));
        var result = _mapper.Map<CountryDTO>(country);
        return Ok(country);

    }

    [Authorize]
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateCountry(int id, [FromBody] UpdateCountryDTO countryDTO)
    {
        if (!ModelState.IsValid || id < 1)
        {
            _logger.LogError($"Invalid UPDATE attempt in {nameof(UpdateCountry)}");
            return BadRequest(ModelState);
        }



        var country = await _db.Countries.GetAsync(q => q.Id == id);
        if (country == null)
        {
            _logger.LogError($"Invalid UPDATE attempt in {nameof(UpdateCountry)}");
            return BadRequest("Submitted data is Invalid.");
        }
        _mapper.Map(countryDTO, country);
        _db.Countries.Update(country);
        await _db.Save();

        return NoContent();




    }

    [HttpPost]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> CreateCountry(CreateCountryDTO countryDTO)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogError($"Unsupported POST request to {nameof(CreateCountry)}");
            return BadRequest(ModelState);
        }


        try
        {
            var country = _mapper.Map<Country>(countryDTO);
            await _db.Countries.InsertAsync(country);

            return CreatedAtRoute("GetCountry", new { id = country.Id }, country);
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Something went wrong in the {nameof(CreateCountry)}");
            return StatusCode(500, "Internal Server Error. Please Try Again later.");

        }
    }


    [Authorize(Roles = "Administrator")]
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteCountry(int id)
    {

        var hotel = await _db.Countries.GetAsync(g => g.Id == id);
        if (hotel == null)
        {
            _logger.LogError($"Invalid Delete attempt in {nameof(DeleteCountry)}");
            return BadRequest($"No Country found with Id = {id}");
        }

        await _db.Countries.DeleteAsync(id);
        await _db.Save();

        return NoContent();

    }
}
