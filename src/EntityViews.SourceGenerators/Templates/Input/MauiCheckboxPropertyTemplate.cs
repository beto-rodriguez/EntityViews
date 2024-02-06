﻿namespace EntityViews.SourceGenerators.Templates.Input;

public class MauiCheckboxPropertyTemplate
{
    public static string Build(InputTemplateParams p)
    {
        var property = p.Property;
        var viewModelName = p.ViewModelName;
        var viewModelNamespace = p.ViewModelNamespace;
        var formNamespace = p.FormNamespace;
        var propertyDisplaySource = p.PropertyDisplaySource;

        return @$"// <auto-generated/>
#nullable enable

using CommunityToolkit.Maui.Markup;
using {viewModelNamespace};

namespace {formNamespace};

public class {property.Name}Input : StackLayout
{{
    public {property.Name}Input()
    {{
        var label = new {Controls.GetDisplayClassName()}();
        {Controls.GetDisplayRef("label")}.Text({propertyDisplaySource});

        var input = new {Controls.GetCheckboxInputClassName()}();
        {Controls.GetCheckboxInputRef("input")}
            .Bind(
                CheckBox.IsCheckedProperty,
                getter: static ({viewModelName} vm) => vm.{property.Name},
                setter: static ({viewModelName} vm, bool value) => vm.{property.Name} = value);

        var validationLabel = new {Controls.GetValidationClassName()}();{Controls.SetValidationTextColor("validationLabel")}
        {Controls.GetValidationRef("validationLabel")}
            .Bind(
                Label.TextProperty,
                getter: static (ToDoViewModel vm) => vm.IsDone);

        Children.Add(label);
        Children.Add(input);
        Children.Add(validationLabel);
    }}
}}
";
    }
}
