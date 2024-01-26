using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using EntityViews.Attributes;

namespace Models;

[GenerateViewModel]
public class TodoItem
{
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    public bool IsComplete { get; set; }
}


public partial class TodoItemViewModel2 : INotifyPropertyChanged
{
    private Dictionary<string, string> _validationErrors = [];

    private int _id;

    private string GetError(string propertyName)
    {
        return _validationErrors.TryGetValue(propertyName, out var result)
            ? result
            : string.Empty;
    }

    public int Id { get => _id; set => SetProperty(ref _id, value, nameof(Id)); }
    public string IdError => GetError("");
    public bool IdHasError => IdError.Length > 0;


    private string _name = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Name { get => _name; set => SetProperty(ref _name, value, nameof(Name)); }
    public string NameError { get => _validationErrors["Name"]; }
    private bool _iscomplete;

    public bool IsComplete { get => _iscomplete; set => SetProperty(ref _iscomplete, value, nameof(IsComplete)); }
    public string IsCompleteError { get => _validationErrors["IsComplete"]; }

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
        }

        var context = new ValidationContext(this);
        var results = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(this, context, results, true);

        foreach (var result in results)
        {
            foreach (var member in result.MemberNames)
            {
                // if there is already an error for this member, skip it
                if (_validationErrors.ContainsKey(member)) continue;

                _validationErrors[member] = result.ErrorMessage ?? "Unknown error.";
                OnPropertyChanged($"{member}Error");
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
}
