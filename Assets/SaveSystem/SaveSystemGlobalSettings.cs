using UnityEngine;

namespace SaveSystem
{
    public static class SaveSystemGlobalSettings
    {
        /// <summary>
        /// The path to save data to - defaults to Application.persistentDataPath
        /// </summary>
        public static string StorageLocation { get; set; } = Application.streamingAssetsPath;
    }
}