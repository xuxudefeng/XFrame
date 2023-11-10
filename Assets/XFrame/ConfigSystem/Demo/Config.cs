using System;
using UnityEngine;

[ConfigPath(@"Config.ini")]
public static class Config
{
    [ConfigSection("TypeA")]
    public static bool TempBoolean { get; set; }
    public static int TempInt { get; set; }
    public static int TempStr { get; set; }
    public static Char TempChar { get; set; }
    [ConfigSection("TypeB")]
    public static TimeSpan TempTimeSpan { get; set; }
    public static DateTime TempDateTime { get; set; }
    public static Vector2 TempVector2 { get; set; }
    public static Vector3 TempVector3 { get; set; }
    public static Color TempColor { get; set; }

}
