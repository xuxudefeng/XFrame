using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMain : GameEntry
{
    public Module A;
    public Module B;
    public Module C;

    public override List<Module> Modules()
    {
        return new List<Module>(){
            new Module(),
            new Module(),
            new Module()
        };
    }

    public void CreateModule(){

    }
}
