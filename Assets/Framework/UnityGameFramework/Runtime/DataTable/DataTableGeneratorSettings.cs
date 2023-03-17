using UnityEngine;

[CreateAssetMenu(fileName = "DataTableSettings", menuName = "ScriptableObjects/DataTableSetting", order = 1)]
public class DataTableGeneratorSettings : ScriptableObject
{
    public string DataTablePath = "Assets/GameMain/DataTables";
    public string CSharpCodePath = "Assets/GameMain/Scripts/DataTable";
    public string CSharpCodeTemplateFileName = "Assets/Framework/UnityGameFramework/Editor/DataTableGenerator/Template/DataTableCodeTemplate.txt";

    public  string[] DataTableNames = new string[]
    {

    };
}
