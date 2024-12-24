using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.UserControls.AdvancedTools
{
    public interface IActionControl
    {
        void PreviewAction(object parameter);

        void CommitAction(object parameter);

        void CancelAction(object parameter);

        void Undo(object parameter);

        bool ValidateControl();
    }
}
