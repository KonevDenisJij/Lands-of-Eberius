using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    private static InventoryUI _instance;
    public static InventoryUI Instance => _instance;

    [SerializeField]
    private Image _coursorFollowerSprite;
    [SerializeField]
    private Transform _inventoryParent;
    [SerializeField]
    private TextMeshProUGUI _characterName;
    [SerializeField]
    private TextMeshProUGUI _raceClassLvlInfo;

    // Start is called before the first frame update
    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }

        _coursorFollowerSprite.raycastTarget = false;
    }

    void Update()
    {
        _coursorFollowerSprite.transform.position = Input.mousePosition;
    }

    public void OpenInventory(CharacterInfo info)
    {
        _characterName.text = info.Name;
        _raceClassLvlInfo.text = info.GetRaceClassLvlInfo();
        foreach(var slot in GetComponentsInChildren<InventorySlot>())
        {
            if (info.CharacterInventory.TryGetValue(slot.SlotIndex, out var item))
            {
                slot.PutItemInSlot(item);
            }
            else
            {
                slot.TakeItemFromSlot();
            }
        }
        _inventoryParent.gameObject.SetActive(true);
    }

    public void CloseInventory()
    {
        _inventoryParent.gameObject.SetActive(false);
    }

    public void SetCursorItem(Item item)
    {
        _coursorFollowerSprite.enabled = true;
        _coursorFollowerSprite.sprite = item.Icon;
    }

    public void ClearCursorItem()
    {
        _coursorFollowerSprite.sprite = null;
        _coursorFollowerSprite.enabled = false;
    }
}
