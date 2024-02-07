using CommunityToolkit.Maui.Behaviors;

namespace MauiApp1;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();

        CreditCardInput.Input.Behaviors.Add(new MaskedBehavior { Mask = "XXXX XXXX XXXX XXXX " });
    }
}

