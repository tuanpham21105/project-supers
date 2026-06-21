using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccessoriesTypeItemUiController : MonoBehaviour
{
    [SerializeField] private StoreItemsType type;

    public void OnPress()
    {
        StoreUiController.instance.SelectType(type);
    }
}
