using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using KeenReloaded2.ControlEventArgs.EventStoreData;
using KeenReloaded2.Utilities;
using KeenReloaded2.Constants;

namespace KeenReloaded2.UserControls.MapMakerUserControls
{
    public partial class PathwayCreatorForm : Form
    {
        private List<Point> _currentPoints;
        
        private const string X_TEXT = "X=";
        private const string Y_TEXT = "Y=";
        private const char SEPARATOR = ',';

        public PathwayCreatorForm()
        {
            InitializeComponent();
        }

        public PathwayCreatorForm(List<Point> currentPoints)
        {
            _currentPoints = currentPoints ?? new List<Point>();
            InitializeComponent();
        }

        #region event handlers

        private void PathwayCreatorForm_Load(object sender, EventArgs e)
        {
            PopulateListBoxWithCurrentPoints();
            SetEditButtonsEnabledStatusBasedOnSelectedStatus();
        }

        private void LstPoints_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetEditButtonsEnabledStatusBasedOnSelectedStatus();
        }

        private void BtnAddPoint_Click(object sender, EventArgs e)
        {
            try
            {
                int x = Convert.ToInt32(txtXPosition.Text);
                int y = Convert.ToInt32(txtYPosition.Text);

                Point p = new Point(x, y);
                string item = ToListItem(p);
                lstPoints.Items.Add(item);
                OnPointsListChanged();
            }
            catch
            {
                MessageBox.Show("Invalid entry for x,y coordinates!");
            }
        }

        private void SetEditButtonsEnabledStatusBasedOnSelectedStatus()
        {
            bool enabled = lstPoints.SelectedIndex != -1;
            btnDelete.Enabled = enabled;
            btnDown.Enabled = enabled;
            btnUp.Enabled = enabled;
        }

        private void OnPointsListChanged()
        {
           var eventArgs = new PointListChangedEventArgs() { NewPoints = this.PathWayPoints };
            EventStore<PointListChangedEventArgs>.Publish(
                MapMakerConstants.EventStoreEventNames.EVENT_POINTS_LIST_CHANGED,
                 eventArgs);
        }

        private void TxtXPosition_TextChanged(object sender, EventArgs e)
        {

        }

        private void TxtYPosition_TextChanged(object sender, EventArgs e)
        {

        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            string item = lstPoints.SelectedItem?.ToString();
            if (!string.IsNullOrEmpty(item))
            {
                lstPoints.Items.RemoveAt(lstPoints.SelectedIndex);
                OnPointsListChanged();
            }
        }

        private void BtnDown_Click(object sender, EventArgs e)
        {

        }

        private void BtnUp_Click(object sender, EventArgs e)
        {

        }

        #endregion

        public List<Point> PathWayPoints
        {
            get
            {
                return GetPointsFromListBoxItems();
            }
        }

        private void PopulateListBoxWithCurrentPoints()
        {
            lstPoints.Items.Clear();
            foreach (var point in _currentPoints)
            {
                string text = ToListItem(point);
                lstPoints.Items.Add(text);
            }
            OnPointsListChanged();
        }

        private List<Point> GetPointsFromListBoxItems()
        {
            List<Point> points = new List<Point>();
            if (lstPoints.Items.Count == 0)
                return points;

            foreach (var item in lstPoints.Items)
            {
                string itemText = item?.ToString();
                if (!string.IsNullOrEmpty(itemText))
                {
                    Point p = FromListItem(itemText);
                    points.Add(p);
                }
            }

            return points;
        }

        private string ToListItem(Point p)
        {
            return $"{X_TEXT}{p.X}{SEPARATOR}{Y_TEXT}{p.Y}";
        }

        private Point FromListItem(string item)
        {
            string[] parts = item.Split(SEPARATOR);
            string part1 = parts[0].Replace(X_TEXT, "");
            string part2 = parts[1].Replace(Y_TEXT, "");

            int x = Convert.ToInt32(part1);
            int y = Convert.ToInt32(part2);
            return new Point(x, y);
        }

        private void BtnDone_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
