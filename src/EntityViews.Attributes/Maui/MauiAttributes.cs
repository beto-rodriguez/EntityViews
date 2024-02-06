namespace EntityViews.Attributes.Maui;

/// <summary>
/// The property will use a Maui Editor control (instead of an Entry that is the default for string type).
/// </summary>
public class MauiEditorInputAttribute : Attribute
{ }

/// <summary>
/// The property will use a Maui Switch control (instead of a CheckBox that is the default for bool type.).
/// </summary>
public class MauiSwitchInputAttribute : Attribute
{ }

/// <summary>
/// The property will use a Maui Stepper control (instead of an Entry with numeric validation that is the default for numeric types).
/// </summary>
public class MauiStepperInputAttribute : Attribute
{ }

/// <summary>
/// The property will use a Maui Slider control (instead of an Entry with numeric validation that is the default for numeric types).
/// </summary>
public class MauiSliderInputAttribute : Attribute
{ }
