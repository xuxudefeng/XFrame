using System;
using System.Collections.Generic;
using JsonNet;

namespace SaveSystem
{
    public class XFile
    {
        #region Json
        /// <summary>
        /// 读取Json文件
        /// </summary>
        public static T LoadJson<T>(string filename)
        {
            var json = LoadString(filename, ".json", new Settings());
            return Json.Deserialise<T>(json);
        }
        /// <summary>
        /// 保存Json文件
        /// </summary>
        public static void SaveJson<T>(string filename, T t)
        {
            var json = Json.Serialise<T>(t);
            Save(filename, ".json", json, new Settings());
        }
        #endregion

        #region Log
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
        #endregion

        #region String
        public static void SaveString(string filename,string content)
        {
            Save(filename, ".txt", content, new Settings());
        }
        #endregion

        /// <summary>
        /// 保存字符串
        /// </summary>
        public static void Save(string filename, string extensions, string content, Settings settings)
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

            if (!FileAccess.SaveString(filename, extensions, contentToWrite))
            {
                throw new SaveSystemException("写入文件失败");
            }
        }

        public static string LoadString(string filename, string extensions, Settings settings)
        {
            var content = FileAccess.LoadString(filename, extensions);

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

        public static byte[] LoadBytes(string filename)
        {
            byte[] content = FileAccess.LoadBytes(filename, ".bytes");

            if (content == null)
            {
                throw new SaveSystemException("加载文件失败");
            }

            return content;
        }

        public static void SaveBytes(string filename, byte[] content)
        {
            if (!FileAccess.SaveBytes(filename, ".bytes", content))
            {
                throw new SaveSystemException("写入文件失败");
            }
        }

        public static T LoadResource<T>(string filename) where T : UnityEngine.Object
        {
            return UnityEngine.Resources.Load<T>(filename);
        }

        public static void Delete(string filename)
        {
            FileAccess.Delete(filename, "");
        }

        public static bool Exists(string filename, string extensions)
        {
            return FileAccess.Exists(filename, extensions);
        }

        public static IEnumerable<string> GetAllFiles(string extensions)
        {
            return FileAccess.Files(extensions);
        }
    }
}