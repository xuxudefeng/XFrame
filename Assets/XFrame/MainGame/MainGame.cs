using SaveSystem;
using System.Text;
using UnityEngine;

public class MainGame : MonoBehaviour
{
    void Start()
    {
        XFile.SaveJson("test", new Vector3(100, 100, 1000));
        XFile.SaveString("字符串", "afafadsfdsfds东方闪电大事发生的打发斯蒂芬斯蒂芬s");
        XFile.SaveLog("aaaaaaaaa");
        XFile.SaveLog("bbbbbbbbb");
        XFile.SaveBytes("bytes", Encoding.UTF8.GetBytes("你好"));


        ConfigReader.Save();
    }
}
