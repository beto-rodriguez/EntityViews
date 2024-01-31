namespace EntityViews.Attributes;

/// <summary>
/// Creates a ViewModel based on the given type.
/// </summary>
public class ViewModelAttribute() : Attribute
{
    /// <summary>
    /// Gets or sets the base type.
    /// </summary>
    public Type BaseType { get; set; } = typeof(object);

    /// <summary>
    /// Gets the ignorable properties.
    /// </summary>
    public string[] Ignore { get; set; } = [];

    /// <summary>
    /// Gets or sets the forms generation mode.
    /// </summary>
    public FormKind Form { get; set; } = FormKind.None;
}

