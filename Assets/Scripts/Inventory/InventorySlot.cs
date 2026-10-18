using System;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private Image _slotImage;
    [SerializeField]
    private int _slotIndex;
    public int SlotIndex => _slotIndex;

    private Item _itemInSlot => InventoryController.Instance.CurrentInventory.CharacterInfo.CharacterInventory[_slotIndex].Item;
    private Item _cursorItem => InventoryController.Instance.CursorItem;
    private Action<Item> _setCursorItem => InventoryController.Instance.SetCursorItem;
    private Action _clearCursorItem => InventoryController.Instance.ClearCursorItem;
    private Func<int, Item, bool> _putItem => InventoryController.Instance.PutItemInSlot;
    private Func<int, Item> _takeItem => InventoryController.Instance.TakeItemFromSlot;

    public void LeftMouseClick()
    {
        if (_cursorItem != null && _itemInSlot != null)
        {
            _setCursorItem(PutItemInSlot(_cursorItem, out var success));
        }
        else if (_cursorItem != null)
        {
            PutItemInSlot(_cursorItem, out var success);

            if (success)
            {
                _clearCursorItem();
            }
        }
        else if (_itemInSlot != null)
        {
            _setCursorItem(TakeItemFromSlot());
        }
    }

    public virtual void RightMouseClick()
    {
    }

    protected virtual Item PutItemInSlot(Item item, out bool success)
    {
        var currentItem = _itemInSlot;
        success = _putItem(_slotIndex, item);
        if (!success)
        {
            return item;
        }

        _slotImage.sprite = _itemInSlot.Icon;
        _slotImage.enabled = true;

        return currentItem;
    }

    protected virtual Item TakeItemFromSlot()
    {
        var item = _takeItem(_slotIndex);

        _slotImage.sprite = null;
        _slotImage.enabled = false;

        return item;
    }
}
