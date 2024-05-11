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
        protected readonly bool _isManualPlacement;

        public MapMakerObject(string objectType, string imageFileName, bool isManualPlacement, params MapMakerObjectProperty[] constructorParamaters)
        {
            _objectType = objectType;
            _isManualPlacement = isManualPlacement;
            this.ImageControl = new PictureBox();
            this.ImageControl.SizeMode = PictureBoxSizeMode.AutoSize;
            this.ImageControl.Image = Image.FromFile(imageFileName);
            this.ImageControl.ImageLocation = imageFileName;
            this.ConstructorParameters = constructorParamaters ?? new MapMakerObjectProperty[0];
        }

        public MapMakerObjectProperty[] ConstructorParameters { get; protected set; }

        public virtual object Construct()
        {
            var type = Type.GetType(_objectType);
            var values = this.ConstructorParameters.Select(p => p.Value).ToArray();
            var obj = Activator.CreateInstance(type, values);
            return obj;
        }

        public string ObjectType
        {
            get
            {
                return _objectType;
            }
        }

        public bool IsManualPlacement
        {
            get
            {
                return _isManualPlacement;
            }
        }

        public PictureBox ImageControl { get; protected set; }
    }
}
