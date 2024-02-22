﻿using EntityViews.SourceGenerators.FormsGeneration;

namespace EntityViews.SourceGenerators.Templates.Maui;

public class MauiCheckboxPropertyTemplate
{
    public static string Build(InputTemplateParams p)
    {
        return @$"// <auto-generated/>
#nullable enable

{MauiGenerator.GetUsingStatements(p)}

namespace {p.FormNamespace};

public class {p.Property.Name}Input : {p.BaseControlClassName ?? "CheckBox"}
{{
    public {p.Property.Name}Input()
    {{
        this{MauiGenerator.GetBindingExpression("CheckBox.IsCheckedProperty", p)}
    }}
}}
";
    }
}
