using Kztek.LedController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolLed_XuanCuong.Objects
{
    public class LED
    {
        public string ID { get; set; } = "";
        public string IP { get; set; } = "";
        public int Port { get; set; } = 0;

        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public EM_ModuleType moduleType { get; set; } = EM_ModuleType.P10FullColor;

        public int Row { get; set; }
        public int Column { get; set; }
        public int LedColor { get; set; }
        public int FontSize { get; set; }
    }
}
