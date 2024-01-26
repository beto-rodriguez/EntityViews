using System.ComponentModel.DataAnnotations;
using EntityViews.Attributes;

namespace Models;

[GenerateViewModel]
public class TodoItem
{
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    public bool IsComplete { get; set; }
}
