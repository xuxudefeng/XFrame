using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using System.Threading.Tasks;

/// <summary>
///  UI管理
///  作者：徐振升
///  最后更新：2021-11-27
///  联系方式：QQ:359059686
///  遵循规则=>约定既是配置
///  约定对象：需要UIManager管理的UIView预设
///  约定1：使用寻址系统加载对象
///  约定2：UIView的预设名字、脚本名称、寻址Key相同
/// </summary>
public class UIManager : Singleton<UIManager>
{
    private Canvas canvas = null;
    private EventSystem eventSystem = null;

    /// <summary>
    /// 已经创建的视图
    /// </summary>
    public Dictionary<string, UIView> Views = new Dictionary<string, UIView>();

    public override void Awake()
    {
        base.Awake();
        InitUICanvas();
        InitUIEventSystem();
        InitUIGroup();
    }

    /// <summary>
    /// 初始化画布
    /// </summary>
    private void InitUICanvas()
    {
        canvas = Object.FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasGO = new GameObject("Canvas");
            canvasGO.layer = LayerMask.NameToLayer("UI");

            canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.pixelPerfect = true;

            CanvasScaler canvasScaler = canvasGO.AddComponent<CanvasScaler>();
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = new Vector2(4096f, 2160f);
            canvasScaler.matchWidthOrHeight = 1f;

            canvasGO.AddComponent<GraphicRaycaster>();
            canvasGO.AddComponent<ClampToWindow>();
        }
    }
    /// <summary>
    /// 初始化UI事件
    /// </summary>
    private void InitUIEventSystem()
    {
        eventSystem = Object.FindObjectOfType<EventSystem>();
        if (eventSystem == null)
        {
            GameObject eventSystemGO = new GameObject("EventSystem");
            eventSystem = eventSystemGO.AddComponent<EventSystem>();
            eventSystemGO.AddComponent<StandaloneInputModule>();
        }
    }
    /// <summary>
    /// 初始化UI分组
    /// </summary>
    public void InitUIGroup()
    {
        foreach (UIGroup uiGroup in Enum.GetValues(typeof(UIGroup)))
        {
            Transform root = canvas.transform.Find(uiGroup.ToString());
            if (root == null)
            {
                GameObject nodeGO = new GameObject(uiGroup.ToString());
                nodeGO.layer = LayerMask.NameToLayer("UI");
                nodeGO.transform.SetParent(canvas.transform);

                RectTransform rectTrans = nodeGO.AddComponent<RectTransform>();
                rectTrans.sizeDelta = Vector2.zero;
                rectTrans.localScale = Vector3.one;
                rectTrans.localPosition = Vector3.zero;
                rectTrans.SetAnchor(AnchorPresets.StretchAll);
            }
        }
    }

    /// <summary>
    /// 获取子分组目录
    /// </summary>
    /// <returns></returns>
    public Transform GetUIGroup(UIGroup uiGroup)
    {
        Transform root = canvas.transform.Find(uiGroup.ToString());
        return root;
    }
    /// <summary>
    /// 是否存在视图
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="viewName"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public bool HasView<T>(string viewName)
    {
        if (string.IsNullOrEmpty(viewName))
        {
            throw new Exception("UI名称不存在.");
        }

        return Views.ContainsKey(viewName);
    }
    /// <summary>
    /// 获取视图
    /// </summary>
    public T GetView<T>(string viewName)
        where T : UIView
    {
        if (HasView<T>(viewName))
        {
            return Views[viewName] as T;
        }
        return null;
    }
    ///// <summary>
    ///// 显示一个View
    ///// </summary>
    ///// <param name="viewName"></param>
    ///// <param name="action"></param>
    //public void ShowView<T>(string viewName, Action<T> action = null)
    //    where T : UIView
    //{
    //    var view = GetView<T>(viewName);
    //    if (view != null)
    //    {
    //        action?.Invoke(view);
    //        view.Show();
    //    }
    //    else
    //    {
    //        InstantiateView<T>(viewName, action);
    //    }

    //}
    /// <summary>
    /// 关闭View
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="viewName"></param>
    public void HideView<T>(string viewName)
        where T : UIView
    {
        var view = GetView<T>(viewName);
        if (view != null)
        {
            view.GetComponent<UIView>().Hide();
        }
    }
    ///// <summary>
    ///// 实例化页面T
    ///// </summary>
    ///// <param name="viewName"></param>
    ///// <returns></returns>
    //public void InstantiateView<T>(string viewName, Action<T> action = null)
    //    where T : UIView
    //{
    //    string key = $"{typeof(T).ToString()}";
    //    AsyncOperationHandle<GameObject> handle = Addressables.InstantiateAsync(key, canvas.transform);
    //    handle.Completed += x =>
    //    {
    //        switch (x.Status)
    //        {
    //            case AsyncOperationStatus.None:
    //                Debug.Log("1");
    //                break;
    //            case AsyncOperationStatus.Succeeded:
    //                T view = x.Result.GetComponent<T>();
    //                action?.Invoke(view);
    //                view.gameObject.transform.SetParent(canvas.transform.Find(view.UIViewType.ToString()), false);
    //                view.gameObject.name = viewName;
    //                view.Show();
    //                break;
    //            case AsyncOperationStatus.Failed:
    //                Debug.LogError($"{key}创建失败！");
    //                break;
    //            default:
    //                break;
    //        }
    //    };
    //}
    ///// <summary>
    ///// 实例化页面T
    ///// </summary>
    ///// <param name="viewName"></param>
    ///// <returns></returns>
    //public async Task<T> CreateView<T>(string viewName)
    //    where T : UIView
    //{
    //    string key = $"{typeof(T).ToString()}";
    //    AsyncOperationHandle<GameObject> handle = Addressables.InstantiateAsync(key, canvas.transform);
    //    await handle.Task;
    //    T view = null;
    //    if (handle.Status == AsyncOperationStatus.Succeeded)
    //    {
    //        view = handle.Result.GetComponent<T>();
    //        view.gameObject.transform.SetParent(canvas.transform.Find(view.UIViewType.ToString()), false);
    //        view.gameObject.name = viewName;
    //        view.Hide();
    //    }
    //    else
    //    {
    //        Debug.LogError($"{key}创建失败！");
    //    }
    //    return view;
    //}
    ///// <summary>
    ///// 释放页面T
    ///// </summary>
    ///// <typeparam name="T"></typeparam>
    ///// <param name="name"></param>
    //public void ReleaseView<T>(string name)
    //    where T : UIView
    //{
    //    var view = GetView<T>(name);
    //    Addressables.Release(view);
    //}
}


