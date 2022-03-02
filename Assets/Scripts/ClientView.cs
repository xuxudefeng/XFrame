using Opc.Ua;
using OpcUaHelper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using XTreeView;

public class ClientView : MonoBehaviour
{
    public TMP_InputField ServeText;
    public Button BtnConection;
    public TMP_Text OPCUAState;
    public TreeViewControl treeViewControl;

    /// <summary>
    /// Opc客户端的核心类
    /// </summary>
    private OpcUaClient m_OpcUaClient = null;

    // Start is called before the first frame update
    void Start()
    {
        // Opc Ua 服务的初始化
        OpcUaClientInitialization();
        BtnConection.onClick.AddListener(() =>
        {
            ConnectServer();
        });
    }
    private void OnDestroy()
    {
        m_OpcUaClient.Disconnect();
    }
    /// <summary>
    /// 初始化
    /// </summary>
    private void OpcUaClientInitialization()
    {
        m_OpcUaClient = new OpcUaClient();
        m_OpcUaClient.OpcStatusChange += M_OpcUaClient_OpcStatusChange1; ;
        m_OpcUaClient.ConnectComplete += M_OpcUaClient_ConnectComplete;
        //// 匿名登录
        //m_OpcUaClient.UserIdentity = new UserIdentity(new AnonymousIdentityToken());
    }

    private async void ConnectServer()
    {
        try
        {
            await m_OpcUaClient.ConnectServer(ServeText.text);
            BtnConection.GetComponent<Image>().color = Color.green;
        }
        catch (Exception ex)
        {
            //ClientUtils.HandleException(Text, ex);
            Debug.Log(ex.Message);
        }
    }

    /// <summary>
    /// 连接服务器结束后马上浏览根节点
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void M_OpcUaClient_ConnectComplete(object sender, EventArgs e)
    {
        try
        {
            OpcUaClient client = (OpcUaClient)sender;
            Debug.Log(client.Connected);
            if (client.Connected)
            {
                // populate the browse view.
                PopulateBranch(ObjectIds.ObjectsFolder);
            }
        }
        catch (Exception exception)
        {
            Debug.Log(exception.Message);
            //ClientUtils.HandleException(Text, exception);
        }
    }
    #region 填入分支

    /// <summary>
    /// Populates the branch in the tree view.
    /// </summary>
    /// <param name="sourceId">The NodeId of the Node to browse.</param>
    /// <param name="nodes">The node collect to populate.</param>
    private async void PopulateBranch(NodeId sourceId)
    {
        // fetch references from the server.
        List<TreeViewData> listNode = await Task.Run(() =>
        {
            ReferenceDescriptionCollection references = GetReferenceDescriptionCollection(sourceId);
            List<TreeViewData> list = new List<TreeViewData>();
            if (references != null)
            {
                // process results.
                for (int ii = 0; ii < references.Count; ii++)
                {
                    ReferenceDescription target = references[ii];
                    TreeViewData treeViewData = new TreeViewData();
                    treeViewData.Name = target.BrowseName.ToString();

                    list.Add(treeViewData);
                }
            }
            return list;
        });
        treeViewControl.Data = listNode;
        treeViewControl.GenerateTreeView();
        treeViewControl.RefreshTreeView();
    }

    private ReferenceDescriptionCollection GetReferenceDescriptionCollection(NodeId sourceId)
    {
        TaskCompletionSource<ReferenceDescriptionCollection> task = new TaskCompletionSource<ReferenceDescriptionCollection>();

        // find all of the components of the node.
        BrowseDescription nodeToBrowse1 = new BrowseDescription();

        nodeToBrowse1.NodeId = sourceId;
        nodeToBrowse1.BrowseDirection = BrowseDirection.Forward;
        nodeToBrowse1.ReferenceTypeId = ReferenceTypeIds.Aggregates;
        nodeToBrowse1.IncludeSubtypes = true;
        nodeToBrowse1.NodeClassMask = (uint)(NodeClass.Object | NodeClass.Variable | NodeClass.Method | NodeClass.ReferenceType | NodeClass.ObjectType | NodeClass.View | NodeClass.VariableType | NodeClass.DataType);
        nodeToBrowse1.ResultMask = (uint)BrowseResultMask.All;

        // find all nodes organized by the node.
        BrowseDescription nodeToBrowse2 = new BrowseDescription();

        nodeToBrowse2.NodeId = sourceId;
        nodeToBrowse2.BrowseDirection = BrowseDirection.Forward;
        nodeToBrowse2.ReferenceTypeId = ReferenceTypeIds.Organizes;
        nodeToBrowse2.IncludeSubtypes = true;
        nodeToBrowse2.NodeClassMask = (uint)(NodeClass.Object | NodeClass.Variable | NodeClass.Method | NodeClass.View | NodeClass.ReferenceType | NodeClass.ObjectType | NodeClass.VariableType | NodeClass.DataType);
        nodeToBrowse2.ResultMask = (uint)BrowseResultMask.All;

        BrowseDescriptionCollection nodesToBrowse = new BrowseDescriptionCollection();
        nodesToBrowse.Add(nodeToBrowse1);
        nodesToBrowse.Add(nodeToBrowse2);

        // fetch references from the server.
        ReferenceDescriptionCollection references = FormUtils.Browse(m_OpcUaClient.Session, nodesToBrowse, false);
        return references;
    }

    #endregion
    /// <summary>
    /// OPC 客户端的状态变化后的消息提醒
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void M_OpcUaClient_OpcStatusChange1(object sender, OpcUaStatusEventArgs e)
    {
        //if (InvokeRequired)
        //{
        //    BeginInvoke(new Action(() =>
        //    {
        //        M_OpcUaClient_OpcStatusChange1(sender, e);
        //    }));
        //    return;
        //}

        if (e.Error)
        {
            OPCUAState.color = Color.red;
        }
        else
        {
            OPCUAState.color = Color.green;
        }
        OPCUAState.text = e.ToString();
    }
}
