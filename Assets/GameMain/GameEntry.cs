using System.Collections.Generic;
using UnityEngine;

public abstract class GameEntry : MonoBehaviour
{
    public List<Module> ModulesList;


    public virtual void Awake(){
        ModulesList = Modules();
    }


    public abstract List<Module> Modules();
}
