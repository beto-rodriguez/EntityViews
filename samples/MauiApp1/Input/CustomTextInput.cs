using System.ComponentModel.DataAnnotations;
using EntityViews.Attributes;
using EntityViews.Input;
using EntityViews.Validation;
using InputKit.Shared.Abstraction;
using InputKit.Shared.Validations;
using UraniumUI.Material.Controls;

namespace MauiApp1.Input;

//[EntityViewsControl(InputTypes.Text)]
public class CustomTextInput : TextField, IEntityViewsInput<Entry>
{
    public Entry? Input => null;

    public Label? Label => null;

    public Label? ValidationLabel => null;

    public void ClearValidationError()
    {
        throw new NotImplementedException();
    }

    public void DisplayValidationError(string message)
    {
        throw new NotImplementedException();
    }

    public void Initialized(string propertyName, string? displayName)
    {
        Title = displayName;
        SetBinding(TextProperty, new Binding(propertyName));

        BindingContextChanged += (_, _) =>
        {
            var validable = (IValidatable)this;
            var validableVm = (ValidableViewModel)BindingContext;

            validable.Validations.Add(new ValidableViewModelValidator(propertyName, validableVm));

            validableVm.Validating += (model, args) =>
            {
                if (args.PropertyName != null) return;

                // when the whole model is being validated, we need to display the validation
                validable.DisplayValidation();
            };
        };
    }
}

public class ValidableViewModelValidator(string propertyName, ValidableViewModel vm) : IValidation
{
    public string Message { get; protected set; } = string.Empty;

    public bool Validate(object value)
    {
        Message = string.Empty;

        var context = new ValidationContext(vm) { MemberName = propertyName };
        var results = new List<ValidationResult>();

        _ = Validator.TryValidateProperty(vm.PropertyValues[propertyName](), context, results);

        var isValid = true;

        foreach (var result in results)
            foreach (var member in result.MemberNames)
            {
                isValid = false;
                Message = result.ErrorMessage ?? string.Empty;
            }

        return isValid;
    }
}
