using UnityEngine;
using UnityEngine.EventSystems;
public class InventorySlot : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        GameObject target = eventData.pointerDrag;
        if (target.TryGetComponent<InventoryItem>(out var inventoryItem))
        {
            if (transform.childCount == 0)
            {
                inventoryItem.rootPosition = transform;
            }
            else
            {
                GameObject child = transform.GetChild(0).gameObject;
                if (child.TryGetComponent<InventoryItem>(out var childItem))
                {
                    if (inventoryItem.CheckEquals(childItem))
                    {
                        int remain = childItem.UpdateItemQuantity(inventoryItem.GetCurrentQuantity());
                        inventoryItem.ChangeCurrentQuantity(remain);
                    }
                }
            }
        }

    }
}
