using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

public class UILayout<T> : IMappedObjectList where T : IMappedObject, new()
{
    public T Original => original;
    public T[] Elements => elements.ToArray();
    public IMappedObject[] MappedObjects => new[] { (IMappedObject)original };

    private readonly T original;
    private readonly ILayouter layouter;
    private List<T> elements;
    private readonly List<T> cachedElements;

    public UILayout(T original, ILayouter layouter = null)
    {
        if (layouter == null) layouter = new NoneLayouter();

        this.original = original;
        this.layouter = layouter;
        this.elements = new List<T>();
        this.cachedElements = new List<T>();

        this.original.Mapper.Get().SetActive(false);
    }

    public LayoutEditor Edit(EditMode editMode = EditMode.Clear)
    {
        if (editMode == EditMode.Clear)
        {
            foreach (var element in Elements.Reverse())
            {
                var gameObject = element.Mapper.Get();
                gameObject.GetComponent<UILayoutElement>().Deactivate();
                gameObject.SetActive(false);
                cachedElements.Insert(0, element);
            }
            elements.Clear();
        }
        return new LayoutEditor(this);
    }

    public void Clear(bool purgeCache = false)
    {
        UpdateContents(new List<T>());
        if (purgeCache)
        {
            foreach (var element in cachedElements)
            {
                Object.Destroy(element.Mapper.Get());
            }
            cachedElements.Clear();
        }
    }

    private void UpdateContents(List<T> newElements)
    {
        foreach (var element in Elements)
        {
            if (newElements.Contains(element)) continue;
            var gameObject = element.Mapper.Get();
            gameObject.GetComponent<UILayoutElement>().Deactivate();
            gameObject.SetActive(false);
            cachedElements.Add(element);
        }

        elements = newElements;
        layouter.Layout(original.Mapper, Elements.Select(x => x.Mapper).ToArray());
    }

    public class LayoutEditor : IDisposable
    {
        public List<T> Elements { get; }
        private readonly UILayout<T> parent;

        public LayoutEditor(UILayout<T> parent)
        {
            this.parent = parent;
            Elements = parent.elements.ToList();
        }

        public T Create()
        {
            T newObject;
            UILayoutElement layoutElement;
            if (parent.cachedElements.Count > 0)
            {
                newObject = parent.cachedElements[0];
                parent.cachedElements.RemoveAt(0);
                layoutElement = newObject.Mapper.Get().GetComponent<UILayoutElement>();
            }
            else
            {
                newObject = parent.original.Duplicate();
                layoutElement = newObject.Mapper.Get().AddComponent<UILayoutElement>();
            }

            newObject.Mapper.Get().SetActive(true);
            Elements.Add(newObject);
            if (newObject is IReusableMappedObject reusableMappedObject)
            {
                reusableMappedObject.Activate();
                layoutElement.ReusableMappedObject = reusableMappedObject;
            }
            return newObject;
        }

        public void Dispose()
        {
            parent.UpdateContents(Elements);
        }
    }
}

public class UILayoutElement : MonoBehaviour
{
    public IReusableMappedObject ReusableMappedObject { get; set; }

    public void OnDestroy()
    {
        Deactivate();
    }

    public void Deactivate()
    {
        ReusableMappedObject?.Deactivate();
        ReusableMappedObject = null;
    }
}



public interface ILayouter
{
    void Layout(IMapper original, IMapper[] elements);
}
