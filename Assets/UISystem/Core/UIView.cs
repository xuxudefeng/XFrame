using AnKuchen.Map;
using System;
using UnityEngine;

[RequireComponent(typeof(UICache))]
public abstract class UIView : MonoBehaviour
{
    public UICache UICache
    {
        get
        {
            return GetComponent<UICache>();
        }
    }
    public UIViewType UIViewType;
    public UIViewName UIViewName { get; set; }
    public virtual void Awake()
    {
        UIManager.Views.Add(name,this);
    }
    public virtual void Destroy()
    {
        UIManager.Views.Remove(name);
    }
    /// <summary>
    /// 显示
    /// </summary>
    public virtual void Show()
    {
        Refresh();
        if (gameObject.activeSelf) return;
        gameObject.SetActive(true);
        // 设置层级到最上层
        transform.SetAsLastSibling();
    }
    /// <summary>
    /// 隐藏
    /// </summary>
    public virtual void Hide()
    {
        //显示的情况下才隐藏
        if (!gameObject.activeSelf) return;
        gameObject.SetActive(false);
    }
    /// <summary>
    /// 刷新
    /// </summary>
    public abstract void Refresh();
}

/// <summary>
/// UI
/// </summary>
/// <typeparam name="T">属性Data的类型</typeparam>
public abstract class UIView<T> : UIView
{
    public T Data { get; set; }
    /// <summary>
    /// 根据数据显示页面
    /// </summary>
    /// <param name="data">数据</param>
    public virtual void Show(T data)
    {
        SetData(data);
        Show();
    }
    /// <summary>
    /// 设置数据
    /// </summary>
    /// <param name="data"></param>
    public virtual void SetData(T data)
    {
        Data = data;
        Refresh();
    }
}