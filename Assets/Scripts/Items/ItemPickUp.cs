using System;
using UnityEngine;

public class ItemPickUp : MonoBehaviour, IInteractableObject, IPointerClickHandler
{
    [SerializeField]
    private Item _item;

    private bool _mooving;
    private Vector3 _startPosition;
    private Vector3 _destination;
    private float _flyDistance;
    private float _movementElapsedFrames;

    private void Start()
    {
        if (_item == null)
        {
            Debug.LogError("не указан предмет");
        }

        _movementElapsedFrames = 0f;
    }

    private void Update()
    {
        if (_mooving)
        {
            transform.position = CalcNextPosition(a => a, a => Mathf.Pow(a, 3), a => a);

            _movementElapsedFrames += Time.deltaTime / _flyDistance;
            if (_movementElapsedFrames >= 1f)
            {
                _movementElapsedFrames = 0f;
                transform.position = _destination;
                _mooving = false;
            }
        }
    }

    private Vector3 CalcNextPosition(Func<float, float> xFunc, Func<float, float> yFunc, Func<float, float> zFunc)
    {
        var xPos = Mathf.Lerp(_startPosition.x, _destination.x, xFunc(_movementElapsedFrames));
        var yPos = Mathf.Lerp(_startPosition.y, _destination.y, yFunc(_movementElapsedFrames));
        var zPos = Mathf.Lerp(_startPosition.z, _destination.z, zFunc(_movementElapsedFrames));
        return new Vector3(xPos, yPos, zPos);
    }

    public void SetItem(Item item)
    {
        if (_item != null)
        {
            Debug.LogError("Нахуй ты это делаешь сучара?");
            return;
        }

        _item = item;
    }

    public void ThrowItem(Vector3 destination, float flyDistance)
    {
        _destination = destination;
        _flyDistance = flyDistance * 0.5f;
        _startPosition = transform.position;
        _mooving = true;
    }

    public virtual void Interact()
    {
        if (InventoryController.Instance.CursorItem != null)
        {
            Debug.Log("Курсор занят");
            return;
        }
        Debug.Log($"Предмет собирается начать гулять за курсором");
        InventoryController.Instance.SetCursorItem(_item);

        Debug.Log($"Предмет {_item.Name} начал гулять за курсором");

        Destroy(gameObject);
    }

    public void LeftMouseClick()
    {
        Interact();
    }

    public void RightMouseClick()
    {
    }
}
