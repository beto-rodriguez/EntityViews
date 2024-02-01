namespace EntityViews.Attributes;

/// <summary>
/// Defines the event arguments for the Validating event.
/// </summary>
/// <param name="propertyName"></param>
public class ValidatingEventArgs(string? propertyName)
{
    /// <summary>
    /// Gets the name of the property being validated, null if the entire view model is being validated.
    /// </summary>
    public string? PropertyName { get; } = propertyName;
}
