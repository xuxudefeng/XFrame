using System;
using UnityEngine;

/// <summary>
/// UIControl
/// </summary>
/// 
public abstract class UIView : UICache
{
    public UIGroup UIViewType;

    /// <summary>
    /// 显示
    /// </summary>
    public virtual void Show()
    {
        if (gameObject.activeSelf) return;
        Refresh();
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
    public abstract void Refresh();

}

public abstract class UIView<T> : UIView
{
    public T Data;
    /// <summary>
    /// 显示
    /// </summary>
    public virtual void Show(T data)
    {
        Data = data;
        base.Show();
    }
    public virtual void SetData(T data)
    {
        Data = data;
        Refresh();
    }
}
/// <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
/// <typeparam name="V"></typeparam>
public abstract class UIView<T, V> : UIView<T>
    where T : new()
    where V : IMappedObject, new()
{
    // 是否初始化UI
    private bool IsInitializedUI = false;
    //public T Data;

    private V ui;
    public V UI { 
        get 
        {
            if (!IsInitializedUI)
            {
                IsInitializedUI = true;
                ui = new V();
                ui.Initialize(this);
            }
            return ui; 
        } 
    }
    ///// <summary>
    ///// 根据数据显示页面
    ///// </summary>
    ///// <param name="data">数据</param>
    //public virtual void Show(T data)
    //{
    //    SetData(data);
    //    base.Show();
    //}
    ///// <summary>
    ///// 设置数据
    ///// </summary>
    ///// <param name="data"></param>
    //public virtual void SetData(T data)
    //{
    //    Data = data;
    //    Refresh();
    //}
}