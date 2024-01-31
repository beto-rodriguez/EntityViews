using System.ComponentModel.DataAnnotations;
using Models.Resources;

namespace Models;

public class ToDoModel
{
    public int Id { get; set; }


    [Required]
    [MinLength(5)]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;


    // custom messages:
    [Display(Name = "Please enter a reminder")]
    [Required(ErrorMessage = "A reminder is a mandatory field!")]
    public string Reminder { get; set; } = string.Empty;


    // use custom messages depending on the app language:
    [Display(
        Name = "DescriptionField",
        ResourceType = typeof(AppResources))]
    [Required(
        ErrorMessageResourceName = "DescriptionRequired",
        ErrorMessageResourceType = typeof(AppResources))]
    [MinLength(10)]
    [MaxLength(200)]
    public string Description { get; set; } = string.Empty;
}
