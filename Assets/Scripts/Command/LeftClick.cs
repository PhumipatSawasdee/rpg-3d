using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class LeftClick : MonoBehaviour
{
    public static LeftClick instance;

    private Camera cam;

    [SerializeField] private LayerMask layerMask;

    [SerializeField] private RectTransform boxSelection;
    private Vector2 oldAnchoredPos;
    private Vector2 startPos;

    private void Start()
    {
        instance = this;
        cam = Camera.main;
        layerMask = LayerMask.GetMask("Ground", "Character", "Building", "Item");

        boxSelection = UIManager.instance.SelectionBox;
    }

    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            startPos = Mouse.current.position.value;

            // If click UI, don't clear
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            //ClearEverthing();
        }

        if (Mouse.current.leftButton.isPressed)
        {
            // If click UI, don't check
            /*if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }*/

            UpdateSelectionBox(Mouse.current.position.value);
        }

        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            ReleaseSelectionBox(Mouse.current.position.value);
            TrySelect(Mouse.current.position.value);
        }
    }

    private int SelectCharacter(RaycastHit hit)
    {
        ClearEverthing();

        Character hero = hit.collider.GetComponent<Character>();
        Debug.Log($"Selected Char: {hit.collider.gameObject}");

        int i = PartyManager.instance.FindIndexFromClass(hero);
        Debug.Log($"Click Release: {i}");
        UIManager.instance.ToggleAvatar[i].isOn = true;

        return i;
    }

    private void TrySelect(Vector2 screenPos)
    {
        Ray ray = cam.ScreenPointToRay(screenPos);
        RaycastHit hit;

        int i = 0;

        if (Physics.Raycast(ray, out hit, 1000, layerMask))
        {
            switch (hit.collider.tag)
            {
                case "Player":
                case "Hero":
                    i = SelectCharacter(hit);
                    break;

                case "Item":
                    SelectItem(hit); 
                    break;
            }
        }

        if (PartyManager.instance.SelectChars.Count == 0)
        {
            UIManager.instance.ToggleAvatar[i].isOn = true;
        }
    }

    private void UpdateSelectionBox(Vector2 mousePos)
    {
        if (!boxSelection.gameObject.activeInHierarchy) boxSelection.gameObject.SetActive(true);

        float width = mousePos.x - startPos.x;
        //Debug.Log($"Update Selection Box : ({width}) => {mousePos.x} - {startPos.x}");
        float height = mousePos.y - startPos.y;
        //Debug.Log($"Update Selection Box : ({height}) => {mousePos.y} - {startPos.y}");

        boxSelection.anchoredPosition = startPos + new Vector2(width / 2, height / 2);

        width = Mathf.Abs(width);
        height = Mathf.Abs(height);

        boxSelection.sizeDelta = new Vector2(width, height);

        oldAnchoredPos = boxSelection.anchoredPosition;
    }

    private void ReleaseSelectionBox(Vector2 mousePos)
    {
        // Down left corner
        Vector2 corner1;
        // Top right corner
        Vector2 corner2;

        boxSelection.gameObject.SetActive(false);

        // sizeDelta : width and height of the boxSelection
        corner1 = oldAnchoredPos - (boxSelection.sizeDelta / 2);
        //Debug.Log($"Release Selection Box : (Down left corner {corner1}) => {oldAnchoredPos} - ({boxSelection.sizeDelta} / 2)");
        corner2 = oldAnchoredPos + (boxSelection.sizeDelta / 2);
        //Debug.Log($"Release Selection Box : (Top right corner {corner2}) => {oldAnchoredPos} + ({boxSelection.sizeDelta} / 2)");

        bool anyNewCharSelect = false;

        foreach (Character member in PartyManager.instance.Members)
        {
            Vector2 unitPos = cam.WorldToScreenPoint(member.transform.position);

            // Check if the unit is in the selection box
            if ((unitPos.x > corner1.x && unitPos.x < corner2.x) 
                && (unitPos.y > corner1.y && unitPos.y < corner2.y))
            {
                if (!anyNewCharSelect)
                {
                    anyNewCharSelect = true;
                    ClearEverthing();
                }

                int i = PartyManager.instance.FindIndexFromClass(member);
                UIManager.instance.ToggleAvatar[i].isOn = true;
            }
        }

        // Clear selection Box's size
        boxSelection.sizeDelta = new Vector2(0, 0);
    }

    private void ClearRingSelection()
    {
        foreach (Character h in PartyManager.instance.SelectChars)
        {
            h.ToggleRingSelection(false);
        }
    }

    private void ClearEverthing()
    {
        foreach (Toggle t in UIManager.instance.ToggleAvatar)
            t.isOn = false;

        ClearRingSelection();
        PartyManager.instance.SelectChars.Clear();
    }

    private void SelectItem(RaycastHit hit)
    {
        ItemPick itemPick = hit.collider.GetComponent<ItemPick>();
        Debug.Log($"Pick Item: {itemPick.Item.ItemName}");

        if (PartyManager.instance.SelectChars.Count == 0)
            UIManager.instance.ToggleAvatar[0].isOn = true;

        if (itemPick != null)
            itemPick.PickUpItem();
    }
}
