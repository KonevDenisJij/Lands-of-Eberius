using System.Linq;
using UnityEngine;
using static UnityEditor.Progress;

public class InventoryController : MonoBehaviour
{
    private static InventoryController _instance;
    public static InventoryController Instance => _instance;

    private Item _cursorItem;
    public Item CursorItem => _cursorItem;

    private CharacterInventory _currentInventory;
    public CharacterInventory CurrentInventory => _currentInventory;

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
        if (_currentInventory == inventory)
        {
            InventoryUI.Instance.CloseInventory();
            _currentInventory = null;
            return;
        }

        _currentInventory = inventory;
        InventoryUI.Instance.OpenInventory(inventory.CharacterInfo);
    }

    /// <summary>
    /// Записать предмет
    /// </summary>
    /// <param name="position">Номер ячейки (если юольше 20 то слот снаряжения)/param>
    /// <param name="item">Предмет (null == удаление предмета)</param>
    /*public bool RecordItem(int position, Item item) 
    {
        if (item != null)
        {
            if (position == 28 || position == 29)
            {
                return _currentInventory.CharacterInfo.AddWeaponItem(position, item);
            }
            if (position >=30)
            {
                var characterInfo = InventoryUI.Instance.CharacterInfos.SingleOrDefault(a => a.RightHand.SlotIndex == position || a.LeftHand.SlotIndex == position);
                return characterInfo.AddWeaponItem(position, item);
            }
            if (position >= 20)
            {
                return _currentInventory.CharacterInfo.AddEquipmentItem(position, item);
            }
            return _currentInventory.CharacterInfo.AddInventoryItem(position, item);
        }

        if (position == 28 || position == 29)
        {
            return _currentInventory.CharacterInfo.RemoveWeaponItem(position);
        }
        if (position >= 30)
        {
            var characterInfo = InventoryUI.Instance.CharacterInfos.SingleOrDefault(a => a.RightHand.SlotIndex == position || a.LeftHand.SlotIndex == position);
            return characterInfo.RemoveWeaponItem(position);
        }
        if (position >= 20)
        {
            return _currentInventory.CharacterInfo.RemoveEquipmentItem(position);
        }
        return _currentInventory.CharacterInfo.RemoveInventoryItem(position);
    } */

    public bool PutItemInSlot(int slotIndex, Item item)
    {
        return _currentInventory.CharacterInfo.AddInventoryItem(slotIndex, item);
    }

    public Item TakeItemFromSlot(int slotIndex)
    {
        return _currentInventory.CharacterInfo.RemoveInventoryItem(slotIndex);
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
        if (item is EquipableItem equipableItem)
        {
            InventoryUI.Instance.HighlightSlot(equipableItem.SlotType);
        }

        InventoryUI.Instance.SetCursorItem(item);
    }

    public void ClearCursorItem()
    {
        _cursorItem = null;
        InventoryUI.Instance.ClearCursorItem();
    }
}
