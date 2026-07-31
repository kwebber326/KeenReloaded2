using KeenReloaded2.Constants;
using KeenReloaded2.Entities;
using KeenReloaded2.Framework.GameEntities;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using KeenReloaded2.Framework.GameEntities.WorldMapEntities;
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
        private readonly string _mapName;
        private WorldMapObjectiveManager _worldMapObjectiveData;
        private List<IActivateable> _activateables;
        private List<IWorldMapLevel> _levels;
        private Dictionary<string, List<ILevelObjective>> _mapLevelObjectives
            = new Dictionary<string, List<ILevelObjective>>();

        public LevelObjectivesForm()
        {
            InitializeComponent();
        }

        public LevelObjectivesForm(string mapName, IEnumerable<IWorldMapLevel> levels,
            IEnumerable<IActivateable> activateables, WorldMapObjectiveManager worldMapObjectiveData)
        {
            InitializeComponent();
            _mapName = mapName;
            _worldMapObjectiveData = worldMapObjectiveData;
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
                    PopulateLevelObjectives(levelObjectives, selectedLevel);
                }
            }
        }

        private void PopulateLevelObjectives(List<ILevelObjective> levelObjectives, string levelName)
        {
            cmbObjectives.Items.Clear();
            foreach (var item in levelObjectives)
            {
                string key = WorldMapObjectiveManager.GetKeyFromLevelObjective(item as ISprite, levelName);
                cmbObjectives.Items.Add(key);
            }
        }

      

        private Image GetImageFromLevelObjectiveKey(string key)
        {
            string selectedLevel = cmbLevelNames.SelectedItem?.ToString();
            var objective = _mapLevelObjectives.TryGetValue(selectedLevel, out List<ILevelObjective> levelObjectives)
                ? levelObjectives.FirstOrDefault(l =>
                   key == WorldMapObjectiveManager.GetKeyFromLevelObjective(l as ISprite, selectedLevel))
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

            var activator = levelObjectives.FirstOrDefault(l => WorldMapObjectiveManager.GetKeyFromLevelObjective(l as ISprite, selectedLevel)
             == selectedItem) as IActivator;

            if (activator == null || !(activator is IInteractiveLevelObjective)) return;

            var activationIds = _worldMapObjectiveData.LevelObjectives.Activators.Values
                .SelectMany(v => v.WorldMapComponents).ToList();
            var toggleObjects = _activateables.Where(a =>
                activationIds.Contains(a.ActivationID)).ToList();
            EditActivatorForm editActivatorForm = new EditActivatorForm(
               toggleObjects, _activateables);

            var result = editActivatorForm.ShowDialog();
            if (result == DialogResult.OK)
            {
                try
                {
                    var activateables = editActivatorForm.ChosenActivateables;
                    ILevelObjective levelObjective = activator as ILevelObjective;
                    if (levelObjective != null)
                    {
                        _worldMapObjectiveData.AddOrUpdateObjectives(
                            new List<ISprite>() { levelObjective as ISprite }, selectedLevel,
                            WorldMapLevelObjectiveType.ACTIVATOR, activateables.Select(a => a.ActivationID));
                        MapUtility.SaveWorldMapObjectives(_worldMapObjectiveData, _mapName);
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

        private void btnAddObjective_Click(object sender, EventArgs e)
        {

        }
    }
}
