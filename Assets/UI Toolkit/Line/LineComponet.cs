using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class LineComponent : MonoBehaviour
{
    Line m_Line;

    void Start()
    {
        m_Line = new Line(new Vector2(0,0),new Vector2(100,100),1);
        GetComponent<UIDocument>().rootVisualElement.Add(m_Line);
    }
}