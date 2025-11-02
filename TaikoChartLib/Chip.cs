using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace TaikoChartLib
{
    public class Chip
    {
        public static bool IsNote(ChipType chipType) => chipType >= ChipType.None && chipType < ChipType.NoteMax;

        public ChipType ChipType { get; set; } = ChipType.None;
        public ChipParams Params { get; set; } = new ChipParams();
    }
}
