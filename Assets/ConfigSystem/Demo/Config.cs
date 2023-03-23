using System;
using UnityEngine;

[ConfigPath(@"Config.ini")]
public static class Config
{
    [ConfigSection("运行模式")]
    public static bool DevMode { get; set; } = false;
    public static int TempInt{get;set;}
    public static Vector3 TempVector3{get;set;}
    
}
