namespace EntityViews.Attributes;

/// <summary>
/// Defines the forms generation mode.
/// </summary>
public enum FormKind
{
    /// <summary>
    /// No forms will be generated.
    /// </summary>
    None,

    /// <summary>
    /// Uses CommunityToolkit.Maui.Markup compiled bindings to generate the forms,
    /// CommunityToolkit.Maui.Markup must be installed manually.
    /// </summary>
    MauiMarkup
}
