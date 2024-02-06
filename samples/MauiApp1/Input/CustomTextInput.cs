using EntityViews.Attributes.Input;

namespace MauiApp1.Input;

[TextControl]
public class CustomTextInput : Border
{
    public CustomTextInput()
    {
        BackgroundColor = Colors.Gray;
        Padding = new Thickness(5);
        Content = TextInput = new Entry
        {
            TextColor = Colors.Black
        };
    }

    [ControlProperty]
    public Entry TextInput { get; set; }
}

