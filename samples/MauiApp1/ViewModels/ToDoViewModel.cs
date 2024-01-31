using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.Input;
using EntityViews.Attributes;
using Models;

namespace MauiApp1.ViewModels;

[ViewModel(BaseType = typeof(ToDoModel), Form = FormKind.MauiMarkup)]
public partial class ToDoViewModel
{
    [RelayCommand]
    public void Save()
    {
        if (!IsValid()) return;

        //Save the ToDo here...
        // Todo.Save();
    }
}


public partial class ToDoViewModel2 : INotifyPropertyChanged
{
    private readonly Dictionary<string, string> _validationErrors = [];


    private int _id;
    public int Id { get => _id; set => SetProperty(ref _id, value, nameof(Id)); }
    public string IdError => GetError(nameof(Id));
    public bool IdHasError => IdError.Length > 0;

    private string _name = string.Empty;
    [System.ComponentModel.DataAnnotations.RequiredAttribute]
    [System.ComponentModel.DataAnnotations.MinLengthAttribute(5)]
    [System.ComponentModel.DataAnnotations.MaxLengthAttribute(50)]
    public string Name { get => _name; set => SetProperty(ref _name, value, nameof(Name)); }
    public string NameError => GetError(nameof(Name));
    public bool NameHasError => NameError.Length > 0;

    private string _description = string.Empty;
    [System.ComponentModel.DataAnnotations.RequiredAttribute(ErrorMessage = "Enter a description!!!")]
    [System.ComponentModel.DataAnnotations.MinLengthAttribute(10)]
    [System.ComponentModel.DataAnnotations.MaxLengthAttribute(200)]
    public string Description { get => _description; set => SetProperty(ref _description, value, nameof(Description)); }
    public string DescriptionError => GetError(nameof(Description));
    public bool DescriptionHasError => DescriptionError.Length > 0;


    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Validates the view model and returns true if there are no validation errors.
    /// </summary>
    public bool IsValid()
    {
        // notify the UI to update and delete the previous error.
        foreach (var error in _validationErrors)
        {
            _ = _validationErrors.Remove(error.Key);
            OnPropertyChanged($"{error.Key}Error");
            OnPropertyChanged($"{error.Key}HasError");
        }

        var context = new ValidationContext(this);
        var results = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(this, context, results, true);

        new RequiredAttribute().IsValid(null);

        foreach (var result in results)
        {
            foreach (var member in result.MemberNames)
            {
                // if there is already an error for this member, skip it
                if (_validationErrors.ContainsKey(member)) continue;

                _validationErrors[member] = result.ErrorMessage ?? "Unknown error.";
                OnPropertyChanged($"{member}Error");
                OnPropertyChanged($"{member}HasError");
            }
        }

        return isValid;
    }

    protected void SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        field = value;
        OnPropertyChanged(propertyName ?? throw new Exception("Unable to find property name."));
    }

    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private string GetError(string propertyName)
    {
        return _validationErrors.TryGetValue(propertyName, out var result)
            ? result
            : string.Empty;
    }
}
