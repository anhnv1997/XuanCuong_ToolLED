using Kztek.LedController;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolLed_XuanCuong.Objects;

namespace ToolLed_XuanCuong.Databases
{
    public class tbl_LED
    {
        public const string TBL_NAME = "tbl_LED";
        public const string COL_ID = "id";
        public const string COL_PORT = "port";
        public const string COL_NAME = "led_Name";
        public const string COL_FUNCTION = "led_Function";
        public const string COL_TYPE = "controller_Type";
        public const string COL_IP = "ip_Address";
        public const string COL_DESCRIPRION = "description";
        public const string COL_ROW = "row";
        public const string COL_Column = "column_Led";
        public const string COL_Color = "color";
        public const string COL_FontSize = "fontSize";

        public static LedCollection GetLedData(LedCollection ledDatas)
        {
           string getCMD = $@"Select {COL_ID},{COL_NAME},{COL_IP},{COL_PORT},{COL_TYPE},{COL_DESCRIPRION},
                                     {COL_ROW},{COL_Column}, {COL_Color}, {COL_FontSize}
                               FROM {TBL_NAME}  
                               ";
            ledDatas.Clear();
            DataTable dtLed = StaticPool.mdb.FillData(getCMD);
            if (dtLed != null)
            {
                if (dtLed.Rows.Count > 0)
                {
                    foreach(DataRow row in dtLed.Rows)
                    {
                        LED led = new LED()
                        {
                            ID = row[COL_ID].ToString(),
                            Name = row[COL_NAME].ToString(),
                            IP = row[COL_IP].ToString(),
                            Port = Convert.ToInt32(row[COL_PORT].ToString()),
                            moduleType = EM_ModuleType.P10FullColor,
                            Description = row[COL_DESCRIPRION].ToString(),
                            Row = Convert.ToInt32(row[COL_ROW].ToString()),
                            Column = Convert.ToInt32(row[COL_Column].ToString()),
                            LedColor = Convert.ToInt32(row[COL_Color].ToString()),
                            FontSize = Convert.ToInt32(row[COL_FontSize].ToString())
                        };
                        ledDatas.Add(led);
                    }
                    return ledDatas;
                }
            }
            return null;
        }
    }
}
