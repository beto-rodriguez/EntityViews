using System.ComponentModel.DataAnnotations;

namespace Models;

public class ToDoModel
{
    public int Id { get; set; }

    [Required]
    [MinLength(5)]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "Please enter a description")]
    [Required(ErrorMessage = "Enter a description!!!")]
    [MinLength(10)]
    [MaxLength(200)]
    public string Description { get; set; } = string.Empty;
}
