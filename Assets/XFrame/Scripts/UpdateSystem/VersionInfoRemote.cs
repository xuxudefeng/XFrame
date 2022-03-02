﻿using System;
using System.Collections.Generic;

using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class RemoteVersion
{
    public int ver
    {
        get;
        private set;
    }
    public void BeginInit(Action<Exception> onload,IEnumerable<string> _groups)
    {
        int groupcount = 0;
        Action<UnityWebRequest, string> onLoadGroup = (uwr, group) =>
        {
            if (string.IsNullOrEmpty(uwr.error) == false)
            {
                Debug.LogWarning("下载" + uwr.url + "错误");
            }
            else
            {
                string t = uwr.downloadHandler.text;
                if (t[0] == 0xFEFF)
                {
                    t = t.Substring(1);
                }
                var rhash = ResourceSystem.Instance.sha1.ComputeHash(uwr.downloadHandler.data);
                var shash = Convert.ToBase64String(rhash);
                if (shash != groups[group].hash)
                {
                    Debug.Log("hash 不匹配:" + group);
                }
                else
                {
                    Debug.Log("hash 匹配:" + group);
                    groups[group].Read(t);
                    if (groups[group].ver != this.ver)
                    {
                        Debug.Log("ver 不匹配:" + group);
                    }
                    if (groups[group].filecount != groups[group].files.Count)
                    {
                        Debug.Log("FileCount 不匹配:" + group);
                    }
                }
            }
            groupcount--;
            Debug.Log("groupcount=" + groupcount +"|"+group);
            if(groupcount==0)
            {
                onload(null);
            }
        };

        Action<UnityWebRequest,string> onLoadAll = (uwr,tag) =>
            {
                if(string.IsNullOrEmpty(uwr.error)==false)
                {
                    onload(new Exception(uwr.error));
                    return;
                    //SthWrong;
                }
                string t = uwr.downloadHandler.text;
                if (t[0] == 0xFEFF)
                {
                    t = t.Substring(1);
                }
                ReadVerAll(t);
                foreach (var g in _groups)
                {
                    if(groups.ContainsKey(g)==false)
                    {
                        Debug.LogWarning("(ver)指定的group:" + g + " 在资源服务器上不存在");
                        continue;
                    }
                    if (ResourceSystem.Instance.verLocal.groups.ContainsKey(g)==false || groups[g].hash != ResourceSystem.Instance.verLocal.groups[g].grouphash)
                    {
                        Debug.Log("(ver)group改变:" + g + " 下载同步");
                        groupcount++;
                        ResourceSystem.Instance.LoadFromRemote(g + ".ver.txt", g, onLoadGroup);
                    }
                    else
                    {
                        Debug.Log("(ver)group未改变:" + g);
                    }
                }
                if (groupcount == 0)
                {
                    onload(null);
                }
            };

        ResourceSystem.Instance.LoadFromRemote("allver.ver.txt", "", onLoadAll);
    }
    void ReadVerAll(string txt)
    {
        string[] lines = txt.Split(new string[] { "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);
        foreach (var l in lines)
        {
            if (l.IndexOf("Ver:") == 0)
            {
                ver = int.Parse(l.Substring(4));
            }
            else
            {
                //Debug.Log(l);
                var sp = l.Split('|');
                groups[sp[0]] = new Group(sp[0], sp[1], int.Parse(sp[2]));
            }
        }
    }
    public class Group
    {
        public Group(string group,string hash,int filecount)
        {
            this.group = group;
            this.hash = hash;
            this.filecount = filecount;
        }
        public string group;
        public string hash;
        public int filecount;
        public int ver;
        public class FileInfo
        {
            public FileInfo(string name,string hash,int len)
            {
                this.name = name;
                this.hash = hash;
                this.length = len;
            }
            public string name;
            public string hash;
            public int length;
        }
        public Dictionary<string, FileInfo> files = new Dictionary<string, FileInfo>();
        public void Read(string txt)
        {
            string[] lines = txt.Split(new string[] { "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var l in lines)
            {
                if (l.IndexOf("Ver:") == 0)
                {
                    var sp = l.Split(new string[] { "Ver:", "|FileCount:" }, StringSplitOptions.RemoveEmptyEntries);
                    int mver = int.Parse(sp[0]);
                    int mcount = int.Parse(sp[1]);
                    this.filecount = mcount;
                    this.ver = mver;

                }
                else
                {
                    var sp = l.Split(new char[] { '|', '@' });
                    //Debug.Log(l);
                    files[sp[0]] = new FileInfo(sp[0], sp[1], int.Parse(sp[2]));
                }
            }
        }
    }
    public Dictionary<string, Group> groups = new Dictionary<string, Group>();
}
