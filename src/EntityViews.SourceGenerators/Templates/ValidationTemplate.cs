﻿namespace EntityViews.SourceGenerators.Templates;

public class ValidationTemplate
{
    public static string Build()
    {
        return @"// <auto-generated />
#nullable enable

using System.Runtime.CompilerServices;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EntityViews.Validation;

/// <summary>
/// Defines the event arguments for the Validating event.
/// </summary>
/// <param name=""propertyName"">The validated property name, null when all properties are validated.</param>
/// <param name=""isValid"">A value indicating whether the validated properties are valid.</param>
public class ValidatingEventArgs(string? propertyName, bool isValid)
{
    /// <summary>
    /// Gets the name of the property being validated, null if the entire view model is being validated.
    /// </summary>
    public string? PropertyName { get; } = propertyName;

    /// <summary>
    /// Gets a value indicating whether the validated properties are valid.
    /// </summary>
    public bool IsValid { get; } = isValid;
}

/// <summary>
/// Defines the special validation cases handled by the EntityViews library.
/// </summary>
public static class SpecialValidationMessages
{
    /// <summary>
    /// Gets or sets the message for numeric inputs.
    /// </summary>
    public static string ValidNumber { get; set; } = ""'{0}' is not a valid number"";
}

public class ValidableViewModel : INotifyPropertyChanged
{
    private readonly Dictionary<string, string> _validationErrors = [];
    private bool _isValid = true;
    private readonly HashSet<string> _dirtyFields = new();

    /// <summary>
    /// Used internally to build validation errors.
    /// </summary>
    public Dictionary<string, Func<object?>> PropertyValues = [];

    /// <summary>
    /// Gets a dictionary with the current validation errors.
    /// </summary>
    public Dictionary<string, string> ValidationErrors { get => _validationErrors; }

    /// <summary>
    /// Occurs when a property value changes.
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Occurs when the view model is validating.
    /// </summary>
    public event Action<ValidableViewModel, ValidatingEventArgs>? Validating;

    /// <summary>
    /// Validates the view model and returns true if there are no validation errors.
    /// </summary>
    public bool IsValid()
    {
        _validationErrors.Clear();

        var context = new ValidationContext(this);
        var results = new List<ValidationResult>();
        _ = Validator.TryValidateObject(this, context, results, true);

        _isValid = true;

        foreach (var result in results)
            foreach (var member in result.MemberNames)
                AddValidationError(member, result.ErrorMessage ?? ""Unknown error."");

        Validating?.Invoke(this, new(null, _isValid));
        if (_isValid) _dirtyFields.Clear();

        return _isValid;
    }

    /// <summary>
    /// Validates the specified property and returns true if there are no validation errors.
    /// </summary>
    /// <param name=""propertyName"">Name of the property.</param>
    public bool ValidateProperty(string propertyName)
    {
        _ = _validationErrors.Remove(propertyName);

        var context = new ValidationContext(this) { MemberName = propertyName };
        var results = new List<ValidationResult>();

        _ = Validator.TryValidateProperty(PropertyValues[propertyName](), context, results);

        _isValid = true;

        foreach (var result in results)
            foreach (var member in result.MemberNames)
                AddValidationError(member, result.ErrorMessage ?? ""Unknown error."");

        Validating?.Invoke(this, new(propertyName, _isValid));

        return _isValid;
    }

    /// <summary>
    /// Validates a property only if the user has modified it.
    /// </summary>
    /// <param name=""propertyName"">The name of the property</param>
    /// <param name=""isDirtyValue"">a value indicating whether the property is dirty on this call.</param>
    public void ValidateDirtyProperty(string propertyName, bool isDirtyValue)
    {
        var isDirty = _dirtyFields.Contains(propertyName) || isDirtyValue;
        if (isDirty)
        {
            _ = ValidateProperty(propertyName);
            _dirtyFields.Add(propertyName);
        }
    }

    public void AddValidationError(string propertyName, string errorMessage)
    {
        // if there is already an error for this member, skip it
        if (_validationErrors.ContainsKey(propertyName)) return;

        _validationErrors[propertyName] = errorMessage;
        _isValid = false;
    }

    protected void SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        field = value;
        OnPropertyChanged(propertyName ?? throw new Exception(""Unable to find property name.""));
    }

    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
";
    }
}
