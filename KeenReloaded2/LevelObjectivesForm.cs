using KeenReloaded2.Constants;
using KeenReloaded2.Entities;
using KeenReloaded2.Framework.GameEntities;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using KeenReloaded2.UserControls.MapMakerUserControls;
using KeenReloaded2.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KeenReloaded2
{
    public partial class LevelObjectivesForm : Form
    {
        private List<IActivateable> _activateables;
        private List<IWorldMapLevel> _levels;
        private Dictionary<string, List<ILevelObjective>> _mapLevelObjectives
            = new Dictionary<string, List<ILevelObjective>>();

        public LevelObjectivesForm()
        {
            InitializeComponent();
        }

        public LevelObjectivesForm(IEnumerable<IWorldMapLevel> levels,
            IEnumerable<IActivateable> activateables)
        {
            InitializeComponent();
            _activateables = activateables.ToList();
            _levels = levels.Where(l => !string.IsNullOrWhiteSpace(l.LevelName)).ToList();
            PopulateLevelList();
        }

        private void PopulateLevelList()
        {
            cmbLevelNames.Items.Clear();
            var levelNames = _levels.Select(l => l.LevelName).Distinct().ToList();
            foreach (var name in levelNames)
            {
                cmbLevelNames.Items.Add(name);
            }
        }

        private void LevelObjectivesForm_Load(object sender, EventArgs e)
        {

        }

        private void cmbLevelNames_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedLevel = cmbLevelNames.SelectedItem?.ToString();

            if (!string.IsNullOrWhiteSpace(selectedLevel))
            {
                string mapFile = MapUtility.GetWorldMapLevelPath(selectedLevel);
                List<ILevelObjective> levelObjectives = new List<ILevelObjective>();
                try
                {
                    if (!_mapLevelObjectives.ContainsKey(selectedLevel))
                    {
                        var mapData = MapUtility.LoadMapData(mapFile);
                        levelObjectives = mapData.MapData.
                            Select(g => g.GameObject).OfType<ILevelObjective>().ToList();
                        _mapLevelObjectives.Add(selectedLevel, levelObjectives);
                    }
                    else
                    {
                        levelObjectives = _mapLevelObjectives[selectedLevel];
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
                finally
                {
                    PopulateLevelObjectives(levelObjectives);
                }
            }
        }

        private void PopulateLevelObjectives(List<ILevelObjective> levelObjectives)
        {
            cmbObjectives.Items.Clear();
            foreach (var item in levelObjectives)
            {
                string key = GetKeyFromLevelObjective(item);
                cmbObjectives.Items.Add(key);
            }
        }

        private static string GetKeyFromLevelObjective(ILevelObjective item)
        {
            string name = item.GetType().Name;
            Point location = (item as ISprite)?.Location ?? new Point(0, 0);
            string key = name + "_" + location.ToString();
            return key;
        }

        private Image GetImageFromLevelObjectiveKey(string key)
        {
            string selectedLevel = cmbLevelNames.SelectedItem?.ToString();
            var objective = _mapLevelObjectives.TryGetValue(selectedLevel, out List<ILevelObjective> levelObjectives)
                ? levelObjectives.FirstOrDefault(l =>
                   key == GetKeyFromLevelObjective(l))
                : null;

            if (objective == null)
                return null;

            ISprite sprite = objective as ISprite;
            if (sprite == null) return null;
            return sprite.Image;
        }

        private void cmbObjectives_SelectedIndexChanged(object sender, EventArgs e)
        {
            string key = cmbObjectives.SelectedItem?.ToString();
            pbObjectiveImage.Image = GetImageFromLevelObjectiveKey(key);
            pnlManageObjective.Visible = cmbObjectives.SelectedIndex >= 0;
        }

        private void btnActivation_Click(object sender, EventArgs e)
        {
            var selectedItem = cmbObjectives.SelectedItem?.ToString();
            var selectedLevel = cmbLevelNames.SelectedItem?.ToString();
            if (selectedItem == null || selectedLevel == null ||
                !_mapLevelObjectives.TryGetValue(selectedLevel, out var levelObjectives)) return;

            var activator = levelObjectives.FirstOrDefault(l => GetKeyFromLevelObjective(l)
             == selectedItem) as IActivator;

            if (activator == null || !(activator is IInteractiveLevelObjective)) return;

            EditActivatorForm editActivatorForm = new EditActivatorForm(
                activator.ToggleObjects, _activateables);

            var result = editActivatorForm.ShowDialog();
            if (result == DialogResult.OK)
            {
                try
                {
                    var levelObjective = (IInteractiveLevelObjective)activator;
                    levelObjective.UpdateSelf(editActivatorForm.ChosenActivateables);
                    string mapFile = MapUtility.GetWorldMapLevelPath(selectedLevel);
                    MapMakerData data = MapUtility.LoadMapData(mapFile);
                    var existingItem = data.MapData.Select(d => d.GameObject)
                        .Where((g) =>
                        {
                            string existingStr = g.ToString();
                            string newStr = levelObjective.ToString();
                            var existingArr = existingStr.Split(MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR[0]);
                            var newArr = newStr.Split(MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR[0]);
                            for (int i = 0; i < 5; i++)
                            {
                                if (newArr[i] != existingArr[i]) return false;
                            }
                            return true;
                        }).FirstOrDefault();
                    if (existingItem != null)
                    {
                        var mapMakerObj = data.MapData.FirstOrDefault(d => d.GameObject == existingItem);
                        mapMakerObj.GameObject = (ISprite)levelObjective;
                        MapUtility.SaveMap(data.MapName, data.GameMode, data.MapSize, data.MapData);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    MessageBox.Show("Error saving map settings",
                        "File I/O error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
