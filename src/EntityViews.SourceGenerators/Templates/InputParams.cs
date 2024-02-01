using Microsoft.CodeAnalysis;

namespace EntityViews.SourceGenerators.Templates;

public struct InputParams(
    string viewModelName,
    string viewModelNamespace,
    string formNamespace,
    string propertyDisplaySource,
    IPropertySymbol property)
{
    public IPropertySymbol Property { get; set; } = property;
    public string ViewModelName { get; set; } = viewModelName;
    public string ViewModelNamespace { get; set; } = viewModelNamespace;
    public string FormNamespace { get; set; } = formNamespace;
    public string PropertyDisplaySource { get; set; } = propertyDisplaySource;
}
