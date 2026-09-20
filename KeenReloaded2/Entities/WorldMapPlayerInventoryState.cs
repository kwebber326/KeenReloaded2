using KeenReloaded2.Constants;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEntities.Items;
using KeenReloaded2.Framework.GameEntities.Weapons;
using KeenReloaded2.Framework.GameEntities.WorldMapEntities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

        public static WorldMapPlayerInventoryState Default
        {
            get
            {
                return new WorldMapPlayerInventoryState()
                {
                    PlayerLives = 3,
                    PlayerWeapons = new List<NeuralStunner>()
                    {
                        new NeuralStunner(null, new System.Drawing.Rectangle())
                    }
                };
            }
        }

        public static WorldMapPlayerInventoryState FromString(string data)
        {
            char separator = MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR[0];
            char elementSeparator = MapMakerConstants.MAP_MAKER_ELEMENT_SEPARATOR[0];
            string arrayStart = MapMakerConstants.MAP_MAKER_ARRAY_START;
            string arrayEnd = MapMakerConstants.MAP_MAKER_ARRAY_END;

            string[] dataElements = data.Split(separator);
            WorldMapPlayerInventoryState state = new WorldMapPlayerInventoryState();
            state.PlayerPoints = Convert.ToInt32(dataElements[0]);
            state.PlayerLives = Convert.ToInt32(dataElements[1]);
            state.HasKeyCard = Convert.ToBoolean(dataElements[2]);

            //gems
            string[] gemData = dataElements[3].Replace(arrayStart, "").Replace(arrayEnd, "")
                .Split(elementSeparator);

            if (gemData.Length > 0 && !string.IsNullOrWhiteSpace(gemData[0]))
            {
                state.PlayerGems = gemData.Select(g =>
                {
                    GemColor color = (GemColor)Enum.Parse(typeof(GemColor), g);
                    Gem gem = new Gem(new Rectangle(), null, null, color, 20);
                    return gem;
                }).ToList();
            }

            //weapons
            string[] weaponData = dataElements[4].Replace(arrayStart, "").Replace(arrayEnd, "")
                .Split(elementSeparator);

            if (weaponData.Length > 0 && !string.IsNullOrWhiteSpace(weaponData[0]))
            {
                state.PlayerWeapons = weaponData.Select(g =>
                {
                    string[] wData = g.Split('_');

                    string typeName = $"KeenReloaded2.Framework.GameEntities.Weapons.{wData[0]}, KeenReloaded2.Framework, Version = 1.0.0.0, Culture = neutral, PublicKeyToken = null";
                    Type type = Type.GetType(typeName);
                    int ammo = Convert.ToInt32(wData[1]);
                    return (NeuralStunner)Activator.CreateInstance(type, null, new Rectangle(), ammo);
                }).ToList();
            }

            //items
            string[] itemData = dataElements[5].Replace(arrayStart, "").Replace(arrayEnd, "")
                .Split(elementSeparator);

            if (itemData.Length > 0 && !string.IsNullOrWhiteSpace(itemData[0]))
            {
                state.WorldMapItems = itemData.Select(g =>
                    (WorldMapItemType)Enum.Parse(typeof(WorldMapItemType), g)).ToList();
            }

            return state;
        }

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
            var gemColors = this.PlayerGems.Select(g => g.Color).ToArray();
            var gems = string.Join(elementSeparator, gemColors);
            stringBuilder.Append(arrayStart + gems + arrayEnd + separator);
            //weapons
            var weapons = this.PlayerWeapons.Select(w =>
                w.GetType().Name + "_" + w.Ammo).ToArray();
            var weaponStr = string.Join(elementSeparator, weapons);
            stringBuilder.Append(arrayStart + weaponStr + arrayEnd + separator);
            //world map Items
            var items = string.Join(elementSeparator, this.WorldMapItems);
            stringBuilder.Append(arrayStart + items + arrayEnd);

            return stringBuilder.ToString();
        }
    }
}
