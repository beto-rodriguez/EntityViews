using CommunityToolkit.Mvvm.Input;
using EntityViews.Attributes;
using Models;

namespace MauiApp1.ViewModels;

[ViewModel(BaseType = typeof(ToDoModel), Form = FormKind.MauiMarkup)]
public partial class ToDoViewModel
{
    [RelayCommand]
    public void Save()
    {
        if (!IsValid()) return;

        //Save the ToDo here...
        // Todo.Save();
    }
}
