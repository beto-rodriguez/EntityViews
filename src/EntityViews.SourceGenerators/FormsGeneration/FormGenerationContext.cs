using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static EntityViews.SourceGenerators.SyntaxNodeHelper;

namespace EntityViews.SourceGenerators.FormsGeneration;

public class FormGenerationContext(
    Compilation c,
    SourceProductionContext spc,
    ClassDeclarationSyntax cds,
    ViewModelAnalysis vma,
    IEnumerable<IPropertySymbol> properties)
{
    public Compilation Compilation { get; } = c;
    public SourceProductionContext SourceProductionContext { get; } = spc;
    public ClassDeclarationSyntax ClassDeclaration { get; } = cds;
    public ViewModelAnalysis ViewModelAnalysis { get; } = vma;
    public IEnumerable<IPropertySymbol> TargetProperties { get; } = properties;
}
