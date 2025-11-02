using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaikoChartLib.TJA
{
    public class TJATaikoChart : TaikoChart
    {
        public TJACompat TJACompat { get; set; } = TJACompat.OOS;
    }
}
