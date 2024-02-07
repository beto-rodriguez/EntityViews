using EntityViews.Attributes;

namespace MauiApp1.Input;

[DisplayControl]
public class CustomDisplayLabel : Border
{
    public CustomDisplayLabel()
    {
        Content = Label = new Label
        {
            TextColor = Colors.Blue
        };
    }

    [ControlProperty]
    public Label Label { get; set; }
}

