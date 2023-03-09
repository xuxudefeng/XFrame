using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class GridComponent : MonoBehaviour
{
    Grid grid;

    public int width = 500;
    public int height = 500;
    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        grid = new Grid(width,height);
        root.Add(grid);
    }

}