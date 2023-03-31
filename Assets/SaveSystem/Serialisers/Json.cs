using System.Collections.Generic;
using SaveSystem.Converters;
using Newtonsoft.Json;

namespace SaveSystem.Serialisers
{
    public static class Json
    {
        private static readonly JsonSerializerSettings _settings = new JsonSerializerSettings()
        {
            Converters = new List<JsonConverter>()
            {
                new ColorConverter(),
                new QuaternionConverter(),
                new Matrix4x4Converter(),
                new Texture2DConverter(),
                new SpriteConverter(),
                new Vector2Converter(),
                new Vector3Converter(),
                new Vector4Converter()
            }
        };
        /// <summary>
        /// Json反序列化
        /// </summary>
        public static T Deserialise<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, _settings);
        }
        /// <summary>
        /// Json序列化
        /// </summary>
        public static string Serialise<T>(T value)
        {
            return JsonConvert.SerializeObject(value, Formatting.Indented, _settings);
        }
    }
}