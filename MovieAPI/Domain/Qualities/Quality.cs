using System.ComponentModel.DataAnnotations;

namespace MovieAPI.Domain.Qualities;


public class Quality
{
    [Key]
    public int QualityId { get; set; }

    [Required, StringLength(50)]
    public string QualityName { get; set; } 

    public ICollection<MovieQuality> MovieQualities { get; set; } = new List<MovieQuality>();
}