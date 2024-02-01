using System.ComponentModel.DataAnnotations;
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

        _ = s_default_typeInput.TryGetValue(property.Type.Name, out var defaultInputKey);

        var inputParams = new InputParams(
            viewModelName, viewModelNamespace, formNamespace, propertyDisplaySource, property);

        var inputResult = s_inputs[defaultInputKey](inputParams);

        return inputResult;
    }

    private static readonly Dictionary<string, Func<InputParams, string>> s_inputs = new()
    {
        ["text"] = MauiEntryPropertyTemplate.Build,
        ["number"] = MauiNumericEntryPropertyTemplate.Build,
    };

    private static readonly Dictionary<string, string> s_default_typeInput = new()
    {
        ["string"] = "text",
        ["String"] = "text",
        ["short"] = "number",
        ["Int16"] = "number",
        ["int"] = "number",
        ["Int32"] = "number",
        ["long"] = "number",
        ["Int64"] = "number",
        ["float"] = "number",
        ["Single"] = "number",
        ["double"] = "number",
        ["Double"] = "number",
        ["decimal"] = "number",
        ["Decimal"] = "number",
    };
}
