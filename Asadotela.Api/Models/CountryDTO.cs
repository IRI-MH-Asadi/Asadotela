using System.ComponentModel.DataAnnotations;

namespace Asadotela.Api.Models;

public class CreateCountryDTO
{
    [Required(ErrorMessage = "Country name is required")]
    [StringLength(maximumLength: 50, ErrorMessage = "Country Name is too long.")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Country short name is required")]
    [StringLength(maximumLength: 6, ErrorMessage = "Short country name is too long.")]
    public string ShortName { get; set; }
}

public class CountryDTO : CreateCountryDTO
{
    public int Id { get; set; }
    public IList<HotelDTO> Hotels { get; set; }

}
