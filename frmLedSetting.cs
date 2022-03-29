using CryptorEngines;
using Databases;
using FileXMLs;
using iParking;
using Kztek.LedController;
using Microsoft.Win32;
using SQLConns;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ToolLed_XuanCuong.Databases;
using ToolLed_XuanCuong.Objects;

namespace ToolLed_XuanCuong
{
    public partial class frmLedSetting : Form
    {
        public static bool isLoadingSuccess = false;
        SQLConn[] sqls = null;
        public bool isStart = false;

        LED led_STT_Top = new LED();
        LED led_STT_Bottom = new LED();

        LED led_ServiceCode_Top = new LED();
        LED led_ServiceCode_Bottom = new LED();

        LED led_ChinaPlate_Top = new LED();
        LED led_ChinaPlate_Bottom = new LED();

        LED led_VietNamPlate_Top = new LED();
        LED led_VietNamPlate_Bottom = new LED();

        LED led_Position_Top = new LED();
        LED led_Position_Bottom = new LED();

        LED led_Group_Top = new LED();
        LED led_Group_Bottom = new LED();

        LED led_Status_Top = new LED();
        LED led_Status_Bottom = new LED();

        List<LED> TopRowLEDs;
        List<LED> BottomRowLEDs;

        private CancellationTokenSource cts;
        ManualResetEvent ForceLoopIteration;
        NotifyIcon notifyIcon1 = new NotifyIcon();

        private int scrollIndex = 0;
        private int currentDay = 1;
        private const int maxRowLedDisplay = 6;



        public frmLedSetting()
        {
            InitializeComponent();
            notifyIcon1.Icon = new Icon(@"./logo/led.ico");
            RegistryKey rkApp = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            if (!IsStartupItem())
                // Add the value in the registry so that the application runs at startup
                rkApp.SetValue("KZTEK_LED_TOOL_CHECK", Application.ExecutablePath.ToString());

            this.FormClosed += FrmLedSetting_FormClosed;
            notifyIcon1.MouseDoubleClick += NotifyIcon1_MouseDoubleClick;
            LogHelper.Logger_Info("Start Application: Startup Path: " + Application.ExecutablePath.ToString());
        }

        private void NotifyIcon1_MouseDoubleClick(object? sender, MouseEventArgs e)
        {
            Show();
            this.WindowState = FormWindowState.Normal;
            notifyIcon1.Visible = false;
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            bool cursorNotInBar = Screen.GetWorkingArea(this).Contains(Cursor.Position);
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.ShowInTaskbar = false;
                notifyIcon1.Visible = true;
                this.Hide();
            }
        }
        private void FrmLedSetting_FormClosed(object? sender, FormClosedEventArgs e)
        {
            Properties.Settings.Default.led_stt_top_ID = ((ListItem)cbLedSTT_TOP_Line1_Name.SelectedItem).Value;
            Properties.Settings.Default.led_stt_bottom_ID = ((ListItem)cbLedSTT_BOTTOM_Line1_Name.SelectedItem).Value;
            Properties.Settings.Default.led_service_code_top_ID = ((ListItem)cbLedServiceCode_TOP_Line1_Name.SelectedItem).Value;
            Properties.Settings.Default.led_service_code_bottom_ID = ((ListItem)cbLedServiceCode_BOTTOM_Line1_Name.SelectedItem).Value;
            Properties.Settings.Default.led_china_plate_code_top_ID = ((ListItem)cbLedChinaPlateNum_TOP_Line1_Name.SelectedItem).Value;
            Properties.Settings.Default.led_china_plate_code_bottom_ID = ((ListItem)cbLedChinaPlateNum_BOTTOM_Line1_Name.SelectedItem).Value;
            Properties.Settings.Default.led_VN_plate_code_top_ID = ((ListItem)cbLedVietNamPlateNum_TOP_Line1_Name.SelectedItem).Value;
            Properties.Settings.Default.led_VN_plate_code_bottom_ID = ((ListItem)cbLedVietNamPlateNum_BOTTOM_Line1_Name.SelectedItem).Value;
            Properties.Settings.Default.led_position_code_top_ID = ((ListItem)cbLedParkingPosition_TOP_Line1_Name.SelectedItem).Value;
            Properties.Settings.Default.led_position_code_bottom_ID = ((ListItem)cbLedParkingPosition_BOTTOM_Line1_Name.SelectedItem).Value;
            Properties.Settings.Default.led_group_top_ID = ((ListItem)cbLedGroup_TOP_Line1_Name.SelectedItem).Value;
            Properties.Settings.Default.led_group_bottom_ID = ((ListItem)cbLedGroup_BOTTOM_Line1_Name.SelectedItem).Value;
            Properties.Settings.Default.led_status_top_ID = ((ListItem)cbLedStatus_TOP_Line1_Name.SelectedItem).Value;
            Properties.Settings.Default.led_status_bottom_ID = ((ListItem)cbLedStatus_BOTTOM_Line1_Name.SelectedItem).Value;
            Properties.Settings.Default.Save();
            var p = new Process();
            string path = Application.ExecutablePath;
            p.StartInfo.FileName = path;  // just for example, you can use yours.
            p.Start();
        }

        private bool IsStartupItem()
        {
            // The path to the key where Windows looks for startup applications
            RegistryKey rkApp = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            if (rkApp.GetValue("KZTEK_LED_TOOL") == null)
                // The value doesn't exist, the application is not set to run at startup
                return false;
            else
                // The value exists, the application is set to run at startup
                return true;
        }

        private void frmLedSetting_Load(object sender, EventArgs e)
        {
            try
            {
                if (File.Exists(Application.StartupPath + "\\SQLConn.xml"))
                {
                    FileXML.ReadXMLSQLConn(Application.StartupPath + "\\SQLConn.xml", ref sqls);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("frmConnectionConfig: " + ex.Message);
            }

            ConnectToSQLServer();

            tbl_LED.GetLedData(StaticPool.leds);
            foreach (Control control in tableLayoutPanel1.Controls.OfType<ComboBox>())
            {
                LoadLedData((ComboBox)control);
            }

            LoadSettingLed(cbLedSTT_TOP_Line1_Name, Properties.Settings.Default.led_stt_top_ID);
            LoadSettingLed(cbLedSTT_BOTTOM_Line1_Name, Properties.Settings.Default.led_stt_bottom_ID);
            LoadSettingLed(cbLedServiceCode_TOP_Line1_Name, Properties.Settings.Default.led_service_code_top_ID);
            LoadSettingLed(cbLedServiceCode_BOTTOM_Line1_Name, Properties.Settings.Default.led_service_code_bottom_ID);
            LoadSettingLed(cbLedChinaPlateNum_TOP_Line1_Name, Properties.Settings.Default.led_china_plate_code_top_ID);
            LoadSettingLed(cbLedChinaPlateNum_BOTTOM_Line1_Name, Properties.Settings.Default.led_china_plate_code_bottom_ID);
            LoadSettingLed(cbLedVietNamPlateNum_TOP_Line1_Name, Properties.Settings.Default.led_VN_plate_code_top_ID);
            LoadSettingLed(cbLedVietNamPlateNum_BOTTOM_Line1_Name, Properties.Settings.Default.led_VN_plate_code_bottom_ID);
            LoadSettingLed(cbLedParkingPosition_TOP_Line1_Name, Properties.Settings.Default.led_position_code_top_ID);
            LoadSettingLed(cbLedParkingPosition_BOTTOM_Line1_Name, Properties.Settings.Default.led_position_code_bottom_ID);
            LoadSettingLed(cbLedGroup_TOP_Line1_Name, Properties.Settings.Default.led_group_top_ID);
            LoadSettingLed(cbLedGroup_BOTTOM_Line1_Name, Properties.Settings.Default.led_group_bottom_ID);
            LoadSettingLed(cbLedStatus_TOP_Line1_Name, Properties.Settings.Default.led_status_top_ID);
            LoadSettingLed(cbLedStatus_BOTTOM_Line1_Name, Properties.Settings.Default.led_status_bottom_ID);

            if (Properties.Settings.Default.isAutoStart)
            {
                chbIsAutoStart.Checked = true;
                btnStart.Text = "Stop";
                PollingStart();
                this.WindowState = FormWindowState.Minimized;
            }

        }

        private void LoadSettingLed(ComboBox cb, string ledID)
        {
            if (ledID != "")
            {
                foreach (dynamic item in cb.Items)
                {
                    if (ledID == ((ListItem)item).Value.ToString())
                    {
                        cb.SelectedItem = item;
                        break;
                    }
                }
                if (cb.SelectedItem == null && cb.Items.Count > 0)
                {
                    cb.SelectedIndex = 0;
                }

            }
        }

        private void PollingStart()
        {
            cts = new CancellationTokenSource();
            btnStart.Text = "Stop";
            ForceLoopIteration = new ManualResetEvent(false);

            List<string[]> TopRowDisplayDatas, BottomDisplayDatas;
            this.currentDay = DateTime.Now.Day;
            this.scrollIndex = 0;
            List<EventData> eventDatas = tblEvent.GetEvent();
            tbl_LED.GetLedData(StaticPool.leds);
            LoadLedData();
            List<ILED> TopRowILEDs, BottomRowILEDs;
            LoadLedControllers(out TopRowILEDs, out BottomRowILEDs);
            CheckLedControllersScreenResolution(TopRowILEDs, BottomRowILEDs);
            LoadLedDisplayData(eventDatas, out TopRowDisplayDatas, out BottomDisplayDatas);
            DisplayLedData(TopRowILEDs, BottomRowILEDs, TopRowDisplayDatas, BottomDisplayDatas);

            Task.Run(() =>
                DisplayTop6Data(cts.Token), cts.Token
            );
        }

        private void ConnectToSQLServer()
        {
            if (sqls != null && sqls.Length > 0)
            {
                string cbSQLServerName = sqls[0].SQLServerName;
                string cbSQLDatabaseName = sqls[0].SQLDatabase;
                string cbSQLAuthentication = sqls[0].SQLAuthentication;
                string txtSQLUserName = sqls[0].SQLUserName;
                string txtSQLPassword = CryptorEngine.Decrypt(sqls[0].SQLPassword, true);
                StaticPool.mdb = new MDB(cbSQLServerName, cbSQLDatabaseName, cbSQLAuthentication, txtSQLUserName, txtSQLPassword);
            }
        }

        public static void LoadLedData(ComboBox comboBox)
        {
            foreach (LED lED in StaticPool.leds)
            {
                ListItem listGroup = new ListItem();
                listGroup.Value = lED.ID;
                listGroup.Name = lED.Name;
                comboBox.Items.Add(listGroup);
            }
            comboBox.DisplayMember = "Name";
            if (comboBox.Items.Count > 0)
            {
                comboBox.SelectedIndex = 0;
            }
        }

        private void cbLedSelectedIndexChange(object sender, EventArgs e)
        {
            foreach (Control control in tableLayoutPanel1.Controls.OfType<Label>())
            {

                string line2Name = ((ComboBox)sender).Name.Replace("cb", "lbl").Replace("Line1", "Line2");
                string line3Name = ((ComboBox)sender).Name.Replace("cb", "lbl").Replace("Line1", "Line3");
                bool isLine2Label = control.Name.ToLower().Trim() == line2Name.ToLower().Trim();
                bool isLine3Label = control.Name.ToLower().Trim() == line3Name.ToLower().Trim();
                if (isLine2Label || isLine3Label)
                {
                    control.Text = ((ComboBox)sender).Text;
                }
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (btnStart.Text == "Start")
            {
                PollingStart();
            }
            else
            {
                PollingStop();
            }
        }

        private void PollingStop()
        {
            btnStart.Text = "Start";
            cts.Cancel();
            WaitHandle.WaitAny(
                        new[] { cts.Token.WaitHandle, ForceLoopIteration },
                        TimeSpan.FromMilliseconds(50));
        }

        private async Task DisplayTop6Data(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(3000);
                    List<string[]> TopRowDisplayDatas, BottomDisplayDatas;
                    List<ILED> TopRowILEDs, BottomRowILEDs;

                    if (DateTime.Now.Day != this.currentDay)
                    {
                        this.currentDay = DateTime.Now.Day;
                        this.scrollIndex = 0;
                        List<EventData> eventDatas = tblEvent.GetEvent();
                        tbl_LED.GetLedData(StaticPool.leds);
                        LoadLedData();
                        LoadLedControllers(out TopRowILEDs, out BottomRowILEDs);
                        CheckLedControllersScreenResolution(TopRowILEDs, BottomRowILEDs);
                        LoadLedDisplayData(eventDatas, out TopRowDisplayDatas, out BottomDisplayDatas);
                        DisplayLedData(TopRowILEDs, BottomRowILEDs, TopRowDisplayDatas, BottomDisplayDatas);
                        continue;
                    }
                    tbl_LED.GetLedData(StaticPool.leds);
                    LoadLedData();
                    LoadLedControllers(out TopRowILEDs, out BottomRowILEDs);
                    CheckLedControllersScreenResolution(TopRowILEDs, BottomRowILEDs);
                    List<EventData> currentEventDatas = tblEvent.GetEvent();
                    if (currentEventDatas.Count >= 7)
                    {
                        if (scrollIndex < (currentEventDatas.Count - 1))
                        {
                            scrollIndex++;
                        }
                        else
                        {
                            scrollIndex = 0;
                        }
                    }
                    else
                    {
                        scrollIndex = 0;
                    }

                    LoadLedDisplayData(currentEventDatas, out TopRowDisplayDatas, out BottomDisplayDatas, scrollIndex);

                    DisplayLedData(TopRowILEDs, BottomRowILEDs, TopRowDisplayDatas, BottomDisplayDatas);
                }
                catch (Exception ex)
                {
                    LogHelper.Logger_Error(ex.Message);
                }
            }
        }

        private void DisplayLedData(List<ILED> TopRowILEDs, List<ILED> BottomRowILEDs, List<string[]> TopRowDisplayDatas, List<string[]> BottomDisplayDatas)
        {
            try
            {

                for (int i = 0; i < TopRowILEDs.Count; i++)
                {
                    Row[] topRowSettings = new Row[3];
                    for (int j = 0; j < 3; j++)
                    {
                        topRowSettings[j] = new Row();
                        topRowSettings[j].Effect = EM_LedEffect.Stand;
                        topRowSettings[j].Speed = 10;
                        topRowSettings[j].FontSize = TopRowLEDs[i].FontSize;
                        topRowSettings[j].CurrentColor = (EM_LedColor)TopRowLEDs[i].LedColor;
                        topRowSettings[j].Data = " ";
                    }
                    TopRowILEDs[i].Row_Settings = topRowSettings;
                    TopRowILEDs[i].Number_Of_Line = TopRowILEDs[i].Row_Settings.Length;

                    Row[] bottomRowSettings = new Row[3];
                    for (int j = 0; j < 3; j++)
                    {
                        bottomRowSettings[j] = new Row();
                        bottomRowSettings[j].Effect = EM_LedEffect.Stand;
                        bottomRowSettings[j].Speed = 10;
                        bottomRowSettings[j].FontSize = BottomRowLEDs[i].FontSize;
                        bottomRowSettings[j].CurrentColor = (EM_LedColor)BottomRowLEDs[i].LedColor;
                        bottomRowSettings[j].Data = " ";
                    }
                    BottomRowILEDs[i].Row_Settings = bottomRowSettings;
                    BottomRowILEDs[i].Number_Of_Line = BottomRowILEDs[i].Row_Settings.Length;

                    bool result = TopRowILEDs[i].Set_Screen_Current(TopRowDisplayDatas[i], EM_DisplayMode.ONE_COLOR_EACH_LINE);
                    if (!result)
                    {
                        result = TopRowILEDs[i].Set_Screen_Current(TopRowDisplayDatas[i], EM_DisplayMode.ONE_COLOR_EACH_LINE);
                        if (!result)
                        {
                            LogHelper.Logger_Error($"Send Data to {BottomRowILEDs[i].ComPort} Datas:{string.Join(",", BottomDisplayDatas[i])} Error");
                        }
                    }

                    result = BottomRowILEDs[i].Set_Screen_Current(BottomDisplayDatas[i], EM_DisplayMode.ONE_COLOR_EACH_LINE);
                    if (!result)
                    {
                        result = BottomRowILEDs[i].Set_Screen_Current(BottomDisplayDatas[i], EM_DisplayMode.ONE_COLOR_EACH_LINE);
                        if (!result)
                        {
                            LogHelper.Logger_Error($"Send Data to {BottomRowILEDs[i].ComPort} Datas:{string.Join(",", BottomDisplayDatas[i])} Error");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Logger_Error(ex.Message);
            }

        }

        private static void LoadLedDisplayData(List<EventData> eventDatas, out List<string[]> TopRowDisplayDatas, out List<string[]> BottomDisplayDatas, int scrollIndex = 0)
        {
            try
            {
                while (eventDatas.Count < 6)
                {
                    eventDatas.Add(new EventData()
                    {
                        ChinaPlateNumber = "",
                        VietNamPlateNumber = "",
                        ServiceCode = "",
                        Status = EM_ParkingStatus.UNKNOWN,
                        ParkingPosition = "",
                        Group = "",
                    });
                }
                string[] TopRowSttDatas = new string[3];
                string[] TopRowServiceCodeDatas = new string[3];
                string[] TopRowChinaPlateDatas = new string[3];
                string[] TopRowVietNamPlateDatas = new string[3];
                string[] TopRowParkingPositionDatas = new string[3];
                string[] TopRowGroupDatas = new string[3];
                string[] TopRowStatusDatas = new string[3];

                string[] BottomRowSttDatas = new string[3];
                string[] BottomRowServiceCodeDatas = new string[3];
                string[] BottomRowChinaPlateDatas = new string[3];
                string[] BottomRowVietNamPlateDatas = new string[3];
                string[] BottomRowParkingPositionDatas = new string[3];
                string[] BottomRowGroupDatas = new string[3];
                string[] BottomRowStatusDatas = new string[3];

                for (int i = 0; i < 3; i++)
                {

                    TopRowSttDatas[i] = (1 + i + scrollIndex) > eventDatas.Count ? (1 + i + scrollIndex - eventDatas.Count).ToString() : (1 + i + scrollIndex).ToString();
                    TopRowServiceCodeDatas[i] = i + scrollIndex >= eventDatas.Count ? eventDatas[i + scrollIndex - eventDatas.Count].ServiceCode : eventDatas[i + scrollIndex].ServiceCode;
                    TopRowServiceCodeDatas[i] = TopRowServiceCodeDatas[i] == "" ? " " : TopRowServiceCodeDatas[i];
                    
                    TopRowChinaPlateDatas[i] = i + scrollIndex >= eventDatas.Count ? eventDatas[i + scrollIndex - eventDatas.Count].ChinaPlateNumber : eventDatas[i + scrollIndex].ChinaPlateNumber;
                    TopRowChinaPlateDatas[i] = TopRowChinaPlateDatas[i] == "" ? " " : TopRowChinaPlateDatas[i];
                    
                    TopRowVietNamPlateDatas[i] = i + scrollIndex >= eventDatas.Count ? eventDatas[i + scrollIndex - eventDatas.Count].VietNamPlateNumber : eventDatas[i + scrollIndex].VietNamPlateNumber;
                    TopRowVietNamPlateDatas[i] = TopRowVietNamPlateDatas[i] == "" ? " " : TopRowVietNamPlateDatas[i];
                    
                    TopRowParkingPositionDatas[i] = i + scrollIndex >= eventDatas.Count ? eventDatas[i + scrollIndex - eventDatas.Count].ParkingPosition : eventDatas[i + scrollIndex].ParkingPosition;
                    TopRowParkingPositionDatas[i] = TopRowParkingPositionDatas[i] == "" ? " " : TopRowParkingPositionDatas[i];
                    
                    TopRowGroupDatas[i] = i + scrollIndex >= eventDatas.Count ? eventDatas[i + scrollIndex - eventDatas.Count].Group : eventDatas[i + scrollIndex].Group;
                    TopRowGroupDatas[i] = TopRowGroupDatas[i] == "" ? " " : TopRowGroupDatas[i];
                    
                    TopRowStatusDatas[i] = i + scrollIndex >= eventDatas.Count ? StaticPool.GetStatusName(eventDatas[i + scrollIndex - eventDatas.Count].Status) : StaticPool.GetStatusName(eventDatas[i + scrollIndex].Status);
                    TopRowStatusDatas[i] = TopRowStatusDatas[i] == "" ? " " : TopRowStatusDatas[i];


                    BottomRowSttDatas[i] = (6 - 2 + i + scrollIndex) > eventDatas.Count ? (6 - 2 + i + scrollIndex - eventDatas.Count).ToString() : (6 - 2 + i + scrollIndex).ToString();
                    BottomRowServiceCodeDatas[i] = 3 + i + scrollIndex >= eventDatas.Count ? eventDatas[3 + i + scrollIndex - eventDatas.Count].ServiceCode : eventDatas[3 + i + scrollIndex].ServiceCode;
                    BottomRowServiceCodeDatas[i] = BottomRowServiceCodeDatas[i] == "" ? " " : BottomRowServiceCodeDatas[i];

                    BottomRowChinaPlateDatas[i] = 3 + i + scrollIndex >= eventDatas.Count ? eventDatas[3 + i + scrollIndex - eventDatas.Count].ChinaPlateNumber : eventDatas[3 + i + scrollIndex].ChinaPlateNumber;
                    BottomRowChinaPlateDatas[i] = BottomRowChinaPlateDatas[i] == "" ? " " : BottomRowChinaPlateDatas[i];

                    BottomRowVietNamPlateDatas[i] = 3 + i + scrollIndex >= eventDatas.Count ? eventDatas[3 + i + scrollIndex - eventDatas.Count].VietNamPlateNumber : eventDatas[3 + i + scrollIndex].VietNamPlateNumber;
                    BottomRowVietNamPlateDatas[i] = BottomRowVietNamPlateDatas[i] == "" ? " " : BottomRowVietNamPlateDatas[i];

                    BottomRowParkingPositionDatas[i] = 3 + i + scrollIndex >= eventDatas.Count ? eventDatas[3 + i + scrollIndex - eventDatas.Count].ParkingPosition : eventDatas[3 + i + scrollIndex].ParkingPosition;
                    BottomRowParkingPositionDatas[i] = BottomRowParkingPositionDatas[i] == "" ? " " : BottomRowParkingPositionDatas[i];

                    BottomRowGroupDatas[i] = 3 + i + scrollIndex >= eventDatas.Count ? eventDatas[3 + i + scrollIndex - eventDatas.Count].Group : eventDatas[3 + i + scrollIndex].Group;
                    BottomRowGroupDatas[i] = BottomRowGroupDatas[i] == "" ? " " : BottomRowGroupDatas[i];

                    BottomRowStatusDatas[i] = 3 + i + scrollIndex >= eventDatas.Count ? StaticPool.GetStatusName(eventDatas[3 + i + scrollIndex - eventDatas.Count].Status) : StaticPool.GetStatusName(eventDatas[3 + i + scrollIndex].Status);
                    BottomRowStatusDatas[i] = BottomRowStatusDatas[i] == "" ? " " : BottomRowStatusDatas[i];
                }

                TopRowDisplayDatas = new List<string[]>() {
                                TopRowSttDatas,
                                TopRowServiceCodeDatas,
                                TopRowChinaPlateDatas,
                                TopRowVietNamPlateDatas,
                                TopRowParkingPositionDatas,
                                TopRowGroupDatas,
                                TopRowStatusDatas
                           };
                BottomDisplayDatas = new List<string[]>()
                           {
                                BottomRowSttDatas,
                                BottomRowServiceCodeDatas,
                                BottomRowChinaPlateDatas,
                                BottomRowVietNamPlateDatas,
                                BottomRowParkingPositionDatas,
                                BottomRowGroupDatas,
                                BottomRowStatusDatas
                           };
            }
            catch (Exception ex)
            {
                LogHelper.Logger_Error(ex.Message);
                TopRowDisplayDatas = new List<string[]>();
                BottomDisplayDatas = new List<string[]>();
            }
        }

        private void CheckLedControllersScreenResolution(List<ILED> TopRowILEDs, List<ILED> BottomRowILEDs)
        {
            try
            {
                for (int i = 0; i < TopRowILEDs.Count; i++)
                {
                    if (TopRowILEDs != null)
                    {
                        string resolution = TopRowILEDs[i].Get_Screen_Resolution();
                        if (resolution != "")
                        {
                            int row = Convert.ToInt32(resolution.Split("=")[1].Substring(0, 2).Trim());
                            int col = Convert.ToInt32(resolution.Split("=")[2].Substring(0, 2).Trim());
                            if (row != TopRowLEDs[i].Row || col != TopRowLEDs[i].Column)
                            {
                                TopRowILEDs[i].Set_Screen_Resolution(TopRowLEDs[i].Row, TopRowLEDs[i].Column);
                            }
                        }
                    }

                }

                for (int i = 0; i < BottomRowILEDs.Count; i++)
                {
                    if (BottomRowILEDs[i] != null)
                    {
                        string resolution = BottomRowILEDs[i].Get_Screen_Resolution();
                        if (resolution != "")
                        {
                            int row = Convert.ToInt32(resolution.Split("=")[1].Substring(0, 2).Trim());
                            int col = Convert.ToInt32(resolution.Split("=")[2].Substring(0, 2).Trim());
                            if (row != BottomRowLEDs[i].Row || col != BottomRowLEDs[i].Column)
                            {
                                BottomRowILEDs[i].Set_Screen_Resolution(BottomRowLEDs[i].Row, BottomRowLEDs[i].Column);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Logger_Error(ex.Message);

            }

        }

        private void LoadLedControllers(out List<ILED> TopRowILEDs, out List<ILED> BottomRowILEDs)
        {
            try
            {
                BottomRowILEDs = new List<ILED>();
                TopRowILEDs = new List<ILED>();
                foreach (LED led in TopRowLEDs)
                {
                    TopRowILEDs.Add(LedFactory.GetLedController(led.moduleType, led.IP, led.Port));
                }
                BottomRowILEDs = new List<ILED>();
                foreach (LED led in BottomRowLEDs)
                {
                    BottomRowILEDs.Add(LedFactory.GetLedController(led.moduleType, led.IP, led.Port));
                }
            }
            catch (Exception ex)
            {
                BottomRowILEDs = new List<ILED>();
                TopRowILEDs = new List<ILED>();
                LogHelper.Logger_Error(ex.Message);

            }

        }

        private void LoadLedData()
        {
            try
            {
                cbLedSTT_TOP_Line1_Name?.Invoke(new Action(() =>
                {
                    led_STT_Top = StaticPool.leds.GetLed(((ListItem)cbLedSTT_TOP_Line1_Name.SelectedItem).Value);
                }));

                cbLedSTT_BOTTOM_Line1_Name?.Invoke(new Action(() =>
                {
                    led_STT_Bottom = StaticPool.leds.GetLed(((ListItem)cbLedSTT_BOTTOM_Line1_Name.SelectedItem).Value);
                }));

                cbLedServiceCode_TOP_Line1_Name?.Invoke(new Action(() =>
                {
                    led_ServiceCode_Top = StaticPool.leds.GetLed(((ListItem)cbLedServiceCode_TOP_Line1_Name.SelectedItem).Value);
                }));

                cbLedServiceCode_BOTTOM_Line1_Name?.Invoke(new Action(() =>
                {
                    led_ServiceCode_Bottom = StaticPool.leds.GetLed(((ListItem)cbLedServiceCode_BOTTOM_Line1_Name.SelectedItem).Value);
                }));

                cbLedChinaPlateNum_TOP_Line1_Name?.Invoke(new Action(() =>
                {
                    led_ChinaPlate_Top = StaticPool.leds.GetLed(((ListItem)cbLedChinaPlateNum_TOP_Line1_Name.SelectedItem).Value);
                }));

                cbLedChinaPlateNum_BOTTOM_Line1_Name?.Invoke(new Action(() =>
                {
                    led_ChinaPlate_Bottom = StaticPool.leds.GetLed(((ListItem)cbLedChinaPlateNum_BOTTOM_Line1_Name.SelectedItem).Value);
                }));

                cbLedVietNamPlateNum_TOP_Line1_Name?.Invoke(new Action(() =>
                {
                    led_VietNamPlate_Top = StaticPool.leds.GetLed(((ListItem)cbLedVietNamPlateNum_TOP_Line1_Name.SelectedItem).Value);
                }));

                cbLedVietNamPlateNum_BOTTOM_Line1_Name?.Invoke(new Action(() =>
                {
                    led_VietNamPlate_Bottom = StaticPool.leds.GetLed(((ListItem)cbLedVietNamPlateNum_BOTTOM_Line1_Name.SelectedItem).Value);
                }));

                cbLedParkingPosition_TOP_Line1_Name?.Invoke(new Action(() =>
                {
                    led_Position_Top = StaticPool.leds.GetLed(((ListItem)cbLedParkingPosition_TOP_Line1_Name.SelectedItem).Value);
                }));

                cbLedParkingPosition_BOTTOM_Line1_Name?.Invoke(new Action(() =>
                {
                    led_Position_Bottom = StaticPool.leds.GetLed(((ListItem)cbLedParkingPosition_BOTTOM_Line1_Name.SelectedItem).Value);
                }));

                cbLedGroup_TOP_Line1_Name?.Invoke(new Action(() =>
                {
                    led_Group_Top = StaticPool.leds.GetLed(((ListItem)cbLedGroup_TOP_Line1_Name.SelectedItem).Value);
                }));

                cbLedGroup_BOTTOM_Line1_Name?.Invoke(new Action(() =>
                {
                    led_Group_Bottom = StaticPool.leds.GetLed(((ListItem)cbLedGroup_BOTTOM_Line1_Name.SelectedItem).Value);
                }));

                cbLedStatus_TOP_Line1_Name?.Invoke(new Action(() =>
                {
                    led_Status_Top = StaticPool.leds.GetLed(((ListItem)cbLedStatus_TOP_Line1_Name.SelectedItem).Value);
                }));

                cbLedStatus_BOTTOM_Line1_Name?.Invoke(new Action(() =>
                {
                    led_Status_Bottom = StaticPool.leds.GetLed(((ListItem)cbLedStatus_BOTTOM_Line1_Name.SelectedItem).Value);
                }));

                TopRowLEDs = new List<LED>()
                            {
                                led_STT_Top,
                                led_ServiceCode_Top,
                                led_ChinaPlate_Top,
                                led_VietNamPlate_Top,
                                led_Position_Top,
                                led_Group_Top,
                                led_Status_Top,
                            };
                BottomRowLEDs = new List<LED>()
                            {
                                led_STT_Bottom,
                                led_ServiceCode_Bottom,
                                led_ChinaPlate_Bottom,
                                led_VietNamPlate_Bottom,
                                led_Position_Bottom,
                                led_Group_Bottom,
                                led_Status_Bottom,
                            };

            }
            catch (Exception ex)
            {
                LogHelper.Logger_Error(ex.Message);
            }

        }

        private void chbIsAutoStart_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.isAutoStart = chbIsAutoStart.Checked;
            Properties.Settings.Default.Save();
        }

        public static int index = 0;

    }
}
