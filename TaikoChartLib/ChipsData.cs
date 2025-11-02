using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace TaikoChartLib
{
    public class ChipsData
    {
        public float InitBPM { get; set; } = 150.0f;
        public Vector2 InitScroll { get; set; } = new Vector2(1.0f, 0.0f);

        public List<Chip> Chips { get; set; } = new List<Chip>();
    }
}
