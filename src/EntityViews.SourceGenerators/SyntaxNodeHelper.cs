using EntityViews.Attributes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.ComponentModel.DataAnnotations;

namespace EntityViews.SourceGenerators;

internal static class SyntaxNodeHelper
{
    private static readonly HashSet<string> s_dataAnnotation =
    [
        nameof(CreditCardAttribute),
        nameof(CustomValidationAttribute),
        nameof(DataTypeAttribute),
        nameof(DisplayAttribute),
        nameof(DisplayColumnAttribute),
        nameof(DisplayFormatAttribute),
        nameof(EditableAttribute),
        nameof(EmailAddressAttribute),
        nameof(FileExtensionsAttribute),
        nameof(MaxLengthAttribute),
        nameof(MinLengthAttribute),
        nameof(PhoneAttribute),
        nameof(RegularExpressionAttribute),
        nameof(RequiredAttribute),
        nameof(RangeAttribute),
        nameof(StringLengthAttribute),
        nameof(TimestampAttribute),
        nameof(UrlAttribute),
    ];

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

    public static string AsField(this PropertyDeclarationSyntax property)
    {
        return $@"_{property.Identifier.ToString().ToLower()}";
    }

    public static AttributeAnalysis Analyze(this PropertyDeclarationSyntax property)
    {
        var analyzer = new AttributeAnalysis();

        var ignorable = nameof(IgnoreViewModelProperty);

        foreach (var attributeList in property.AttributeLists)
        {
            foreach (var attribute in attributeList.Attributes)
            {
                if (attribute.Name.GetText().ToString() == ignorable) analyzer.Ignore = true;
                if (IsDataAnnotation(attribute)) analyzer.DataAnnotations.Add(attribute);
            }
        }

        return analyzer;
    }

    public static bool IsDataAnnotation(this AttributeSyntax attribute)
    {
        var name = attribute.Name.GetText().ToString();
        if (!name.EndsWith("Attribute")) name += "Attribute";
        return s_dataAnnotation.Contains(name);
    }

    public class AttributeAnalysis
    {
        public bool Ignore { get; set; }

        public List<AttributeSyntax> DataAnnotations { get; set; } = [];
    }
}
