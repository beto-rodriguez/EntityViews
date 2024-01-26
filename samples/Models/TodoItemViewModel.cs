using System.Windows.Input;

namespace Models;

public partial class TodoItemViewModel
{
    public TodoItemViewModel()
    {
        SaveCommand = new Command(() =>
        {
            // the IsValid() method will notify the UI
            // to update if there are errors to show to the user.

            if (IsValid())
            {
                // save here!
            }
        });
    }

    public ICommand SaveCommand { get; }
}
