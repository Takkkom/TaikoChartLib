using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace TaikoChartLib
{
    public class ChipParams : ICloneable
    {
        public double Time { get; set; } = 0.0;
        public float BPM { get; set; } = 150.0f;
        public Vector2 Scroll { get; set; } = new Vector2(1.0f, 0.0f);
        public Vector2 Measure { get; set; } = new Vector2(4.0f, 4.0f);
        public BranchType Branch { get; set; } = BranchType.Normal;

        public object Clone()
        {
            return new ChipParams()
            {
                Time = Time,
                BPM = BPM,
                Scroll = Scroll,
                Measure = Measure,
                Branch = Branch
            };
        }

        public ChipParams Copy()
        {
            return (ChipParams)Clone();
        }
    }
}
