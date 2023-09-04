using System.ComponentModel;
using UnityEngine;

public abstract class Item : ScriptableObject
{
    [SerializeField]
    protected string _name;

    [SerializeField]
    private ItemPickUp _itemPrefab;

    [SerializeField]
    private float _flyDistance;
    public ItemPickUp ItemPrefab => _itemPrefab;

    [SerializeField]
    private Sprite _icon;
    public Sprite Icon => _icon;

    public string Name => _name;

    protected bool _inInventory = false;
    public bool InInventory => _inInventory;

    /// <summary>
    /// Положить предмет в место клика
    /// </summary>
    /// <param name="ray">Луч позиции мыши</param>
    public virtual void PlaceItem(Ray ray)
    {
        var pickUpItem = GetItemPickUp();
        pickUpItem.transform.position = CalculateItemPosition(ray, pickUpItem.transform.localScale.y);
    }

    /// <summary>
    /// Положить предмет в определенную позицию
    /// </summary>
    /// <param name="position">Позиция</param>
    public virtual void PlaceItem(Vector3 position)
    {
        var pickUpItem = GetItemPickUp();
        pickUpItem.transform.position = position;
    }

    /// <summary>
    /// Положить предмет в определенную позицию
    /// </summary>
    /// <param name="position">Позиция</param>
    public virtual void ThrowItem(Vector3 position, Vector3 direction)
    {
        var pickUpItem = GetItemPickUp();
        pickUpItem.transform.position = position + new Vector3(0f, 0.35f, 0f) + direction * 0.1f;

        var destination = position + direction * _flyDistance + new Vector3(0f, pickUpItem.transform.localScale.y / 2, 0f);

        pickUpItem.ThrowItem(destination, _flyDistance);
    }

    protected virtual ItemPickUp GetItemPickUp()
    {
        var pickUpItem = Instantiate(_itemPrefab);
        pickUpItem.SetItem(this);

        return pickUpItem;
    }

    protected virtual Vector3 CalculateItemPosition(Ray ray, float yValue)
    {
        var plane = new Plane(Vector3.up, 0);
        var floorPosition = (plane.Raycast(ray, out var distance)
        ? ray.GetPoint(distance)
            : PlayerController.Instance.transform.position)
            + new Vector3(0f, yValue / 2, 0f);
        var position = PlayerController.Instance.transform.position;
        var maxForwardValue = position + PlayerController.Instance.transform.forward * 0.6f;


        if (PlayerController.Instance.transform.forward.z > 0.9f)
        {
            floorPosition = new Vector3(Mathf.Clamp(floorPosition.x, position.x - 0.6f, position.x + 0.6f),
                floorPosition.y,
                Mathf.Clamp(floorPosition.z, position.z, maxForwardValue.z));
        }
        else if (PlayerController.Instance.transform.forward.z < -0.9f)
        {
            floorPosition = new Vector3(Mathf.Clamp(floorPosition.x, position.x - 0.6f, position.x + 0.6f),
                floorPosition.y,
                Mathf.Clamp(floorPosition.z, maxForwardValue.z, position.z));
        }
        else if (PlayerController.Instance.transform.forward.x > 0.9f)
        {
            floorPosition = new Vector3(Mathf.Clamp(floorPosition.x, position.x, maxForwardValue.x),
                floorPosition.y,
                Mathf.Clamp(floorPosition.z, position.z - 0.6f, position.z + 0.6f));
        }
        else
        {
            floorPosition = new Vector3(Mathf.Clamp(floorPosition.x, maxForwardValue.x, position.x),
                floorPosition.y,
                Mathf.Clamp(floorPosition.z, position.z - 0.6f, position.z + 0.6f));
        }


        return floorPosition;
    }

    public abstract void UseItem();

    protected abstract void StartInternal();

    protected abstract void UpdateInternal();
}
