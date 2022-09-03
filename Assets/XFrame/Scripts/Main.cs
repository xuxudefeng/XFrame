using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    private void Awake()
    {
        UIManager.Initialize();
        //ChineseMessages chineseMessages = new ChineseMessages();
        ConfigManager.Load();
        //Debug.Log(chineseMessages.BannedWrongPassword);


        //chineseMessages.BannedWrongPassword = "测试";
        ConfigManager.Save();


    }
    // Start is called before the first frame update
    void Start()
    {
        //ConfigManager.Load();
        //ConfigManager.Save();
        //User user = new User();
        //user.name = "abc";
        //user.password = "123";
        //List<User> users = new List<User>();
        //users.Add(user);
        //var abc =  ConverterHelp.JSON2CSV(JsonConvert.SerializeObject(users));
        //Debug.Log(abc);

    }
}

public class User
{
    public string name;
    public string password;
}
