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

    private Item _itemInSlot;

    private Item _cursorItem => InventoryController.Instance.CursorItem;
    private Action<Item> _setCursorItem => InventoryController.Instance.SetCursorItem;
    private Action _clearCursorItem => InventoryController.Instance.ClearCursorItem;
    private Action<int, Item> _recordItem => InventoryController.Instance.RecordItem;

    public void LeftMouseClick()
    {
        if (_cursorItem == null)
        {
            _recordItem(_slotIndex, null);
            var item = TakeItemFromSlot();
            if (item != null)
            {
                _setCursorItem(item);
            }
            return;
        }

        var oldItem = PutItemInSlot(_cursorItem);
        if (oldItem != null)
        {
            _recordItem(_slotIndex, null);
            _recordItem(_slotIndex, _itemInSlot);
            _setCursorItem(oldItem);
            return;
        }

        _recordItem(_slotIndex, _itemInSlot);
        _clearCursorItem();
    }

    public void RightMouseClick()
    {
    }

    public Item PutItemInSlot(Item item)
    {
        var currentItem = _itemInSlot;
        _itemInSlot = item;
        _slotImage.sprite = _itemInSlot.Icon;
        _slotImage.enabled = true;

        return currentItem;
    }

    public Item TakeItemFromSlot()
    {
        var item = _itemInSlot;
        _slotImage.sprite = null;
        _slotImage.enabled = false;
        _itemInSlot = null;

        return item;
    }
}
