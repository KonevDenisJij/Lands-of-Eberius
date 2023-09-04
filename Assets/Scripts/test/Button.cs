using UnityEngine;

public class Button : MonoBehaviour, IInteractableObject, IPointerClickHandler
{
    [SerializeField]
    private WallWithDoor _mechanism;

    [SerializeField]
    IMechanismObject Mechanism => _mechanism;

    public void Interact()
    {
        _mechanism.ProcessSignal();
    }

    public void LeftMouseClick()
    {
        Interact();
    }

    public void RightMouseClick()
    {
    }
}
