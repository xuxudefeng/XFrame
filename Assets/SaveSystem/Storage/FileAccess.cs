using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using UnityGameFramework.Runtime;

namespace SaveSystem.Storage
{
    public static class FileAccess
    {
        private const string _defaultExtension = ".json";

        private static string BasePath => Path.Combine(SaveSystemGlobalSettings.StorageLocation, "SaveSystem");

        /// <summary>
        /// 写日志
        /// </summary>
        public static bool WriteLog(string strLog)
        {
            //创建一个年月的文件夹，日.log文件
            string sFilePath = Path.Combine(BasePath,DateTime.Now.ToString("Y"));
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
                Log.Debug(ex.ToString());
            }
            return false;
        }

        public static bool SaveCsv<T>(string filename, IEnumerable<T> records)
        {
            filename = filename + ".csv";

            try
            {
                CreateRootFolder();

                using (var writer = new StreamWriter(Path.Combine(BasePath, filename)))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteRecords(records);
                }
                return true;
            }
            catch
            {   
            }
            return false;
        }
        public static IEnumerable<T> LoadCsv<T>(string filename,Action<List<T>> action)
        {
            filename = filename + ".csv";

            try
            {
                CreateRootFolder();

                if (Exists(filename, true))
                {
                    using (var reader = new StreamReader(Path.Combine(BasePath, filename)))
                    {
                        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                        {
                            var records = csv.GetRecords<T>().ToList();
                            action(records);
                        }
                    }
                }
            }
            catch
            {
            }

            return null;
        }
        /// <summary>
        /// 保存字符串
        /// </summary>
        public static bool SaveString(string filename, bool includesExtension, string value)
        {
            filename = GetFilenameWithExtension(filename, includesExtension);

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

        public static bool SaveBytes(string filename, bool includesExtension, byte[] value)
        {
            filename = GetFilenameWithExtension(filename, includesExtension);

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

        public static string LoadString(string filename, bool includesExtension)
        {
            filename = GetFilenameWithExtension(filename, includesExtension);

            try
            {
                CreateRootFolder();

                if (Exists(filename, true))
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

        public static byte[] LoadBytes(string filename, bool includesExtension)
        {
            filename = GetFilenameWithExtension(filename, includesExtension);

            try
            {
                CreateRootFolder();

                if (Exists(filename, true))
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

        public static void Delete(string filename, bool includesExtension)
        {
            filename = GetFilenameWithExtension(filename, includesExtension);

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

        public static bool Exists(string filename, bool includesExtension)
        {
            filename = GetFilenameWithExtension(filename, includesExtension);

            string fileLocation = Path.Combine(BasePath, filename);

            return File.Exists(fileLocation);
        }

        public static IEnumerable<string> Files(bool includeExtensions)
        {
            try
            {
                CreateRootFolder();

                if (includeExtensions)
                {
                    return Directory.GetFiles(BasePath, "*.json").Select(x => Path.GetFileName(x));
                }
                else
                {
                    return Directory.GetFiles(BasePath, "*.json").Select(x => Path.GetFileNameWithoutExtension(x));
                }
            }
            catch
            {
            }

            return new List<string>();
        }

        private static string GetFilenameWithExtension(string filename, bool includesExtension)
        {
            return includesExtension ? filename : filename + _defaultExtension;
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