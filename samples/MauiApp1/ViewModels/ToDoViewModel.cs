using CommunityToolkit.Mvvm.Input;
using EntityViews.Attributes;
using Models;

namespace MauiApp1.ViewModels;

[ViewModel(BaseType = typeof(ToDoModel), Form = FormKind.MauiMarkup, Ignore = [nameof(ToDoModel.Id)])]
[FormInput(PropertyName = nameof(ToDoModel.Description), InputType = InputTypes.TextArea)]

public partial class ToDoViewModel
{
    [RelayCommand]
    public void Save()
    {
        if (!IsValid())
        {
            // access the errors here
            var currentErrors = ValidationErrors;

            // Do something with the errors...

            return;
        }

        //Save the Todo here...
        // Todo.Save();
    }
}

//[EntityViewsControl(InputType = InputTypes.Text)]
public abstract class EntityViewsInput<T> : VerticalStackLayout, EntityViews.Input.IEntityViewsInput<T>
    where T : View, new()
{
    public EntityViewsInput(string inputType)
    {
        InputType = inputType ?? InputTypes.Default;

        var label = new Label();
        var input = new T();
        var validationLabel = new Label();

        Children.Add(label);
        Children.Add(input);
        Children.Add(validationLabel);

        Label = label;
        Input = input;
        ValidationLabel = validationLabel;
    }

    public string InputType { get; }

    public Label Label { get; }

    public T Input { get; }

    public Label ValidationLabel { get; }
}

public class EntityViewsTextInput : EntityViewsInput<Entry>
{
    public EntityViewsTextInput() : base(InputTypes.Text) { }
}

public class EntityViewsTextAreaInput : EntityViewsInput<Editor>
{
    public EntityViewsTextAreaInput() : base(InputTypes.TextArea) { }
}

public class EntityViewsNumberInput : EntityViewsInput<Entry>
{
    public EntityViewsNumberInput() : base(InputTypes.Number) { }
}

public class EntityViewsSwitchInput : EntityViewsInput<Switch>
{
    public EntityViewsSwitchInput() : base(InputTypes.Switch) { }
}

public class EntityViewsCheckBoxInput : EntityViewsInput<CheckBox>
{
    public EntityViewsCheckBoxInput() : base(InputTypes.Checkbox) { }
}

public class EntityViewsDatePickerInput : EntityViewsInput<DatePicker>
{
    public EntityViewsDatePickerInput() : base(InputTypes.DateTime) { }
}

public class EntityViewsTimePickerInput : EntityViewsInput<TimePicker>
{
    public EntityViewsTimePickerInput() : base(InputTypes.TimeSpan) { }
}

public class EntityViewsSliderInput : EntityViewsInput<Slider>
{
    public EntityViewsSliderInput() : base(InputTypes.Slider) { }
}

public class EntityViewsStepperInput : EntityViewsInput<Stepper>
{
    public EntityViewsStepperInput() : base(InputTypes.Stepper) { }
}
