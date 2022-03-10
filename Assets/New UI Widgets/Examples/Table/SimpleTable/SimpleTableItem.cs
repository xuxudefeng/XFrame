namespace UIWidgets.Examples
{
	using System;
	using UIWidgets;
	using UnityEngine;

	/// <summary>
	/// SimpleTable item.
	/// </summary>
	[Serializable]
	public class SimpleTableItem
	{
		/// <summary>
		/// Name.
		/// </summary>
		[SerializeField]
		public string Name;

		/// <summary>
		/// Value.
		/// </summary>
		[SerializeField]
		public string Value;

		/// <summary>
		/// Type.
		/// </summary>
		[SerializeField]
		public string Type;

		/// <summary>
		/// AccessLevel.
		/// </summary>
		[SerializeField]
		public string AccessLevel;

		/// <summary>
		/// Description.
		/// </summary>
		[SerializeField]
		public string Description;

		/// <summary>
		/// Convert instance to string.
		/// </summary>
		/// <returns>String.</returns>
		public override string ToString()
		{
			return string.Format("{0} | {1} | {2} | {3} | {4}", Name, Value, Type,AccessLevel,Description);
		}
	}
}