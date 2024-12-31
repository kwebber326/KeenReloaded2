using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using KeenReloaded2.Utilities;
using KeenReloaded2.Constants;
using KeenReloaded2.ControlEventArgs;

namespace KeenReloaded2.UserControls.MusicAndSound
{
    public partial class SoundPlayer : UserControl
    {
        public SoundPlayer()
        {
            InitializeComponent();
            AudioSettings settings = FileIOUtility.LoadAudioSettings();
            if (settings.Sounds)
                EventStore<string>.Subscribe(MapMakerConstants.EventStoreEventNames.EVENT_SOUND_PLAY, Sound_Play);
            else
                EventStore<string>.UnSubscribe(MapMakerConstants.EventStoreEventNames.EVENT_SOUND_PLAY, Sound_Play);
        }

        public const string SOUNDS_FOLDER = "Sounds";
        public readonly string SOUNDS_PATH = Path.Combine(Environment.CurrentDirectory, SOUNDS_FOLDER);

        private void SoundPlayer_Load(object sender, EventArgs e)
        {

        }

        protected void Sound_Play(object sender, ControlEventArgs<string> eventArgs)
        {
            this.PlaySound(eventArgs.Data);
        }

        public void PlaySound(string soundName)
        {
            if (String.IsNullOrWhiteSpace(soundName))
                return;

            Thread thread = new Thread(new ThreadStart(() =>
            {
                string path = Path.Combine(SOUNDS_PATH, soundName);
                using (System.Media.SoundPlayer player = new System.Media.SoundPlayer(path))
                {
                    player.Play();
                }
            }));
            thread.Start();
        }
    }
}
