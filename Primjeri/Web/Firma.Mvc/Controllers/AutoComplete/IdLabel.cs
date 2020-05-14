using System.Text.Json.Serialization;

namespace Firma.Mvc.Controllers.AutoComplete
{  
  public class IdLabel
  {
    [JsonPropertyName("label")]
    public string Label { get; set; }
    [JsonPropertyName("id")]
    public int Id { get; set; }
    public IdLabel() { }
    public IdLabel(int id, string label)
    {
      Id = id;
      Label = label;
    }
  }
}
