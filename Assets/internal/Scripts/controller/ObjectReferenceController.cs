using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectReferenceController : MonoBehaviour
{
    public static ObjectReferenceController instance;
    public List<ObjectReferenceType> objects = new();

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }
    public GameObject GetGameObject(ItemType itemType, ItemName itemName)
    {
        foreach (var item in objects)
        {
            if (item.itemType == itemType)
            {
                foreach (var worldItem in item.references)
                {
                    if (worldItem.itemName == itemName)
                    {
                        return worldItem.item;
                    }
                }
            }
        }
        return null;
    }
}
[System.Serializable]
public class ObjectReferenceType
{
    public ItemType itemType;
    public List<ObjectReference> references = new();
}
[System.Serializable]
public class ObjectReference
{
    public GameObject item;
    public ItemName itemName;
}