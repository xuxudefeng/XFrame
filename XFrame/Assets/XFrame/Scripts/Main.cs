using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XFrame.UI;

public class Main : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        UIManager.ShowAsync<UIViewStart>();
        Debug.Log("abc");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
