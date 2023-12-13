using UnityEngine;

namespace Player
{
    public class GunController : MonoBehaviour
    {
        [SerializeField] private LayerMask _raycastLayerMask;
        [SerializeField] private Gun _mainGun;
        private PlayerMovement _playerMovement;

        private void Awake()
        {
            _playerMovement = GetComponent<PlayerMovement>();
        }

        private void Update()
        {
            if (InputManager.Singleton.IsShootButtonPressed)
            {
                _mainGun.Shoot();
            }

            if (InputManager.Singleton.IsReloadButtonPressed)
            {
                _mainGun.Reload();
            }

            if (InputManager.Singleton.IsZoomButtonPressed)
            {
                _mainGun.Zoom();
            }
        }
    }
}
