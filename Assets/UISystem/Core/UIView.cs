using AnKuchen.Map;
using System;
using UnityEngine;
public interface IView
{
    public UIViewType UIViewType { get; set; }
    public UIViewName UIViewName { get; set; }
    public void Show();
    public void Hide();
    public void Refresh();
}
public interface IView<T, V> : IView
{
    public T Data { get; set; }
    public V UI { get; set; }
    public void Show(T data);
    public void SetData(T data);
}
/// <summary>
/// UI
/// </summary>
/// <typeparam name="T">属性Data的类型</typeparam>
/// <typeparam name="V">属性UI的类型</typeparam>
public abstract class UIView<T, V> : UICache, IView<T, V>
    where T : new()
    where V : IMappedObject, new()
{
    public T Data { get; set; }
    public V UI { get; set; }
    public UIViewType UIViewType { get; set; }
    public UIViewName UIViewName { get; set; }

    public virtual void Awake()
    {
        Data = new T();
        UI = new V();
        UI.Initialize(this);
    }
    public virtual void Destroy()
    {

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