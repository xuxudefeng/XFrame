using UnityEngine;

public class Singleton<T> : MonoBehaviour
    where T : MonoBehaviour
{
    // 检查单例是否被销毁
    private static bool m_ShuttingDown = false;
    private static object m_Lock = new object();
    private static T m_Instance;
    /// <summary>
    /// 单例基类
    /// </summary>
    public static T Instance
    {
        get
        {
            if (m_ShuttingDown)
            {
                Debug.LogWarning("单例实例 '" + typeof(T) +
                    "' 已销毁，返回null");
                return null;
            }

            lock (m_Lock)
            {
                if (m_Instance == null)
                {
                    //查找类型脚本
                    m_Instance = (T)FindObjectOfType(typeof(T));

                    // 找不到，创建一个新的
                    if (m_Instance == null)
                    {
                        var singletonObject = new GameObject();
                        m_Instance = singletonObject.AddComponent<T>();
                        singletonObject.name = typeof(T).ToString() + " (Singleton)";
                    }
                }

                return m_Instance;
            }
        }
    }

    public static void Initialize()
    {
        Debug.Log($"手动初始化{typeof(T)}");
        if (m_ShuttingDown)
        {
            Debug.LogWarning("单例实例 '" + typeof(T) +
                "' 已销毁，返回null");
        }

        lock (m_Lock)
        {
            if (m_Instance == null)
            {
                //查找类型脚本
                m_Instance = (T)FindObjectOfType(typeof(T));

                // 找不到，创建一个新的
                if (m_Instance == null)
                {
                    var singletonObject = new GameObject();
                    m_Instance = singletonObject.AddComponent<T>();
                    singletonObject.name = typeof(T).ToString() + " (Singleton)";
                }
            }
        }
    }

    public virtual void Awake()
    {
        // 禁止加载场景后销毁
        DontDestroyOnLoad(gameObject);
    }

    private void OnApplicationQuit()
    {
        m_ShuttingDown = true;
    }


    private void OnDestroy()
    {
        m_ShuttingDown = true;
    }
}