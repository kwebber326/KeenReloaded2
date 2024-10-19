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
            var selectedItem = lstPoints.SelectedItem;
            if (selectedItem != null)
            {
                Point selectedPoint = FromListItem(selectedItem.ToString());
                txtXPosition.Text = selectedPoint.X.ToString();
                txtYPosition.Text = selectedPoint.Y.ToString();
                EventStore<int>.Publish(MapMakerConstants.EventStoreEventNames.EVENT_SELECTED_INDEX_CHANGED, lstPoints.SelectedIndex + 1);
            }
        }

        private void BtnAddPoint_Click(object sender, EventArgs e)
        {
            try
            {
                Point p = GetPointFromUserEntry();
                string item = ToListItem(p);
                lstPoints.Items.Add(item);
                lstPoints.SelectedIndex = lstPoints.Items.Count - 1;
                OnPointsListChanged();
            }
            catch
            {
                MessageBox.Show("Invalid entry for x,y coordinates!");
            }
        }

        private Point GetPointFromUserEntry()
        {
            int x = Convert.ToInt32(txtXPosition.Text);
            int y = Convert.ToInt32(txtYPosition.Text);

            Point p = new Point(x, y);
            return p;
        }

        private void SetEditButtonsEnabledStatusBasedOnSelectedStatus()
        {
            bool enabled = lstPoints.SelectedIndex != -1;
            btnDelete.Enabled = enabled;
            btnDown.Enabled = enabled;
            btnUp.Enabled = enabled;
            btnUpdate.Enabled = enabled;
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
            MoveByDirection(true);
        }

    

        private void BtnUp_Click(object sender, EventArgs e)
        {
            MoveByDirection(false);
        }

        private void BtnDone_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            Point p = GetPointFromUserEntry();
            if (lstPoints.SelectedIndex != -1)
            {
                string text = ToListItem(p);
                lstPoints.Items[lstPoints.SelectedIndex] = text;
                EventStore<IndexedPoint>.Publish(
                    MapMakerConstants.EventStoreEventNames.EVENT_LOCATION_CHANGED, 
                    new IndexedPoint()
                    {
                        Index = lstPoints.SelectedIndex + 1,
                        Location = p
                    });
            }
        }

        #endregion

        public List<Point> PathWayPoints
        {
            get
            {
                return GetPointsFromListBoxItems();
            }
        }

        private void MoveByDirection(bool downDirection)
        {
            if (lstPoints.Items.Count < 2)
                return;

            string item = lstPoints.SelectedItem?.ToString();
            if (!string.IsNullOrEmpty(item))
            {

                int currentIndex = lstPoints.SelectedIndex;
                int downIndex = currentIndex == lstPoints.Items.Count - 1 ? 0 : currentIndex + 1;
                int upIndex = currentIndex == 0 ? lstPoints.Items.Count - 1 : currentIndex - 1;
                int nextIndex = downDirection ? downIndex : upIndex;
                string tmp = item;
                string other = lstPoints.Items[nextIndex].ToString();
                lstPoints.Items[currentIndex] = other;
                lstPoints.Items[nextIndex] = tmp;
                lstPoints.SelectedIndex = nextIndex;
                OnPointsListChanged();
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
    }
}
