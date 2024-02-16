using CommunityToolkit.Mvvm.Input;
using EntityViews.Attributes;
using Models;

namespace MauiApp1.ViewModels;

[ViewModel(typeof(ToDoModel), Form = FormKind.MauiMarkup, Ignore = [nameof(ToDoModel.Id)])]
[FormInput(PropertyName = nameof(ToDoModel.Description), InputType = InputTypes.TextArea)]
[FormInput(PropertyName = nameof(ToDoModel.Slider), InputType = InputTypes.Slider)]
[FormInput(PropertyName = nameof(ToDoModel.Stepper), InputType = InputTypes.Stepper)]
[FormInput(PropertyName = nameof(ToDoModel.Switch), InputType = InputTypes.Switch)]
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
    }
}
