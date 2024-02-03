﻿using System.ComponentModel.DataAnnotations;
using EntityViews.SourceGenerators.Templates.Input;
using Microsoft.CodeAnalysis;

namespace EntityViews.SourceGenerators.Templates;

public class MauiFormPropertyTemplate
{
    public static string Build(
        Compilation compilation,
        string viewModelName,
        string viewModelNamespace,
        string formNamespace,
        IPropertySymbol property)
    {
        var attributes = property.GetAttributes();

        var displayAttribute = attributes
            .FirstOrDefault(x => x.AttributeClass?.ToDisplayString() == SyntaxNodeHelper.s_displayAnnotation);

        string propertyDisplaySource;

        if (displayAttribute is null)
        {
            // if the display attribute is not present, we use the property name.
            propertyDisplaySource = @$"""{property.Name}""";
        }
        else
        {
            var name = displayAttribute.NamedArguments
                .FirstOrDefault(x => x.Key == nameof(DisplayAttribute.Name)).Value.Value;

            var resourceType = displayAttribute.NamedArguments
                .FirstOrDefault(x => x.Key == nameof(DisplayAttribute.ResourceType)).Value.Value;

            if (resourceType is null)
            {
                // if the ResourceType is null, we use a string literal.
                propertyDisplaySource = @$"""{(name is null ? null : (string)name) ?? property.Name}""";
            }
            else
            {
                // otherwise, we get it from the resource manager.
                var namedTypeSymbol = (INamedTypeSymbol)resourceType;
                propertyDisplaySource = @$"{namedTypeSymbol.ToDisplayString()}.ResourceManager.GetString(""{name}"")";
            }
        }

        _ = s_default_typeInput.TryGetValue(property.Type.Name.ToLower(), out var defaultInputKey);

        var mauiAttribute = attributes
            .FirstOrDefault(x => s_maui_attributes.ContainsKey(x.AttributeClass?.ToDisplayString() ?? "?"));

        if (mauiAttribute is not null)
            defaultInputKey = s_maui_attributes[mauiAttribute.AttributeClass!.ToDisplayString()];

        if (!s_inputs.TryGetValue(defaultInputKey, out var inputTemplate))
        {
            return @$"// <auto-generated/>
// Unable to find a property template.

// Key:         {defaultInputKey}
// Property:    {property.Name}
// Type:        {property.Type.Name}
// Attribute:   {mauiAttribute?.AttributeClass?.ToDisplayString() ?? "null"}";
        }

        var templateParams = new InputTemplateParams(
            viewModelName, viewModelNamespace, formNamespace, propertyDisplaySource, property);

        return inputTemplate(templateParams);
    }

    private static readonly Dictionary<string, Func<InputTemplateParams, string>> s_inputs = new()
    {
        ["text"] = MauiEntryPropertyTemplate.Build,
        ["editor"] = MauiEditorPropertyTemplate.Build,
        ["number"] = MauiNumericEntryPropertyTemplate.Build,
        ["switch"] = MauiSwitchPropertyTemplate.Build,
        ["checkbox"] = MauiCheckboxPropertyTemplate.Build,
    };

    private static readonly Dictionary<string, string> s_default_typeInput = new()
    {
        ["string"] = "text",
        ["short"] = "number",
        ["int16"] = "number",
        ["int"] = "number",
        ["int32"] = "number",
        ["long"] = "number",
        ["int64"] = "number",
        ["float"] = "number",
        ["single"] = "number",
        ["double"] = "number",
        ["decimal"] = "number",
        ["boolean"] = "switch",
    };

    private static readonly Dictionary<string, string> s_maui_attributes = new()
    {
        ["EntityViews.Attributes.Maui.MauiCheckboxInputAttribute"] = "checkbox",
        ["EntityViews.Attributes.Maui.MauiEditorInputAttribute"] = "editor",
        ["EntityViews.Attributes.Maui.MauiEntryInputAttribute"] = "text",
        ["EntityViews.Attributes.Maui.MauiSwitchInputAttribute"] = "switch",
    };
}
