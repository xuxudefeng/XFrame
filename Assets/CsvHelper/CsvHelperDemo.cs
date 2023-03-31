using System.Collections.Generic;
using UnityEngine;
using Newtonsoft;
using Newtonsoft.Json;

public class CsvHelperDemo :MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //using (var reader = new StreamReader(@"E:\Project\GitHub_Project\XFrame\Assets\CsvHelper\Foo.csv"))
        //{
        //    using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        //    {
        //        List<Foo> records = csv.GetRecords<Foo>().ToList();
        //        foreach (var item in records)
        //        {
        //            Debug.Log(item.Name);
        //        }
        //    }
        //}

        var records1 = new List<Foo>
        {
            new Foo { Id = 1, Name = "one",Position = new Vector3(100,100,100) },
            new Foo { Id = 2, Name = "two",Position = new Vector3(100,100,100)  },
            new Foo { Id = 3, Name = "three",Position = new Vector3(100,100,100)  },
        };


        var json = JsonUtility.ToJson(records1);
        var jsonNet = JsonConvert.SerializeObject(records1);
        Debug.Log(json);
        Debug.Log(jsonNet); 

        //using (var writer = new StreamWriter(@"E:\Project\Unity_Project\XFrame\Assets\CsvHelper\Foo111.csv"))
        //using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        //{
        //    csv.WriteRecords(records1);
        //}
    }
}
public class Foo
{
    public int Id { get; set; }
    public string Name { get; set; }
    public Vector3 Position { get; set; }

}
