using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace iParking
{
    public class LogHelper
    {
        public static void Logger_Error(string s)
        {
            try
            {
                string pathFile = Path.GetDirectoryName(Application.ExecutablePath) + @"\logs\ERROR_LOG_" + DateTime.Now.ToString("dd_MM_yyyy") + ".txt";
                using (StreamWriter writer = new StreamWriter(pathFile, true))
                {
                    try
                    {
                        writer.WriteLine("ERROR " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + " - " + s);
                    }
                    catch
                    {
                    }
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }
        }

        public static void Logger_LedError(string s)
        {
            try
            {
                string pathFile = Path.GetDirectoryName(Application.ExecutablePath) + @"\logs\ERROR_LED_" + DateTime.Now.ToString("dd_MM_yyyy") + ".txt";
                using (StreamWriter writer = new StreamWriter(pathFile, true))
                {
                    try
                    {
                        writer.WriteLine("ERROR " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + " - " + s);
                    }
                    catch
                    {
                    }
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }
        }
        public static void Logger_TCSError(string s)
        {
            try
            {
                string pathFile = Path.GetDirectoryName(Application.ExecutablePath) + @"\logs\ERROR_API_TCS_" + DateTime.Now.ToString("dd_MM_yyyy") + ".txt";
                using (StreamWriter writer = new StreamWriter(pathFile, true))
                {
                    try
                    {
                        writer.WriteLine("ERROR " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + " - " + s);
                    }
                    catch
                    {
                    }
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }
        }
        public static void Logger_Info(string s)
        {
            try
            {
                string pathFile = Path.GetDirectoryName(Application.ExecutablePath) + @"\logs\INFO_LOG_" + DateTime.Now.ToString("dd_MM_yyyy") + ".txt";
                using (StreamWriter writer = new StreamWriter(pathFile, true))
                {
                    try
                    {
                        writer.WriteLine("INFO " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + " - " + s);
                    }
                    catch
                    {
                    }
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }
        }
        public static void Logger_IO_Error(string s)
        {
            try
            {
                string pathFile = Path.GetDirectoryName(Application.ExecutablePath) + @"\logs\IO_Error" + DateTime.Now.ToString("dd_MM_yyyy") + ".txt";
                using (StreamWriter writer = new StreamWriter(pathFile, true))
                {
                    try
                    {
                        writer.WriteLine("INFO " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + " - " + s);
                    }
                    catch
                    {
                    }
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }
        }
        public static void Logger_LEDInfo(string s)
        {
            try
            {
                string pathFile = Path.GetDirectoryName(Application.ExecutablePath) + @"\logs\INFO_LED_" + DateTime.Now.ToString("dd_MM_yyyy") + ".txt";
                using (StreamWriter writer = new StreamWriter(pathFile, true))
                {
                    try
                    {
                        writer.WriteLine("INFO " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + " - " + s);
                    }
                    catch
                    {
                    }
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }
        }
        public static void Logger_TCSInfo(string s)
        {
            try
            {
                string pathFile = Path.GetDirectoryName(Application.ExecutablePath) + @"\logs\INFO_API_TCS_" + DateTime.Now.ToString("dd_MM_yyyy") + ".txt";
                using (StreamWriter writer = new StreamWriter(pathFile, true))
                {
                    try
                    {
                        writer.WriteLine("INFO " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + " - " + s);
                    }
                    catch
                    {
                    }
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }
        }
        public static void Logger_IO_Info(string s)
        {
            try
            {
                string pathFile = Path.GetDirectoryName(Application.ExecutablePath) + @"\logs\IO_" + DateTime.Now.ToString("dd_MM_yyyy") + ".txt";
                using (StreamWriter writer = new StreamWriter(pathFile, true))
                {
                    try
                    {
                        writer.WriteLine("INFO " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + " - " + s);
                    }
                    catch
                    {
                    }
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }
        }
        public static void Logger_Warn(string s)
        {
            try
            {
                string pathFile = Path.GetDirectoryName(Application.ExecutablePath) + @"\logs\WARN_LOG_" + DateTime.Now.ToString("dd_MM_yyyy") + ".txt";
                using (StreamWriter writer = new StreamWriter(pathFile, true))
                {
                    try
                    {
                        writer.WriteLine("WARN " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + " - " + s);
                    }
                    catch
                    {
                    }
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }
        }
    }
}
