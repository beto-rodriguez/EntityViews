﻿namespace EntityViews.SourceGenerators.Templates.Input;

public class MauiEditorPropertyTemplate
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

        var input = new {Controls.GetTextAreaInputClassName()}();
        {Controls.GetTextAreaInputRef("input")}
            .Bind(
                Editor.TextProperty,
                getter: static ({viewModelName} vm) => vm.{property.Name},
                setter: static ({viewModelName} vm, {property.Type.Name} value) => vm.{property.Name} = value);
        {Controls.GetTextAreaInputRef("input")}.Triggers.Add(
            new DataTrigger(typeof(Editor))
            {{
                Binding = new Binding(""{property.Name}HasError""),
                Value = true,
                Setters = {{ new Setter {{ Property = BackgroundColorProperty, Value = {Controls.OnErrorBackgroundColor} }} }},
            }});
        async Task<bool> UserKeepsTyping()
        {{
            var txt = {Controls.GetTextAreaInputRef("input")}.Text;
            await Task.Delay(500);
            return txt != {Controls.GetTextAreaInputRef("input")}.Text;
        }}
        {Controls.GetTextAreaInputRef("input")}.TextChanged += async (_, _) =>
        {{
            if (await UserKeepsTyping()) return;
            (({viewModelName})BindingContext).ValidateDirtyProperty(
                ""{property.Name}"", {Controls.GetTextAreaInputRef("input")}.Text is not null && {Controls.GetTextAreaInputRef("input")}.Text.Length > 0);
        }};
        Input = {Controls.GetDateInputRef("input")};

        var validationLabel = new {Controls.GetValidationClassName()}();{Controls.SetValidationTextColor("validationLabel")};
        {Controls.GetValidationRef("validationLabel")}
            .Bind(
                Label.TextProperty,
                getter: static ({viewModelName} vm) => vm.{property.Name}Error);

        Children.Add(label);
        Children.Add(input);
        Children.Add(validationLabel);
    }}

    public Editor Input {{ get; }}
}}
";
    }
}
