﻿using Asadotela.Api.Data;
using Asadotela.Api.IRepository;
using Asadotela.Api.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
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
                var hotels = await _db.Hotels.GetAllAsync(includes: new List<string> { "Country" });
                var result = _mapper.Map<List<HotelDTO>>(hotels);

                return Ok(result);

            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Something went wrong in the {nameof(GetHotels)}");
                return StatusCode(500, "Internal Server Error. Please Try Again later.");
            }
        }



        //todo: here Not work by any token
        [HttpGet("{id:int}", Name = "GetHotel")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetHotel(int id)
        {
            try
            {
                var hotel = await _db.Hotels.GetAsync(q => q.Id == id, new List<string> { "Country" });
                var result = _mapper.Map<HotelDTO>(hotel);

                return Ok(result);

            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Something went wrong in the {nameof(GetHotel)}");
                return StatusCode(500, "Internal Server Error. Please Try Again later.");
            }
        }
        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateHotel([FromBody] CreateHotelDTO dto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError($"Invalid POST attempt in {nameof(CreateHotel)}");
                return BadRequest(ModelState);
            }


            try
            {
                var data = _mapper.Map<Hotel>(dto);
                await _db.Hotels.InsertAsync(data);
                await _db.Save();

                return CreatedAtRoute("GetHotel", new { id = data.Id }, data);

            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Something went wrong in the {nameof(CreateHotel)}");
                return StatusCode(500, "Internal Server Error. Please Try Again later.");
            }
        }

        [Authorize]
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateHotel(int id, [FromBody] CreateHotelDTO dto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError($"Invalid UPDATE attempt in {nameof(UpdateHotel)}");
                return BadRequest(ModelState);
            }


            try
            {
                var hotel = await _db.Hotels.GetAsync(q => q.Id == id);
                if (hotel == null)
                {
                    _logger.LogError($"Invalid UPDATE attempt in {nameof(UpdateHotel)}");
                    return BadRequest("Submitted data is invalid");
                }

                _mapper.Map(dto, hotel);
                _db.Hotels.Update(hotel);
                await _db.Save();
                return NoContent();
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Something went wrong in the {nameof(UpdateHotel)}");
                return StatusCode(500, "Internal Server Error. Please Try Again later.");
            }
        }

        [Authorize(Roles = "Administrator")]
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteHotel(int id)
        {
            try
            {
                var hotel = await _db.Hotels.GetAsync(g => g.Id == id);
                if (hotel == null)
                {
                    _logger.LogError($"Invalid Delete attempt in {nameof(DeleteHotel)}");
                    return BadRequest($"No Hotel found with Id = {id}");
                }

                await _db.Hotels.DeleteAsync(id);
                await _db.Save();

                return NoContent();
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Something went wrong in the {nameof(DeleteHotel)}");
                return StatusCode(500, "Internal Server Error. Please Try Again later.");
            }
        }
    }
}
