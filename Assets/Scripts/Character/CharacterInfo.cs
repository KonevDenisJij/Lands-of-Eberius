using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEditor.Progress;

public class CharacterInfo : MonoBehaviour
{
    [SerializeField]
    private string _name;
    public string Name => _name;
    [SerializeField]
    private string _race;
    [SerializeField]
    private string _class;
    [SerializeField]
    private WeaponSlot _leftHand;
    public WeaponSlot LeftHand => _leftHand;
    [SerializeField]
    private WeaponSlot _rightHand;
    public WeaponSlot RightHand => _rightHand;

    private Dictionary<int, Equipment> _characterInventory;
    public Dictionary<int, Equipment> CharacterInventory => _characterInventory;

    public string GetRaceClassLvlInfo() => $"{_race} {_class} Level 1";

    private void Start()
    {
        if (_characterInventory == null)
        {
            _characterInventory = new Dictionary<int, Equipment>();
            FillCharacterEquipment();
        }
    }

    private void FillCharacterEquipment()
    {
        for (int i = 0; i < 20; i++)
        {
            _characterInventory.Add(i, new Equipment(EquipmentSlotType.Any, null));
        }

        _characterInventory.Add(20, new Equipment(EquipmentSlotType.Cape, null));
        _characterInventory.Add(21, new Equipment(EquipmentSlotType.Mittens, null));
        _characterInventory.Add(22, new Equipment(EquipmentSlotType.Head, null));
        _characterInventory.Add(23, new Equipment(EquipmentSlotType.Body, null));
        _characterInventory.Add(24, new Equipment(EquipmentSlotType.Legs, null));
        _characterInventory.Add(25, new Equipment(EquipmentSlotType.Boots, null));
        _characterInventory.Add(26, new Equipment(EquipmentSlotType.Necklace, null));
        _characterInventory.Add(27, new Equipment(EquipmentSlotType.Bracer, null));
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            Debug.Log($"{_name} Предметы: {string.Join(", ", _characterInventory.Select(a => a.Key + " " + a.Value))}");
        }
    }

    public bool AddInventoryItem(int position, Item item)
    {
        if (!_characterInventory.TryGetValue(position, out var slot) && slot.SlotType != EquipmentSlotType.Any && slot.SlotType != item.SlotType)
        {
            Debug.LogError($"Предмет {item.Name} не подходит для слота {position}");
            return false;
        }

        _characterInventory[position].SetItem(item);
        return true;
    }

    public Item RemoveInventoryItem(int position)
    {
        if (_characterInventory.TryGetValue(position, out var oldItem) && oldItem.Item == null)
        {
            Debug.LogError($"В позиции {position} нет предмета для удаления");
            return oldItem.Item;
        }

        _characterInventory[position].SetItem(null);
        return oldItem.Item;
    }

    public class Equipment
    {
        public Equipment(EquipmentSlotType slotType, Item item)
        {
            SlotType = slotType;
            Item = item;
        }

        public EquipmentSlotType SlotType;
        public Item Item;

        public void SetItem(Item item)
        {
            Item = item;
        }
    }
}
