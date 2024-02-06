using EntityViews.Attributes.Input;

namespace MauiApp1.Input;

[DisplayControl]
public class CustomDisplayLabel : Border
{
    public CustomDisplayLabel()
    {
        BackgroundColor = Colors.Red;
        Padding = new Thickness(5);
        Content = Label = new Label
        {
            TextColor = Colors.White
        };
    }

    [ControlProperty]
    public Label Label { get; set; }
}

