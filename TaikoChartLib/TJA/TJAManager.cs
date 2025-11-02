using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaikoChartLib.TJA
{
    public static class TJAManager
    {
        public static TJATaikoChart TJAToChart(string text) => TJAReader.TJAToChart(text);

        public static string ChartToTJAText(TaikoChart taikoChart)
        {
            return "";
        }
    }
}
