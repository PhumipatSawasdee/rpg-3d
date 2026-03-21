using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] Item item;
    public Item Item { get { return item; } set { item = value; } }

    [SerializeField] Transform iconParrent;
    public Transform IconParrent { get { return iconParrent; } set { iconParrent = value; } }

    [SerializeField] Image image;
    public Image Image { get { return image; } set { image = value; } }

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
}
