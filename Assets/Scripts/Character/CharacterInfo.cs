using System.Collections.Generic;
using UnityEngine;

public class CharacterInfo : MonoBehaviour
{
    [SerializeField]
    private string _name;
    public string Name => _name;
    [SerializeField]
    private string _race;
    [SerializeField]
    private string _class;

    private Dictionary<int, Item> _characterInventory;
    public Dictionary<int, Item> CharacterInventory => _characterInventory;
    private Dictionary<EquipmentSlot, Item> _characterEquipment;

    public string GetRaceClassLvlInfo() => $"{_race} {_class} Level 1";

    private void Start()
    {
        if (_characterInventory == null)
        {
            _characterInventory = new Dictionary<int, Item>();
        }
    }

    public void AddInventoryItem(int position, Item item)
    {
        if (_characterInventory.TryGetValue(position, out var oldItem))
        {
            Debug.LogError($"В позиции {position} уже есть проедмет {oldItem.Name}");
            return;
        }

        _characterInventory[position] = item;
    }

    public void RemoveInventoryItem(int position)
    {
        if (!_characterInventory.TryGetValue(position, out var oldItem))
        {
            Debug.LogError($"В позиции {position} нет предмета для удаления");
            return;
        }

        _characterInventory[position] = null;
    }
}
