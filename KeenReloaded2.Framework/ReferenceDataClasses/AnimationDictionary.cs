using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.ReferenceDataClasses
{
    public static class AnimationDictionary
    {
        private static Dictionary<string, Image[]> _animationDictionary;
        public static Dictionary<string, Image[]> Animations
        {
            get
            {
                if (_animationDictionary == null)
                {
                    _animationDictionary = new Dictionary<string, Image[]>();
                }
                return _animationDictionary;
            }
        }
    }
}
