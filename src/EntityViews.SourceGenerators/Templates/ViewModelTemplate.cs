﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace EntityViews.SourceGenerators.Templates;

public class ViewModelTemplate
{
    public static string Build(
        Compilation compilation, ClassDeclarationSyntax classDeclaration, IEnumerable<IPropertySymbol> properties)
    {
        return $@"// <auto-generated/>
#nullable enable

using System.Runtime.CompilerServices;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace {classDeclaration.GetNameSpace()};

public partial class {classDeclaration.Identifier} : INotifyPropertyChanged
{{
    private readonly Dictionary<string, string> _validationErrors = [];
    private bool _isValid = true;
    private readonly Dictionary<string, Func<object?>> _propertyValue;
    private readonly HashSet<string> _dirtyFields = new();

    public {classDeclaration.Identifier}()
    {{
        _propertyValue = new()
        {{
{properties.Aggregate(string.Empty, (currentString, property) => currentString + @$"            {{ ""{property.Name}"", () => {property.Name} }},
")}
        }};
    }}

{properties.Aggregate(string.Empty, (currentString, property) => currentString + property.AsValidableProperty())}

    /// <summary>
    /// Gets a dictionary with the current validation errors.
    /// </summary>
    public Dictionary<string, string> ValidationErrors {{ get => _validationErrors; }}

    public event PropertyChangedEventHandler? PropertyChanged;

    public event Action<{classDeclaration.Identifier}, EntityViews.Attributes.ValidatingEventArgs>? Validating;

    /// <summary>
    /// Validates the view model and returns true if there are no validation errors.
    /// </summary>
    public bool IsValid()
    {{
        // notify the UI to update and delete the previous error.
        foreach (var error in _validationErrors)
        {{
            _ = _validationErrors.Remove(error.Key);
            OnPropertyChanged($""{{error.Key}}Error"");
            OnPropertyChanged($""{{error.Key}}HasError"");
        }}

        var context = new ValidationContext(this);
        var results = new List<ValidationResult>();
        _ = Validator.TryValidateObject(this, context, results, true);

        _isValid = true;

        foreach (var result in results)
            foreach (var member in result.MemberNames)
                AddValidationError(member, result.ErrorMessage ?? ""Unknown error."");

        Validating?.Invoke(this, new(null));
        if (_isValid) _dirtyFields.Clear();

        return _isValid;
    }}

    /// <summary>
    /// Validates the specified property.
    /// </summary>
    /// <param name=""propertyName"">Name of the property.</param>
    public void ValidateProperty(string propertyName)
    {{
        _ = _validationErrors.Remove(propertyName);
        OnPropertyChanged($""{{propertyName}}Error"");
        OnPropertyChanged($""{{propertyName}}HasError"");

        var context = new ValidationContext(this) {{ MemberName = propertyName }};
        var results = new List<ValidationResult>();

        _ = Validator.TryValidateProperty(_propertyValue[propertyName](), context, results);

        _isValid = true;

        foreach (var result in results)
            foreach (var member in result.MemberNames)
                AddValidationError(member, result.ErrorMessage ?? ""Unknown error."");

        Validating?.Invoke(this, new(propertyName));
    }}

    /// <summary>
    /// Validates a property only if the user has modified it.
    /// </summary>
    /// <param name=""propertyName"">The name of the property</param>
    /// <param name=""isDirtyValue"">a value indicating whether the property is dirty on this call.</param>
    public void ValidateDirtyProperty(string propertyName, bool isDirtyValue)
    {{
        var isDirty = _dirtyFields.Contains(propertyName) || isDirtyValue;
        if (isDirty)
        {{
            ValidateProperty(propertyName);
            _dirtyFields.Add(propertyName);
        }}
    }}

    public void AddValidationError(string propertyName, string errorMessage)
    {{
        // if there is already an error for this member, skip it
        if (_validationErrors.ContainsKey(propertyName)) return;

        _validationErrors[propertyName] = errorMessage;
        OnPropertyChanged($""{{propertyName}}Error"");
        OnPropertyChanged($""{{propertyName}}HasError"");
        _isValid = false;
    }}

    protected void SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {{
        field = value;
        OnPropertyChanged(propertyName ?? throw new Exception(""Unable to find property name.""));
    }}

    protected void OnPropertyChanged(string propertyName)
    {{
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }}

    private string GetError(string propertyName)
    {{
        return _validationErrors.TryGetValue(propertyName, out var result)
            ? result
            : string.Empty;
    }}
}}
    ";
    }
}
