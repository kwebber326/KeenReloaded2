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
using SharpDX;
using SharpDX.Multimedia;
using SharpDX.XAudio2;
using SharpDX.IO;
using KeenReloaded2.Framework.GameEventArgs;
using KeenReloaded2.Framework.GameEntities.Players;

namespace KeenReloaded2.UserControls.MusicAndSound
{
    public partial class SoundPlayer : UserControl
    {
        public SoundPlayer()
        {
            InitializeComponent();
            AudioSettings settings = FileIOUtility.LoadAudioSettings();
            if (settings.Sounds)
            {
                EventStore<SoundPlayEventArgs>.Subscribe(MapMakerConstants.EventStoreEventNames.EVENT_SOUND_PLAY, Sound_Play);
                _soundDevice.StartEngine();
                _voice = new MasteringVoice(_soundDevice);
            }
            else
                EventStore<SoundPlayEventArgs>.UnSubscribe(MapMakerConstants.EventStoreEventNames.EVENT_SOUND_PLAY, Sound_Play);

            if (settings.Music)
            {
                _song = settings.SelectedSong;
                this.PlayMusic(_song);
            }

            EventStore<string>.Subscribe(MapMakerConstants.EventStoreEventNames.KEEN_LEVEL_COMPLETE,
                    Level_Complete);
        }

        private const string SOUNDS_FOLDER = "Sounds";
        private const string MUSIC_FOLDER = "Music";
        private readonly string SOUNDS_PATH = Path.Combine(Environment.CurrentDirectory, SOUNDS_FOLDER);
        private readonly string MUSIC_PATH = Path.Combine(Environment.CurrentDirectory, MUSIC_FOLDER);
        private readonly string _song;
        private XAudio2 _soundDevice = new XAudio2();
        private MasteringVoice _voice;
        private System.Media.SoundPlayer _musicPlayer;
        List<SourceVoice> _sounds = new List<SourceVoice>();

        public CommanderKeen Keen { get; set; }

        private void SoundPlayer_Load(object sender, EventArgs e)
        {

        }

        protected void Level_Complete(object sender, ControlEventArgs<string> e)
        {
            _musicPlayer?.Stop();
        }

        protected void Sound_Play(object sender, ControlEventArgs<SoundPlayEventArgs> eventArgs)
        {
            if (eventArgs.Data == null)
                return;

            this.PlaySound(eventArgs.Data);
        }

        public void PlayMusic(string songName)
        {
            if (String.IsNullOrWhiteSpace(songName))
                return;

            if (_musicPlayer == null)
            {
                string path = Path.Combine(MUSIC_PATH, songName);
                _musicPlayer = new System.Media.SoundPlayer(path);
                _musicPlayer.PlayLooping();
            }
        }

        public void StopMusic(bool dispose = false)
        {
            try
            {
                if (_musicPlayer != null)
                {
                    _musicPlayer.Stop();
                    _musicPlayer.Dispose();
                }
            }
            finally
            {
                EventStore<SoundPlayEventArgs>.UnSubscribe(MapMakerConstants.EventStoreEventNames.EVENT_SOUND_PLAY, Sound_Play);
                EventStore<string>.UnSubscribe(MapMakerConstants.EventStoreEventNames.KEEN_LEVEL_COMPLETE,
                   Level_Complete);
                _soundDevice.StopEngine();
                if (dispose)
                {
                    DisposeSoundDevices();
                }
            }
        }

        public void PlaySound(SoundPlayEventArgs data)
        {
            string soundName = data?.Sound;
            if (String.IsNullOrWhiteSpace(soundName))
                return;

            ThreadStart ts = new ThreadStart(() =>
            {
                try
                {
                    string path = Path.Combine(SOUNDS_PATH, soundName);
                    var nativeFileStream = new NativeFileStream(path, NativeFileMode.Open, NativeFileAccess.Read, NativeFileShare.Read);
                    var soundStream = new SoundStream(nativeFileStream);
                    var buffer = new AudioBuffer() { Stream = soundStream, AudioBytes = (int)soundStream.Length, Flags = BufferFlags.EndOfStream };
                    SourceVoice sourceVoice = new SourceVoice(_soundDevice, soundStream.Format, true);

                    sourceVoice.SubmitSourceBuffer(buffer, soundStream.DecodedPacketsInfo);
                    float volumePercentage = this.GetVolumePercentage(data);
                    sourceVoice.SetVolume(volumePercentage);
                    sourceVoice.Start();
                    Thread.Sleep(5000);
                    sourceVoice.Stop();
                    sourceVoice.FlushSourceBuffers();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex);
                }
            });

            Thread thread = new Thread(ts);
            thread.Start();
        }

        private float GetVolumePercentage(SoundPlayEventArgs data)
        {
            if (data?.SenderPosition == null || this.Keen == null)
                return 1;

            Point objectPos = data.SenderPosition.Value;
            Point playerPos = this.Keen.HitBox.Location;

            var distance = CommonGameFunctions.GetEuclideanDistance(objectPos, playerPos);

            if (distance <= GeneralGameConstants.VIEW_RADIUS)
                return 1;

            if (distance > GeneralGameConstants.AUDIBLE_RADIUS)
                return 0;

            float viewRadiusAudibleRadiusDiff = GeneralGameConstants.AUDIBLE_RADIUS - GeneralGameConstants.VIEW_RADIUS;

            float objectDistanceFromViewRadius = (float)distance - GeneralGameConstants.VIEW_RADIUS;

            float pct = (viewRadiusAudibleRadiusDiff - objectDistanceFromViewRadius) / viewRadiusAudibleRadiusDiff;
            return pct;
        }

        ~SoundPlayer()
        {
            DisposeSoundDevices();
        }

        private void DisposeSoundDevices()
        {
            if (_voice != null && !_voice.IsDisposed)
                _voice.Dispose(); ;
            if (_soundDevice != null && !_soundDevice.IsDisposed)
                _soundDevice.Dispose();
        }
    }
}
