using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    [SerializeField] Item item;
    public Item Item { get { return item; } set { item = value; } }

    [SerializeField] Transform iconParrent;
    public Transform IconParrent { get { return iconParrent; } set { iconParrent = value; } }

    [SerializeField] Image image;
    public Image Image { get { return image; } set { image = value; } }

    [SerializeField] private UIManager uiManager;
    public UIManager UIManager { get {  return uiManager; } set { uiManager = value; } }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("BeginDrag");
        iconParrent = transform.parent;

        // Set drag item to root of parrent
        transform.SetParent(transform.root);
        // Set to last parent
        transform.SetAsLastSibling();

        image.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("Dragging");
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("EndDrag");
        transform.SetParent(iconParrent);
        image.raycastTarget = true;
    }

    private int FindIndexOfSlotParrent()
    {
        int id = iconParrent.GetComponent<InventorySlot>().ID;
        return id;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            Debug.Log("Right Click on Item");

            if (item.Type == ItemType.Consumable)
            {
                uiManager.SetCurItemInUse(this, FindIndexOfSlotParrent());
                uiManager.ToggleItemDialog(true);
            }
        }
    }
}
