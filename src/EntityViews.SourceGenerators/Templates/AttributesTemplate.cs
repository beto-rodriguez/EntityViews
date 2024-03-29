﻿namespace EntityViews.SourceGenerators.Templates;

public class AttributesTemplate
{
    public static string Build()
    {
        return @"// <auto-generated />
#nullable enable

namespace EntityViews.Attributes;

/// <summary>
/// Creates a ViewModel based on the given type.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class ViewModelAttribute(Type baseType) : Attribute
{
    /// <summary>
    /// Gets or sets the base type.
    /// </summary>
    public Type BaseType { get; set; } = baseType;

    /// <summary>
    /// Gets the ignorable properties.
    /// </summary>
    public string[] Ignore { get; set; } = [];

    /// <summary>
    /// Gets or sets the forms generation mode.
    /// </summary>
    public int Form { get; set; } = 0;
}

/// <summary>
/// Defines the input type to use for the specified property.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class FormInput() : Attribute
{
    /// <summary>
    /// Gets or sets the name of the property.
    /// </summary>
    public string PropertyName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the input type to use.
    /// </summary>
    public string InputType { get; set; } = string.Empty;
}

/// <summary>
/// Marks a property as ignorable, it will not be included in the generated ViewModel.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class IgnorePropertyAttribute : Attribute
{ }

/// <summary>
/// Indicates that the marked class is a control for EntityViews.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class EntityViewsControlAttribute(string inputType) : Attribute
{
    public string InputType { get; } = inputType;
}

/// <summary>
/// Defines the forms generation mode.
/// </summary>
public static class FormKind
{
    /// <summary>
    /// No forms will be generated.
    /// </summary>
    public const int None = 0;

    /// <summary>
    /// Generates form for Maui, if CommunityToolkit.Maui.Markup is available, compiled bindings will be used.
    /// </summary>
    public const int Maui = 1;
}

/// <summary>
/// Defines the input types to use in the forms.
/// </summary>
public static class InputTypes
{
    /// <summary>
    /// The default input type based on the property type.
    /// </summary>
    public const string Default = ""default"";
    public const string Text = ""text"";
    public const string TextArea = ""text-area"";
    public const string Number = ""number"";
    public const string Switch = ""switch"";
    public const string Checkbox = ""checkbox"";
    public const string DateTime = ""datetime"";
    public const string TimeSpan = ""timespan"";
    public const string Slider = ""slider"";
    public const string Stepper = ""stepper"";
}
";
    }
}
