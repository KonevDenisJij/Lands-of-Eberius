using System;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private Image _slotImage;

    private Item _itemInSlot;

    private Item _cursorItem => InventoryController.Instance.CursorItem;
    private Action<Item> _setCursorItem => InventoryController.Instance.SetCursorItem;
    private Action _clearCursorItem => InventoryController.Instance.ClearCursorItem;

    public void LeftMouseClick()
    {
        if (_cursorItem == null)
        {
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
            _setCursorItem(oldItem);
            return;
        }

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
