using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class EquipmentSlot : MonoBehaviour, IDropHandler
{
    [SerializeField] private List<ItemType> itemTypeList = new();
    [SerializeField] private GameObject defaultImage;
    [SerializeField] private GameObject container;
    private void Update()
    {
        defaultImage.SetActive(container.transform.childCount == 0);
    }
    public void OnDrop(PointerEventData eventData)
    {
        GameObject target = eventData.pointerDrag;
        if (target.TryGetComponent<InventoryItem>(out var inventoryItem))
        {
            if (container.transform.childCount == 0)
            {
                if (itemTypeList.Contains(inventoryItem.GetItemType()))
                {
                    inventoryItem.rootPosition = container.transform;
                }
            }
            else
            {

            }
        }

    }
}
