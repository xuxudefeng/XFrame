using CsvHelper;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

public class CsvHelperDemo : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        using (var reader = new StreamReader(@"E:\Project\GitHub_Project\XFrame\Assets\XFrame\CsvHelper\Foo.csv"))
        {
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                var records = csv.GetRecords<Foo>();
                foreach (var item in records)
                {
                    Debug.Log(item.Name);
                }
            }
        }

        var records1 = new List<Foo>
        {
            new Foo { Id = 1, Name = "one" },
            new Foo { Id = 2, Name = "two" },
            new Foo { Id = 3, Name = "three" },
        };
        using (var writer = new StreamWriter(@"E:\Project\GitHub_Project\XFrame\Assets\XFrame\CsvHelper\Foo1.csv"))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteRecords(records1);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
public class Foo
{
    public int Id { get; set; }
    public string Name { get; set; }
}
