using DG.Tweening.Core.Easing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Characters.Player
{
    /// <summary>
    /// This is the main class that handles the player's movement
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerMovement : MonoBehaviour
    {
        #region Movement Variables

        [Header("Movement Settings")]
        public bool CanMove = true;
        [SerializeField] private float _moveSpeed = 250f;
        [SerializeField] private float _moveMultiplier = 9f;
        [SerializeField] private float _groundMaxSpeed = 20f;
        [SerializeField] private float _friction = 230f;
        [SerializeField] private float _gravity = 10f;
        [SerializeField, Range(0, 90)] private float _maxSlope = 15f;

        [Space(15)]
        [SerializeField] private bool _enableInAirDrag = false;
        [SerializeField] private float _inAirMaxSpeed = 30f;
        [SerializeField, Range(0, 2)] private float _inAirMovementModifier = 0.8f;
        [SerializeField] private float _inAirDrag = 160f;

        [Header("Sprint Settings")]
        [SerializeField] private bool _enableSprint = true;
        [SerializeField] private float _sprintMultiplier = 12f;
        [Tooltip("Adds to the original max speed.")][SerializeField] private float _sprintMaxSpeedModifier = 5f;

        [Header("Jump Settings")]
        public bool EnableJump = true;
        [SerializeField] private float _jumpForce = 7f;
        [SerializeField] private float _jumpMultiplier = 1.5f;

        [Header("Ground Check Settings")]
        [SerializeField] private float _slopeRaycastDistance = 1f;
        [SerializeField] private LayerMask _groundLayer;

        [Header("Crouch Settings")]
        [SerializeField] private bool _enableCrouch = true;
        [SerializeField] private float _crouchMoveMultiplier = 0.5f;
        [SerializeField] private float _crouchMaxSpeed = 10f;
        [SerializeField] private float _crouchJumpMultiplier = 1.4f;
        [SerializeField] private GameObject _standingCollider;
        [SerializeField] private GameObject _crouchingCollider;

        [Header("Player visibility")]
        public bool IsHidden = false;

        // Private variables
        private Rigidbody _rb = null;
        private Controls _controls = null;

        private Vector2 _moveInput = Vector2.zero;
        private float _currentSlope = 0f;
        private bool _jumping, _crouching, _sprinting;

        private ContactPoint _groundCollider;

        // Public getter variables
        public float CurSpeed
        {
            get
            {
                // If sprint is disabled, dc
                if (!_enableSprint)
                {
                    return _moveSpeed * _moveMultiplier;
                }

                return _moveSpeed * (_sprinting ? _sprintMultiplier : _moveMultiplier);
            }
        }

        public float MaxSpeed
        {
            get
            {
                if (!_enableSprint)
                {
                    if (!Grounded || _jumping)
                    {
                        return _inAirMaxSpeed;
                    }
                    else if (_crouching && Grounded)
                    {
                        return _crouchMaxSpeed;
                    }
                    else
                    {
                        return _groundMaxSpeed;
                    }
                }

                if (!Grounded || _jumping)
                {
                    return _inAirMaxSpeed;
                }
                else if (!_crouching && Grounded && _sprinting)
                {
                    return _groundMaxSpeed + _sprintMaxSpeedModifier;
                }
                else if (_crouching && Grounded)
                {
                    return _crouchMaxSpeed;
                }
                else
                {
                    return _groundMaxSpeed;
                }
            }
        }

        public bool Grounded { get; private set; }

        #endregion

        // Start is called before the first frame update
        void Awake()
        {
            // Get necessary components
            _rb = GetComponent<Rigidbody>();
            _controls = new Controls();

            AssignControls();
        }

        private void Start()
        {
            _standingCollider.SetActive(true);
            _crouchingCollider.SetActive(false);
        }

        private void OnDisable()
        {
            _controls.Disable();
        }

        /// <summary>
        /// Assign movement controls to player
        /// </summary>
        private void AssignControls()
        {
            if (!_controls.MainGameplay.enabled)
            {
                _controls.MainGameplay.Enable();
            }

            // fun with delegates!
            _controls.MainGameplay.Movement.performed += ctx => _moveInput = ctx.ReadValue<Vector2>();
            _controls.MainGameplay.Movement.canceled += ctx => _moveInput = Vector2.zero;

            // jump
            _controls.MainGameplay.Jump.performed += ctx =>
            {
                if (EnableJump)
                {
                    _jumping = true;
                    OnJump();
                }
            };

            _controls.MainGameplay.Jump.performed += ctx => _jumping = false;

            _controls.MainGameplay.Crouch.performed += ctx => ToggleCrouch(true);
            _controls.MainGameplay.Crouch.canceled += ctx => ToggleCrouch(false);

            _controls.MainGameplay.Sprint.performed += ctx => SetSprint(true);
            _controls.MainGameplay.Sprint.canceled += ctx => SetSprint(false);
        }

        /// <summary>
        /// Set the current player is sprinting or not
        /// </summary>
        private void SetSprint(bool isSprinting)
        {
            if (!isSprinting)
            {
                _sprinting = false;
            }
            if (PlayerManager.Instance.Health.GetEnergy() > 0.05)
            {
                _sprinting = true;
            }
            
        }

        // Update is called once per frame
        void Update()
        {
            PlayerManager.Instance.Camera.CameraBob(_moveInput, _sprinting ? (2.0f) : (_crouching ? (0.5f) : (1.0f)));
        }

        private void FixedUpdate() => UpdateMovement();

        private void UpdateMovement()
        {
            if (!CanMove) return;

            Vector3 dir = transform.right * _moveInput.x + transform.forward * _moveInput.y;
            // Gravity
            _rb.AddForce(Vector3.down * Time.deltaTime * _gravity);

            if (_crouching && Grounded && _currentSlope >= _maxSlope)
            {
                // Force Gravity ?
                _rb.AddForce(Vector3.down * Time.deltaTime * _gravity * 500f);
                return;
            }

            // Calculate multiplier 
            float multiplier = Grounded && _crouching ? _crouchMoveMultiplier : 1f;

            if (_rb.velocity.magnitude > _crouchMaxSpeed)
            {
                dir = Vector3.zero;
            }
            _rb.AddForce(GetMovementVector(-_rb.velocity, dir.normalized, CurSpeed * Time.deltaTime) * ((Grounded && !_jumping) ? multiplier : _inAirMovementModifier) * _rb.mass);

            if (_sprinting)
            {
                PlayerManager.Instance.Health.UseEnergy(0.25f);

                if (PlayerManager.Instance.Health.GetEnergy() < 0.05)
                {
                    SetSprint(false);
                }
            }
        }

        private Vector3 GetMovementVector(Vector3 velocity, Vector3 dir, float speed)
        {
            if (!Grounded && velocity.magnitude != 0 && _enableInAirDrag || velocity.magnitude != 0 && _enableInAirDrag && _jumping)
            {
                float drop = _inAirDrag * Time.deltaTime;
                velocity *= drop != 0f ? drop : 1f;

                return new Vector3(velocity.x, 0f, velocity.z) + dir * speed;
            }

            if (Grounded && velocity.magnitude != 0f && _crouching && _currentSlope >= _maxSlope)
            {
                return velocity + dir * speed;
            }

            if (Grounded && velocity.magnitude != 0f) velocity *= _friction * Time.deltaTime;

            return velocity + dir * speed;
        }

        private void OnJump()
        {
            if (!EnableJump) return;

            if (Grounded)
            {
                //If crouching and not sliding: crouch jump multiplier, if sliding: slide jump multiplier, and if all else is false: normal jump multiplier.
                float slideJumpMultiplier = _rb.velocity.y < 0 ? _rb.velocity.magnitude * 0.1f + _jumpMultiplier : _crouchJumpMultiplier; //scales to speed
                float currentMultiplier = _crouching && _currentSlope < _maxSlope ? _crouchJumpMultiplier : _crouching && _currentSlope >= _maxSlope ? slideJumpMultiplier : _jumpMultiplier;
                _rb.AddForce(Vector2.up * _jumpForce * currentMultiplier * _rb.mass, ForceMode.Impulse);
                Grounded = false;
            }
        }

        private void ToggleCrouch(bool crouched)
        {
            if (!_enableCrouch) return;

            if (crouched)
            {
                _crouching = true;
                PlayerManager.Instance.Camera.Crouch();
                _crouchingCollider.SetActive(true);
                _standingCollider.SetActive(false);
            }
            else
            {
                _crouching = false;
                PlayerManager.Instance.Camera.GetUp();

                _crouchingCollider.SetActive(false);
                _standingCollider.SetActive(true);
            }
        }

        private void OnCollisionStay(Collision other)
        {
            if (((1 << other.gameObject.layer) & _groundLayer) != 0)
            {
                for (int i = 0; i < other.contactCount; i++)
                {
                    if (Mathf.Round(other.GetContact(i).normal.y) == 1.0f)
                    {
                        Grounded = true;
                        _groundCollider = other.GetContact(i);
                        break;
                    }
                }

                // Check for floor
                Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, _slopeRaycastDistance, _groundLayer);
                _currentSlope = Vector3.Angle(Vector3.up, hit.normal);
            }
        }

        private void OnCollisionExit(Collision other) => Grounded = false;

        public void HidePlayer(bool isHidden)
        {
            IsHidden = isHidden;
        }

        /// <summary>
        /// For teleporting the player to where it's supposed to be
        /// </summary>
        public void MovePlayer(Vector3 newPosition)
        {
            transform.position = newPosition;
        }

        public void MovePlayer(Vector3 newPosition, Quaternion newRotation)
        {
            transform.position = newPosition;
            PlayerManager.Instance.Camera.transform.rotation = newRotation;
        }
    }
}
