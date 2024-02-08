﻿namespace EntityViews.SourceGenerators.Templates.Maui;

public class MauiStepperPropertyTemplate
{
    public static string Build(InputTemplateParams p)
    {
        var property = p.Property;
        var viewModelName = p.ViewModelName;
        var viewModelNamespace = p.ViewModelNamespace;
        var formNamespace = p.FormNamespace;
        var propertyDisplaySource = p.PropertyDisplaySource;

        return @$"// <auto-generated/>
#nullable enable

using CommunityToolkit.Maui.Markup;
using {viewModelNamespace};

namespace {formNamespace};

public class {property.Name}Input : {p.BaseControlClassName ?? "EntityViews.Input.EntityViewsStepperInput"}
{{
    public {property.Name}Input()
    {{
        if (Label is not null)
            Label.Text({propertyDisplaySource});

        if (Input is not null)
            Input
                .Bind(
                    Stepper.ValueProperty,
                    getter: static ({viewModelName} vm) => vm.{property.Name},
                    setter: static ({viewModelName} vm, {property.Type.Name} value) => vm.{property.Name} = value);

        if (ValidationLabel is not null)
            ValidationLabel
                .Bind(
                    Label.TextProperty,
                    getter: static ({viewModelName} vm) => vm.{property.Name}Error);

        Initialized(""{property.Name}"", {propertyDisplaySource});
    }}
}}
";
    }
}
