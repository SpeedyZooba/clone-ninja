using UnityEngine;

public class Blade : MonoBehaviour
{
    #region Components
    private StateManager _stateManager;
    [SerializeField] private BoxCollider _blade;
    [SerializeField] private TrailRenderer _trail;
    #endregion
    #region Status
    [SerializeField] private float _swipeTimer = 2f;
    [SerializeField] private float _cooldownTimer = 0f;
    private bool _isSwiping;
    #endregion
    #region Bounds
    private Camera _cam;
    private float _camAxisZ;
    private Vector3 _bladePos;
    #endregion

    // Initialize variables related to the state of the blade object at the start of the game
    void Awake()
    {
        _cam = Camera.main;
        if (GetComponent<BoxCollider>() != null || GetComponent<TrailRenderer>() != null)
        {
            _blade = GetComponent<BoxCollider>();
            _trail = GetComponent<TrailRenderer>();
            _blade.enabled = false;
            _trail.enabled = false;
        }
        else
        {
            Debug.LogError($"Missing Components found for {gameObject.name}.");
        }
    }

    // Cache the Z-Axis of the camera for position updates
    void Start()
    {
        _isSwiping = false;
        _camAxisZ = _cam.transform.position.z;
    }

    // Handle input in Update()
    void Update()
    {
        if (_stateManager.IsActive && !_stateManager.IsPaused)
        {
            if (_cooldownTimer > 0f)
            {
                _cooldownTimer -= Time.deltaTime;
                return;
            }
            if (Input.GetMouseButtonDown(0))
            {
                _isSwiping = true;
                _swipeTimer = 0f;
                SetComponentStatus();
            }
            else if (Input.GetMouseButtonUp(0))
            {
                _isSwiping = false;
                SetComponentStatus();
            }

            if (_isSwiping)
            {
                _swipeTimer += Time.deltaTime;
                if (_swipeTimer >= 3f)
                {
                    _isSwiping = false;
                    _cooldownTimer = 2f;
                    SetComponentStatus();
                }
                else 
                {
                    SetBladePosition();
                }
            }
        }
    }
    public void SetManager(StateManager manager)
    {
        _stateManager = manager;
    }

    // The blade is supposed to have a collider with a trail as the mouse is dragged across the screen
    private void SetBladePosition()
    {
        Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, -_camAxisZ);
        // ScreenToWorldPoint converts a screen point, which is a point defined in your screen, to a world point that is defined in the coordinate system
        // which is shown in the Editor (basically a 2D -> 3D conversion you can use)
        _bladePos = _cam.ScreenToWorldPoint(mousePos);
        // Set the position of the blade to the converted position of the mouse
        transform.position = _bladePos;
    }

    private void SetComponentStatus()
    {
        _blade.enabled = _isSwiping;
        _trail.enabled = _isSwiping;
    }
}
