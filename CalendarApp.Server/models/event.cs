using System.ComponentModel.DataAnnotations;
public class Event
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Title { get; set; } = string.Empty;
    public required string Description { get; set; } = string.Empty;
    public required DateTime StartDate { get; set; }
    public required DateTime EndDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public required Guid UserId { get; set; }
}

public class EventDto
{
    [Required(ErrorMessage = "Event title is required.")]
    [StringLength(30, ErrorMessage = "Event title cannot exceed 30 characters.")]
    [RegularExpression(@"^[\w\s.,'!&()\-]+$", ErrorMessage = "Event title can contain letters, numbers, spaces, and common punctuation.")]
    [MinLength(3, ErrorMessage = "Event title must be at least 3 characters long.")]

    public required string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Event description is required.")]
    [StringLength(100, ErrorMessage = "Event description cannot exceed 100 characters.")]
    [RegularExpression(@"^[\w\s.,'!&()\-]+$", ErrorMessage = "Event description can contain letters, numbers, spaces, and common punctuation.")]
    public required string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Start date is required.")]
    [DataType(DataType.DateTime)]
    public required DateTime StartDate { get; set; }

    [Required(ErrorMessage = "End date is required.")]
    [DataType(DataType.DateTime)]
    public required DateTime EndDate { get; set; }

    public required Guid UserId { get; set; }
}