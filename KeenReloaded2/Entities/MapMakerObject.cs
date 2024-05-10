using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KeenReloaded2.Entities
{
    public class MapMakerObject
    {
        protected readonly string _objectType;

        public MapMakerObject(string objectType, string imageFileName, params MapMakerObjectProperty[] constructorParamaters)
        {
            _objectType = objectType;
            this.ImageControl = new PictureBox();
            this.ImageControl.SizeMode = PictureBoxSizeMode.AutoSize;
            this.ImageControl.Image = Image.FromFile(imageFileName);
            this.ConstructorParameters = constructorParamaters ?? new MapMakerObjectProperty[0];
        }

        protected MapMakerObjectProperty[] ConstructorParameters { get; set; }

        public virtual object Construct()
        {
            var type = Type.GetType(_objectType);
            var values = this.ConstructorParameters.Select(p => p.Value).ToArray();
            var obj = Activator.CreateInstance(type, values);
            return obj;
        }

        public PictureBox ImageControl { get; protected set; }
    }
}
