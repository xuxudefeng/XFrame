namespace SaveSystem
{
    public class SaveSystemSettings
    {
        /// <summary>
        /// 对数据使用的加密类型
        /// </summary>
        public SecurityMode SecurityMode { get; set; }

        /// <summary>
        /// 对数据使用的压缩类型
        /// </summary>
        public CompressionMode CompressionMode { get; set; }

        /// <summary>
        /// 如果选择 aes 作为安全模式，请指定一个密码用作加密密钥
        /// </summary>
        public string Password { get; set; }

        public SaveSystemSettings()
        {
            SecurityMode = SecurityMode.None;
            CompressionMode = CompressionMode.None;
        }
    }
}