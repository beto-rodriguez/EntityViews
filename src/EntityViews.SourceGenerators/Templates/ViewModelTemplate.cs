﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace EntityViews.SourceGenerators.Templates;

public class ViewModelTemplate
{
    public static string Build(
        Compilation compilation, ClassDeclarationSyntax classDeclaration, IEnumerable<IPropertySymbol> properties)
    {
        return $@"// <auto-generated/>
#nullable enable

using System.Runtime.CompilerServices;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace {classDeclaration.GetNameSpace()};

public partial class {classDeclaration.Identifier} : EntityViews.Validation.ValidableViewModel, INotifyPropertyChanged
{{
    public {classDeclaration.Identifier}()
    {{
        PropertyValues = new()
        {{
{properties.Aggregate(string.Empty, (currentString, property) => currentString + @$"            {{ ""{property.Name}"", () => {property.Name} }},
")}
        }};
    }}

{properties.Aggregate(string.Empty, (currentString, property) => currentString + property.AsValidableProperty())}

}}
";
    }
}
