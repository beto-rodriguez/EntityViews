# EntityViews

This project generates ViewModels based on the models of the application using source generators.

The generated ViewModels implement `INotifyPropertyChanged` and re-use any `DataAnnotations` attribute to simplify the client-side validation by reducing boilerplate code.

## Example

Imagine we have a WebApi to build a TodoList (the next class is also a valid class for EntityFramework):

```c#
public class TodoItem
{
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    public bool IsComplete { get; set; }
}
```

Now we need to build a client so the user can populate the ToDo list, this projects uses source generators to build a ViewModel based on the previous object, the generated code 
will use the `Required` and `MaxLength` attributes to validate the object and quickly build the user interface.

This project generates a lot of boring, repeating, and neccessary code for us. the auto-generated view model will look like:

```c#
public partial class TodoItemViewModel : INotifyPropertyChanged
{
    private Dictionary<string, string> _validationErrors = [];

    // ...

    private string _name = string.Empty;
    [Required]
    [MaxLength(50)]
    public string Name { get => _name; set => SetProperty(ref _name, value, nameof(Name)); }
    public string NameError => GetError("Name");
    public bool NameHasError => NameError.Length > 0;

    // ...

    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Validates the view model and returns true if there are no validation errors.
    /// </summary>
    public bool IsValid()
    {
        // ...
    }

    // ...
}
```

Finally we can quickily validate user interfaces in a client app:

![val](https://github.com/beto-rodriguez/EntityViews/assets/10853349/a7f0005c-318f-40c7-91ff-00973b9e1922)

In this case, it is a Maui client defined with the next XAML:

```xml
<Label Text="Name" />

<Entry Text="{Binding Name}">
    <Entry.Triggers>
        <DataTrigger TargetType="Entry" Binding="{Binding NameHasError}" Value="True">
            <Setter Property="BackgroundColor" Value="#50ff0000"/>
        </DataTrigger>
    </Entry.Triggers>
</Entry>

<Label Text="{Binding NameError}" TextColor="Red"/>

<Button Text="Save" Command="{Binding SaveCommand}"/>
```
