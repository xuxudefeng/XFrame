using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using System.Threading.Tasks;


public enum UIViewName
{
    MainView,
    RankingView
}
/// <summary>
///  UI管理
///  作者：徐振升
///  最后更新：2022-05-31
/// </summary>
public class UIManager
{
    public static Canvas canvas = null;
    public static EventSystem eventSystem = null;
    /// <summary>
    /// 已经创建的UIView对象引用
    /// </summary>
    public static Dictionary<string, UIView> Views = new Dictionary<string, UIView>();
    /// <summary>
    /// 初始化
    /// </summary>
    public static void Initialize()
    {
        InitCanvas();
        InitEventSystem();
        InitNode();
        InitView();
    }
    private static void InitView()
    {
        CreateView<MainView>("MainView");
    }
    /// <summary>
    /// 初始化画布
    /// </summary>
    private static void InitCanvas()
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
        }
    }
    /// <summary>
    /// 初始化事件系统
    /// </summary>
    private static void InitEventSystem()
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
    /// 初始化节点
    /// </summary>
    private static void InitNode()
    {
        // 添加UIViewType
        foreach (UIViewType uiViewType in Enum.GetValues(typeof(UIViewType)))
        {
            Transform root = GetSubroot(uiViewType);
            if (root == null)
            {
                GameObject nodeGO = new GameObject(uiViewType.ToString());
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
    /// 获取子路径
    /// </summary>
    /// <param name="viewType">视图类型</param>
    /// <returns></returns>
    public static Transform GetSubroot(UIViewType viewType)
    {
        if (viewType == UIViewType.None)
            return canvas.transform;
        else
            return canvas.transform.Find(viewType.ToString());
    }
    /// <summary>
    /// 获得一个名称第一个为name的T
    /// </summary>
    public static T GetView<T>(string viewName)
        where T : UIView
    {
        // 直接使用Firest,数组为空时会引发异常
        UIView view;
        T viewAsT = null;
        if (Views.TryGetValue(viewName, out view))
        {
            viewAsT = view as T;
        }
        return viewAsT;
    }
    /// <summary>
    /// 显示一个View
    /// </summary>
    /// <param name="viewName"></param>
    /// <param name="action"></param>
    public static void ShowView<T>(string viewName, Action<T> action = null)
        where T : UIView
    {
        var view = GetView<T>(viewName);
        if (view != null)
        {
            action?.Invoke(view);
            view.Show();
        }
        else
        {
            MessageBox.Show(string.Format("{0}不存在！", viewName));
        }
    }
    /// <summary>
    /// 关闭View
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="viewName"></param>
    public static void HideView<T>(string viewName)
        where T : UIView
    {
        UIView view;
        if (Views.TryGetValue(viewName, out view))
        {
            view.Hide();
        }
        else
        {
            MessageBox.Show(string.Format("{0}不存在！",viewName));
        }
    }
    /// <summary>
    /// 实例化页面T
    /// </summary>
    /// <param name="viewName"></param>
    /// <returns></returns>
    private static void CreateView<T>(string viewName)
        where T : UIView
    {
        string key = $"{typeof(T).ToString()}";
        AsyncOperationHandle<GameObject> handle = Addressables.InstantiateAsync(key, canvas.transform);
        handle.Completed += x =>
        {
            if(x.Status == AsyncOperationStatus.Succeeded)
            {
                T view = x.Result.GetComponent<T>();
                view.gameObject.transform.SetParent(GetSubroot(view.UIViewType), false);
                view.gameObject.name = viewName.ToString();
                view.transform.localScale = Vector3.zero;
            }
            else
            {
                MessageBox.Show(string.Format("{0}创建失败！", viewName));
            }
        };
    }
    /// <summary>
    /// 释放页面T
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name"></param>
    public static void DestroyView<T>(string name)
        where T : UIView
    {
        var view = GetView<T>(name);
        Addressables.Release(view);
    }
    /// <summary>
    /// 异步创建页面
    /// </summary>
    /// <param name="viewName"></param>
    /// <returns></returns>
    public static async Task<T> CreateViewAsync<T>(UIViewName viewName)
        where T : UIView
    {
        string key = $"{typeof(T).ToString()}";
        AsyncOperationHandle<GameObject> handle = Addressables.InstantiateAsync(key, canvas.transform);
        await handle.Task;
        T view = null;
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            view = handle.Result.GetComponent<T>();
            view.gameObject.transform.SetParent(canvas.transform.Find(view.UIViewType.ToString()), false);
            view.gameObject.name = viewName.ToString();
            view.Hide();
        }
        else
        {
            Debug.LogError($"{key}创建失败！");
        }
        return view;
    }
}


