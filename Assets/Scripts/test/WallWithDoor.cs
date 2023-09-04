using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallWithDoor : MonoBehaviour, IMechanismObject
{
    [SerializeField]
    private Transform _door;
    [SerializeField]
    private float _speed = 2f;

    private bool _doorState = false; //false = closed, true = open
    private bool _moveDoor;
    private Vector3 _nextPos;
    private Vector3 _oldPos;
    private float _elapsedFrames = 0f;

    private void Start()
    {
        _nextPos = _door.localPosition;
        _oldPos = _nextPos;
    }

    private void Update()
    {
        if (!_moveDoor)
        {
            return;
        }

        MoveDoor();
    }

    public void ProcessSignal()
    {
        _doorState = !_doorState;

        _moveDoor = true;
        if (_doorState)
        {
            _nextPos += new Vector3(0f, 1.5f, 0f);
            _oldPos = _elapsedFrames > 0f ? _oldPos - new Vector3(0f, 1.5f, 0f) : _oldPos;

        }
        else
        {
            _nextPos -= new Vector3(0f, 1.5f, 0f);
            _oldPos = _elapsedFrames > 0f ? _oldPos + new Vector3(0f, 1.5f, 0f) : _oldPos;
        }

        _elapsedFrames = _elapsedFrames > 0f  ? _speed - _elapsedFrames : 0f;
    }

    public void MoveDoor()
    {
        var interpolationRatio = _elapsedFrames / _speed;

        var interpolatedPosition = Vector3.LerpUnclamped(_oldPos, _nextPos, interpolationRatio);
        _door.localPosition = interpolatedPosition;

        _elapsedFrames += Time.deltaTime;  // reset elapsedFrames to zero after it reached (interpolationFramesCount + 1)
        if (_elapsedFrames >= _speed)
        {
            _elapsedFrames = 0f;
            _door.localPosition = _nextPos;
            _oldPos = _nextPos;
            _moveDoor = false;
        }
    }
}
