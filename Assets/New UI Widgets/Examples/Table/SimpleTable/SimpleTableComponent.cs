namespace UIWidgets.Examples
{
	using System;
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// SimpleTable component.
	/// </summary>
	public class SimpleTableComponent : ListViewItem, IViewData<SimpleTableItem>, IUpgradeable
	{
		/// <summary>
		/// Name.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		[System.Obsolete("Replaced with Cell01TextAdapter.")]
		public Text Name;

		/// <summary>
		/// Value.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		[System.Obsolete("Replaced with Cell02TextAdapter.")]
		public Text Value;

		/// <summary>
		/// Name.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		[System.Obsolete("Replaced with Cell01TextAdapter.")]
		public Text Type;

		/// <summary>
		/// Value.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		[System.Obsolete("Replaced with Cell02TextAdapter.")]
		public Text AccessLevel;

		/// <summary>
		/// Name.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		[System.Obsolete("Replaced with Cell01TextAdapter.")]
		public Text Description;

		/// <summary>
		/// Name.
		/// </summary>
		[SerializeField]
		public TextAdapter NameAdapter;

		/// <summary>
		/// Value.
		/// </summary>
		[SerializeField]
		public TextAdapter ValueAdapter;

		/// <summary>
		/// Type.
		/// </summary>
		[SerializeField]
		public TextAdapter TypeAdapter;

		/// <summary>
		/// AccessLevel.
		/// </summary>
		[SerializeField]
		public TextAdapter AccessLevelAdapter;

		/// <summary>
		/// Description.
		/// </summary>
		[SerializeField]
		public TextAdapter DescriptionAdapter;

		/// <summary>
		/// Init graphics foreground.
		/// </summary>
		protected override void GraphicsForegroundInit()
		{
			if (GraphicsForegroundVersion == 0)
			{
				Foreground = new Graphic[]
				{
					UtilitiesUI.GetGraphic(NameAdapter),
					UtilitiesUI.GetGraphic(ValueAdapter),
					UtilitiesUI.GetGraphic(TypeAdapter),
					UtilitiesUI.GetGraphic(AccessLevelAdapter),
					UtilitiesUI.GetGraphic(DescriptionAdapter),
				};
				GraphicsForegroundVersion = 1;
			}
		}

		/// <summary>
		/// Init graphics background.
		/// </summary>
		protected override void GraphicsBackgroundInit()
		{
			if (GraphicsBackgroundVersion == 0)
			{
				graphicsBackground = Compatibility.EmptyArray<Graphic>();
				GraphicsBackgroundVersion = 1;
			}
		}

		/// <summary>
		/// Gets the objects to resize.
		/// </summary>
		/// <value>The objects to resize.</value>
		public GameObject[] ObjectsToResize
		{
			get
			{
				return new[]
				{
					NameAdapter.transform.parent.gameObject,
					ValueAdapter.transform.parent.gameObject,
					TypeAdapter.transform.parent.gameObject,
					AccessLevelAdapter.transform.parent.gameObject,
					DescriptionAdapter.transform.parent.gameObject,
				};
			}
		}

		/// <summary>
		/// Set data.
		/// </summary>
		/// <param name="item">Item.</param>
		public void SetData(SimpleTableItem item)
		{
			NameAdapter.text = item.Name;
			ValueAdapter.text = item.Value;
			TypeAdapter.text = item.Type;
			AccessLevelAdapter.text = item.AccessLevel;
			DescriptionAdapter.text = item.Description;
		}

		/// <summary>
		/// Upgrade this instance.
		/// </summary>
		public override void Upgrade()
		{
#pragma warning disable 0612, 0618
			Utilities.GetOrAddComponent(Name, ref NameAdapter);
			Utilities.GetOrAddComponent(Value, ref ValueAdapter);
			Utilities.GetOrAddComponent(Type, ref TypeAdapter);
			Utilities.GetOrAddComponent(AccessLevel, ref AccessLevelAdapter);
			Utilities.GetOrAddComponent(Description, ref DescriptionAdapter);
#pragma warning restore 0612, 0618
		}
	}
}