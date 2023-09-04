using UnityEngine;

public class InventoryController : MonoBehaviour
{
    private static InventoryController _instance;
    public static InventoryController Instance => _instance;

    private Item _cursorItem;
    public Item CursorItem => _cursorItem;

    private CharacterInventory _openedInventory;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
    }

    public bool HandleMouseAction(Ray ray, RaycastHit hit)
    {
        if (_cursorItem == null)
        {
            return false;
        }

        if (Camera.main.ScreenToViewportPoint(Input.mousePosition).y > 0.45f)
        {
            ThrowItem(PlayerController.Instance.transform.position, PlayerController.Instance.transform.forward);

            return true;
        }

        if (hit.transform?.gameObject.tag == "Floor")
        {
            PlaceItem(ray);

            return true;
        }

        return false;
    }

    public void OpenInventory(CharacterInventory inventory)
    {
        if (_openedInventory == inventory)
        {
            InventoryUI.Instance.CloseInventory();
            _openedInventory = null;
            return;
        }

        _openedInventory = inventory;
        InventoryUI.Instance.OpenInventory(inventory.CharacterInfo);
    }

    /// <summary>
    /// Записать предмет
    /// </summary>
    /// <param name="position">Номер ячейки</param>
    /// <param name="item">Предмет (null == удаление предмета)</param>
    public void RecordItem(int position, Item item) 
    {
        if (item != null)
        {
            _openedInventory.CharacterInfo.AddInventoryItem(position, item);
            return;
        }

        _openedInventory.CharacterInfo.RemoveInventoryItem(position);
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
