using UnityEngine;

namespace SaveSystem
{
    public static class SaveSystemGlobalSettings
    {
        /// <summary>
        /// 保存数据的路径 - 默认为 Application.persistentDataPath
        /// </summary>
        public static string StorageLocation { get; set; } = Application.streamingAssetsPath;
    }
}