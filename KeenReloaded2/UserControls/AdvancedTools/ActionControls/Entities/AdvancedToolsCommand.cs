using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace KeenReloaded2.UserControls.AdvancedTools.ActionControls.Entities
{
    public class AdvancedToolsCommand : ICommand
    {
        private readonly Action<object> _action;
        private readonly Func<object, bool> _canExecuteFunction;

        public event EventHandler CanExecuteChanged;

        public AdvancedToolsCommand(Action<object> action, Func<object, bool> canExecuteFunction = null)
        {
            _action = action ?? throw new ArgumentNullException("action");
            _canExecuteFunction = canExecuteFunction;
        }

        public bool CanExecute(object parameter)
        {
            if (_canExecuteFunction == null)
                return true;

            return _canExecuteFunction(parameter);
        }

        public void Execute(object parameter)
        {
            _action.Invoke(parameter);
        }
    }
}
