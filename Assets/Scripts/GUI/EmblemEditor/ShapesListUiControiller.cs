using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapesListUiControiller : MonoBehaviour
{
    [SerializeField] private Transform content;
    [SerializeField] private ShapeItemUiController prefab;

    public event Action<int> onShapeSelected;

    void OnEnable()
    {
        if (content.childCount == 0)
        {
            for (int i = 0; i < StoreData.instance.shapesList.Count; i++)
            {
                Shape a = StoreData.instance.shapesList[i];
                ShapeItemUiController item = Instantiate(prefab, content);
                item.Initialize(i, a.shape, a.name);
                item.onSelected += handleShapeSelected;
            }
        }
    }

    void handleShapeSelected(int index)
    {
        onShapeSelected?.Invoke(index);
    }
}
