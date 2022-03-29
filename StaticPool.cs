using Databases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolLed_XuanCuong.Objects;

namespace ToolLed_XuanCuong
{
    public class StaticPool
    {
        public static MDB mdb = null;
        public static LedCollection leds = new LedCollection();

        public static string GetStatusName(EM_ParkingStatus status)
        {
            switch (status)
            {
                case EM_ParkingStatus.UNKNOWN:
                    return "";
                case EM_ParkingStatus.CHUA_XAC_NHAN:
                    return "Chưa Xác Nhận";
                case EM_ParkingStatus.CHUA_PHAN_TO:
                    return "Chưa Phân Tổ";
                case EM_ParkingStatus.DA_PHAN_TO:
                    return "Đã Phân Tổ";
                case EM_ParkingStatus.DANG_THUC_HIEN:
                    return "Đang Thực Hiện";
                case EM_ParkingStatus.CHO_DUYET:
                    return "Chờ Duyệt";
                case EM_ParkingStatus.HOAN_THANH:
                    return "Hoàn Thành";
                default:
                    return "";
            }
        }
    }
}
