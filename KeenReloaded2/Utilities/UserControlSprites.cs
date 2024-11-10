using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Utilities
{
    public static class UserControlSprites
    {
        private static Image[] _keenOptionButtonImages;

        public static Image[] KeenOptionButtonImages
        {
            get
            {
                if (_keenOptionButtonImages == null)
                {
                    _keenOptionButtonImages = new Image[]
                    {
                        Properties.Resources.keen_message_window_option_selected1,
                        Properties.Resources.keen_message_window_option_selected2
                    };
                }
                return _keenOptionButtonImages;
            }
        }
    }
}
