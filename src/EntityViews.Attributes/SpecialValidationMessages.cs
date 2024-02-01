namespace EntityViews.Attributes;

/// <summary>
/// Defines the special validation cases handled by the EntityViews library.
/// </summary>
public static class SpecialValidationMessages
{
    /// <summary>
    /// Gets or sets the message for numeric inputs.
    /// </summary>
    public static string ValidNumber { get; set; } = "'{0}' is not a valid number";
}
