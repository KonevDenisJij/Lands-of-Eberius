using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float _rotationSpeed = 0.5f; //в секундах
    [SerializeField]
    private float _movementSpeed = 1f; //в секундах
    [SerializeField]
    private float _mouseSensitivity = 100f;
    [SerializeField]
    private Transform _head;

    [SerializeField]
    private GraphicRaycaster _raycaster;
    [SerializeField]
    private EventSystem _eventSystem;

    private float _rotationElapsedFrames = 0f;
    private float _movementElapsedFrames = 0f;
    private bool _turning = false;
    private bool _mooving = false;
    private bool _freeRotationMode = false;

    private float _xRotation = 0f;

    private Quaternion _nextRotation;
    private Quaternion _oldBodyRotation;
    private Quaternion _oldCameraRotation;

    private Vector3 _nextPosition;
    private Vector3 _oldPosition;

    private static PlayerController _instance;
    public static PlayerController Instance => _instance;

    private void Start()
    {
        _instance = this;
    }

    private void Update()
    {
        HandleMouse();
        HandleRotation();
        HandleMovement();
    }

    private void HandleMouse()
    {
        if (!_freeRotationMode && Input.GetMouseButtonDown(0))
        {
            // Определяем физ объекты
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Physics.Raycast(ray, out var hit, 1f);

            // Определяем UI объекты
            var m_PointerEventData = new PointerEventData(_eventSystem);
            m_PointerEventData.position = Input.mousePosition;
            List<RaycastResult> results = new List<RaycastResult>();
            _raycaster.Raycast(m_PointerEventData, results);

            // Проверка на размещение/метание предмета
            if (!results.Any(a => a.gameObject.GetComponent<IPointerClickHandler>() != null) && InventoryController.Instance.HandleMouseAction(ray, hit))
            {
                return;
            }

            var interactableObject = results.Any()
                ? results.SingleOrDefault(a => a.gameObject.GetComponent<IPointerClickHandler>() != null).gameObject?.GetComponent<IPointerClickHandler>()
                : hit.transform != null
                    ? hit.transform.gameObject.GetComponent<IPointerClickHandler>()
                    : null;

            if (interactableObject != null)
            {
                interactableObject.LeftMouseClick();
            }
        }
    }

    /// <summary>
    /// Кнопки управления движением
    /// </summary>
    private void HandleMovement()
    {
        if (_mooving)
        {
            Move();
            return;
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            SetDirection(MovementDirection.Forward);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            SetDirection(MovementDirection.Backwards);
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            SetDirection(MovementDirection.Left);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            SetDirection(MovementDirection.Right);
        }
    }

    /// <summary>
    /// Кнопки управления поворотом
    /// </summary>
    private void HandleRotation()
    {
        if (Input.GetMouseButton(1))
        {
            FreeRotation();
            _freeRotationMode = true;
            return;
        }

        if (Input.GetMouseButtonUp(1))
        {
            SetRotation(0);
            _freeRotationMode = false;
        }

        if (_turning)
        {
            Turn();
            return;
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            SetRotation(-1);
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            SetRotation(1);
        }
    }

    /// <summary>
    /// Определить направление движения
    /// </summary>
    /// <param name="direction">Направление движения</param>
    private void SetDirection(MovementDirection direction)
    {
        var nextTile = Physics.OverlapSphere(transform.position + GetDirectionVector(direction), 0.01f).SingleOrDefault(a => a.tag == "Floor");

        if (nextTile != null)
        {
            _mooving = true;
            _nextPosition = nextTile.transform.position;
            _oldPosition = transform.position;
        }

        Vector3 GetDirectionVector(MovementDirection direction)
        {
            var newTransform = transform;
            newTransform.rotation = _nextRotation;
            switch (direction)
            {
                case MovementDirection.Forward:
                    return newTransform.forward;
                case MovementDirection.Right:
                    return newTransform.right;
                case MovementDirection.Backwards:
                    return newTransform.forward * -1;
                case MovementDirection.Left:
                    return newTransform.right * -1;
                default:
                    Debug.LogError("Непонятное направление движения");
                    return Vector3.zero;
            }
        }
    }

    /// <summary>
    /// Сдвинуться
    /// </summary>
    private void Move()
    {
        var interpolationRatio = _movementElapsedFrames / _movementSpeed;

        var interpolatedPosition = Vector3.LerpUnclamped(_oldPosition, _nextPosition, interpolationRatio);
        transform.position = interpolatedPosition;

        _movementElapsedFrames += Time.deltaTime;  // reset elapsedFrames to zero after it reached (interpolationFramesCount + 1)
        if (_movementElapsedFrames >= _movementSpeed)
        {
            _movementElapsedFrames = 0f;
            transform.position = _nextPosition;
            _mooving = false;
        }
    }

    /// <summary>
    /// Свободное вращение камеры
    /// </summary>
    private void FreeRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * _mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * _mouseSensitivity * Time.deltaTime;

        _xRotation -= mouseY;
        _xRotation = Mathf.Clamp(_xRotation, -60f, 60f);

        _head.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    /// <summary>
    /// Определить вращение (через кнопки)
    /// </summary>
    /// <param name="direction"> направление вращение</param>
    private void SetRotation(int direction)
    {
        _turning = true;
        _oldBodyRotation = transform.rotation;
        _oldCameraRotation = _head.localRotation;
        _nextRotation = GetRotation(transform.rotation.eulerAngles.y + (90 * direction));
        _xRotation = 0f;

        Quaternion GetRotation(float yAngle)
        {
            if (yAngle < 0f)
            {
                yAngle = 360f + yAngle;
            }

            if (yAngle >= 315f || yAngle <= 45f)
            {
                return Quaternion.Euler(0f, 0f, 0f);
            }
            else if (yAngle >= 225f && yAngle <= 315f)
            {
                return Quaternion.Euler(0f, -90f, 0f);
            }
            else if (yAngle >= 135f && yAngle <= 225f)
            {
                return Quaternion.Euler(0f, 180f, 0f);
            }
            else if (yAngle >= 45f && yAngle <= 135f)
            {
                return Quaternion.Euler(0f, 90f, 0f);
            }
            else
            {
                Debug.LogError("Неверный угол");
                return Quaternion.Euler(0f, 0f, 0f);
            }
        }
    }

    /// <summary>
    /// Повернуться
    /// </summary>
    private void Turn()
    {
        var interpolationRatio = _rotationElapsedFrames / _rotationSpeed;

        var interpolatedPosition = Quaternion.Lerp(_oldBodyRotation, _nextRotation, interpolationRatio);
        var cameraInterpolation = Quaternion.Lerp(_oldCameraRotation, Quaternion.Euler(0f, 0f, 0f), interpolationRatio);
        transform.rotation = interpolatedPosition;
        _head.localRotation = cameraInterpolation;

        _rotationElapsedFrames += Time.deltaTime;  // reset elapsedFrames to zero after it reached (interpolationFramesCount + 1)
        if (_rotationElapsedFrames >= _rotationSpeed)
        {
            _rotationElapsedFrames = 0f;
            transform.rotation = _nextRotation;
            _head.localRotation = Quaternion.Euler(0f, 0f, 0f);
            _turning = false;
        }
    }
}
