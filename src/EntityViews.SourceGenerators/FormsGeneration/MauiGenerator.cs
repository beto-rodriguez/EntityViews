using EntityViews.SourceGenerators.Templates;
using EntityViews.SourceGenerators.Templates.Maui;
using Microsoft.CodeAnalysis;

namespace EntityViews.SourceGenerators.FormsGeneration;

public class MauiGenerator : FormGenerator
{
    public static bool UseCommunityToolkitBindings { get; private set; }

    public override void Generate(FormGenerationContext context)
    {
        var classDeclaration = context.ClassDeclaration;

        var rootNamespace =
            context.Compilation.Assembly.GlobalNamespace.GetNamespaceMembers().FirstOrDefault()?.Name ?? string.Empty;

        foreach (var property in context.TargetProperties)
            context.SourceProductionContext.AddSource(
                $"{classDeclaration.Identifier}.{property.Name}.Input.g.cs",
                MauiFormPropertyTemplate.Build(
                    context.Compilation,
                    classDeclaration.Identifier.ToString(),
                    classDeclaration.GetNameSpace() ?? string.Empty,
                    $"{rootNamespace}.EntityForms.{context.ViewModelAnalysis.ViewModelOf.Name}",
                    property,
                    context.ViewModelAnalysis));
    }

    public override void Initialize(Compilation compilation, SourceProductionContext context)
    {
        UseCommunityToolkitBindings = compilation.ReferencedAssemblyNames
            .Any(x => x.Name == "CommunityToolkit.Maui.Markup");

        context.AddSource("EntityViews.Input.g.cs", _MauiDefaultInputs.Build());
    }

    public static string GetUsingStatements(InputTemplateParams templateParams)
    {
        return UseCommunityToolkitBindings
            ? $"using CommunityToolkit.Maui.Markup;\r\nusing {templateParams.ViewModelNamespace};\r\nusing EntityViews.Validation;"
            : $"using {templateParams.ViewModelNamespace};\r\nusing EntityViews.Validation;";
    }

    public static string GetBindingExpression(
        string bindableProperty, InputTemplateParams templateParams)
    {
        return UseCommunityToolkitBindings
            ? GetCTK(bindableProperty, templateParams)
            : GetBinding(bindableProperty, templateParams);
    }

    private static string GetCTK(string bindableProperty, InputTemplateParams templateParams)
    {
        var viewModelName = templateParams.ViewModelName;
        var propertyName = templateParams.Property.Name;
        var propertyType = templateParams.Property.Type.Name;

        return @$"
            .Bind(
                {bindableProperty},
                getter: static ({viewModelName} vm) => vm.{propertyName},
                setter: static ({viewModelName} vm, {propertyType} value) => vm.{propertyName} = value);";
    }

    private static string GetBinding(string bindableProperty, InputTemplateParams templateParams)
    {
        var propertyName = templateParams.Property.Name;

        return @$"
            .SetBinding(
                {bindableProperty},
                new Binding(""{propertyName}""));";
    }
}
