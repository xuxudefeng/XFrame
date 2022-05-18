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
///  最后更新：2021-11-27
///  联系方式：QQ:359059686
///  遵循规则=>约定既是配置
///  约定对象：需要UIManager管理的UIView预设
///  约定1：使用寻址系统加载对象
///  约定2：UIView的预设名字、脚本名称、寻址Key相同
/// </summary>
public class UIManager : Singleton<UIManager>
{

    public static Canvas canvas = null;
    public static EventSystem eventSystem = null;
    /// <summary>
    /// 已经创建的UIView对象引用
    /// </summary>
    public static Dictionary<string, IView> Views = new Dictionary<string, IView>();

    public override void Awake()
    {
        base.Awake();
        InitCanvas();
        InitEventSystem();
        InitNode();
    }
    public void InitView()
    {
        
    }

    // 创建新的UI目录
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
    // 创建UIEvent
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
    // 获取UI子目录
    public static void InitNode()
    {
        // 添加UIViewType
        foreach (UIViewType uiViewType in Enum.GetValues(typeof(UIViewType)))
        {
            Transform root = canvas.transform.Find(uiViewType.ToString());
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
    // 获取子目录
    public static Transform GetSubroot(UIViewType viewType)
    {
        Transform root = canvas.transform.Find(viewType.ToString());
        return root;
    }
    /// <summary>
    /// 获得一个名称第一个为name的T
    /// </summary>
    public static T GetViewByName<T>(string viewName)
        where T : class, IView
    {
        // 直接使用Firest,数组为空时会引发异常
        IView view;
        if (Views.TryGetValue(viewName, out view))
        {
            return view as T;
        }
        else
        {
            return null;
        }
    }
    /// <summary>
    /// 显示一个View
    /// </summary>
    /// <param name="viewName"></param>
    /// <param name="action"></param>
    public static void ShowViewByName<T>(string viewName,Action<T> action = null)
        where T : MonoBehaviour, IView
    {
        var view = GetViewByName<T>(viewName);
        if (view != null)
        {
            action?.Invoke(view);
            view.Show();
        }
        else
        {
            CreateViewByName<T>(viewName, action);
        }
    }
    /// <summary>
    /// 关闭View
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="viewName"></param>
    public static void HideViewByName<T>(string viewName)
        where T : MonoBehaviour, IView
    {
        IView view;
        if (Views.TryGetValue(viewName, out view))
        {
            view.Hide();
        }
    }
    /// <summary>
    /// 实例化页面T
    /// </summary>
    /// <param name="viewName"></param>
    /// <returns></returns>
    private static void CreateViewByName<T>(string viewName, Action<T> action = null)
        where T : MonoBehaviour, IView
    {
        string key = $"{typeof(T).ToString()}";
        AsyncOperationHandle<GameObject> handle = Addressables.InstantiateAsync(key, canvas.transform);
        handle.Completed += x =>
        {
            switch (x.Status)
            {
                case AsyncOperationStatus.None:
                    break;
                case AsyncOperationStatus.Succeeded:
                    T view = x.Result.GetComponent<T>();
                    action?.Invoke(view);
                    view.gameObject.transform.SetParent(canvas.transform.Find(view.UIViewType.ToString()), false);
                    view.gameObject.name = viewName;
                    view.Show();
                    break;
                case AsyncOperationStatus.Failed:
                    Debug.LogError($"{key}创建失败！");
                    break;
                default:
                    break;
            }
        };
    }
    /// <summary>
    /// 释放页面T
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name"></param>
    public static void DestroyView<T>(string name)
        where T : MonoBehaviour, IView
    {
        var view = GetViewByName<T>(name);
        Addressables.Release(view);
    }
    /// <summary>
    /// 异步创建页面
    /// </summary>
    /// <param name="viewName"></param>
    /// <returns></returns>
    public static async Task<T> CreateViewAsync<T>(string viewName)
        where T : MonoBehaviour, IView
    {
        string key = $"{typeof(T).ToString()}";
        AsyncOperationHandle<GameObject> handle = Addressables.InstantiateAsync(key, canvas.transform);
        await handle.Task;
        T view = null;
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            view = handle.Result.GetComponent<T>();
            view.gameObject.transform.SetParent(canvas.transform.Find(view.UIViewType.ToString()), false);
            view.gameObject.name = viewName;
            view.Hide();
        }
        else
        {
            Debug.LogError($"{key}创建失败！");
        }
        return view;
    }
}


