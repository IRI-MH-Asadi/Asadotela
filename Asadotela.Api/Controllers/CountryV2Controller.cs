using Asadotela.Api.IRepository;
using Asadotela.Api.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Asadotela.Api.Controllers;

[ApiVersion("2.0", Deprecated = true)]
[Route("api/Country")]
[ApiController]
public class CountryV2Controller : ControllerBase
{
    private readonly IUnitOfWork _db;
    private readonly ILogger<CountryController> _logger;
    private readonly IMapper _mapper;

    public CountryV2Controller(IUnitOfWork db, ILogger<CountryController> logger, IMapper mapper)
    {
        _logger = logger;
        _db = db;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetCountries([FromQuery] RequestParams requestParams)
    {
        var countries = await _db.Countries.GetAllAsync(requestParams, includes: i => i.Include(x => x.Hotels));
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
}