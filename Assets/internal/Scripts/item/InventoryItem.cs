using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class InventoryItem : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [Tooltip("Dont change it")]
    [Header("UI Config")]
    [SerializeField] private Image img;
    [SerializeField] private TextMeshProUGUI quantityTxt;

    [Header("Item config")]
    [SerializeField] private ItemName itemName;
    [SerializeField] private ItemType itemType;
    [SerializeField] private bool useStack = false;
    [SerializeField] private int maxQuantity = 1;
    [SerializeField] private int currentQuantity = 1;

    [HideInInspector]
    public Transform rootPosition;
    private void Start()
    {
        rootPosition = transform.parent;
        quantityTxt.text = useStack ? currentQuantity.ToString() : "";
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        img.raycastTarget = false;
        transform.SetParent(PreferenceController.instance.uiContainer.transform);
        quantityTxt.text = "";
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    void IEndDragHandler.OnEndDrag(PointerEventData eventData)
    {
        img.raycastTarget = true;
        transform.SetParent(rootPosition);
        quantityTxt.text = useStack ? currentQuantity.ToString() : "";
    }
    public bool UseStack()
    {
        return useStack;
    }
    public int GetCurrentQuantity()
    {
        return currentQuantity;
    }
    public int UpdateItemQuantity(int amount = 1)
    {
        if (!UseStack())
        {
            return amount;
        }
        int remain = currentQuantity + amount;
        if (amount < 0)
        {
            if (remain <= 0)
            {
                currentQuantity = 0;
                Invoke(nameof(CheckQuantity), 0.1f);
                ChangeQuantityTxt();
                return remain;
            }
            else
            {
                currentQuantity = remain;
                ChangeQuantityTxt();
                return 0;
            }
        }
        else
        {
            if (remain <= maxQuantity)
            {
                currentQuantity = remain;
                ChangeQuantityTxt();
                return 0;
            }
            else
            {
                currentQuantity = maxQuantity;
                ChangeQuantityTxt();
                return remain - maxQuantity;
            }
        }
    }
    public void ChangeCurrentQuantity(int amount)
    {
        currentQuantity = amount;
        ChangeQuantityTxt();
        CheckQuantity();
    }
    public void CheckQuantity()
    {
        if (currentQuantity <= 0)
        {
            Destroy(gameObject);
        }
    }
    private void ChangeQuantityTxt()
    {
        quantityTxt.text = currentQuantity.ToString();
    }
    public bool CheckEquals(ItemType type, ItemName name)
    {
        return itemType == type && itemName == name;
    }
    public bool CheckEquals(InventoryItem item)
    {
        return itemType == item.GetItemType() && itemName == item.GetItemName();
    }
    public ItemType GetItemType()
    {
        return itemType;
    }
    public ItemName GetItemName()
    {
        return itemName;
    }
}
