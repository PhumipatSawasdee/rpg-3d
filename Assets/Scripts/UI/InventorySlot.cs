using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    [SerializeField] private int id;
    public int ID {  get { return id; } set { id = value; } }

    [SerializeField] private InventoryManager inventoryManager;
    private void Start()
    {
        inventoryManager = InventoryManager.instance;
    }

    public void OnDrop(PointerEventData eventData)
    {
        GameObject objA = eventData.pointerDrag;
        ItemDrag itemDragA = objA.GetComponent<ItemDrag>();
        InventorySlot slotA = itemDragA.IconParrent.GetComponent<InventorySlot>();

        inventoryManager.RemoveItemInBag(slotA.ID);

        if (transform.childCount > 0)
        {
            GameObject objB = transform.GetChild(0).gameObject;
            ItemDrag itemDragB = objB.GetComponent<ItemDrag>();

            itemDragB.transform.SetParent(itemDragA.IconParrent);
            itemDragB.IconParrent = itemDragA.IconParrent;
            inventoryManager.SaveItemInBag(slotA.ID, itemDragB.Item);
        }

        itemDragA.IconParrent = transform;
        inventoryManager.SaveItemInBag(id, itemDragA.Item);
    }
}
