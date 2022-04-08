using AnKuchen.Map;
using System;
using UnityEngine;

/// <summary>
/// UIView
/// </summary>
/// 
public class UIView : UICache
{
    public string ViewName;
    public UIViewType UIViewType;
    public virtual void Awake()
    {
        UIManager.Views.AddOrSet(ViewName, this);
    }
    public virtual void Destroy()
    {
        UIManager.Views.Remove(ViewName);
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
    public void ShowHide()
    {
        gameObject.SetActive(!gameObject.activeSelf);
        // 设置层级到最上层
        transform.SetAsLastSibling();
    }
    /// <summary>
    /// 刷新
    /// </summary>
    public virtual void Refresh()
    {

    }

}