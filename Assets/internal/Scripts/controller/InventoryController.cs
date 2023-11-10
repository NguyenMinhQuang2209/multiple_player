using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    public static InventoryController instance;

    [SerializeField] private int inventorySlot = 1;
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }
    private void Start()
    {
        SpawnInventorySlot();
    }
    private void SpawnInventorySlot()
    {
        GameObject container = PreferenceController.instance.inventoryContainer;
        InventorySlot slot = PreferenceController.instance.inventorySlot;
        for (int i = 0; i < inventorySlot; i++)
        {
            Instantiate(slot, container.transform);
        }
    }
}
