using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaikoChartLib
{
    public class Course
    {
        public int Level { get; set; } = 0;
        public List<string> Charter { get; set; } = new List<string>();
        public Dictionary<StyleSide, ChipsData> ChipsDatas { get; set; } = new Dictionary<StyleSide, ChipsData>();
    }
}
