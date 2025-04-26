using System.ComponentModel.DataAnnotations;

public class User
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Name { get; set; } 
    public DateTime CreatedAt { get; set; }
}


public class UserDto
{
    [Required(ErrorMessage = "User name is required.")]
    [StringLength(15, ErrorMessage = "User name cannot exceed 15 characters.")]
    [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "User name can only contain alphanumeric characters.")]
    [MinLength(3, ErrorMessage = "User name must be at least 3 characters long.")]
    public required string Name { get; set; } = string.Empty;
}