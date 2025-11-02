using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaikoChartLib
{
    public class TaikoChart
    {
        public Dictionary<string, string> Title { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> SubTitle { get; set; } = new Dictionary<string, string>();

        public List<string> Charter { get; set; } = new List<string>();
        public List<string> Artist { get; set; } = new List<string>();
        public List<string> Genre { get; set; } = new List<string>();

        public string Author { get; set; } = "";
        public string SongFileName { get; set; } = "";
        public string JacketFileName { get; set; } = "";
        public string BGImageFileName { get; set; } = "";
        public string BGMovieFileName { get; set; } = "";
        public string ScenePreset { get; set; } = "";
        public string TowerType { get; set; } = "";
        public string SongSelectionBGFileName { get; set; } = "";

        public SideVisible Visible { get; set; } = SideVisible.Both;

        public float SongOffset { get; set; } = 0.0f;
        public float SongPreviewOffset { get; set; } = 0.0f;
        public float BGImageOffset { get; set; } = 0.0f;
        public float BGMovieOffset { get; set; } = 0.0f;

        public int SongVolume { get; set; } = 100;
        public int SoundEffectVolume { get; set; } = 100;

        public Dictionary<Difficulty, Course> Courses { get; set; } = new Dictionary<Difficulty, Course>();
    }
}
