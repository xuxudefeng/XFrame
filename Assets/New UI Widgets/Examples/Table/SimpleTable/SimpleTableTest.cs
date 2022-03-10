namespace UIWidgets.Examples
{
	using UnityEngine;

	/// <summary>
	/// Test SimpleTable.
	/// </summary>
	public class SimpleTableTest : MonoBehaviour
	{
		/// <summary>
		/// SimpleTable.
		/// </summary>
		public SimpleTable Table;

		/// <summary>
		/// Add item.
		/// </summary>
		public void Add()
		{
			var item = new SimpleTableItem() { Name = "First row 1", Value = "First row 2", Type = "First row 3",AccessLevel = "dfdfsfd",Description = "dfasdfsdlfsd" };
			Table.DataSource.Add(item);
		}

		/// <summary>
		/// Remove item.
		/// </summary>
		public void Remove()
		{
			Table.DataSource.RemoveAt(0);
		}

		/// <summary>
		/// Add item at start.
		/// </summary>
		public void AddAtStart()
		{
			//var item = new SimpleTableItem() { Field1 = "First row 1", Field2 = "First row 2", Field3 = "First row 3" };
			//Table.DataSource.Insert(0, item);
		}
	}
}