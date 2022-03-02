using System;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.UI;
using UniRx;
namespace XFrame.UI
{
    /// <summary>
    /// UI页面
    /// </summary>
    public class UIView : MonoBehaviour
    {
        // 页面名称
        public string ViewName = "Default";
        public object Data = null;
        public UIViewType UIViewType;
        public virtual void Awake()
        {
            Debug.Log("Awake()");
            UIManager.Add(this);
        }
        public virtual void OnDestroy()
        {
            UIManager.Remove(this);
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

    /// <summary>
    /// UIView-Data
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <typeparam name="TReactiveModel"></typeparam>
    public class UIView<TModel, TReactiveModel> : UIView
        where TReactiveModel : IViewModel<TModel>, new()
        //where TModel : class
    {
        /// <summary>
        /// 数据
        /// </summary>
        protected TReactiveModel DataSource { get; private set; } = new TReactiveModel();
        /// <summary>
        /// 显示
        /// </summary>
        /// <param name="data"></param>
        public void Show(TModel data)
        {
            DataSource.SetData(data);
            base.Show();
        }
    }
}