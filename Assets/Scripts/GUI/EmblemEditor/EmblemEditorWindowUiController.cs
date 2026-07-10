using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmblemEditorWindowUiController : WindowUiController
{
    [SerializeField] private ShapesListUiControiller shapesListUiControiller;
    [SerializeField] private EmblemLayersListUiController emblemLayersListUiController;
    [SerializeField] private DecalPropertiesUiController decalPropertiesUiController;
    [SerializeField] private EmblemCanvasUiController emblemCanvasUiController;

    [SerializeField] private Emblem tempEmblem;
    [SerializeField] private int decalSelectedIndex;

    private bool isApplyEmblem = false;

    public override void OnOpenWindow()
    {
        tempEmblem = PlayerData.instance.emblem.Clone();

        shapesListUiControiller.onShapeSelected += HandleShapeSelected;

        emblemLayersListUiController.onLayerSelected += HandleLayerSelected;
        emblemLayersListUiController.onAddNewLayer += HandleAddNewLayer;
        emblemLayersListUiController.onLayerDeleted += HandleLayerDeleted;

        decalPropertiesUiController.onColorSelected += HandleColorSelected;
        decalPropertiesUiController.onXPosChange += HandleXPosChange;
        decalPropertiesUiController.onYPosChange += HandleYPosChange;
        decalPropertiesUiController.onScaleChange += HandleScaleChange;
        decalPropertiesUiController.onRotateChange += HandleRotateChange;

        if (!isApplyEmblem)
        {
            ApplyFromTempEmblem();
            isApplyEmblem = true;
        }
    }

    public override void OnCloseWindow()
    {
        PlayerInventoryService.instance.SavePlayerEmblem(
            new EmblemRequest() {
                emblem = tempEmblem.ToJson()
            },
            (response) =>
            {
                PlayerData.instance.emblem = tempEmblem.Clone();
            },
            (statusCode, error) =>
            {
                Debug.LogError("[EmblemEditor] Failed to save emblem");
                isApplyEmblem = false;
            }
        );
    }

    void OnDestroy()
    {
        shapesListUiControiller.onShapeSelected -= HandleShapeSelected;

        emblemLayersListUiController.onLayerSelected -= HandleLayerSelected;
        emblemLayersListUiController.onAddNewLayer -= HandleAddNewLayer;
        emblemLayersListUiController.onLayerDeleted -= HandleLayerDeleted;

        decalPropertiesUiController.onColorSelected -= HandleColorSelected;
        decalPropertiesUiController.onXPosChange -= HandleXPosChange;
        decalPropertiesUiController.onYPosChange -= HandleYPosChange;
        decalPropertiesUiController.onScaleChange -= HandleScaleChange;
        decalPropertiesUiController.onRotateChange -= HandleRotateChange;
    }

    private Decal CurrentDecal => tempEmblem.decals[decalSelectedIndex];

    private void HandleShapeSelected(int shapeIndex)
    {
        CurrentDecal.shapeIndex = shapeIndex;
        emblemCanvasUiController.SetShape(shapeIndex);
        emblemLayersListUiController.SetSelectedLayerShape(StoreData.instance.shapesList[shapeIndex].shape);
    }

    private void HandleLayerSelected(int layerIndex)
    {
        decalSelectedIndex = layerIndex;
        emblemCanvasUiController.SetSelectedIndex(layerIndex);

        var decal = tempEmblem.decals[layerIndex];
        decalPropertiesUiController.SetProperties(decal.color, decal.x, decal.y, decal.scale, decal.rotate);
    }

    private void HandleAddNewLayer()
    {
        tempEmblem.decals.Add(new Decal());
        emblemCanvasUiController.AddNewDecal();
    }

    private void HandleLayerDeleted(int layerIndex)
    {
        tempEmblem.decals.RemoveAt(layerIndex);
        emblemCanvasUiController.RemoveDecalByIndex(layerIndex);
    }

    private void HandleColorSelected(Color color)
    {
        CurrentDecal.color = color;
        emblemCanvasUiController.SetColor(color);
        emblemLayersListUiController.SetSelectedLayerShapeColor(color);
    }

    private void HandleXPosChange(float xPos)
    {
        CurrentDecal.x = xPos;
        emblemCanvasUiController.SetXPos(xPos);
    }

    private void HandleYPosChange(float yPos)
    {
        CurrentDecal.y = yPos;
        emblemCanvasUiController.SetYPos(yPos);
    }

    private void HandleScaleChange(float scale)
    {
        CurrentDecal.scale = scale;
        emblemCanvasUiController.SetScale(scale);
    }

    private void HandleRotateChange(int rotate)
    {
        CurrentDecal.rotate = rotate;
        emblemCanvasUiController.SetRotate(rotate);
    }

    private void ApplyFromTempEmblem()
    {
        emblemLayersListUiController.ClearContent();
        emblemCanvasUiController.ClearContent();

        for (int i = 0; i < tempEmblem.decals.Count; i++)
        {
            decalSelectedIndex = i;

            emblemCanvasUiController.AddNewDecal();
            emblemCanvasUiController.SetSelectedIndex(i);

            emblemLayersListUiController.InitNewLayer();

            var decal = tempEmblem.decals[i];
            HandleShapeSelected(decal.shapeIndex);
            HandleColorSelected(decal.color);
            HandleXPosChange(decal.x);
            HandleYPosChange(decal.y);
            HandleScaleChange(decal.scale);
            HandleRotateChange(decal.rotate);
        }
    }
}
