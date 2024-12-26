using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using KeenReloaded2.Entities;
using KeenReloaded2.ControlEventArgs.EventStoreData;

namespace KeenReloaded2.UserControls.AdvancedTools.ActionControls.Entities
{
    public static class AdvancedToolsCommonFunctions
    {
        public static Rectangle GetExtensionArea(List<GameObjectMapping> selectedObjects)
        {
            if (selectedObjects == null || !selectedObjects.Any())
                return new Rectangle();

            int minLeft = selectedObjects.Select(o => o.GameObject.Location.X).Min();
            int maxRight = selectedObjects.Select(o => o.GameObject.Location.X + o.Image.Width).Max();
            int minTop = selectedObjects.Select(o => o.GameObject.Location.Y).Min();
            int maxBottom = selectedObjects.Select(o => o.GameObject.Location.Y + o.GameObject.Image.Height).Max();

            int width = maxRight - minLeft;
            int height = maxBottom - minTop;

            return new Rectangle(minLeft, minTop, width, height);
        }
    }
}
