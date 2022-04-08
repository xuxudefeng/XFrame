using System;
using SaveSystem.Core.Serialisers;

namespace SaveSystem
{
    public class SaveSystemReader : SaveSystemBase
    {
        private SaveSystemReader(string root, SaveSystemSettings settings)
        {
            _root = root;
            _settings = settings;
        }

        /// <summary>
        /// Creates a SaveSystemReader on the specified root
        /// </summary>
        /// <param name="root">The root to read from</param>
        /// <returns>A SaveSystemReader instance</returns>
        public static SaveSystemReader Create(string root)
        {
            return Create(root, new SaveSystemSettings());
        }

        /// <summary>
        /// Creates a SaveSystemReader on the specified root using the specified settings
        /// </summary>
        /// <param name="root">The root to read from</param>
        /// <param name="settings">Settings</param>
        /// <returns>A SaveSystemReader instance</returns>
        public static SaveSystemReader Create(string root, SaveSystemSettings settings)
        {
            SaveSystemReader SaveSystemReader = new SaveSystemReader(root, settings);
            SaveSystemReader.Load(false);
            return SaveSystemReader;
        }

        /// <summary>
        /// Reads an object under the specified key
        /// </summary>
        /// <typeparam name="T">The type of object to read</typeparam>
        /// <param name="key">The key this object was saved under</param>
        /// <returns>The object that was loaded</returns>
        public T Read<T>(string key)
        {
            if (!Exists(key))
            {
                throw new SaveSystemException("Key does not exists");
            }

            try
            {
                return JsonSerialiser.DeserialiseKey<T>(key, _items);
            }
            catch
            {
                throw new SaveSystemException("Deserialisation failed");
            }
        }

        /// <summary>
        /// Reads an object under the specified key
        /// </summary>
        /// <typeparam name="T">The type of object to read</typeparam>
        /// <param name="key">The key this object was saved under</param>
        /// <param name="result">An action to be called when the read completes</param>
        /// <returns>The SaveSystemReader</returns>
        public SaveSystemReader Read<T>(string key, Action<T> result)
        {
            if (!Exists(key))
            {
                throw new SaveSystemException("Key does not exists");
            }

            try
            {
                result(JsonSerialiser.DeserialiseKey<T>(key, _items));
            }
            catch
            {
                throw new SaveSystemException("Deserialisation failed");
            }

            return this;
        }

        /// <summary>
        /// Attempts to read an object under the specified key
        /// </summary>
        /// <typeparam name="T">The type of object to read</typeparam>
        /// <param name="key">The key this object was saved under</param>
        /// <param name="result">The object that was loaded</param>
        /// <returns>Was the read successful</returns>
        public bool TryRead<T>(string key, out T result)
        {
            result = default(T);

            if (!Exists(key))
            {
                return false;
            }

            try
            {
                result = JsonSerialiser.DeserialiseKey<T>(key, _items);

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Reloads data from the root
        /// </summary>
        public void Reload()
        {
            Load(false);
        }
    }
}