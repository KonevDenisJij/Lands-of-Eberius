using UnityEngine;

public class InventoryController : MonoBehaviour
{
    private static InventoryController _instance;
    public static InventoryController Instance => _instance;

    private Item _cursorItem;
    public Item CursorItem => _cursorItem;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
    }

    public bool HandleMouseAction(out RaycastHit hit)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        hit = new RaycastHit();

        /*if (InventoryUI.Instance.HandleMouseAction(out var inventorySlot))
        {
            HandleSlotClick(inventorySlot);

            return true;
        }*/
        
        if (_cursorItem != null && Camera.main.ScreenToViewportPoint(Input.mousePosition).y > 0.45f)
        {
            Debug.Log("Кинули предмет");
            ThrowItem(PlayerController.Instance.transform.position, PlayerController.Instance.transform.forward);

            return true;
        }

        if (!Physics.Raycast(ray, out hit, 1f))
        {
            return true;
        }

        if (_cursorItem != null && hit.transform.gameObject.tag == "Floor")
        {
            PlaceItem(ray);

            return true;
        }

        return false;
    }

    /// <summary>
    /// Положить предмет в определенную позицию
    /// </summary>
    /// <param name="position">Позиция</param>
    private void ThrowItem(Vector3 position, Vector3 direction)
    {
        _cursorItem.ThrowItem(position, direction);

        ClearCursorItem();
    }

    /// <summary>
    /// Положить предмет в место клика
    /// </summary>
    /// <param name="ray">Луч позиции мыши</param>
    private void PlaceItem(Ray ray)
    {
        _cursorItem.PlaceItem(ray);

        ClearCursorItem();
    }

    private void HandleSlotClick(InventorySlot slot)
    {
        if (_cursorItem == null)
        {
            var item = slot.TakeItemFromSlot();
            if (item != null)
            {
                SetCursorItem(item);
            }
            return;
        }

        var oldItem = slot.PutItemInSlot(_cursorItem);
        if (oldItem != null)
        {
            SetCursorItem(oldItem);
            return;
        }

        ClearCursorItem();
    }

    public void SetCursorItem(Item item)
    {
        _cursorItem = item;
        InventoryUI.Instance.SetCursorItem(item);
    }

    public void ClearCursorItem()
    {
        _cursorItem = null;
        InventoryUI.Instance.ClearCursorItem();
    }
}
