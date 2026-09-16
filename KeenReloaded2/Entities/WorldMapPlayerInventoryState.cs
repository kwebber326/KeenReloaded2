using KeenReloaded2.Constants;
using KeenReloaded2.Framework.GameEntities.Items;
using KeenReloaded2.Framework.GameEntities.Weapons;
using KeenReloaded2.Framework.GameEntities.WorldMapEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Entities
{
    public class WorldMapPlayerInventoryState
    {
        public long PlayerPoints { get; set; }

        public int PlayerLives { get; set; }

        public List<Gem> PlayerGems { get; set; }

        public List<NeuralStunner> PlayerWeapons { get; set; }

        public List<WorldMapItemType> WorldMapItems { get; set; }

        public bool HasKeyCard { get; set; }

        public override string ToString()
        {
            string separator = MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR;
            string elementSeparator = MapMakerConstants.MAP_MAKER_ELEMENT_SEPARATOR;
            string arrayStart = MapMakerConstants.MAP_MAKER_ARRAY_START;
            string arrayEnd = MapMakerConstants.MAP_MAKER_ARRAY_END;

            StringBuilder stringBuilder = new StringBuilder();
            //points
            stringBuilder.Append(PlayerPoints + separator);
            //lives
            stringBuilder.Append(PlayerLives + separator);
            //key card status
            stringBuilder.Append(HasKeyCard + separator);
            //gems
            var gemColors = this.PlayerGems.Select(g =>  g.Color).ToArray();
            var gems = string.Join(elementSeparator, gemColors);
            stringBuilder.Append(arrayStart + gems + arrayEnd);
            //weapons
            var weapons = this.PlayerWeapons.Select(w =>
                w.GetType().Name + "_" + w.Ammo).ToArray();
            var weaponStr = string.Join(elementSeparator, weapons);
            stringBuilder.Append(arrayStart + weaponStr + arrayEnd);
            //world map Items
            var items = string.Join(elementSeparator, this.WorldMapItems);
            stringBuilder.Append(arrayStart + items + arrayEnd);

            return stringBuilder.ToString();
        }
    }
}
