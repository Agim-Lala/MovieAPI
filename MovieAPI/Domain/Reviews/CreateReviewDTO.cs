using System.ComponentModel.DataAnnotations;

namespace MovieAPI.Domain.Reviews;

public class CreateReviewDTO
{
    [Required] public string Text { get; set; } = "";
    [Range(0,10)] public double Rating { get; set; }
    public int MovieId { get; set; }
}