﻿using EntityViews.Attributes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace EntityViews.SourceGenerators;

[Generator]
public class ViewModelsGenerator : ISourceGenerator
{
    public void Execute(GeneratorExecutionContext context)
    {
        var classesWithAttribute = context.Compilation.SyntaxTrees
            .SelectMany(st => st.GetRoot()
                    .DescendantNodes()
                    .Where(n => n is ClassDeclarationSyntax)
                    .Select(n => n as ClassDeclarationSyntax)
                    .Where(r => r!.AttributeLists
                        .SelectMany(al => al.Attributes)
                        .Any(a => a.Name.GetText().ToString() == nameof(GenerateViewModel))));

        foreach (var vmClassDeclaration in classesWithAttribute)
        {
            if (vmClassDeclaration is null) continue;

            var properties = vmClassDeclaration.Members
                .Where(m => m.Kind() == SyntaxKind.PropertyDeclaration)
                .Cast<PropertyDeclarationSyntax>();

            var source = $@"// <auto-generated/>
#nullable enable

using System.Runtime.CompilerServices;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace {vmClassDeclaration.GetNameSpace()};

public partial class {vmClassDeclaration.Identifier}ViewModel : INotifyPropertyChanged
{{
    private Dictionary<string, string> _validationErrors = [];

{properties.Aggregate(string.Empty, (currentString, property) =>
{
    var propertyAnalysis = property.Analyze();
    if (propertyAnalysis.Ignore) return currentString;

    var annotations = propertyAnalysis.DataAnnotations
        .Aggregate(string.Empty, (currentString, annotation) => currentString +
            (currentString.Length > 0 ? "\r\n" : "") +
            $"    [{annotation}]");

    var initialValue = string.Empty;
    if (property.Type.ToString() == "string") initialValue = " = string.Empty"; // special case for strings.

    return currentString + @$"    private {property.Type} {property.AsField()}{initialValue};{(annotations.Length > 0 ? "\r\n" + annotations : string.Empty)}
    public {property.Type} {property.Identifier} {{ get => {property.AsField()}; set => SetProperty(ref {property.AsField()}, value, nameof({property.Identifier})); }}
    public string {property.Identifier}Error => GetError(""{property.Identifier}"");
    public bool {property.Identifier}HasError => {property.Identifier}Error.Length > 0;

";
})}
    public event PropertyChangedEventHandler? PropertyChanged;

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
        }}

        var context = new ValidationContext(this);
        var results = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(this, context, results, true);

        foreach (var result in results)
        {{
            foreach (var member in result.MemberNames)
            {{
                // if there is already an error for this member, skip it
                if (_validationErrors.ContainsKey(member)) continue;

                _validationErrors[member] = result.ErrorMessage ?? ""Unknown error."";
                OnPropertyChanged($""{{member}}Error"");
                OnPropertyChanged($""{{member}}HasError"");
            }}
        }}

        return isValid;
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

            context.AddSource($"{vmClassDeclaration.Identifier}ViewModel.g.cs", source);
        }
    }

    public void Initialize(GeneratorInitializationContext context)
    {
        // No initialization required for this one
    }
}