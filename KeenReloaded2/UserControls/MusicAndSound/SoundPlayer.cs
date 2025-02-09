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
                EventStore<string>.Subscribe(MapMakerConstants.EventStoreEventNames.EVENT_SOUND_PLAY, Sound_Play);
                _soundDevice.StartEngine();
                _voice = new MasteringVoice(_soundDevice);
            }
            else
                EventStore<string>.UnSubscribe(MapMakerConstants.EventStoreEventNames.EVENT_SOUND_PLAY, Sound_Play);

            if (settings.Music)
            {
                _song = settings.SelectedSong;
                this.PlayMusic(_song);
            }
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

        private void SoundPlayer_Load(object sender, EventArgs e)
        {

        }

        protected void Sound_Play(object sender, ControlEventArgs<string> eventArgs)
        {
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
                EventStore<string>.UnSubscribe(MapMakerConstants.EventStoreEventNames.EVENT_SOUND_PLAY, Sound_Play);
                _soundDevice.StopEngine();
                if (dispose)
                {
                    DisposeSoundDevices();
                }
            }
        }

        public void PlaySound(string soundName)
        {
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
