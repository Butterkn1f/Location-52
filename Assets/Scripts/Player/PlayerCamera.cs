using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Characters.Player
{
    /// <summary>
    /// Player camera component
    /// </summary>
    public class PlayerCamera : MonoBehaviour
    {
        [SerializeField] private Camera _camera;

        // Tracking and movement settings
        [Header("Movement settings")]
        [Tooltip("The main player (for movement)")][SerializeField] private Transform _target;
        [Tooltip("The player decoration")][SerializeField] private GameObject _character;
        [Tooltip("The actual camera object")][SerializeField] private GameObject _cameraObject;
        [Tooltip("Camera offset")][SerializeField] private Vector3 _offset;
        [Tooltip("Camera rotation offset")][SerializeField] private Vector3 _targetRotationOffset;

        [Header("Camera height settings")]
        [SerializeField] private float _crouchHeight = 1f;
        [SerializeField] private float _normalHeight = 2f;

        [Header("Mouse Look Settings")]
        [SerializeField] private bool _enableMouseLook = true;
        [SerializeField] private Vector2 _sensitivity = new Vector2(20f, 20f);
        [SerializeField] private float _sensMultiplier = 0.2f;
        [SerializeField] private float _maxAngle = 90f;

        [Header("Aim Assist")]
        [SerializeField] private bool _enableAimAssist = true;
        [SerializeField] private float _mouseSlowdown = 0.5f;
        private float _aimAssistSensMultiplier = 1.0f;

        [Header("Interactable Object Settings")]
        [SerializeField] private float _cameraReach = 50f;
        [SerializeField] private LayerMask _cameraLayerMask;

        [Header("Camera effects")]
        [SerializeField] private float _step = 0.01f;
        [SerializeField] private float _maxStepDistance = 0.06f;
        private Vector3 _swayPos;

        [SerializeField] private float _rotationStep = 4f;
        [SerializeField] private float _maxRotationStep = 5f;
        private Vector3 _swayEulerRot;

        [SerializeField] private float _smooth = 10f;
        [SerializeField] private float _smoothRot = 12f;

        [SerializeField] private float _speedCurve;
        private float _curveSin { get => Mathf.Sin(_speedCurve); }
        private float _curveCos { get => Mathf.Cos(_speedCurve); }

        [SerializeField] private Vector3 _travelLimit = Vector3.one * 0.025f;
        [SerializeField] private Vector3 _bobLimit = Vector3.one * 0.01f;

        [SerializeField] private Vector3 _multiplier;
        private Vector3 _bobPosition;
        private Vector3 _bobEulerRotation;

        private float xRotation = 0f;
        private Controls _controls = null;
        private Vector2 mouseInput = Vector2.zero;

        private PlayerMovement _movement;

        private float _fov;
        private float _idealHeight;

        private bool _isADS;

        // Start is called before the first frame update
        void Start()
        {
            _controls = new Controls();

            // Lock the camera
            Cursor.lockState = CursorLockMode.Locked;

            _idealHeight = _normalHeight;
            _fov = _camera.fieldOfView;

            _movement = PlayerManager.Instance.Movement;
            _isADS = false;

            GetUp();
            AssignControls();

        }

        private void FixedUpdate()
        {

        }

        private void OnDisable()
        {
            _controls?.Disable();
        }


        private void AssignControls()
        {
            if (!_controls.MainGameplay.enabled)
            {
                _controls.MainGameplay.Enable();
            }

            _controls.MainGameplay.Camera.performed += ctx => mouseInput = GetMouseInput();
            _controls.MainGameplay.Camera.canceled += ctx => mouseInput = Vector2.zero;

            //_controls.MainGameplay.Interact.performed += ctx => InteractObject();
        }

        /// <summary>
        /// Get the current mouse input
        /// </summary>
        public Vector2 GetMouseInput()
        {
            return _controls.MainGameplay.Camera.ReadValue<Vector2>() * _sensitivity * Time.deltaTime * _sensMultiplier * _aimAssistSensMultiplier;
        }

        private void Update()
        {
            UpdateMouseLook();
            //UpdateHoverInteractable();

            // Smooth movement of camera
            if (Mathf.Abs(_offset.y - _idealHeight) > 0)
            {
                _offset.y = Mathf.MoveTowards(_offset.y, _idealHeight, 0.25f);
            }
        }

        /// <summary>
        /// Update camera and player rotation based on mouse input
        /// </summary>
        private void UpdateMouseLook()
        {
            if (!_enableMouseLook) return;

            var rot = transform.localRotation.eulerAngles;
            float xTo = rot.y + mouseInput.x;

            xRotation -= mouseInput.y;
            xRotation = Mathf.Clamp(xRotation, -_maxAngle, _maxAngle);

            //ransform.localRotation = Quaternion.Euler(xRotation, xTo, 0f);
            transform.rotation = Quaternion.Euler(xRotation, xTo, 0f);

            _target.localRotation = Quaternion.Euler(0f, xTo, 0f);

            // Check if gun is equipped, and then sway
            CameraSway();
        }

        ///// <summary>
        ///// Check if the player is looking at anything that they can interact with (for popups and stuff)
        ///// Call out the hover function
        ///// </summary>
        //private void UpdateHoverInteractable()
        //{
        //    if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, _cameraReach, _cameraLayerMask))
        //    {
        //        if (hit.collider.TryGetComponent(out IInteractable interactable))
        //        {
        //            // Ignore if not interactable
        //            if (interactable.IsInteractable == false) { return; }
        //            interactable.Hover(transform.position, hit.point);

        //        }
        //    }
        //}

        ///// <summary>
        ///// If the player pressed the interact button
        ///// Check if we are interacting with anything
        ///// </summary>
        //private void InteractObject()
        //{
        //    if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, _cameraReach, _cameraLayerMask))
        //    {
        //        if (hit.collider.TryGetComponent(out IInteractable interactable))
        //        {
        //            // Ignore if not interactable
        //            interactable.Interact(transform.position, hit.point);
        //        }
        //    }
        //}

        // Height stuff
        // TODO: make this lerp
        public void Crouch()
        {
            _idealHeight = _crouchHeight;
        }

        public void GetUp()
        {
            _idealHeight = _normalHeight;
        }

        private void LateUpdate()
        {
            transform.position = _target.position + _offset;

            ApplyCameraEffects();
        }

        public void ShakeCameraImpulse(float magnitude, float duration)
        {
            _cameraObject.transform.DOShakePosition(duration, (_isADS) ? (magnitude * 0.65f) : (magnitude));
        }

        public void CameraSway()
        {
            // Movement
            Vector3 invertLook = mouseInput * -_step;
            invertLook.x = Mathf.Clamp(invertLook.x, -_maxStepDistance, _maxStepDistance);
            invertLook.y = Mathf.Clamp(invertLook.y, -_maxStepDistance, _maxStepDistance);

            _swayPos = invertLook * ((_isADS) ? (0.1f) : (1f));

            // Rotation
            Vector2 invertLookRot = mouseInput * -_rotationStep;
            invertLookRot.x = Mathf.Clamp(invertLookRot.x, -_maxRotationStep, _maxRotationStep);
            invertLookRot.y = Mathf.Clamp(invertLookRot.y, -_maxRotationStep, _maxRotationStep);

            _swayEulerRot = new Vector3(invertLookRot.y, invertLookRot.x, invertLookRot.x) * ((_isADS) ? (0.1f) : (1f));
        }

        public void CameraBob(Vector2 movementIndex, float magnitude)
        {
            _multiplier = Vector3.one * magnitude;
            Vector2 walkInput = movementIndex.normalized;
            _speedCurve += Time.deltaTime * (_movement.Grounded ? (movementIndex.x + movementIndex.y) * magnitude : 1f) + 0.01f;

            _bobPosition.x = (_curveCos * _bobLimit.x * (_movement.Grounded ? 1 : 0)) - (walkInput.x * _travelLimit.x);
            _bobPosition.y = (_curveSin * _bobLimit.y) - (movementIndex.y * _travelLimit.y);
            _bobPosition.z = -(walkInput.y * _travelLimit.z);

            // Rotation
            _bobEulerRotation.x = (walkInput != Vector2.zero ? _multiplier.x * (Mathf.Sin(5 * _speedCurve)) : _multiplier.x * (Mathf.Sin(5 * _speedCurve) / 2));
            _bobEulerRotation.y = (walkInput != Vector2.zero ? _multiplier.y * _curveCos : 0);
            _bobEulerRotation.z = (walkInput != Vector2.zero ? _multiplier.z * _curveCos * movementIndex.normalized.x : 0);

            Debug.Log(_curveCos);
        }

        public void ApplyCameraEffects()
        {
            _cameraObject.transform.localPosition = Vector3.Lerp(_cameraObject.transform.localPosition, _bobPosition, Time.deltaTime * _smooth);
            _cameraObject.transform.localRotation = Quaternion.Slerp(_cameraObject.transform.localRotation, (Quaternion.Euler(_bobEulerRotation)), Time.deltaTime * _smoothRot);

            PlayerManager.Instance.Arms.SetWeaponSway(_swayPos + _bobPosition, _swayEulerRot + _bobEulerRotation);
        }
    }
}
