﻿using CommunityToolkit.Mvvm.Input;
using EntityViews.Attributes;
using Models;

namespace MauiApp1.ViewModels;

[ViewModel(BaseType = typeof(ToDoModel), Form = FormKind.MauiMarkup)]
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
