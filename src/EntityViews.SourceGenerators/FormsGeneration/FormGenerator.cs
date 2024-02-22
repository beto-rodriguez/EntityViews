using Microsoft.CodeAnalysis;

namespace EntityViews.SourceGenerators.FormsGeneration;

public abstract class FormGenerator
{
    public abstract void Initialize(Compilation compilation, SourceProductionContext context);

    public abstract void Generate(FormGenerationContext context);
}
