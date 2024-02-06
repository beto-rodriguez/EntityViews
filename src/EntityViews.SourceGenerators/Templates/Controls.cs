namespace EntityViews.SourceGenerators.Templates;

public static class Controls
{
    public static string OnErrorBackgroundColor = "Color.FromRgba(255, 0, 0, 50)";
    public static string OnErrorTextColor = "Color.FromRgba(255, 0, 0, 255)";

    public static Control? Display;
    public static string GetDisplayClassName()
    {
        return Display?.ClassName ?? "Label";
    }
    public static string GetDisplayRef(string defaultRefName)
    {
        return Display?.TargetProperty is null
            ? defaultRefName
            : $"{defaultRefName}.{Display.TargetProperty}";
    }

    public static Control? TextInput;
    public static string GetTextInputClassName()
    {
        return TextInput?.ClassName ?? "Entry";
    }
    public static string GetTextInputRef(string defaultRefName)
    {
        return TextInput?.TargetProperty is null
            ? defaultRefName
            : $"{defaultRefName}.{TextInput.TargetProperty}";
    }

    public static Control? TextAreaInput;
    public static string GetTextAreaInputClassName()
    {
        return TextAreaInput?.ClassName ?? "Editor";
    }
    public static string GetTextAreaInputRef(string defaultRefName)
    {
        return TextAreaInput?.TargetProperty is null
            ? defaultRefName
            : $"{defaultRefName}.{TextAreaInput.TargetProperty}";
    }

    public static Control? CheckboxInput;
    public static string GetCheckboxInputClassName()
    {
        return CheckboxInput?.ClassName ?? "CheckBox";
    }
    public static string GetCheckboxInputRef(string defaultRefName)
    {
        return CheckboxInput?.TargetProperty is null
            ? defaultRefName
            : $"{defaultRefName}.{CheckboxInput.TargetProperty}";
    }

    public static Control? SwitchInput;
    public static string GetSwitchInputClassName()
    {
        return SwitchInput?.ClassName ?? "Switch";
    }
    public static string GetSwitchInputRef(string defaultRefName)
    {
        return SwitchInput?.TargetProperty is null
            ? defaultRefName
            : $"{defaultRefName}.{SwitchInput.TargetProperty}";
    }

    public static Control? Validation;
    public static string GetValidationClassName()
    {
        return Validation?.ClassName ?? "Label";
    }
    public static string GetValidationRef(string defaultRefName)
    {
        return Validation?.TargetProperty is null
            ? defaultRefName
            : $"{defaultRefName}.{Validation.TargetProperty}";
    }
    public static string SetValidationTextColor(string defaultRefName)
    {
        // only when the validation was not defined by the user
        // we set the text color to the error color
        return Validation is null
            ? $"\r\n{GetValidationRef(defaultRefName)}.TextColor = {OnErrorTextColor};"
            : string.Empty;
    }

    public class Control(string className, string? propertyName)
    {
        public string ClassName { get; } = className;
        public string? TargetProperty { get; } = propertyName;
    }

    public static void Clear()
    {
        Display = null;
        TextInput = null;
        Validation = null;
    }
}
