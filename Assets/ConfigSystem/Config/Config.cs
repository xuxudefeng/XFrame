using System;
using UnityEngine;

[ConfigPath(@"DataAcquisitionTool.ini")]
public static class Config
{
    public const string DefaultIPAddress = "127.0.0.1";
    public const int DefaultPort = 7000;

    public const string AssetServer = "http://localhost:59086";


    [ConfigSection("Network")]
    public static bool UseNetworkConfig { get; set; } = false;
    public static string IPAddress { get; set; } = DefaultIPAddress;
    public static int Port { get; set; } = DefaultPort;
    public static TimeSpan TimeOutDuration { get; set; } = TimeSpan.FromSeconds(15);
    public static DateTime CreateTime { get;set; } = DateTime.Now;
    public static Color NewColor { get; set; } = Color.red;
    public static Color OldColor { get; set; } = Color.green;
    public static Vector2 Vector2 { get; set; } = new Vector2(100f, 100f);
    public static Vector3 Vector3 { get; set; } = new Vector3(100f, 100f,100f);
}
