using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameDevTV.RTS
{
    public class PlayerInput : MonoBehaviour
    {
        [SerializeField] private Transform _cameraTarget;
        [SerializeField] private CinemachineCamera _cinemachineCamera;
        [SerializeField] private float _keyboardPanSpeed = 15.0f;
        [SerializeField] private float _zoomSpeed = 1.0f;
        [SerializeField] private float _rotationSpeed = 1.0f;
        [SerializeField] private float _minZoomDistance = 7.5f;

        private CinemachineFollow _cinemachineFollow;
        private Vector2 _moveAmount;
        private float _zoomStartTime;
        private float _rotationStartTime;
        private Vector3 _startingFollowOffset;
        private float _maxRotationAmount;

        private void Awake()
        {
            _moveAmount = Vector2.zero;
            _cinemachineFollow = _cinemachineCamera.GetComponent<CinemachineFollow>();
            _startingFollowOffset = _cinemachineFollow.FollowOffset;
            _maxRotationAmount = Mathf.Abs(_cinemachineFollow.FollowOffset.z);
        }

        private void Update()
        {
            HandlePanning();
            HandleZooming();
            HandleRotation();
        }

        private void HandleRotation()
        {
            if(IsRotationStartTime())
                _rotationStartTime = Time.time;

            float ratationTime = Mathf.Clamp01((Time.time - _rotationStartTime) * _rotationSpeed);
            Vector3 targetFollowOffset = CalculateDesiredOffsetRotation();
            _cinemachineFollow.FollowOffset = Vector3.Slerp
                (
                    _cinemachineFollow.FollowOffset,
                    targetFollowOffset,
                    ratationTime
                );
        }

        private Vector3 CalculateDesiredOffsetRotation()
        {
            if (Keyboard.current.pageDownKey.isPressed)
                return new Vector3
                    (
                        _maxRotationAmount,
                        _cinemachineFollow.FollowOffset.y,
                        0
                    );
            else if (Keyboard.current.pageUpKey.isPressed)
                return new Vector3
                    (
                        -_maxRotationAmount,
                        _cinemachineFollow.FollowOffset.y,
                        0
                    );
            else
                return  new Vector3
                    (
                        _startingFollowOffset.x,
                        _cinemachineFollow.FollowOffset.y,
                        _startingFollowOffset.z
                    );
        }

        private bool IsRotationStartTime()
        {
            return Keyboard.current.pageUpKey.wasPressedThisFrame
                || Keyboard.current.pageDownKey.wasPressedThisFrame
                || Keyboard.current.pageUpKey.wasReleasedThisFrame
                || Keyboard.current.pageDownKey.wasReleasedThisFrame;
        }

        private void HandleZooming()
        {
            if (IsZoomStartTime())
                _zoomStartTime = Time.time;

            float zoomTime = Mathf.Clamp01((Time.time - _zoomStartTime) * _zoomSpeed);
            Vector3 targetFollowOffset = CalculateDesiredOffsetZoom();
            _cinemachineFollow.FollowOffset = Vector3.Slerp
                (
                    _cinemachineFollow.FollowOffset,
                    targetFollowOffset,
                    zoomTime
                );
        }

        private Vector3 CalculateDesiredOffsetZoom()
        {
            return Keyboard.current.endKey.isPressed ?
                new Vector3
                    (
                        _cinemachineFollow.FollowOffset.x,
                        _minZoomDistance,
                        _cinemachineFollow.FollowOffset.z
                    )
                    :
                new Vector3
                    (
                        _cinemachineFollow.FollowOffset.x,
                        _startingFollowOffset.y,
                        _cinemachineFollow.FollowOffset.z
                     );
        }

        private bool IsZoomStartTime()
        {
            return Keyboard.current.endKey.wasPressedThisFrame
                || Keyboard.current.endKey.wasReleasedThisFrame;
        }

        private void HandlePanning()
        {
            if (Keyboard.current.upArrowKey.isPressed)
                _moveAmount.y += _keyboardPanSpeed;

            if (Keyboard.current.downArrowKey.isPressed)
                _moveAmount.y -= _keyboardPanSpeed;

            if (Keyboard.current.leftArrowKey.isPressed)
                _moveAmount.x -= _keyboardPanSpeed;

            if (Keyboard.current.rightArrowKey.isPressed)
                _moveAmount.x += _keyboardPanSpeed;

            _moveAmount *= Time.deltaTime;

            ///TODO: Данный код надо будет перенести в отдельный класс управления камерой, 
            ///для разделения логики управления и перемещения камеры что сократить отвественности.
            _cameraTarget.position += new Vector3(_moveAmount.x, 0, _moveAmount.y);
        }
    }
}
