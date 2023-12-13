using UnityEngine;
using Mirror;
using System.Collections;

namespace Player
{
    public class PlayerMovement : NetworkBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float _speed = 150;
        public float SpeedBoost = 0;
        [SerializeField] private float _maxSpeed = 10;
        [SerializeField] private float _jumpForce = 3;
        [SerializeField] private float _friction = 10;
        [SerializeField] private float _slopeLimit = 50;

        [Header("Bunnyhop Settings")]
        [SerializeField] private float _airLimit = 1;
        [SerializeField] private float _airAcceleration = 200;
        [SerializeField, Range(0, 1)] private float _jumpStaminaPerSecond = 0.5f;

        [Header("Rotation Settings")]
        [SerializeField] private float _mouseSensitivity = 2;

        [Header("Camera Settings")]
        [SerializeField] private float _cameraRotationSpeed = 125;
        [SerializeField] private Vector3 _cameraPositionOffset = new Vector3(0, 0.75f, 0);

        private bool _isFocused;
        protected Vector2 _rotation;

        private Transform _camera;
        private Rigidbody _rb;

        protected Vector3 _moveDir;
        private int _collisionCount;
        public bool OnGround { get; protected set; }
        protected Vector3 _groundNormal { get; private set; }
        protected float _jumpStamina { get; private set; }


        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _camera = Camera.main.transform;
            _jumpStamina = 1;
        }

        private void Start()
        {
            // вызова ивента
            if (isLocalPlayer)
            {
                NetManager.Singleton.InvokeOnAddPlayer(this);
            }
        }

        protected void Update()
        {
            if (!isLocalPlayer) return;
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                _isFocused = !_isFocused;
                Cursor.lockState = _isFocused ? CursorLockMode.Locked : CursorLockMode.None;
            }
        }

        protected void FixedUpdate()
        {
            SetInputData();

            if (_rb.velocity.magnitude < 0.1 && OnGround && _moveDir == Vector3.zero)
                _rb.velocity = Vector3.zero;

            _rb.useGravity = !OnGround;

            if (_collisionCount == 0)
                OnGround = false;

            if (isLocalPlayer && _isFocused)
                MovementController();
        }

        protected void OnCollisionStay(Collision other)
        {
            foreach (ContactPoint contactPoint in other.contacts)
            {
                if (contactPoint.normal.normalized.y > Mathf.Sin(_slopeLimit * 0.017453292f + 1.5707964f))
                {
                    _groundNormal = contactPoint.normal.normalized;
                    OnGround = true;
                    return;
                }
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            _collisionCount += 1;
        }

        private void OnCollisionExit(Collision collision)
        {
            _collisionCount -= 1;
            if (_collisionCount < 0)
                _collisionCount = 0;
        }

        private void SetInputData()
        {
            _moveDir = transform.right * InputManager.Singleton.MoveDirection.x + transform.forward * InputManager.Singleton.MoveDirection.x;
            _moveDir = Vector3.Cross(Vector3.Cross(_groundNormal, _moveDir), _groundNormal);
            _moveDir.Normalize();

            _rotation = new Vector2(_rotation.x % 360, _rotation.y % 360);
            _rotation += new Vector2(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X")) * _mouseSensitivity * 2;
        }

        private void MovementController()
        {
            RotatePlayer();

            _jumpStamina += _jumpStaminaPerSecond * Time.fixedDeltaTime;
            if (_jumpStamina > 1)
                _jumpStamina = 1;

            if (OnGround)
            {
                if (Input.GetKey(KeyCode.Space))
                {
                    if (_jumpStamina > 0)
                    {
                        Jump();
                    }
                    OnGround = false;
                    return;
                }

                Friction();
                GroundAccelerate();
            }
            else
            {
                AirAccelerate();
            }
            OnGround = false;
        }

        private void RotatePlayer()
        {
            if (_isFocused)
            {
                _rotation.x = Mathf.Clamp(_rotation.x, -90, 90);
                transform.transform.rotation = Quaternion.Euler(0, _rotation.y, 0);
            }
            _camera.position = transform.position + _cameraPositionOffset;
            _camera.rotation = Quaternion.Lerp(_camera.transform.rotation, Quaternion.Euler(_rotation), _cameraRotationSpeed * Time.deltaTime);
        }

        protected void GroundAccelerate()
        {
            float speedLimit = _maxSpeed + SpeedBoost - Vector3.Dot(_rb.velocity, _moveDir);
            float velocityAdd = (_speed + SpeedBoost) * Time.fixedDeltaTime;

            if (speedLimit <= 0f) return;

            if (velocityAdd > speedLimit)
            {
                velocityAdd = speedLimit;
            }
            SpeedBoost = 0;
            _rb.velocity += velocityAdd * _moveDir;
        }

        protected void AirAccelerate()
        {
            var lhs = _rb.velocity;
            lhs.y = 0f;

            float speedLimit = _airLimit - Vector3.Dot(lhs, _moveDir);
            float num2 = _airAcceleration * Time.fixedDeltaTime;

            if (speedLimit <= 0f) return;

            if (num2 > speedLimit)
            {
                num2 = speedLimit;
            }
            _rb.velocity += num2 * _moveDir;
        }

        protected virtual void Jump()
        {
            if (_rb.velocity.y < 0f)
            {
                _rb.velocity = new Vector3(_rb.velocity.x, 0, _rb.velocity.z);
            }

            _rb.velocity += Vector3.up * _jumpForce * _jumpStamina;
            OnGround = false;
            _jumpStamina = 0;
        }

        protected void Friction()
        {
            var friction = Mathf.Clamp01(1 - _friction * Time.fixedDeltaTime);
            if (OnGround)
                _rb.velocity *= friction;
//            else
//                _rb.velocity = new Vector3(_rb.velocity.x * friction, _rb.velocity.y, _rb.velocity.z * friction);
        }
    }
}
