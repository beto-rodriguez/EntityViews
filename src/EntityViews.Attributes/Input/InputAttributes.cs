namespace EntityViews.Attributes.Input;

/// <summary>
/// Indicates that the marked class is the display control, the display control is used to display the property name.
/// </summary>
public class DisplayControlAttribute : Attribute
{ }

/// <summary>
/// Indicates that the marked class is the text control, the text control is where the user enters the property value.
/// </summary>
public class TextControlAttribute : Attribute
{ }

/// <summary>
/// Indicates that the marked class is the validation control, the validation control is where validation errors are shown.
/// </summary>
public class ValidationControlAttribute : Attribute
{ }

/// <summary>
/// Indicates that the marked property is the actual control to use. The containing class must be marked with
/// <see cref="DisplayControlAttribute"/>, <see cref="TextControlAttribute"/> or <see cref="ValidationControlAttribute"/>,
/// then the marked property will behave as the main control of the class.
/// </summary>
public class ControlPropertyAttribute : TextControlAttribute
{ }
