using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public Vector2 textOffset;
    public GameObject _player;
    [SerializeField] private float displayRadius;
    [SerializeField] private LayerMask interactableLayer;

    private Spawner _spawner;
    public ReplicationManager _replicationManager;
    private IInteractable _interactible;

    private void Start()
    {
        _spawner = GetComponent<Spawner>();
    }

    private void Update()
    {
        Collider2D circleRadius = Physics2D.OverlapCircle(_player.transform.position, displayRadius, interactableLayer);

        if(circleRadius != null)
        {
            DetermineChoice(circleRadius);
        }
    }

    private void DetermineChoice(Collider2D circleRadius)
{
    if (circleRadius == null)
    {
        Debug.Log("No collider detected in radius.");
        return;
    }

    _interactible = circleRadius.GetComponent<IInteractable>();
    if (_interactible == null)
    {
        Debug.LogWarning("No IInteractable found on object: " + circleRadius.name);
        return;
    }

    var mono = (MonoBehaviour)_interactible;
    if (mono == null)
    {
        Debug.LogError("Interactible is not a MonoBehaviour.");
        return;
    }

    Health _health = mono.GetComponent<Health>();

    if (_health == null)
    {
        Debug.LogWarning("No Health component found on object: " + mono.gameObject.name);
        return;
    }

    if (_replicationManager == null)
    {
        Debug.LogError("ReplicationManager is not assigned in PlayerInteraction.");
        return;
    }

    if (_replicationManager.Replication == null || _replicationManager.Replication.Length == 0)
    {
        Debug.LogError("Replication data array is null or empty.");
        return;
    }

    Debug.Log("Interactible detected: " + mono.gameObject.name);

    if (_health != null && Input.GetKeyDown(InputManager.ExecuteKey) &&
        _health.currentHealth <= _replicationManager.Replication[0].healthForReplicationOption)
    {
        Debug.Log("ExecuteCalled! (PlayerInteraction.cs)");
        _interactible.OnExecute();
        //Change this, just a test to check for working. 
        GameManager.IncreaseHungerOfAllInArray(_spawner._AgermOB, 500f);
    }

    if (_spawner != null &&
        Input.GetKeyDown(InputManager.ConvertKey) &&
        _health.currentHealth <= _replicationManager.Replication[0].healthForReplicationOption)
    {
        Debug.Log("ConvertCalled! (PlayerInteraction.cs)");
        _interactible.OnConvert(_spawner);
    }
}

    void OnDrawGizmosSelected()
    {
        if (_player != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(_player.transform.position, displayRadius);
        }

        if (_interactible is MonoBehaviour interactibleMB && interactibleMB != null)
        {
            GameObject go = interactibleMB.gameObject;
            if(go != null)
            {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(_player.transform.position, go.transform.position);
            Gizmos.DrawSphere(go.transform.position + Vector3.up * 0.5f, 0.1f);
            }
        }
    }
}
