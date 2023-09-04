using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    private static InventoryUI _instance;
    public static InventoryUI Instance => _instance;

    [SerializeField]
    private Image _coursorFollowerSprite;
    [SerializeField]
    private GraphicRaycaster _raycaster;
    [SerializeField]
    private EventSystem _eventSystem;

    private CharacterInventory _openedInventory;

    // Start is called before the first frame update
    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }

        _coursorFollowerSprite.raycastTarget = false;
    }

    void Update()
    {
        _coursorFollowerSprite.transform.position = Input.mousePosition;
    }

    public void OpenInventory(CharacterInventory inventory)
    {
        _openedInventory = inventory;
        _openedInventory.OpenInentory();
    }

    public bool HandleMouseAction(out InventorySlot slot)
    {
        var m_PointerEventData = new PointerEventData(_eventSystem);
        //Set the Pointer Event Position to that of the mouse position
        m_PointerEventData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();

        //Raycast using the Graphics Raycaster and mouse click position
        _raycaster.Raycast(m_PointerEventData, results);

        var result = results.FirstOrDefault(a => a.gameObject.GetComponent<InventorySlot>() != null).gameObject;

        slot = result != null ? result.gameObject.GetComponent<InventorySlot>() : null;

        return slot != null;
    }

    public void SetCursorItem(Item item)
    {
        _coursorFollowerSprite.enabled = true;
        _coursorFollowerSprite.sprite = item.Icon;
    }

    public void ClearCursorItem()
    {
        _coursorFollowerSprite.sprite = null;
        _coursorFollowerSprite.enabled = false;
    }
}
