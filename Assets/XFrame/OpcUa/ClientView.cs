using Opc.Ua;
using OpcUaHelper;
using System;
using System.Threading.Tasks;
using UIWidgets;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UIWidgets.Examples;

public class ClientView : MonoBehaviour
{
    public InputField ServeText;
    public Button BtnConnect;
    public Button BtnDeConnect;
    public Text OPCUAState;

    public InputField SelectNodeId;
    public Text TextTimeSpeed;

    public TreeView TreeView;
    /// <summary>
    /// SimpleTable.
    /// </summary>
    public SimpleTable Table;

    /// <summary>
    /// Opc客户端的核心类
    /// </summary>
    private OpcUaClient m_OpcUaClient = null;

    // Start is called before the first frame update
    void Start()
    {
        // Opc Ua 服务的初始化
        OpcUaClientInitialization();
        SetConnectButton();
        TreeView.Init();
        TreeView.Nodes = new ObservableList<TreeNode<TreeViewItem>>();
        BtnConnect.onClick.AddListener(() =>
        {
            ConnectServer();
        });
        BtnDeConnect.onClick.AddListener(() =>
        {
            m_OpcUaClient.Disconnect();
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
        m_OpcUaClient.OpcStatusChange += OnOpcStatusChange1; ;
        m_OpcUaClient.ConnectComplete += OnConnectComplete;
        //m_OpcUaClient.UserIdentity = new UserIdentity(new AnonymousIdentityToken());
        Login("admin", "admin");
    }
    private void SetConnectButton()
    {
        BtnConnect.gameObject.SetActive(!m_OpcUaClient.Connected);
        BtnDeConnect.gameObject.SetActive(m_OpcUaClient.Connected);
    }
    private async void ConnectServer()
    {
        try
        {
            await m_OpcUaClient.ConnectServer(ServeText.text);
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
        }
    }

    /// <summary>
    /// 连接服务器结束后马上浏览根节点
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnConnectComplete(object sender, EventArgs e)
    {
        try
        {
            OpcUaClient client = (OpcUaClient)sender;
            SetConnectButton();
            if (client.Connected)
            {
                PopulateBranch(ObjectIds.ObjectsFolder, TreeView.Nodes);
            }
        }
        catch (Exception exception)
        {
            Debug.Log(exception.Message);
        }
    }
    /// <summary>
    /// 当OPC 客户端的状态变化后
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnOpcStatusChange1(object sender, OpcUaStatusEventArgs e)
    {
        if (e.Error)
        {
            OPCUAState.color = Color.red;
        }
        else
        {
            OPCUAState.color = Color.black;
        }
        OPCUAState.text = "状态："+e.ToString();
    }
    /// <summary>
    /// 用户名密码登录
    /// </summary>
    /// <param name="username"></param>
    /// <param name="password"></param>
    private void Login(string username,string password)
    {
        m_OpcUaClient.UserIdentity = new UserIdentity(username, password);
    }
    public void Nodes_Expand(TreeNode<TreeViewItem> node)
    {
        try
        {
            if (node.Nodes.Count != 1)
            {
                return;
            }

            if (node.Nodes.Count > 0)
            {
                if (node.Nodes[0].Item.Name != String.Empty)
                {
                    return;
                }
            }

            ReferenceDescription reference = node.Item.Tag as ReferenceDescription;

            if (reference == null || reference.NodeId.IsAbsolute)
            {
                return;
            }

            PopulateBranch((NodeId)reference.NodeId, node.Nodes);
        }
        catch (Exception exception)
        {
            MessageBox.Show(exception.Message);
        }
    }

    private async void PopulateBranch(NodeId sourceId, ObservableList<TreeNode<TreeViewItem>> nodes)
    {
        nodes.Clear();
        nodes.Add(new TreeNode<TreeViewItem>(new TreeViewItem("读取中...")));

        ObservableList<TreeNode<TreeViewItem>> listNode = await Task.Run(() =>
        {
            ReferenceDescriptionCollection references = GetReferenceDescriptionCollection(sourceId);
            ObservableList<TreeNode<TreeViewItem>> list = new ObservableList<TreeNode<TreeViewItem>>();
            if (references != null)
            {
                for (int ii = 0; ii < references.Count; ii++)
                {
                    ReferenceDescription target = references[ii];
                    var test_item = new TreeViewItem(target.DisplayName.Text);
                    var child = new TreeNode<TreeViewItem>(test_item);
                    child.Item.Tag = target;
                    if (GetReferenceDescriptionCollection((NodeId)target.NodeId).Count > 0)
                    {
                        child.Nodes = new ObservableList<TreeNode<TreeViewItem>>();
                        child.Nodes.Add(new TreeNode<TreeViewItem>(new TreeViewItem("")));
                    }

                    list.Add(child);
                }
            }

            return list;
        });

        nodes.Clear();
        nodes.AddRange(listNode);
    }

    private IEnumerator GetNodeData(NodeId sourceId, ObservableList<TreeNode<TreeViewItem>> nodes)
    {

        ReferenceDescriptionCollection references = GetReferenceDescriptionCollection(sourceId);
        if (references != null)
        {
            for (int ii = 0; ii < references.Count; ii++)
            {
                ReferenceDescription target = references[ii];
                var test_item = new TreeViewItem(target.DisplayName.Text);
                var child = new TreeNode<TreeViewItem>(test_item);

                // 查找是否有下级菜单
                if (GetReferenceDescriptionCollection((NodeId)target.NodeId).Count > 0)
                {
                    child.Nodes = new ObservableList<TreeNode<TreeViewItem>>();
                    yield return GetNodeData((NodeId)target.NodeId, child.Nodes);
                }

                yield return new WaitForEndOfFrame();
                nodes.Add(child);
            }
        }
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

    public void Nodes_Select(TreeNode<TreeViewItem> node)
    {
        try
        {
            RemoveAllSubscript();

            ReferenceDescription reference = node.Item.Tag as ReferenceDescription;

            if (reference == null || reference.NodeId.IsAbsolute)
            {
                return;
            }

            ShowMember((NodeId)reference.NodeId);
        }
        catch (Exception exception)
        {
            MessageBox.Show(exception.Message);
        }
    }
    private void RemoveAllSubscript()
    {
        m_OpcUaClient?.RemoveAllSubscription();
    }
    private async void ShowMember(NodeId sourceId)
    {
        ClearDataGridViewRows(0);
        SelectNodeId.text = sourceId.ToString();

        // dataGridView1.Rows.Clear();
        int index = 0;
        ReferenceDescriptionCollection references;
        try
        {
            references = await Task.Run(() =>
            {
                return GetReferenceDescriptionCollection(sourceId);
            });
        }
        catch (Exception exception)
        {
            MessageBox.Show(exception.Message);
            return;
        }


        if (references?.Count > 0)
        {
            // 获取所有要读取的子节点
            List<NodeId> nodeIds = new List<NodeId>();
            for (int ii = 0; ii < references.Count; ii++)
            {
                ReferenceDescription target = references[ii];
                nodeIds.Add((NodeId)target.NodeId);
            }

            DateTime dateTimeStart = DateTime.Now;

            // 获取所有的值
            DataValue[] dataValues = await Task.Run(() =>
            {
                return ReadOneNodeFiveAttributes(nodeIds);
            });

            TextTimeSpeed.text = (int)(DateTime.Now - dateTimeStart).TotalMilliseconds + " ms";

            // 显示
            for (int jj = 0; jj < dataValues.Length; jj += 5)
            {
                AddDataGridViewNewRow(dataValues, jj, index++, nodeIds[jj / 5]);
            }

        }
        else
        {
            // 子节点没有数据的情况
            try
            {
                DateTime dateTimeStart = DateTime.Now;
                DataValue dataValue = m_OpcUaClient.ReadNode(sourceId);

                if (dataValue.WrappedValue.TypeInfo?.ValueRank == ValueRanks.OneDimension)
                {
                    // 数组显示
                    AddDataGridViewArrayRow(sourceId, out index);
                }
                else
                {
                    // 显示单个数本身
                    TextTimeSpeed.text = (int)(DateTime.Now - dateTimeStart).TotalMilliseconds + " ms";
                    AddDataGridViewNewRow(ReadOneNodeFiveAttributes(new List<NodeId>() { sourceId }), 0, index++, sourceId);
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
                return;
            }
        }

        //ClearDataGridViewRows(index);

    }
    private void AddDataGridViewNewRow(DataValue[] dataValues, int startIndex, int index, NodeId nodeId)
    {
        var item = new SimpleTableItem();
        if (dataValues[startIndex].WrappedValue.Value == null) return;
        NodeClass nodeclass = (NodeClass)dataValues[startIndex].WrappedValue.Value;

        item.Name = dataValues[3 + startIndex].WrappedValue.Value?.ToString();
        item.AccessLevel = GetDiscriptionFromAccessLevel(dataValues[2 + startIndex]);
        item.Description = dataValues[4 + startIndex].WrappedValue.Value?.ToString();
        if (nodeclass == NodeClass.Object)
        {
            item.Value = "";
            item.Type = nodeclass.ToString();
        }
        else if (nodeclass == NodeClass.Method)
        {
            item.Value = "";
            item.Type = nodeclass.ToString();
        }
        else if (nodeclass == NodeClass.Variable)
        {
            DataValue dataValue = dataValues[1 + startIndex];

            if (dataValue.WrappedValue.TypeInfo != null)
            {
                item.Type = dataValue.WrappedValue.TypeInfo.BuiltInType.ToString();
                if (dataValue.WrappedValue.TypeInfo.ValueRank == ValueRanks.Scalar)
                {
                    item.Value = dataValue.WrappedValue.Value?.ToString();
                }
                else if (dataValue.WrappedValue.TypeInfo.ValueRank == ValueRanks.OneDimension)
                {
                    item.Value = dataValue.Value?.GetType().ToString();
                }
                else if (dataValue.WrappedValue.TypeInfo.ValueRank == ValueRanks.TwoDimensions)
                {
                    item.Value = dataValue.Value?.GetType().ToString();
                }
                else
                {
                    item.Value = dataValue.Value?.GetType().ToString();
                }
            }
            else
            {
                item.Value = dataValue.Value?.ToString();
                item.Type = "null";
            }
        }
        else
        {
            item.Value = "";
            item.Type = nodeclass.ToString();
        }
        Table.DataSource.Add(item);
    }
    /// <summary>
    /// 0:NodeClass  1:Value  2:AccessLevel  3:DisplayName  4:Description
    /// </summary>
    /// <param name="nodeIds"></param>
    /// <returns></returns>
    private DataValue[] ReadOneNodeFiveAttributes(List<NodeId> nodeIds)
    {
        ReadValueIdCollection nodesToRead = new ReadValueIdCollection();
        foreach (var nodeId in nodeIds)
        {
            NodeId sourceId = nodeId;
            nodesToRead.Add(new ReadValueId()
            {
                NodeId = sourceId,
                AttributeId = Attributes.NodeClass
            });
            nodesToRead.Add(new ReadValueId()
            {
                NodeId = sourceId,
                AttributeId = Attributes.Value
            });
            nodesToRead.Add(new ReadValueId()
            {
                NodeId = sourceId,
                AttributeId = Attributes.AccessLevel
            });
            nodesToRead.Add(new ReadValueId()
            {
                NodeId = sourceId,
                AttributeId = Attributes.DisplayName
            });
            nodesToRead.Add(new ReadValueId()
            {
                NodeId = sourceId,
                AttributeId = Attributes.Description
            });
        }

        // read all values.
        m_OpcUaClient.Session.Read(
            null,
            0,
            TimestampsToReturn.Neither,
            nodesToRead,
            out DataValueCollection results,
            out DiagnosticInfoCollection diagnosticInfos);

        ClientBase.ValidateResponse(results, nodesToRead);
        ClientBase.ValidateDiagnosticInfos(diagnosticInfos, nodesToRead);

        return results.ToArray();
    }
    private void AddDataGridViewArrayRow(NodeId nodeId, out int index)
    {

        DateTime dateTimeStart = DateTime.Now;
        DataValue[] dataValues = ReadOneNodeFiveAttributes(new List<NodeId>() { nodeId });
        TextTimeSpeed.text = (int)(DateTime.Now - dateTimeStart).TotalMilliseconds + " ms";

        DataValue dataValue = dataValues[1];

        if (dataValue.WrappedValue.TypeInfo?.ValueRank == ValueRanks.OneDimension)
        {
            string access = GetDiscriptionFromAccessLevel(dataValues[2]);
            BuiltInType type = dataValue.WrappedValue.TypeInfo.BuiltInType;
            object des = dataValues[4].Value ?? "";
            object dis = dataValues[3].Value ?? type;

            Array array = dataValue.Value as Array;
            int i = 0;
            foreach (object obj in array)
            {
                var item = new SimpleTableItem() {
                    Name = $"{dis} [{i++}]",
                    Value = obj?.ToString(),
                    Type = type.ToString(),
                    AccessLevel = access,
                    Description = des?.ToString()
                };
                Table.DataSource.Add(item);
            }
            index = i;
        }
        else
        {
            index = 0;
        }
    }

    private string GetDiscriptionFromAccessLevel(DataValue value)
    {
        if (value.WrappedValue.Value != null)
        {
            switch ((byte)value.WrappedValue.Value)
            {
                case 0: return "None";
                case 1: return "CurrentRead";
                case 2: return "CurrentWrite";
                case 3: return "CurrentReadOrWrite";
                case 4: return "HistoryRead";
                case 8: return "HistoryWrite";
                case 12: return "HistoryReadOrWrite";
                case 16: return "SemanticChange";
                case 32: return "StatusWrite";
                case 64: return "TimestampWrite";
                default: return "None";
            }
        }
        else
        {
            return "null";
        }
    }
    private void ClearDataGridViewRows(int index)
    {

        Table.DataSource.Clear();
    }

}
