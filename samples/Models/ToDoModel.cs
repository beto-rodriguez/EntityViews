using System.ComponentModel.DataAnnotations;
using EntityViews.Attributes.Maui;
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


    [MauiEditorInput] // <- use an Editor control instead of an entry (default for string)
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

    // numeric properties use an Entry control
    // but there is a special validation rule that parses the input text to the numeric type,
    // if failed, the error message is shown
    public double DoubleProp { get; set; }

    public DateTime Date { get; set; }
    public TimeSpan Time { get; set; }

    [MauiSwitchInput]
    public bool IsDone { get; set; }

    [MauiSliderInput]
    public double Slider { get; set; }

    [MauiStepperInput]
    public double Stepper { get; set; }
}
