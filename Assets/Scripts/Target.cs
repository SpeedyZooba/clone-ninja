using UnityEngine;

public class Target : MonoBehaviour
{
    public int Points;
    #region Components
    public ParticleSystem HitFX;
    private StateManager _stateManager;
    private Rigidbody _targetBody;
    #endregion
    #region Bounds
    private const float _minSpeed = 14;
    private const float _maxSpeed = 16;
    private const float _maxTorque = 10;
    private const float _spawnBoundX = 4;
    private const float _spawnBoundY = -6;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        // Make sure to do null checks to account for missing components and whatnot, be defensive
        if (GetComponent<Rigidbody>() != null || GetComponent<ParticleSystem>() != null)
        {
            _targetBody = GetComponent<Rigidbody>();
            _targetBody.AddForce(GenerateForce(), ForceMode.Impulse);
            _targetBody.AddTorque(GenerateTorque(), GenerateTorque(), GenerateTorque(), ForceMode.Impulse);
            transform.position = GeneratePosition();
        }
        else
        {
            Debug.LogError($"Not suitable for Rigidbody access as none are attached for {gameObject.name}.");
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Blade"))
        {
            Destroy(gameObject);
            Instantiate(HitFX, transform.position, HitFX.transform.rotation);
            _stateManager.UpdateScore(Points);
        }
    }

    // The sensor below the screen is marked as a trigger, and is the only one in this project
    void OnTriggerEnter(Collider collider)
    {
        Destroy(gameObject);
        if (!gameObject.CompareTag("Kill"))
        {
            _stateManager.EndGame();
        }
    }

    private Vector3 GeneratePosition()
    {
        return new Vector3(Random.Range(-_spawnBoundX, _spawnBoundX), _spawnBoundY);
    }

    private Vector3 GenerateForce()
    {
        return Vector3.up * Random.Range(_minSpeed, _maxSpeed);
    }

    private float GenerateTorque()
    {
        return Random.Range(-_maxTorque, _maxTorque);
    }

    // Avoid using GameObject.Find as much as possible as it is costly and not robust.
    public void SetManager(StateManager manager) 
    {
        _stateManager = manager;
    }
}
