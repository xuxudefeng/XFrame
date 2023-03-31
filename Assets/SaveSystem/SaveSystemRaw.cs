using System;
using System.Collections.Generic;
using SaveSystem.Serialisers;
using SaveSystem.Settings;
using SaveSystem.Storage;

namespace SaveSystem
{
    public class SaveSystemRaw
    {
        /// <summary>
        /// 读取Json文件
        /// </summary>
        public static T Load<T>(string filename)
        {
            var json = LoadString(filename, new SaveSystemSettings());
            return Json.Deserialise<T>(json);
        }
        /// <summary>
        /// 保存Json文件
        /// </summary>
        public static void Save<T>(string filename, T t)
        {
            var json = Json.Serialise<T>(t);
            SaveString(filename, json);
        }
        /// <summary>
        /// 保存Log文件
        /// </summary>
        public static void SaveLog(string log)
        {
            if (!FileAccess.WriteLog(log))
            {
                throw new SaveSystemException("无法将日志写入文件");
            }
        }
        /// <summary>
        /// 保存Csv文件
        /// </summary>
        public static void SaveCsv<T>(string filename, IEnumerable<T> content)
        {
            if (!FileAccess.SaveCsv<T>(filename, content))
            {
                throw new SaveSystemException("写入文件失败");
            }
        }
        /// <summary>
        /// 保存字符串
        /// </summary>
        public static void SaveString(string filename, string text)
        {
            SaveString(filename, text, new SaveSystemSettings());
        }
        /// <summary>
        /// 保存字符串
        /// </summary>
        public static void SaveString(string filename, string content, SaveSystemSettings settings)
        {
            string contentToWrite;

            try
            {
                contentToWrite = Compression.Compress(content, settings.CompressionMode);
            }
            catch (Exception e)
            {
                throw new SaveSystemException("压缩失败", e);
            }

            // Gzip 无论如何都会输出 base64，因此无需执行两次
            if (settings.CompressionMode != CompressionMode.Gzip || settings.SecurityMode != SecurityMode.Base64)
            {
                try
                {
                    contentToWrite = Cryptography.Encrypt(contentToWrite, settings.SecurityMode, settings.Password);
                }
                catch (Exception e)
                {
                    throw new SaveSystemException("加密失败", e);
                }
            }

            if (!FileAccess.SaveString(filename, true, contentToWrite))
            {
                throw new SaveSystemException("写入文件失败");
            }
        }

        public static void SaveBytes(string filename, byte[] content)
        {
            if (!FileAccess.SaveBytes(filename, true, content))
            {
                throw new SaveSystemException("写入文件失败");
            }
        }

        public static string LoadString(string filename)
        {
            return LoadString(filename, new SaveSystemSettings());
        }

        public static string LoadString(string filename, SaveSystemSettings settings)
        {
            var content = FileAccess.LoadString(filename, true);

            if (content == null)
            {
                throw new SaveSystemException("加载文件失败");
            }

            // Gzip 无论如何都会解析 base64，所以不需要解析两次
            if (settings.CompressionMode != CompressionMode.Gzip || settings.SecurityMode != SecurityMode.Base64)
            {
                try
                {
                    content = Cryptography.Decrypt(content, settings.SecurityMode, settings.Password);
                }
                catch (Exception e)
                {
                    throw new SaveSystemException("解密失败", e);
                }
            }

            try
            {
                content = Compression.Decompress(content, settings.CompressionMode);
            }
            catch (Exception e)
            {
                throw new SaveSystemException("解压失败", e);
            }

            return content;
        }
        public static void LoadCsv<T>(string filename, Action<List<T>> action)
        {
            FileAccess.LoadCsv<T>(filename, action);
        }

        public static byte[] LoadBytes(string filename)
        {
            byte[] content = FileAccess.LoadBytes(filename, true);

            if (content == null)
            {
                throw new SaveSystemException("加载文件失败");
            }

            return content;
        }

        public static T LoadResource<T>(string filename) where T : UnityEngine.Object
        {
            return UnityEngine.Resources.Load<T>(filename);
        }

        public static void Delete(string filename)
        {
            FileAccess.Delete(filename, true);
        }

        public static bool Exists(string filename)
        {
            return FileAccess.Exists(filename, true);
        }

        public static IEnumerable<string> GetAllFiles()
        {
            return FileAccess.Files(false);
        }
    }
}