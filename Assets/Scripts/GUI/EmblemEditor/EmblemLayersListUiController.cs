using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EmblemLayersListUiController : MonoBehaviour
{
    [SerializeField] private Transform content;
    [SerializeField] private Button addNewButton;

    [SerializeField] private EmblemLayerItemUiController itemPrefab;
    [SerializeField] private int maxLayers = 50;

    [SerializeField] private List<EmblemLayerItemUiController> items = new List<EmblemLayerItemUiController>();

    [SerializeField] private int selectedIndex;

    public void ClearContent()
    {
        for (int i = content.childCount - 1; i >= 0; i--)
        {
            Destroy(content.GetChild(i).gameObject);
        }

        items.Clear();
    }

    public event Action<int> onLayerSelected;
    public event Action onAddNewLayer;
    public event Action<int> onLayerDeleted;

    // void OnEnable()
    // {
    //     addNewButton.onClick.AddListener(AddNewLayer);
    // }

    // void OnDisable()
    // {
    //     addNewButton.onClick.RemoveListener(AddNewLayer);
    // }

    public void AddNewLayer()
    {
        if (items.Count >= maxLayers) return;

        EmblemLayerItemUiController item = Instantiate(itemPrefab, content);
        items.Add(item);

        item.onSelected += () => SelectItem(items.IndexOf(item));
        item.onDeleted += () => DeleteItem(items.IndexOf(item));

        onAddNewLayer?.Invoke();
        SelectItem(items.Count - 1);
    }

    public void InitNewLayer()
    {
        if (items.Count >= maxLayers) return;

        EmblemLayerItemUiController item = Instantiate(itemPrefab, content);
        items.Add(item);

        item.onSelected += () => SelectItem(items.IndexOf(item));
        item.onDeleted += () => DeleteItem(items.IndexOf(item));

        SelectItem(items.Count - 1);
    }

    public void SetSelectedLayerShape(Sprite shape)
    {
        if (!IsValidIndex(selectedIndex)) return;
        items[selectedIndex].SetShape(shape);
    }

    public void SetSelectedLayerShapeColor(Color color)
    {
        if (!IsValidIndex(selectedIndex)) return;
        items[selectedIndex].SetColor(color);
    }

    public void SelectItem(int index)
    {
        if (!IsValidIndex(index)) return;

        for (int i = 0; i < items.Count; i++)
            items[i].SetSelected(i == index);

        selectedIndex = index;
        onLayerSelected?.Invoke(index);
    }

    private void DeleteItem(int index)
    {
        if (!IsValidIndex(index)) return;

        Destroy(items[index].gameObject);
        items.RemoveAt(index);
        onLayerDeleted?.Invoke(index);

        if (items.Count == 0)
        {
            return;
        }

        int newIndex = selectedIndex >= index ? Mathf.Clamp(selectedIndex - 1, 0, items.Count - 1) : selectedIndex;
        SelectItem(newIndex);
    }

    private bool IsValidIndex(int index)
    {
        return index >= 0 && index < items.Count;
    }
}
