using UnityEngine;

public class PreferenceController : MonoBehaviour
{
    public static PreferenceController instance;

    [Header("UI")]
    public GameObject uiContainer;
    public GameObject inventoryUI;
    public GameObject inventoryContainer;
    public GameObject equipmentContainer;

    [Header("Slot")]
    public InventorySlot inventorySlot;

    private void Start()
    {
        foreach (Transform tran in uiContainer.transform)
        {
            tran.gameObject.SetActive(false);
        }
    }
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }
}
