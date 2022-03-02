using System;
using System.Collections.Generic;

//public delegate void Callback();
//public delegate void Callback<T>(T arg1);
//public delegate void Callback<T, U>(T arg1, U arg2);

public class MessageSystem
{
    #region 消息表
    private static Dictionary<short, Delegate> eventTable = new Dictionary<short, Delegate>();

    public static void AddListener(short eventType, Action handler)
    {
        // 加锁保证线程安全
        lock (eventTable)
        {
            if (!eventTable.ContainsKey(eventType))
            {
                eventTable.Add(eventType, null);
            }
            eventTable[eventType] = (Action)eventTable[eventType] + handler;
        }
    }

    public static void RemoveListener(short eventType, Action handler)
    {
        lock (eventTable)
        {
            if (eventTable.ContainsKey(eventType))
            {
                eventTable[eventType] = (Action)eventTable[eventType] - handler;

                if (eventTable[eventType] == null)
                {
                    eventTable.Remove(eventType);
                }
            }
        }
    }

    public static void Broadcast(short eventType)
    {
        Delegate d;
        if (eventTable.TryGetValue(eventType, out d))
        {
            Action callback = (Action)d;

            if (callback != null)
            {
                callback();
            }
        }
    }

    public static void AddListener<T>(short eventType, Action<T> handler)
    {
        lock (eventTable)
        {

            if (!eventTable.ContainsKey(eventType))
            {
                eventTable.Add(eventType, null);
            }
            eventTable[eventType] = (Action<T>)eventTable[eventType] + handler;
        }
    }

    public static void RemoveListener<T>(short eventType, Action<T> handler)
    {
        lock (eventTable)
        {
            if (eventTable.ContainsKey(eventType))
            {
                eventTable[eventType] = (Action<T>)eventTable[eventType] - handler;

                if (eventTable[eventType] == null)
                {
                    eventTable.Remove(eventType);
                }
            }
        }
    }

    public static void Broadcast<T>(short eventType, T arg1)
    {
        Delegate d;
        if (eventTable.TryGetValue(eventType, out d))
        {
            Action<T> callback = (Action<T>)d;

            if (callback != null)
            {
                callback(arg1);
            }
        }
    }

    public static void AddListener<T, U>(short eventType, Action<T, U> handler)
    {
        lock (eventTable)
        {
            if (!eventTable.ContainsKey(eventType))
            {
                eventTable.Add(eventType, null);
            }
            eventTable[eventType] = (Action<T, U>)eventTable[eventType] + handler;
        }
    }

    public static void RemoveListener<T, U>(short eventType, Action<T, U> handler)
    {
        lock (eventTable)
        {
            if (eventTable.ContainsKey(eventType))
            {
                eventTable[eventType] = (Action<T, U>)eventTable[eventType] - handler;

                if (eventTable[eventType] == null)
                {
                    eventTable.Remove(eventType);
                }
            }
        }
    }

    public static void Broadcast<T, U>(short eventType, T arg1, U arg2)
    {
        Delegate d;
        if (eventTable.TryGetValue(eventType, out d))
        {
            Action<T, U> callback = (Action<T, U>)d;

            if (callback != null)
            {
                callback(arg1, arg2);
            }
        }
    }
    #endregion
}