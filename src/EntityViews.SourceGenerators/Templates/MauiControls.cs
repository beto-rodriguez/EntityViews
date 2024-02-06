namespace EntityViews.SourceGenerators.Templates;

public static class Controls
{
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
