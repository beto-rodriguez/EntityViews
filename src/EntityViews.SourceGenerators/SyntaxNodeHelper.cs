using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.ComponentModel.DataAnnotations;

namespace EntityViews.SourceGenerators;

public static class SyntaxNodeHelper
{
    internal static readonly string s_displayAnnotation =
        "System.ComponentModel.DataAnnotations." + nameof(DisplayAttribute);

    private static readonly HashSet<string> s_dataAnnotation =
    [
        "System.ComponentModel.DataAnnotations." + nameof(CreditCardAttribute),
        "System.ComponentModel.DataAnnotations." + nameof(CustomValidationAttribute),
        "System.ComponentModel.DataAnnotations." + nameof(DataTypeAttribute),
        //"System.ComponentModel.DataAnnotations." + nameof(DisplayAttribute), // <- ignore this one here
        "System.ComponentModel.DataAnnotations." + nameof(DisplayColumnAttribute),
        "System.ComponentModel.DataAnnotations." + nameof(DisplayFormatAttribute),
        "System.ComponentModel.DataAnnotations." + nameof(EditableAttribute),
        "System.ComponentModel.DataAnnotations." + nameof(EmailAddressAttribute),
        "System.ComponentModel.DataAnnotations." + nameof(FileExtensionsAttribute),
        "System.ComponentModel.DataAnnotations." + nameof(MaxLengthAttribute),
        "System.ComponentModel.DataAnnotations." + nameof(MinLengthAttribute),
        "System.ComponentModel.DataAnnotations." + nameof(PhoneAttribute),
        "System.ComponentModel.DataAnnotations." + nameof(RegularExpressionAttribute),
        "System.ComponentModel.DataAnnotations." + nameof(RequiredAttribute),
        "System.ComponentModel.DataAnnotations." + nameof(RangeAttribute),
        "System.ComponentModel.DataAnnotations." + nameof(StringLengthAttribute),
        "System.ComponentModel.DataAnnotations." + nameof(TimestampAttribute),
        "System.ComponentModel.DataAnnotations." + nameof(UrlAttribute),
    ];

    public class ViewModelAnalysis(
        ClassDeclarationSyntax classDeclaration,
        ITypeSymbol viewModelOf,
        HashSet<string> ignore,
        int form,
        Dictionary<string, string> inputs)
    {
        public ClassDeclarationSyntax ClassDeclaration { get; } = classDeclaration;
        public ITypeSymbol ViewModelOf { get; } = viewModelOf;
        public HashSet<string> Ignore { get; } = ignore;
        public int Form { get; } = form;
        public Dictionary<string, string> Inputs { get; } = inputs;
        public static Dictionary<string, string> BaseClasses { get; } = [];
        public static string? GetBaseClass(string key)
        {
            return BaseClasses.TryGetValue(key, out var value)
                ? value
                : null;
        }
    }

    public static ITypeSymbol? FindViewModelOfSymbol(this ViewModelAnalysis analysis, Compilation compilation)
    {
        var lookInCurrentAssembly = compilation.GetTypeByMetadataName(analysis.ViewModelOf.ToDisplayString());
        if (lookInCurrentAssembly is not null) return lookInCurrentAssembly;

        // if we can't find it in the current assembly, we'll try to find it in the referenced assemblies.
        // NOTE: is this even necessary?
        var assemblyName = analysis.ViewModelOf.ContainingAssembly.ToDisplayString();

        var assemblySymbol = compilation.SourceModule
            .ReferencedAssemblySymbols
            .FirstOrDefault(x => x.ToDisplayString() == assemblyName);

        var namespaceSymbol = assemblySymbol?.GlobalNamespace.GetNamespaceMembers()
            .FirstOrDefault(m => m.Name == analysis.ViewModelOf.ContainingNamespace.Name);

        return namespaceSymbol?.FindTypes().FirstOrDefault(x => x.Name == analysis.ViewModelOf.Name);
    }

    private static IEnumerable<ITypeSymbol> FindTypes(this INamespaceSymbol root)
    {
        foreach (var namespaceOrTypeSymbol in root.GetMembers())
        {
            if (namespaceOrTypeSymbol is INamespaceSymbol @namespace)
                foreach (var nested in FindTypes(@namespace))
                    yield return nested;

            else if (namespaceOrTypeSymbol is ITypeSymbol type)
                yield return type;
        }
    }

    public static string? GetNameSpace(this SyntaxNode syntaxNode)
    {
        foreach (var ancestor in syntaxNode.Ancestors())
        {
            if (ancestor is FileScopedNamespaceDeclarationSyntax fsns)
                return fsns.Name.ToString();

            if (ancestor is NamespaceDeclarationSyntax ns)
                return ns.Name.ToString();
        }

        return null;
    }

    public static string? GetControlProperty(this INamedTypeSymbol symbol)
    {
        const string control = "EntityViews.Attributes.Input.ControlPropertyAttribute";

        return symbol.GetMembers()
            .Where(m => m.Kind == SymbolKind.Property)
            .Cast<IPropertySymbol>()
            .FirstOrDefault(p =>
                p.GetAttributes()
                    .Any(x => x.AttributeClass?.ToDisplayString() == control))?
            .Name;
    }

    public static string GetPropertyDisplaySource(this IPropertySymbol property)
    {
        string propertyDisplaySource;

        var displayAttribute = property
            .GetAttributes()
            .FirstOrDefault(x => x.AttributeClass?.ToDisplayString() == s_displayAnnotation);

        if (displayAttribute is null)
        {
            // if the display attribute is not present, we use the property name.
            propertyDisplaySource = @$"""{property.Name}""";
        }
        else
        {
            var name = displayAttribute.NamedArguments
                .FirstOrDefault(x => x.Key == nameof(DisplayAttribute.Name)).Value.Value;

            var resourceType = displayAttribute.NamedArguments
                .FirstOrDefault(x => x.Key == nameof(DisplayAttribute.ResourceType)).Value.Value;

            if (resourceType is null)
            {
                // if the ResourceType is null, we use a string literal.
                propertyDisplaySource = @$"""{(name is null ? null : (string)name) ?? property.Name}""";
            }
            else
            {
                // otherwise, we get it from the resource manager.
                var namedTypeSymbol = (INamedTypeSymbol)resourceType;
                propertyDisplaySource = @$"{namedTypeSymbol.ToDisplayString()}.ResourceManager.GetString(""{name}"")";
            }
        }

        return propertyDisplaySource;
    }

    public static string AsValidableProperty(this IPropertySymbol property)
    {
        var annotations = property
            .GetAttributes()
            .Where(x => s_dataAnnotation.Contains(x.AttributeClass?.ToDisplayString() ?? "?"))
            .Aggregate(string.Empty, (currentString, annotation) => currentString +
                (currentString.Length > 0 ? "\r\n" : "") +
                $"    [{annotation}]");

        var initialValue = string.Empty;
        if (property.Type.ToString() == "string") initialValue = " = string.Empty"; // special case for strings.

        var propertyField = $"_{property.Name.ToLower()}";

        return
@$"
    private {property.Type} {propertyField}{initialValue};{(annotations.Length > 0 ? "\r\n" + annotations : string.Empty)}
    public {property.Type} {property.Name} {{ get => {propertyField}; set => SetProperty(ref {propertyField}, value, nameof({property.Name})); }}
    public string {property.Name}Error => GetError(nameof({property.Name}));
    public bool {property.Name}HasError => {property.Name}Error.Length > 0;
";
    }
}
