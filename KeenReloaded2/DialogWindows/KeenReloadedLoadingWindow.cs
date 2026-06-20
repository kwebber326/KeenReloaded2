using KeenReloaded2.Constants;
using KeenReloaded2.Entities;
using KeenReloaded2.Framework.GameEntities.Animations;
using KeenReloaded2.Framework.GameEventArgs;
using KeenReloaded2.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KeenReloaded2.DialogWindows
{
    public class KeenReloadedLoadingWindow : KeenReloadedMessageWindow
    {
        private PictureBox pbLoadingImage;
        private UserControls.MusicAndSound.SoundPlayer soundPlayer1;
        private Animation _keenAnimation;
        private bool _animationDone;
        private bool _mapLoaded;
        private Timer _animationStartTimer;
        private Timer _eventsCompletedCheckTimer;
        public event EventHandler<WorldMapEventArgs> WorldMapLoaded;
        public event EventHandler WorldMapLoadError;

        public KeenReloadedLoadingWindow()
        {
            InitializeSoundPlayer();
            this.InitializeComponent();
            _animationStartTimer = new Timer();
            _animationStartTimer.Interval = 500;
            _animationStartTimer.Tick += _animationStartTimer_Tick;
            _eventsCompletedCheckTimer = new Timer();
            _eventsCompletedCheckTimer.Interval = 500;
            _eventsCompletedCheckTimer.Tick += _eventsCompletedCheckTimer_Tick;
        }

        public MapMakerData MapData
        {
            get; private set;
        }

        public void UnmuteMusic()
        {
            this.soundPlayer1.UnmuteMusic();
        }

        public void MuteMusic()
        {
            this.soundPlayer1.UnmuteMusic();
        }

        private void _eventsCompletedCheckTimer_Tick(object sender, EventArgs e)
        {
            if (_animationDone && _mapLoaded)
            {
                _eventsCompletedCheckTimer.Stop();
                this.DialogResult = DialogResult.OK;
                this.soundPlayer1.Mute();
                _animationDone = false;
                _mapLoaded = false;
                this.Close();
            }
        }

        public void LoadLevel(string level, string episode, string loadingText)
        {
            _messageText = loadingText;
            EnableTextWrapping();
            base.KeenReloadedMessageWindow_Load(this, EventArgs.Empty);
            string sound = this.GetSoundFromEpisode(episode);
            pbLoadingImage.Image = Properties.Resources.Keen_level_entry1;
            this.soundPlayer1.Unmute();
            this.soundPlayer1.MuteMusic();
            this.PublishSoundPlayEvent(sound);
            _animationStartTimer.Start();
            LoadMapInBackground(level);
            this.ShowDialog();
        }

        private void EnableTextWrapping()
        {
            lblText.Location = new Point(lblText.Location.X, 20);
            lblText.AutoSize = false;
            lblText.Width = 300;
            lblText.Height = 100;
        }

        private string GetSoundFromEpisode(string episode)
        {
            switch (episode)
            {
                case GeneralGameConstants.Episodes.EPISODE_4:
                    return GeneralGameConstants.Sounds.KEEN4_LEVEL_ENTER;
            }

            return GeneralGameConstants.Sounds.KEEN4_LEVEL_ENTER;
        }

        protected override void KeenReloadedMessageWindow_Load(object sender, EventArgs e)
        {
            base.KeenReloadedMessageWindow_Load(sender, e);

        }

        private void _animationStartTimer_Tick(object sender, EventArgs e)
        {
            InitializeAnimation();
            if (!_eventsCompletedCheckTimer.Enabled)
            {
                _eventsCompletedCheckTimer.Start();
            }
            _animationStartTimer.Stop();
        }

        private void LoadMapInBackground(string level)
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    string folder = Path.Combine(MapMakerConstants.SAVED_MAPS_FOLDER,
                        MapUtility.GetFolderFromGameMode(
                        MainMenuConstants.OPTION_LABEL_NORMAL_MODE));
                    string path = Path.Combine(Environment.CurrentDirectory, folder, level + ".txt");

                    var mapData = MapUtility.LoadMapData(path);
                    this.MapData = mapData;
                    _mapLoaded = true;
                }
                catch (Exception ex)
                {
                    Debug.Write(ex);
                    this.DialogResult = DialogResult.Cancel;
                    this.Close();
                }
            });
        }

        private void PublishSoundPlayEvent(string soundName)
        {
            SoundPlayEventArgs soundPlayEventArgs = new SoundPlayEventArgs()
            {
                SenderPosition = null,
                Sound = soundName
            };
            EventStore<SoundPlayEventArgs>.Publish(MapMakerConstants.EventStoreEventNames.EVENT_SOUND_PLAY,
                soundPlayEventArgs);
        }

        private void InitializeAnimation()
        {
            _keenAnimation = new Animation(new List<Image>
            {
                Properties.Resources.Keen_level_entry1,
                Properties.Resources.Keen_level_entry2,
                Properties.Resources.Keen_level_entry3,
                Properties.Resources.Keen_level_entry4,
                Properties.Resources.Keen_level_entry6,
                Properties.Resources.Keen_level_entry7,
            }, 200, false);
            _keenAnimation.AnimationMoveNext += _keenAnimation_AnimationMoveNext;
            _keenAnimation.AnimationEnd += _keenAnimation_AnimationEnd;
            _keenAnimation.Start();

        }

        private void _keenAnimation_AnimationEnd(object sender, EventArgs e)
        {
            _animationDone = true;
        }

        private void _keenAnimation_AnimationMoveNext(object sender, EventArgs e)
        {
            pbLoadingImage.Image = _keenAnimation.CurrentImage;
        }

        protected override void KeenReloadedMessageWindow_KeyUp(object sender, KeyEventArgs e)
        {

        }

        private void InitializeSoundPlayer()
        {
            this.soundPlayer1 = new KeenReloaded2.UserControls.MusicAndSound.SoundPlayer();
            // 
            // soundPlayer1
            // 
            this.soundPlayer1.Keen = null;
            this.soundPlayer1.Location = new System.Drawing.Point(301, 38);
            this.soundPlayer1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.soundPlayer1.Name = "soundPlayer1";
            this.soundPlayer1.Size = new System.Drawing.Size(100, 98);
            this.soundPlayer1.TabIndex = 3;
            this.soundPlayer1.Visible = false;

        }

        private void InitializeComponent()
        {
            this.pbLoadingImage = new System.Windows.Forms.PictureBox();

            ((System.ComponentModel.ISupportInitialize)(this.pbLoadingImage)).BeginInit();
            this.SuspendLayout();
            // 
            // lblText
            // 
            this.lblText.Size = new System.Drawing.Size(0, 18);
            this.lblText.Text = "";
            // 
            // pbLoadingImage
            // 
            this.pbLoadingImage.BackColor = System.Drawing.Color.Transparent;
            this.pbLoadingImage.Image = global::KeenReloaded2.Properties.Resources.Keen_level_entry1;
            this.pbLoadingImage.Location = new System.Drawing.Point(12, 29);
            this.pbLoadingImage.Name = "pbLoadingImage";
            this.pbLoadingImage.Size = new System.Drawing.Size(102, 116);
            this.pbLoadingImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbLoadingImage.TabIndex = 2;
            this.pbLoadingImage.TabStop = false;

            // 
            // KeenReloadedLoadingWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.ClientSize = new System.Drawing.Size(440, 157);
            this.Controls.Add(this.pbLoadingImage);
            this.Name = "KeenReloadedLoadingWindow";
            this.Controls.SetChildIndex(this.lblText, 0);
            this.Controls.SetChildIndex(this.pbLoadingImage, 0);
            this.lblText.BringToFront();
            ((System.ComponentModel.ISupportInitialize)(this.pbLoadingImage)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}
