using EntityViews.Attributes.Input;

namespace MauiApp1.Input;

//[ValidationControl]
public class CustomValidationLabel : Border
{
    public CustomValidationLabel()
    {
        Content = Label = new Label
        {
            TextColor = Colors.Blue
        };
    }

    [ControlProperty]
    public Label Label { get; set; }
}

