using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MicroCBuilder
{
    public class Command<T> : ICommand
    {
        protected Action<T> Action;
        public Command(Action<T> action)
        {
            Action = action;
        }
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            Action.Invoke((T)parameter);
        }
    }

    public class Command : Command<object>
    {
        public Command(Action<object> action) : base(action)
        {

        }
    }
}
