using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolLed_XuanCuong.Objects
{
    public class EventData
    {
        public string ID { get; set; }
        public int STT { get; set; }
        public string ServiceCode { get; set; } = "";
        public string ChinaPlateNumber { get; set; } = "";
        public string VietNamPlateNumber { get; set; } = "";
        public string ParkingPosition { get; set; } = "";
        public string Group { get; set; } = "";
        public EM_ParkingStatus Status { get; set; }

    }
}
