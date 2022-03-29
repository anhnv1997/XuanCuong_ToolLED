using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolLed_XuanCuong.Objects;

namespace ToolLed_XuanCuong.Databases
{
    public class tblEvent
    {
        public static List<EventData> GetEvent()
        {
            List<EventData> eventDatas = new List<EventData>();
            string startTime = DateTime.Now.ToString("yyyy/MM/dd 00:00:00");
            string endTime = DateTime.Now.ToString("yyyy/MM/dd 23:59:59");
            string cmd = $@"SELECT Id, ServiceCode,EventType,PlateVN,PlateCN,ParkingPosition,GroupId from tbl_Event where StartDate!='9999-12-31 23:59:59.997' AND (CAST(StartDate as datetime) between '{startTime}' And '{endTime}') order by StartDate ASC";
            DataTable dtTop6Datas = StaticPool.mdb.FillData(cmd); 
            if (dtTop6Datas != null)
            {
                if (dtTop6Datas.Rows.Count > 0)
                {
                    foreach(DataRow row in dtTop6Datas.Rows)
                    {
                        EventData e = new EventData()
                        {
                            ID = row["Id"].ToString(),
                            ChinaPlateNumber = row["PlateCN"].ToString(),
                            VietNamPlateNumber = row["PlateVN"].ToString(),
                            ServiceCode = row["ServiceCode"].ToString(),
                            Status = (EM_ParkingStatus)Convert.ToInt32(row["EventType"].ToString()),
                            ParkingPosition = row["ParkingPosition"].ToString(),
                        };
                        string groupID = row["GroupId"].ToString();
                        if (groupID != "")
                        {
                            DataTable dtGroup = StaticPool.mdb.FillData($@"Select [Name] from [Group] where Id = '{groupID}'");
                            if (dtGroup != null)
                            {
                                if (dtGroup.Rows.Count > 0)
                                {
                                    e.Group = dtGroup.Rows[0]["Name"].ToString();
                                }
                                else
                                {
                                    e.Group = "";
                                }
                            }
                            else
                            {
                                e.Group = "";
                            }
                        }
                        else
                        {
                            e.Group = "";
                        }
                        eventDatas.Add(e);
                    }
                }
            }

            return eventDatas;
        }
    }
}
