using System.Collections;
using TMPro;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [Header("General Settings")]
    [SerializeField] private float _damage;
    [SerializeField] private int _maxCartridgesCount;
    [SerializeField] private int _stockÑartridgesCount;
    [SerializeField] private float[] _zoomApproximations;
    [SerializeField, Range(0.05f, 1)] private float _shootCD;
    [SerializeField, Range(0.05f, 2.5f)] private float _reloadCD;
    [SerializeField, Range(0.05f, 2.5f)] private float _zoomCD;
    [SerializeField] private LayerMask _raycastLayerMask;

    
    [Space(10)] 
    [SerializeField] private TextMeshProUGUI _cartridgesText;
    [SerializeField] private GameObject _shootSound;
    [SerializeField] private GameObject _reloadSound;
    [SerializeField] private GameObject _emptySound;

    private MeshRenderer _myMeshRenderer;
    private Camera _camera;
    private Animator _animator;

    private int _cartridgesCount;
    private bool _isReloading;
    private bool _isSelected;
    private bool _canShoot;
    private bool _canZoom;


    private void Awake()
    {
        _myMeshRenderer = GetComponent<MeshRenderer>();
        _camera = Camera.main;
        _cartridgesText = GameObject.Find("Ñartridges Text").GetComponent<TextMeshProUGUI>();
        _animator = GetComponent<Animator>();

        _cartridgesCount = _maxCartridgesCount;

        _canShoot = true;
        _canZoom = true;
        SetCartridgesText();
    }

    private void Update()
    {
        _myMeshRenderer.enabled = _isSelected;
        if (!_isSelected) return;
    }

    public void Shoot()
    {
        if (!_canShoot || _isReloading) return;
        
        if (_cartridgesCount == 0)
        {
//            if (Input.GetKeyDown(KeyCode.Mouse0))
//                Destroy(Instantiate(_emptySound), 5);
            return;
        }

        _cartridgesCount--;
        SetCartridgesText();
        StartCoroutine(ShootCD());
//        Destroy(Instantiate(_shootSound), 5);
        _animator.SetTrigger("IsShooting");

        if (Physics.Raycast(_camera.transform.position, _camera.transform.forward, out var hit, float.PositiveInfinity, _raycastLayerMask, QueryTriggerInteraction.Collide))
        {

        }
    }

    public void Reload()
    {
        if (_isReloading || _cartridgesCount == _maxCartridgesCount || _stockÑartridgesCount == 0) return;
        
//        Destroy(Instantiate(_reloadSound), 5);
        _animator.SetTrigger("IsReloading");

        StartCoroutine(ReloadIE());
    }

    public void Zoom()
    {
        if (!_canZoom)
            return;

        StartCoroutine(ZoomCD());

        if (_camera.fieldOfView == 90)
        {
            _camera.fieldOfView = _zoomApproximations[0];
            return;
        }

        for (int i = 0; i < _zoomApproximations.Length; i++)
        {
            if (_zoomApproximations[i] == _camera.fieldOfView)
                _camera.fieldOfView = _zoomApproximations[i+1];
            else if (i == _zoomApproximations.Length)
                _camera.fieldOfView = 90;
        }
    }

    private IEnumerator ShootCD()
    {
        _canShoot = false;
        yield return new WaitForSeconds(_shootCD);
        _canShoot = true;
    }

    private IEnumerator ReloadIE()
    {
        _isReloading = true;
        _cartridgesText.text = "**/**";
        
        yield return new WaitForSeconds(_reloadCD);

        if (_stockÑartridgesCount < _maxCartridgesCount)
        {
            _cartridgesCount += _stockÑartridgesCount;
            _stockÑartridgesCount = 0;
        }
        else
        {
            var a = _maxCartridgesCount - _cartridgesCount;
            _cartridgesCount = _maxCartridgesCount;
            _stockÑartridgesCount -= a;
        }

        _isReloading = false;
        SetCartridgesText();
    }

    private IEnumerator ZoomCD()
    {
        _canZoom = false;
        yield return new WaitForSeconds(_shootCD);
        _canZoom = true;
    }

    private void SetCartridgesText()
    {
        if (_cartridgesCount >= 10)
            _cartridgesText.text = $"{_cartridgesCount}/{_stockÑartridgesCount}";
        else
            _cartridgesText.text = $" {_cartridgesCount}/{_stockÑartridgesCount}";
    }
}
