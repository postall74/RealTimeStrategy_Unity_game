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
        [SerializeField] private float _minZoomDistance = 7.5f;

        private CinemachineFollow _cinemachineFollow;
        private Vector2 _moveAmount;
        private float _zoomStartTime;
        private Vector3 _startingFollowOffset;

        private void Awake()
        {
            _moveAmount = Vector2.zero;
            _cinemachineFollow = _cinemachineCamera.GetComponent<CinemachineFollow>();
            _startingFollowOffset = _cinemachineFollow.FollowOffset;
        }

        private void Update()
        {
            HandlePanning();
            HandleZooming();
        }

        private void HandleZooming()
        {
            if (IsZoomInputChanged())
                _zoomStartTime = Time.time;

            float zoomTime = Mathf.Clamp01((Time.time - _zoomStartTime) * _zoomSpeed);
            Vector3 targetFollowOffset = CalculateDesiredOffset();
            _cinemachineFollow.FollowOffset = Vector3.Slerp
                (
                    _cinemachineFollow.FollowOffset,
                    targetFollowOffset,
                    zoomTime
                );
        }

        private Vector3 CalculateDesiredOffset()
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

        private bool IsZoomInputChanged()
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
