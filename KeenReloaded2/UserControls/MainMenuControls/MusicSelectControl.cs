using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using KeenReloaded2.Utilities;
using KeenReloaded2.Constants;

namespace KeenReloaded2.UserControls.MainMenuControls
{
    public partial class MusicSelectControl : UserControl
    {
        public MusicSelectControl()
        {
            InitializeComponent();
        }

        private void InitializeSongList()
        {
            List<string> songfiles = FileIOUtility.LoadWavFormatSongs()
                .Select(s => s.Substring(0, s.LastIndexOf('.')))
                .ToList();

            foreach (var file in songfiles)
            {
                cmbSongs.Items.Add(file);
            }
            var settings = FileIOUtility.LoadAudioSettings();
            string songName = GetSelectedSongFromSettings(settings);
            int index = cmbSongs.Items.IndexOf(songName);
            cmbSongs.SelectedIndex = index != -1 ? index : 0;
        }

        private static string GetSelectedSongFromSettings(AudioSettings settings)
        {
            var song = settings.SelectedSong;
            if (!string.IsNullOrEmpty(song) && song.Contains("."))
                return song.Substring(0, song.LastIndexOf('.'));

            return string.Empty;
        }

        private void BtnRandomSong_Click(object sender, EventArgs e)
        {
            Random random = new Random();
            int index = random.Next(0, cmbSongs.Items.Count);
            cmbSongs.SelectedIndex = index;
        }

        private void CmbSongs_SelectedIndexChanged(object sender, EventArgs e)
        {
            EventStore<string>.Publish(MapMakerConstants.EventStoreEventNames.EVENT_SELECTED_SONG_CHANGED,
                cmbSongs.SelectedItem + ".wav");
        }

        private void MusicSelectControl_Load(object sender, EventArgs e)
        {
            InitializeSongList();
        }
    }
}
