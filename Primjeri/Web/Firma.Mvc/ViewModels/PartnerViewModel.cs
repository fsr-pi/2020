using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Firma.Mvc.ViewModels
{
  public class PartnerViewModel : IValidatableObject
  {
    public int IdPartnera { get; set; }
    [Required]
    [RegularExpression("[OT]")]
    public string TipPartnera { get; set; }
    [Display(Name = "Prezime")]
    public string PrezimeOsobe { get; set; }
    [Display(Name = "Ime")]
    public string ImeOsobe { get; set; }
    [Display(Name = "Matični broj tvrtke")]
    public string MatBrTvrtke { get; set; }
    [Display(Name = "Naziv")]
    public string NazivTvrtke { get; set; }
    [Required]
    [RegularExpression("[0-9]{11}")]
    [Display(Name = "OIB")]
    public string Oib { get; set; }
    [Display(Name = "Adresa")]
    public string AdrPartnera { get; set; }
    [Display(Name = "Mjesto")]
    public int? IdMjestaPartnera { get; set; }
    public string NazMjestaPartnera { get; set; }
    [Display(Name = "Adresa isporuke")]
    public string AdrIsporuke { get; set; }
    [Display(Name = "Mjesto isporuke")]
    public int? IdMjestaIsporuke { get; set; }
    public string NazMjestaIsporuke { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
      if (TipPartnera == "O")
      {
        if (string.IsNullOrWhiteSpace(ImeOsobe)) { yield return new ValidationResult("Potrebno je upisati ime osobe", new[] { nameof(ImeOsobe) }); }
        if (string.IsNullOrWhiteSpace(PrezimeOsobe))
        {
          yield return new ValidationResult("Potrebno je upisati prezime osobe", new[] { nameof(PrezimeOsobe) });
        }
      }
      else if (TipPartnera == "T")
      {
        if (string.IsNullOrWhiteSpace(NazivTvrtke))
        {
          yield return new ValidationResult("Potrebno je upisati naziv tvrtke", new[] { nameof(NazivTvrtke) });
        }
        if (string.IsNullOrWhiteSpace(MatBrTvrtke))
        {
          yield return new ValidationResult("Potrebno je upisati matiÄni broj tvrtke", new[] { nameof(MatBrTvrtke) });
        }
      }

    }
  }
}
