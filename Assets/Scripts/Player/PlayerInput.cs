using UnityEngine;
using UnityEngine.InputSystem;

namespace GameDevTV.RTS
{
    public class PlayerInput : MonoBehaviour
    {
        [SerializeField] private Transform _cameraTarget;
        [SerializeField] private float _keyboardPanSpeed = 15f;

        private Vector2 _moveAmount;

        private void Awake()
        {
            _moveAmount = Vector2.zero;
        }

        private void Update()
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
