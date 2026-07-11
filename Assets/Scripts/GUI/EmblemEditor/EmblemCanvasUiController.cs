using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EmblemCanvasUiController : MonoBehaviour
{
    [SerializeField] private RectTransform content;
    [SerializeField] private RectTransform decalPrefab;

    [SerializeField] private GameObject selectedMarkerObject;
    [SerializeField] private List<RectTransform> decalObjects = new List<RectTransform>();

    [SerializeField] private int decalSelectedIndex;

    private RectTransform SelectedDecal => decalObjects[decalSelectedIndex];

    public void ClearContent()
    {
        selectedMarkerObject.SetActive(false);
        selectedMarkerObject.transform.SetParent(content, false);

        for (int i = decalObjects.Count - 1; i >= 0; i--)
        {
            Destroy(decalObjects[i].gameObject);
        }

        decalObjects.Clear();
    }

    public void AddNewDecal()
    {
        RectTransform decal = Instantiate(decalPrefab, content);
        decalObjects.Add(decal);
    }

    public void RemoveDecalByIndex(int index)
    {
        if (index == decalSelectedIndex)
        {
            selectedMarkerObject.SetActive(false);
            selectedMarkerObject.transform.SetParent(content, false);
        }

        Destroy(decalObjects[index].gameObject);
        decalObjects.RemoveAt(index);

        if (decalObjects.Count == 0)
            selectedMarkerObject.SetActive(false);
    }

    public void SetSelectedIndex(int index)
    {
        decalSelectedIndex = index;
        selectedMarkerObject.transform.SetParent(SelectedDecal, false);
        selectedMarkerObject.GetComponent<Image>().sprite = SelectedDecal.GetComponent<Image>().sprite;
        selectedMarkerObject.SetActive(true);
    }

    public void SetShape(int shapeIndex)
    {
        Sprite sprite = StoreData.instance.shapesList[shapeIndex].shape;
        SelectedDecal.GetComponent<Image>().sprite = sprite;
        
        SetSelectedIndex(decalSelectedIndex);
    }

    public void SetColor(Color color)
    {
        SelectedDecal.GetComponent<Image>().color = color;
    }

    public void SetXPos(float x)
    {
        Vector2 anchor = SelectedDecal.anchorMin;
        anchor.x = x;
        SelectedDecal.anchorMin = anchor;

        anchor = SelectedDecal.anchorMax;
        anchor.x = x;
        SelectedDecal.anchorMax = anchor;

        SelectedDecal.anchoredPosition = Vector2.zero;
    }

    public void SetYPos(float y)
    {
        Vector2 anchor = SelectedDecal.anchorMin;
        anchor.y = y;
        SelectedDecal.anchorMin = anchor;

        anchor = SelectedDecal.anchorMax;
        anchor.y = y;
        SelectedDecal.anchorMax = anchor;

        SelectedDecal.anchoredPosition = Vector2.zero;
    }

    public void SetRotate(int rotate)
    {
        SelectedDecal.localEulerAngles = new Vector3(0, 0, rotate);
    }

    public void SetScale(float scale)
    {
        SelectedDecal.localScale = new Vector3(scale, scale, scale);
    }

    public void ApplyEmblem(Emblem emblem)
    {
        ClearContent();

        for (int i = 0; i < emblem.decals.Count; i++)
        {
            Decal decal = emblem.decals[i];
            AddNewDecal();
            decalSelectedIndex = i;

            SetShape(decal.shapeIndex);
            SetColor(decal.color);
            SetXPos(decal.x);
            SetYPos(decal.y);
            SetRotate(decal.rotate);
            SetScale(decal.scale);
        }

        if (emblem.decals.Count > 0)
            SetSelectedIndex(0);
    }
}
