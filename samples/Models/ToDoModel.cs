using System.ComponentModel.DataAnnotations;

namespace Models;

public class ToDoModel
{
    public int Id { get; set; }
    [Required]
    [MaxLength(50)]
    [Display(Name = "Title")]
    public string Name { get; set; } = string.Empty;
}
