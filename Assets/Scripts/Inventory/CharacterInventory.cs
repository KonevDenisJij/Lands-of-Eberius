using UnityEngine;

public class CharacterInventory : MonoBehaviour, IPointerClickHandler
{
    private CharacterInfo _characterInfo;
    public CharacterInfo CharacterInfo => _characterInfo ?? SetCharacterInfo();

    public void LeftMouseClick()
    {
        OpenInventory();
    }

    public void RightMouseClick()
    {
        LeftMouseClick();
    }

    public void OpenInventory()
    {
        InventoryController.Instance.OpenInventory(this);
    }

    private CharacterInfo SetCharacterInfo()
    {
        _characterInfo = GetComponentInParent<CharacterInfo>();
        return _characterInfo;
    }
}
