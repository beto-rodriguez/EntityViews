using EntityViews.Attributes;

namespace MauiApp1.Input;

//[DisplayControl]
public class CustomDisplayLabel : Border
{
    public CustomDisplayLabel()
    {
        Content = Label = new Label
        {
            TextColor = Colors.Blue
        };
    }

    public Label Label { get; set; }
}

