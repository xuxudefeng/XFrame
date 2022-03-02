using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace XFrame.UI
{
    /// <summary>
    ///  UI管理
    ///  作者：徐振升
    ///  最后更新：2019-11-18
    ///  联系方式：QQ:359059686
    ///  遵循规则=>约定既是配置
    ///  约定对象：需要UIManager管理的UIView预设
    ///  约定1：使用寻址系统加载对象
    ///  约定2：UIView的预设名字、脚本名称、寻址名称相同
    ///  约定3：ShowView<T>显示的是名称为typeof(T).ToString()的T页面
    ///         ShowView<T>(name)显示的是名称为typeof(T).ToString()+name的T页面
    /// </summary>
    public class UIManager
    {
        /// <summary>
        /// 已经创建的UIView对象引用
        /// </summary>
        private static Dictionary<string, UIView> UIViews = new Dictionary<string, UIView>();

        /// <summary>
        /// 添加uiView到对象引用中
        /// </summary>
        /// <param name="uiView">UIView对象</param>
        public static void Add(UIView uiView)
        {
            if (UIViews.ContainsKey(uiView.name))
            {
                Debug.LogError($"[{uiView.name}]已存在，无法创建");
            }
            else
            {
                UIViews.Add(uiView.name, uiView);
            }
        }
        /// <summary>
        /// 从对象池中移除uiView
        /// </summary>
        /// <param name="uiView">UIView对象</param>
        public static void Remove(UIView uiView)
        {
            if (UIViews.ContainsKey(uiView.name))
            {
                UIViews.Remove(uiView.name);
            }
        }
        /// <summary>
        /// 获取UI画布
        /// </summary>
        public static GameObject GetCanvas()
        {
            GameObject gameObject = GameObject.Find("Canvas");
            if (gameObject == null || gameObject.GetComponent<Canvas>() == null)
            {
                gameObject = CreateCanvas("Canvas");
            }
            return gameObject;
        }
        // 创建新的UI目录
        private static GameObject CreateCanvas(string name)
        {
            GameObject gameObject = new GameObject(name);
            gameObject.layer = LayerMask.NameToLayer("UI");
            Canvas canvas = gameObject.AddComponent<Canvas>();
            //UICanvas = canvas;
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler canvasScaler = gameObject.AddComponent<CanvasScaler>();
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = new Vector2(1920f, 1080f);
            canvasScaler.matchWidthOrHeight = 1f;
            gameObject.AddComponent<GraphicRaycaster>();
            CreateUIRoot(UIViewType.Panel, gameObject.transform);
            CreateUIRoot(UIViewType.Popup, gameObject.transform);
            CreateEventSystem();
            return gameObject;
        }
        // 创建UI子目录
        private static Transform CreateUIRoot(UIViewType uiViewType, Transform transform)
        {
            GameObject gameObject = new GameObject(uiViewType.ToString());
            gameObject.layer = LayerMask.NameToLayer("UI");
            RectTransform rect = gameObject.AddComponent<RectTransform>();
            gameObject.transform.SetParent(transform);
            rect.sizeDelta = Vector2.zero;
            rect.localScale = Vector3.one;
            rect.localPosition = Vector3.zero;
            rect.SetAnchor(AnchorPresets.StretchAll);
            return gameObject.transform;
        }
        // 获取UI子目录
        public static Transform GetUIRoot(UIViewType uiViewType)
        {
            Transform canvas = GetCanvas().transform;
            Transform root = canvas.Find(uiViewType.ToString());
            if (root == null)
            {
                return CreateUIRoot(uiViewType, canvas);
            }
            return root;
        }
        // 创建UIEvent
        private static void CreateEventSystem()
        {
            EventSystem eventSystem = Object.FindObjectOfType<EventSystem>();
            if (eventSystem == null)
            {
                GameObject gameObject = new GameObject("EventSystem");
                eventSystem = gameObject.AddComponent<EventSystem>();
                gameObject.AddComponent<StandaloneInputModule>();
            }
        }


        #region 加载
        /// <summary>
        /// 加载UIView预设
        /// </summary>
        /// <param name="viewName"></param>
        /// <returns></returns>
        private static async Task<T> LoadViewAsync<T>(string viewName)
            where T : UIView
        {
            string prefabPath = $"{typeof(T).ToString()}";
            AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(prefabPath);
            var prefab = await handle.Task;
            prefab.SetActive(false);
            // 实例化
            var parent = GetUIRoot(prefab.GetComponent<T>().UIViewType);
            var go = Object.Instantiate(prefab, parent);
            Addressables.Release(handle);
            var view = go.GetComponent<T>();
            //view.ViewName = viewName;
            return view;
        }
        #endregion
        /// <summary>
        /// 获取页面
        /// </summary>
        /// <typeparam name="T">预设上脚本类型</typeparam>
        /// <param name="viewName">预设上脚本属性ViewName</param>
        /// <returns></returns>
        public static T GetView<T>(string viewName = "Default")
            where T : UIView
        {
            if (UIViews.ContainsKey(viewName))
            {
                return UIViews[viewName] as T;
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 显示页面
        /// </summary>
        /// <typeparam name="T">页面脚本类型</typeparam>
        /// <param name="viewName">页面名称</param>
        public static async void ShowAsync<T>(string viewName = "Default")
            where T : UIView
        {
            var view = GetView<T>(viewName);
            if (view == null)
            {
                Task<T> task = LoadViewAsync<T>(viewName);
                await task;
                view = task.Result;
            }
            view.Show();
        }
        /// <summary>
        /// 隐藏页面
        /// </summary>
        /// <param name="viewName"></param>
        public static void Hide<T>(string viewName = "Default")
            where T : UIView
        {
            UIView view = GetView<T>(viewName);
            if (view != null)
            {
                view.Hide();
            }
        }
        /// <summary>
        /// 显示隐藏界面
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="viewName"></param>
        public static void ShowHide<T>(string viewName = "Default")
            where T : UIView
        {
            UIView view = GetView<T>(viewName);
            if (view != null)
            {
                view.ShowHide();
            }
            else
            {
                ShowAsync<T>(viewName);
            }
        }
    }
}

