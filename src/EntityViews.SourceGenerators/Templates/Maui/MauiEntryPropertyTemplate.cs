﻿namespace EntityViews.SourceGenerators.Templates.Maui;

public class MauiEntryPropertyTemplate
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

        var input = new {Controls.GetTextInputClassName()}();
        {Controls.GetTextInputRef("input")}
            .Bind(
                Entry.TextProperty,
                getter: static ({viewModelName} vm) => vm.{property.Name},
                setter: static ({viewModelName} vm, {property.Type.Name} value) => vm.{property.Name} = value);
        {Controls.GetTextInputRef("input")}.Triggers.Add(
            new DataTrigger(typeof(Entry))
            {{
                Binding = new Binding(""{property.Name}HasError""),
                Value = true,
                Setters = {{ new Setter {{ Property = BackgroundColorProperty, Value = {Controls.OnErrorBackgroundColor} }} }},
            }});
        async Task<bool> UserKeepsTyping()
        {{
            var txt = {Controls.GetTextInputRef("input")}.Text;
            await Task.Delay(500);
            return txt != {Controls.GetTextInputRef("input")}.Text;
        }}
        {Controls.GetTextInputRef("input")}.TextChanged += async (_, _) =>
        {{
            if (await UserKeepsTyping()) return;
            (({viewModelName})BindingContext).ValidateDirtyProperty(
                ""{property.Name}"", {Controls.GetTextInputRef("input")}.Text is not null && {Controls.GetTextInputRef("input")}.Text.Length > 0);
        }};
        Input = {Controls.GetTextInputRef("input")};

        var validationLabel = new {Controls.GetValidationClassName()}();{Controls.SetValidationTextColor("validationLabel")}
        {Controls.GetValidationRef("validationLabel")}
            .Bind(
                Label.TextProperty,
                getter: static ({viewModelName} vm) => vm.{property.Name}Error);

        Children.Add(label);
        Children.Add(input);
        Children.Add(validationLabel);
    }}

    public Entry Input {{ get; }}
}}
";
    }
}