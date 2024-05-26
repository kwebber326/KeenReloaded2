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
        protected readonly Type _objectType;
        protected readonly bool _isManualPlacement;

        public MapMakerObject(Type objectType, string imageFilePath, bool isManualPlacement, params MapMakerObjectProperty[] constructorParamaters)
        {
            _objectType = objectType;
            _isManualPlacement = isManualPlacement;
            this.ImageControl = new PictureBox();
            this.ImageControl.SizeMode = PictureBoxSizeMode.AutoSize;
            this.ImageControl.Image = Image.FromFile(imageFilePath);
            this.ImageControl.ImageLocation = imageFilePath;
            this.ConstructorParameters = constructorParamaters ?? new MapMakerObjectProperty[0];
        }

        public MapMakerObjectProperty[] ConstructorParameters { get; protected set; }

        public MapMakerObjectProperty[] CloneParameterList()
        {
            List<MapMakerObjectProperty> clonedProperties = new List<MapMakerObjectProperty>();
            foreach (var parameter in this.ConstructorParameters)
            {
                MapMakerObjectProperty clone = new MapMakerObjectProperty()
                {
                    PropertyName = parameter.PropertyName,
                    DisplayName = parameter.DisplayName,
                    DataType = parameter.DataType,
                    Value = parameter.Value,
                    IsSpriteProperty = parameter.IsSpriteProperty,
                    PossibleValues = parameter.PossibleValues,
                    Hidden = parameter.Hidden,
                    Readonly = parameter.Readonly,
                    IsIgnoredInMapData = parameter.IsIgnoredInMapData
                };
                clonedProperties.Add(clone);
            }
            return clonedProperties.ToArray();
        }

        public virtual object Construct()
        {
            var type = _objectType;
            var values = this.ConstructorParameters.Select(p => p.Value).ToArray();
            var obj = Activator.CreateInstance(type, values);
            return obj;
        }

        public Type ObjectType
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
