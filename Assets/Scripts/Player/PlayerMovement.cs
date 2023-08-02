using UnityEngine;
using Mirror;
using System.Collections;

namespace Player
{
    public class PlayerMovement : NetworkBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float _speed = 125;
        public float SpeedBoost = 0;
        [SerializeField] private float _maxSpeed = 1000;
        [SerializeField] private float _jumpForce = 4;
        [SerializeField] private float _friction = 10;
        [SerializeField] private float _slopeLimit = 45;

        [Header("Bunnyhop Settings")]
        [SerializeField] private float _airLimit = 1;
        [SerializeField] private float _airAcceleration = 100;

        [Header("Rotation Settings")]
        [SerializeField] private float _mouseSensitivity = 2;

        [Header("Camera Settings")]
        [SerializeField] private float _cameraRotationSpeed = 125;
        [SerializeField] private Vector3 _cameraPositionOffset;

        private bool _isFocused;
        protected Vector2 _rotation;

        private Transform _camera;
        private Rigidbody _rb;

        protected Vector3 _inputDir;
        private int _collisionCount;
        public bool OnGround { get; protected set; }
        protected bool _ableJump { get; private set; }
        protected Vector3 _groundNormal { get; private set; }


        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _camera = Camera.main.transform;
            _ableJump = true;
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

        private void LateUpdate()
        {
            if (!isLocalPlayer) return;

            if (_isFocused)
            {
                Rotate();
            }
            CameraControll();
        }

        protected void FixedUpdate()
        {
            if (_rb.velocity.magnitude < 0.1 && OnGround && _inputDir == Vector3.zero)
                _rb.velocity = Vector3.zero;

            _rb.useGravity = !OnGround;

            if (_collisionCount == 0)
                OnGround = false;

            if (!isLocalPlayer) return;

            _inputDir = transform.right * Input.GetAxisRaw("Horizontal") + transform.forward * Input.GetAxisRaw("Vertical");
            _inputDir.Normalize();

            if (OnGround)
            {
                if (Input.GetKey(KeyCode.Space))
                {
                    if (_ableJump)
                    {
                        Jump();
                    }
                    OnGround = false;
                    return;
                }

                Friction();
                if (_isFocused)
                {
                    // делаем нормальное передвижение по наклонённым объектам
                    _inputDir = Vector3.Cross(Vector3.Cross(_groundNormal, _inputDir), _groundNormal);
                    GroundAccelerate();
                }
            }
            else
            {
                if (_isFocused)
                {
                    AirAccelerate();
                }
            }
            OnGround = false;
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

        private void Rotate()
        {
            _rotation = new Vector2(_rotation.x % 360, _rotation.y % 360);
            _rotation += new Vector2(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X")) * _mouseSensitivity * 2;
            _rotation.x = Mathf.Clamp(_rotation.x, -90, 90);
            transform.transform.rotation = Quaternion.Euler(0, _rotation.y, 0);
        }

        private void CameraControll()
        {
            _camera.position = transform.position + _cameraPositionOffset;
            _camera.rotation = Quaternion.Lerp(_camera.transform.rotation, Quaternion.Euler(_rotation), _cameraRotationSpeed * Time.deltaTime);
        }

        protected void GroundAccelerate()
        {
            float speedLimit = _maxSpeed + SpeedBoost - Vector3.Dot(_rb.velocity, _inputDir);
            float velocityAdd = (_speed + SpeedBoost) * Time.fixedDeltaTime;

            if (speedLimit <= 0f) return;

            if (velocityAdd > speedLimit)
            {
                velocityAdd = speedLimit;
            }
            SpeedBoost = 0;
            _rb.velocity += velocityAdd * _inputDir;
        }

        protected void AirAccelerate()
        {
            var lhs = _rb.velocity;
            lhs.y = 0f;

            float speedLimit = _airLimit - Vector3.Dot(lhs, _inputDir);
            float num2 = _airAcceleration * Time.fixedDeltaTime;

            if (speedLimit <= 0f) return;

            if (num2 > speedLimit)
            {
                num2 = speedLimit;
            }
            _rb.velocity += num2 * _inputDir;
        }

        protected virtual void Jump()
        {
            if (_rb.velocity.y < 0f)
            {
                _rb.velocity = new Vector3(_rb.velocity.x, 0, _rb.velocity.z);
            }

            _rb.velocity += Vector3.up * _jumpForce;
            OnGround = false;
            StartCoroutine(JumpTimer());
        }

        protected void Friction()
        {
            var friction = Mathf.Clamp01(1 - _friction * Time.fixedDeltaTime);
            if (OnGround)
                _rb.velocity *= friction;
//            else
//                _rb.velocity = new Vector3(_rb.velocity.x * friction, _rb.velocity.y, _rb.velocity.z * friction);
        }

        private IEnumerator JumpTimer()
        {
            _ableJump = false;
            yield return new WaitForSeconds(0.5f);
            _ableJump = true;
        }
    }
}
