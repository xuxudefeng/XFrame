using SaveSystem.Core.Serialisers;

namespace SaveSystem
{
    public class SaveSystemWriter : SaveSystemBase
    {
        private SaveSystemWriter(string root, SaveSystemSettings settings)
        {
            _root = root;
            _settings = settings;
        }

        /// <summary>
        /// Creates a SaveSystemWriter on the specified root
        /// </summary>
        /// <param name="root">The root to write to</param>
        /// <returns>A SaveSystemWriter instance</returns>
        public static SaveSystemWriter Create(string root)
        {
            return Create(root, new SaveSystemSettings());
        }

        /// <summary>
        /// Creates a SaveSystemWriter on the specified root using the specified settings
        /// </summary>
        /// <param name="root">The root to write to</param>
        /// <param name="settings">Settings</param>
        /// <returns>A SaveSystemWriter instance</returns>
        public static SaveSystemWriter Create(string root, SaveSystemSettings settings)
        {
            SaveSystemWriter SaveSystemWriter = new SaveSystemWriter(root, settings);
            SaveSystemWriter.Load(true);
            return SaveSystemWriter;
        }

        /// <summary>
        /// Writes an object to the specified key - you must called commit to write the data to file
        /// </summary>
        /// <typeparam name="T">The type of object to write</typeparam>
        /// <param name="key">The key this object will be saved under</param>
        /// <param name="value">The object to save</param>
        /// <returns>The SaveSystemWriter</returns>
        public SaveSystemWriter Write<T>(string key, T value)
        {
            if (Exists(key))
            {
                _items.Remove(key);
            }

            _items.Add(key, JsonSerialiser.SerialiseKey(value));

            return this;
        }

        /// <summary>
        /// Deletes the specified key if it exists
        /// </summary>
        /// <param name="key">The key to delete</param>
        public void Delete(string key)
        {
            if (Exists(key))
            {
                _items.Remove(key);
            }
        }

        /// <summary>
        /// Commits the changes to file
        /// </summary>
        public void Commit()
        {
            Save();
        }

        /// <summary>
        /// Attempts to commit the changes to file
        /// </summary>
        /// <returns>Was the commit successful</returns>
        public bool TryCommit()
        {
            try
            {
                Save();

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}