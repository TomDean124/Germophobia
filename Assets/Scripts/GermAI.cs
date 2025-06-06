using UnityEngine;
using System.Collections.Generic;

public class GermAI : MonoBehaviour
{
    private BTNode behaviourTree;
    private Spawner _spawner;
    public GameObject Player;
    private GermMovementManager _movementMan;
    private Health _health;
    private float _searchTimer;
    private GameObject _targetStaticEnemy;
    public float moveSpeed;
    public float germDetectionRange;
    public float searchDelay;
    public float wanderStrength;
    public float recallSpeedMultiplier;
    public float attackRange;
    [Tooltip("Determines the health in which the player gets the option to execute or turn the object into a germ duplicator")]
    public float executionOptionHealth;

    [Tooltip("Gives the player option to infect to create more germs")]
    public float deathBuffer; 
    public bool visuals;
    private bool b_recall;
    private bool b_recallComplete;
    public bool canExecute;
    
    public float recallStopDistance;
    private enum _states { Searching, Attacking, Chasing, Recalling };
    private _states _currentState;
    private _states _previousState;

    // Debug variables
    public bool debugMode = false;
    private float _targetValidationTimer = 0f;
    private float _targetValidationInterval = 1f; // Check target validity every second

    void Start()
    {
        // Cache references to avoid FindObjectOfType calls later
        if (_spawner == null) _spawner = FindObjectOfType<Spawner>();
        if (_movementMan == null) _movementMan = FindObjectOfType<GermMovementManager>();

        // Initialize search timer to start searching immediately
        _searchTimer = 0f;
        
        // Restructured behavior tree for better priority handling
        behaviourTree = new SelectorNode(new List<BTNode>
        {
            // 1. Recall has highest priority when active
            new SequenceNode(new List<BTNode>
            {
                new ConditionNode(() => Player != null && b_recall && !b_recallComplete),
                new ActionNode(() => Recall())
            }),

            // 2. Attack when in range
            new SequenceNode(new List<BTNode>
            {
                new ConditionNode(() => IsTargetValid() && isInAttackRange()),
                new ActionNode(() => Attack())
            }),

            // 3. Chase existing target
            new SequenceNode(new List<BTNode>{
                new ConditionNode(() => IsTargetValid() && Vector2.Distance(transform.position, _targetStaticEnemy.transform.position) < germDetectionRange),
                new ActionNode(() => ChaseStaticEnemy())
            }),

            // 4. Search for new targets
            new SequenceNode(new List<BTNode>
            {
                new ConditionNode(() => SearchForStaticEnemy()),
                new ActionNode(() => ChaseStaticEnemy())
            }),

            new ActionNode(() => WanderWhileSearching())
        });

        if (debugMode) Debug.Log($"[GermAI {gameObject.name}] Initialized with detection range: {germDetectionRange}, attack range: {attackRange}");
    }

    void Update()
    {
        if(_targetStaticEnemy != null)
        {
            _movementMan.DisableFlocking();
        }
        if (InputManager.RecallKeyPress())
        {
            b_recall = true;
            b_recallComplete = false;
            if (debugMode) Debug.Log($"[GermAI {gameObject.name}] Recall Key Pressed!");
        }
        else if (b_recallComplete)
        {
            b_recall = false;
            if (debugMode) Debug.Log($"[GermAI {gameObject.name}] Recall Complete!");
        }

        behaviourTree.Execute();
        
        Visuals();
    }


    bool IsTargetValid()
    {
        return _targetStaticEnemy != null && _targetStaticEnemy.activeInHierarchy;
    }

    void ChaseStaticEnemy()
    {
        if (!IsTargetValid()) return;
        
        _previousState = _currentState;
        _currentState = _states.Chasing;
        
        transform.position = Vector2.MoveTowards(transform.position, _targetStaticEnemy.transform.position, moveSpeed * Time.deltaTime);
        
        if (debugMode && _previousState != _states.Chasing)
        { 
            Debug.Log($"[GermAI {gameObject.name}] Chasing target {_targetStaticEnemy.name} at distance {Vector2.Distance(transform.position, _targetStaticEnemy.transform.position):F2}");
        }
    }

    bool isInAttackRange()
    {
        if (!IsTargetValid()) return false;

        float distance = Vector2.Distance(transform.position, _targetStaticEnemy.transform.position);
        bool inRange = distance < attackRange;
        
        if (debugMode && inRange) 
        {
            Debug.Log($"[GermAI {gameObject.name}] Target {_targetStaticEnemy.name} is in attack range ({distance:F2} < {attackRange})");
        }
        
        return inRange;
    }

    bool SearchForStaticEnemy()
    {
        _previousState = _currentState;
        _currentState = _states.Searching;
        
        if (_spawner == null)
        {
            _spawner = FindObjectOfType<Spawner>();
            if (_spawner == null)
            {
                if (debugMode) Debug.LogWarning($"[GermAI {gameObject.name}] Cannot find Spawner!");
                return false;
            }
        }
        
        if (_spawner._AstaticenemyOB == null || _spawner._AstaticenemyOB.Count == 0)
        {
            if (debugMode) Debug.Log($"[GermAI {gameObject.name}] No static enemies in spawner list");
            return false;
        }

        if (!IsTargetValid())
        {
            _searchTimer -= Time.deltaTime;
        }

        if (_searchTimer <= 0)
        {
            if (debugMode) Debug.Log($"[GermAI {gameObject.name}] Searching for targets among {_spawner._AstaticenemyOB.Count} static enemies");
            
            GameObject nearestGerm = null;
            float closestDist = germDetectionRange;
            int validTargetsInRange = 0;

            foreach (GameObject staticGerm in _spawner._AstaticenemyOB)
            {
                if (staticGerm != null && staticGerm.activeInHierarchy)
                {
                    float distance = Vector2.Distance(transform.position, staticGerm.transform.position);
                    
                    if (distance <= germDetectionRange)
                    {
                        validTargetsInRange++;
                        if (debugMode) Debug.Log($"[GermAI {gameObject.name}] Found target {staticGerm.name} at distance {distance:F2}");
                        
                        if (distance < closestDist)
                        {
                            closestDist = distance;
                            nearestGerm = staticGerm;
                        }
                    }
                }
            }

            if (debugMode) Debug.Log($"[GermAI {gameObject.name}] Found {validTargetsInRange} valid targets in range");
            
            if (nearestGerm != null && (_targetStaticEnemy == null || nearestGerm != _targetStaticEnemy))
            {
                _targetStaticEnemy = nearestGerm;
                if (debugMode) Debug.Log($"[GermAI {gameObject.name}] New target acquired: {_targetStaticEnemy.name} at distance {closestDist:F2}");
            }
            
            _searchTimer = searchDelay;

            return IsTargetValid();
        }

        return IsTargetValid();
    }

    void WanderWhileSearching()
    {
        _previousState = _currentState;
        _currentState = _states.Searching;
        
        Vector2 wanderDirection = ((Vector2)transform.position + Random.insideUnitCircle * wanderStrength) - (Vector2)transform.position;
        transform.position += (Vector3)(wanderDirection.normalized * moveSpeed * Time.deltaTime * 0.5f);
    }

    void Attack()
    {
        if (!IsTargetValid()) return;
        
        _previousState = _currentState;
        _currentState = _states.Attacking;
        
        if (debugMode) Debug.Log($"[GermAI {gameObject.name}] Attacking {_targetStaticEnemy.name}!");
        
        AttackHandler handler = this.gameObject.GetComponent<AttackHandler>();
        Health _health = _targetStaticEnemy.GetComponent<Health>(); 
        if (handler != null) 
        {
            handler.TryUseAttack(_targetStaticEnemy, "Enemy");
        }
        else
        { 
            if (debugMode) Debug.LogWarning($"[GermAI {gameObject.name}] No AttackHandler component found!");
        }
    }


    void Recall()
    {
        if (Player == null)
        {
            if (debugMode) Debug.LogWarning($"[GermAI {gameObject.name}] Cannot recall: Player is null!");
            b_recallComplete = true;
            return;
        }
        
        float dist = Vector2.Distance(transform.position, Player.transform.position);

        _previousState = _currentState;
        _currentState = _states.Recalling;

        if (dist > recallStopDistance)
        {
            if (debugMode && _previousState != _states.Recalling)
            {
                Debug.Log($"[GermAI {gameObject.name}] Recalling to player at distance {dist:F2}");
            }

            Vector3 newPosition = Vector2.MoveTowards(transform.position, Player.transform.position, (moveSpeed * recallSpeedMultiplier) * Time.deltaTime);
            transform.position = newPosition;
        }
        else
        {
            if (debugMode) Debug.Log($"[GermAI {gameObject.name}] Recall complete at distance {dist:F2}");
            b_recallComplete = true;
        }
    }

    void Visuals()
    {

        if (_currentState == _states.Searching)
        {
            GetComponent<SpriteRenderer>().color = Color.white;
        }
        else if (_currentState == _states.Chasing)
        {
            if (visuals)
            {
                GetComponent<SpriteRenderer>().color = Color.red;
            }
            if (_movementMan != null) _movementMan.enabled = false;
        }
        else if (_currentState == _states.Recalling)
        {
            if (visuals)
            {
                GetComponent<SpriteRenderer>().color = Color.blue;
            }
        }
        else if (_currentState == _states.Attacking)
        {
            if (visuals)
            {
                GetComponent<SpriteRenderer>().color = Color.yellow;
            }
        }
    }
    
    void OnDrawGizmosSelected()
{
    if (!visuals) return;

    // Detection, Attack, and Recall range spheres
    Gizmos.color = Color.yellow;
    Gizmos.DrawWireSphere(transform.position, germDetectionRange);

    Gizmos.color = Color.red;
    Gizmos.DrawWireSphere(transform.position, attackRange);

    Gizmos.color = Color.blue;
    Gizmos.DrawWireSphere(transform.position, recallStopDistance);

    // Targeting visuals
    if (_targetStaticEnemy != null && _targetStaticEnemy.activeInHierarchy)
    {
        // Line to the target
        Gizmos.color = _currentState == _states.Attacking ? Color.red : Color.green;
        Gizmos.DrawLine(transform.position, _targetStaticEnemy.transform.position);

        // Sphere to indicate target position
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(_targetStaticEnemy.transform.position + Vector3.up * 0.5f, 0.15f);

        // Label for clarity in scene view
#if UNITY_EDITOR
        UnityEditor.Handles.color = Color.white;
        UnityEditor.Handles.Label(_targetStaticEnemy.transform.position + Vector3.up * 0.75f,
            $"Target: {_targetStaticEnemy.name}");
#endif
    }
}

}