using Asadotela.Api.IRepository;
using Asadotela.Api.Models;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Asadotela.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HotelController : ControllerBase
    {
        private readonly IUnitOfWork _db;
        private readonly ILogger<HotelController> _logger;
        private readonly IMapper _mapper;
        public HotelController(IUnitOfWork db, ILogger<HotelController> logger, IMapper mapper)
        {
            _logger = logger;
            _db = db;
            _mapper = mapper;
        }




        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetHotels()
        {
            try
            {
                var hotels = await _db.Hotels.GetAllAsync();
                var result = _mapper.Map<List<HotelDTO>>(hotels);

                return Ok(result);

            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Something went wrong in the {nameof(GetHotels)}");
                return StatusCode(500, "Internal Server Error. Please Try Again later.");
            }
        }
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetHotel(int id)
        {
            try
            {
                var hotel = await _db.Hotels.GetAsync(q=>q.Id == id,new List<string> { "Country"} );
                var result = _mapper.Map<HotelDTO>(hotel);

                return Ok(result);

            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Something went wrong in the {nameof(GetHotel)}");
                return StatusCode(500, "Internal Server Error. Please Try Again later.");
            }
        }



    }
}
