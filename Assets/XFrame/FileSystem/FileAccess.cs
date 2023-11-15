using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace SaveSystem
{
    public static class FileAccess
    {
        private static string BasePath => Path.Combine(GlobalSettings.StorageLocation, "SaveSystem");

        /// <summary>
        /// 写日志
        /// </summary>
        public static bool WriteLog(string strLog)
        {
            //创建一个年月的文件夹，日.log文件
            string sFilePath = Path.Combine(BasePath, "Log", DateTime.Now.ToString("Y"));
            string sFileName = DateTime.Now.ToString("D") + ".log";
            //绝对路径
            sFileName = Path.Combine(sFilePath, sFileName);
            try
            {
                //验证文件是否存在
                if (!Directory.Exists(sFilePath))
                {
                    //不存在则创建
                    Directory.CreateDirectory(sFilePath);
                }
                FileStream fs;
                StreamWriter sw;
                if (File.Exists(sFileName))
                {
                    fs = new FileStream(sFileName, FileMode.Append, System.IO.FileAccess.Write);
                }
                else
                {
                    fs = new FileStream(sFileName, FileMode.Create, System.IO.FileAccess.Write);
                }
                sw = new StreamWriter(fs);
                sw.WriteLine(DateTime.Now.ToString("F") + "   ---    " + strLog);
                sw.Close();
                fs.Close();
                return true;
            }
            catch (Exception ex)
            {
                Debug.Log(ex.ToString());
            }
            return false;
        }
        /// <summary>
        /// 保存字符串
        /// </summary>
        public static bool SaveString(string filename, string extensions, string value)
        {
            filename = GetFileFullName(filename, extensions);

            try
            {
                CreateRootFolder();

                using (StreamWriter writer = new StreamWriter(Path.Combine(BasePath, filename)))
                {
                    writer.Write(value);
                }

                return true;
            }
            catch
            {
            }

            return false;
        }

        public static bool SaveBytes(string filename, string extensions, byte[] value)
        {
            filename = GetFileFullName(filename, extensions);

            try
            {
                CreateRootFolder();

                using (FileStream fileStream = new FileStream(Path.Combine(BasePath, filename), FileMode.Create))
                {
                    fileStream.Write(value, 0, value.Length);
                }

                return true;
            }
            catch
            {
            }

            return false;
        }

        public static string LoadString(string filename, string extensions)
        {
            filename = GetFileFullName(filename, extensions);

            try
            {
                CreateRootFolder();

                if (Exists(filename, extensions))
                {
                    using (StreamReader reader = new StreamReader(Path.Combine(BasePath, filename)))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
            catch
            {
            }

            return null;
        }

        public static byte[] LoadBytes(string filename, string extensions)
        {
            filename = GetFileFullName(filename, extensions);

            try
            {
                CreateRootFolder();

                if (Exists(filename, extensions))
                {
                    using (FileStream fileStream = new FileStream(Path.Combine(BasePath, filename), FileMode.Open))
                    {
                        byte[] buffer = new byte[fileStream.Length];

                        fileStream.Read(buffer, 0, buffer.Length);

                        return buffer;
                    }
                }
            }
            catch
            {
            }

            return null;
        }

        public static void Delete(string filename, string extensions)
        {
            filename = GetFileFullName(filename, extensions);

            try
            {
                CreateRootFolder();

                string fileLocation = Path.Combine(BasePath, filename);

                File.Delete(fileLocation);
            }
            catch
            {
            }
        }

        public static bool Exists(string filename, string extensions)
        {
            filename = GetFileFullName(filename, extensions);

            string fileLocation = Path.Combine(BasePath, filename);

            return File.Exists(fileLocation);
        }

        public static IEnumerable<string> Files(string extensions)
        {
            try
            {
                CreateRootFolder();

                return Directory.GetFiles(BasePath, string.Format("*.{0}", extensions)).Select(x => Path.GetFileName(x));
            }
            catch
            {
            }

            return new List<string>();
        }

        public static string GetFileFullName(string filename, string extension)
        {
            return filename + extension;
        }

        private static void CreateRootFolder()
        {
            if (!Directory.Exists(BasePath))
            {
                Directory.CreateDirectory(BasePath);
            }
        }
    }
}