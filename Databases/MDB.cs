using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Databases
{
    public class MDB
    {
        int timeout = 0;

        // MS SQL SERVER
        private string SQLServerName = @"DCTHANH\SQLEXPRESS";
        private string SQLDatabaseName = "";
        private string SQLAuthentication = "Windows Authentication";
        private string SQLUserName = "sa";
        private string SQLPassword = "123456";

        // MS ACCESS
        private string MDBFilePath = "";
        private string MDBPassword = "17032008";

        private SqlConnection sqlconn;
        private OleDbConnection mdbconn = null;

        private bool IsMDB = false;

        public MDB()
        {
        }

        public MDB(string sqlservername, string sqldatabasename, string sqlauthentication, string sqlusername, string sqlpassword, string mdbfilepath, string mdbpassword, bool ismdb)
        {
            // MS SQL SERVER
            SQLServerName = sqlservername;
            SQLDatabaseName = sqldatabasename;
            SQLAuthentication = sqlauthentication;
            SQLUserName = sqlusername;
            SQLPassword = sqlpassword;

            // MS ACCESS
            MDBFilePath = mdbfilepath;
            MDBPassword = mdbpassword;

            IsMDB = ismdb;

            //OpenMDB();//ref sqlconn, ref mdbconn);
        }

        // MS SQL SERVER
        public MDB(string sqlservername, string sqldatabasename, string sqlauthentication, string sqlusername, string sqlpassword)
        {
            SQLServerName = sqlservername;
            SQLDatabaseName = sqldatabasename;
            SQLAuthentication = sqlauthentication;
            SQLUserName = sqlusername;
            SQLPassword = sqlpassword;

            IsMDB = false;
            //OpenMDB();//ref sqlconn, ref mdbconn);
        }

        // MS ACCESS
        public MDB(string mdbfilepath, string mdbpassword)
        {
            MDBFilePath = mdbfilepath;
            MDBPassword = mdbpassword;

            IsMDB = true;
        }
        // Mo ket noi den co so du lieu
        public bool OpenMDB()//ref SqlConnection sqlconn, ref OleDbConnection mdbconn)
        {
            if (!IsMDB)
            {
            reconnect:
                try
                {
                    timeout = timeout + 1;
                    // Gan chuoi ket noi vao bien
                    string strConn = "";
                    if (SQLAuthentication == "Windows Authentication")
                    {
                        strConn = "Data Source=" + SQLServerName + ";" +
                                         "Initial Catalog=" + SQLDatabaseName + ";" +
                                         "Integrated Security=true;Pooling=False" +
                                         ";MultipleActiveResultSets=True";

                    }
                    else
                    {
                        strConn = "server=" + SQLServerName + ";database=" + SQLDatabaseName + ";uid=" + SQLUserName + ";pwd=" + SQLPassword + ";MultipleActiveResultSets=True";
                    }
                    // Thuc thi chuoi ket noi
                    sqlconn = new SqlConnection(strConn);
                    // Mo ket noi
                    sqlconn.Open();
                    if (sqlconn.State == ConnectionState.Open)
                        return true;
                }
                catch (Exception ex)
                {

                    Thread.Sleep(1000);
                    if (timeout < 2)
                        goto reconnect;
                }
            }
            else
            {
                try
                {
                    // Thuc thi chuoi ket noi
                    mdbconn = new OleDbConnection(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source='" + MDBFilePath + "';Persist Security Info=false;Jet OLEDB:Database Password=" + MDBPassword);
                    // Mo ket noi
                    mdbconn.Open();
                    if (mdbconn.State == ConnectionState.Open)
                        return true;
                }
                catch
                {
                    //  SystemUI.SaveLogFile("OpenMDB\n" + ex.Message);
                    // MessageBox.Show("OpenMDB\n" + ex.Message);
                }
            }
            return false;
        }

        // Dong ket noi den co so du lieu
        public void CloseMDB()
        {
            try
            {
                if (sqlconn != null)
                {
                    if (sqlconn.State == ConnectionState.Open)
                        sqlconn.Close();
                }
                if (mdbconn != null)
                {
                    if (mdbconn.State == ConnectionState.Open)
                        mdbconn.Close();
                }
            }
            catch
            {
                //nothing
            }
        }

        // Execute Command
        public bool ExecuteCommand(string commandString)
        {
            if (!IsMDB)
            {
                if (State())
                {
                    try
                    {
                        SqlCommand sqlCommand = new SqlCommand(commandString, sqlconn);
                        sqlCommand.ExecuteNonQuery();
                        //sqlconn.Close();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        //  SystemUI.SaveLogFile("ExecuteCommand\n" + ex.Message + "\n" + commandString);
                        MessageBox.Show("ExecuteCommand\n" + ex.Message + "\n" + commandString);
                    }
                }
                else if (OpenMDB())
                {
                    try
                    {
                        SqlCommand sqlCommand = new SqlCommand(commandString, sqlconn);
                        sqlCommand.ExecuteNonQuery();
                        //sqlconn.Close();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        //  SystemUI.SaveLogFile("ExecuteCommand\n" + ex.Message + "\n" + commandString);
                        MessageBox.Show("ExecuteCommand\n" + ex.Message + "\n" + commandString);
                    }
                }

            }
            else
            {
                if (StateMDB())
                {
                    try
                    {
                        OleDbCommand cmd = new OleDbCommand(commandString, mdbconn);
                        cmd.ExecuteNonQuery();
                        //mdbconn.Close();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        // SystemUI.SaveLogFile("ExecuteCommand\n" + ex.Message + "\n" + commandString);
                        MessageBox.Show("ExecuteCommand\n" + ex.Message + "\n" + commandString);
                    }
                }
                else if (OpenMDB())
                {
                    try
                    {
                        OleDbCommand cmd = new OleDbCommand(commandString, mdbconn);
                        cmd.ExecuteNonQuery();
                        //mdbconn.Close();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        //  SystemUI.SaveLogFile("ExecuteCommand\n" + ex.Message + "\n" + commandString);
                        MessageBox.Show("ExecuteCommand\n" + ex.Message + "\n" + commandString);
                    }
                }
            }
            return false;
        }

        // Excute command
        public bool ExecuteCommand(string commandString, string parameters, byte[] values)
        {
            if (!IsMDB)
            {
                if (State())
                {
                    SqlCommand sqlCommand = new SqlCommand(commandString, sqlconn);
                    sqlCommand.Parameters.Add(parameters, SqlDbType.Image);
                    if (values != null)
                        sqlCommand.Parameters[parameters].Value = values;
                    else
                        sqlCommand.Parameters[parameters].Value = DBNull.Value;
                    try
                    {
                        sqlCommand.ExecuteNonQuery();
                        //sqlconn.Close();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("ExecuteCommand\n" + ex.Message + "\n" + commandString);
                    }
                }
                else if (OpenMDB())
                {
                    SqlCommand sqlCommand = new SqlCommand(commandString, sqlconn);
                    sqlCommand.Parameters.Add(parameters, SqlDbType.Image);
                    if (values != null)
                        sqlCommand.Parameters[parameters].Value = values;
                    else
                        sqlCommand.Parameters[parameters].Value = DBNull.Value;
                    try
                    {
                        sqlCommand.ExecuteNonQuery();
                        //sqlconn.Close();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("ExecuteCommand\n" + ex.Message + "\n" + commandString);
                    }
                }
            }
            else
            {
                if (StateMDB())
                {
                    OleDbCommand cmd = new OleDbCommand(commandString, mdbconn);
                    cmd.Parameters.Add(parameters, OleDbType.Binary);
                    if (values != null)
                        cmd.Parameters[parameters].Value = values;
                    else
                        cmd.Parameters[parameters].Value = DBNull.Value;
                    try
                    {
                        cmd.ExecuteNonQuery();
                        //mdbconn.Close();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("ExecuteCommand\n" + ex.Message + "\n" + commandString);
                    }
                }
                else if (OpenMDB())
                {
                    OleDbCommand cmd = new OleDbCommand(commandString, mdbconn);
                    cmd.Parameters.Add(parameters, OleDbType.Binary);
                    if (values != null)
                        cmd.Parameters[parameters].Value = values;
                    else
                        cmd.Parameters[parameters].Value = DBNull.Value;
                    try
                    {
                        cmd.ExecuteNonQuery();
                        //mdbconn.Close();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("ExecuteCommand\n" + ex.Message + "\n" + commandString);
                    }
                }
            }

            return false;
        }

        // Lay du lieu tu bang hoac thu tuc
        public DataTable FillData(string commandString)
        {
            if (!IsMDB)
            {
                if (State())
                {
                    try
                    {
                        SqlDataAdapter dataAdapter = new SqlDataAdapter(commandString, sqlconn);
                        DataSet dataSet = new DataSet();
                        dataAdapter.Fill(dataSet);
                        dataAdapter.Dispose();
                        //sqlconn.Close();
                        return dataSet.Tables[0];
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("FillData\n" + ex.Message + "\n" + commandString);
                    }
                }
                else if (OpenMDB())
                {
                    try
                    {
                        SqlDataAdapter dataAdapter = new SqlDataAdapter(commandString, sqlconn);
                        DataSet dataSet = new DataSet();
                        dataAdapter.Fill(dataSet);
                        dataAdapter.Dispose();
                        //sqlconn.Close();
                        return dataSet.Tables[0];
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("FillData\n" + ex.Message + "\n" + commandString);
                    }
                }
            }
            else
            {
                if (StateMDB())
                {
                    try
                    {
                        OleDbDataAdapter dataAdapter = new OleDbDataAdapter(commandString, mdbconn);
                        DataSet dataSet = new DataSet();
                        dataAdapter.Fill(dataSet);
                        dataAdapter.Dispose();
                        //mdbconn.Close();
                        return dataSet.Tables[0];
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("FillData\n" + ex.Message + "\n" + commandString);
                    }
                }
                else if (OpenMDB())
                {
                    try
                    {
                        OleDbDataAdapter dataAdapter = new OleDbDataAdapter(commandString, mdbconn);
                        DataSet dataSet = new DataSet();
                        dataAdapter.Fill(dataSet);
                        dataAdapter.Dispose();
                        //mdbconn.Close();
                        return dataSet.Tables[0];
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("FillData\n" + ex.Message + "\n" + commandString);
                    }
                }
            }
            return null;
        }

        // Hien thi du lieu trong ComboBox (Code & Name)
        public void LoadObject(string commandString, ComboBox cbOject, bool IsDropDownList)
        {
            try
            {
                cbOject.Items.Clear();
                ListItem listitem = new ListItem();
                listitem.Value = "#";
                listitem.Name = "#";
                cbOject.Items.Add(listitem);

                DataTable dt = FillData(commandString);
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        DataRowView _drv = dt.DefaultView[i];
                        listitem = new ListItem();
                        listitem.Value = _drv["Code"].ToString();
                        listitem.Name = _drv["Code"].ToString() + ". " + _drv["Name"].ToString();
                        cbOject.Items.Add(listitem);
                    }
                    cbOject.DisplayMember = "Name";
                    cbOject.ValueMember = "Value";
                }

                cbOject.SelectedIndex = 0;
                if (IsDropDownList)
                    cbOject.DropDownStyle = ComboBoxStyle.DropDownList;
            }
            catch (Exception ex)
            {
                MessageBox.Show("LoadObject\n" + ex.Message + "\n" + commandString);
            }
        }

        public void LoadObject2(string commandString, ComboBox cbOject, string obj1, string obj2, bool IsDropDownList)
        {
            try
            {
                cbOject.Items.Clear();
                ListItem listitem = new ListItem();
                listitem.Value = "";
                listitem.Name = "";
                cbOject.Items.Add(listitem);

                DataTable dt = FillData(commandString);
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        DataRowView _drv = dt.DefaultView[i];
                        listitem = new ListItem();
                        listitem.Value = _drv[obj1].ToString().Trim();

                        listitem.Name = _drv[obj2].ToString().Trim();

                        cbOject.Items.Add(listitem);
                    }

                    dt.Dispose();
                }
                cbOject.DisplayMember = "Name";
                cbOject.ValueMember = "Value";
                // cbOject.Text = str;
                cbOject.SelectedIndex = 0;
                if (IsDropDownList)
                    cbOject.DropDownStyle = ComboBoxStyle.DropDownList;
            }
            catch (Exception ex)
            {
                //SystemUI.SaveLogFile("LoadObject\n" + ex.Message + "\n" + commandString);
                MessageBox.Show("LoadObject\n" + ex.Message + "\n" + commandString);
            }
        }

        // Hien thi danh sach code trong ComboBox
        public void DisplayCode(string commandString, ComboBox cbCode, string format, int count, bool isDropDownList, string fieldName, string defaultvalue)
        {
            try
            {
                // Hien thi danh sach code
                if (isDropDownList)
                    cbCode.DropDownStyle = ComboBoxStyle.DropDownList;
                cbCode.Items.Clear();
                if (defaultvalue != "")
                    cbCode.Items.Add(defaultvalue);
                DataTable dt = FillData(commandString);
                for (int i = 1; i < count; i++)
                {
                    bool IsExist = false;
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        foreach (DataRow drv in dt.Rows)
                        {
                            if (drv[fieldName].ToString() == i.ToString(format))
                                IsExist = true;
                        }
                    }
                    if (!IsExist)
                        cbCode.Items.Add(i.ToString(format));
                }
                if (cbCode.Items.Count > 0)
                    if (defaultvalue == "")
                        cbCode.SelectedIndex = 0;
                    else
                        cbCode.Text = defaultvalue;
            }
            catch (Exception ex)
            {
                // SystemUI.SaveLogFile("DisplayCode\n" + ex.Message + "\n" + commandString);
                MessageBox.Show("DisplayCode\n" + ex.Message + "\n" + commandString);
            }
        }

        //Kiem tra trang thai ket noi SQLServer
        public bool State()
        {
            try
            {
                if (sqlconn != null)
                {
                    if (sqlconn.State == ConnectionState.Open)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        //Kiem tra trang thai ket noi cua MSACCESS
        public bool StateMDB()
        {
            try
            {
                if (mdbconn != null)
                {
                    if (mdbconn.State == ConnectionState.Open)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
