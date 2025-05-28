using UnityEngine;
using System.Collections.Generic;

public class EnemyAI : MonoBehaviour
{
    private BTNode behaviourTree;
    public GameObject Player;
    public float playerDetectionRange;
    public float germDetectionRange;
    public float fleeRange;
    public float patrolSpeed;
    public float searchDelay;
    public float wanderStrength;
    public float moveSpeed;
    public float germAttackRange;  

    public float playerAttackRange;

    private Spawner _spawner;
    private GameObject _targetGerm;
    private float _searchTimer;

    private Vector2 _patrolTarget;
    private float _patrolTimer;

    public bool Visuals;

    
    private enum _states {Attacking, Searching, Chasing};
    private _states _currentState;


    void Start()
    {
        _spawner = FindObjectOfType<Spawner>();
        _searchTimer = searchDelay;
        behaviourTree = new SelectorNode(new List<BTNode>
        {
        // 1. Try to attack germ if in range
        new SequenceNode(new List<BTNode>
        {
        new ConditionNode(() => _targetGerm != null && Vector2.Distance(transform.position, _targetGerm.transform.position) < germAttackRange),
        new ActionNode(() => Attack(_targetGerm))
        }),

        new SequenceNode(new List<BTNode>
        {
        new ConditionNode(() => Player != null && Vector2.Distance(transform.position, Player.transform.position) < playerAttackRange),
        new ActionNode(() => Attack(Player))
        }),

        // 2. Chase player if in detection range
        new SequenceNode(new List<BTNode>
        {
        new ConditionNode(() => Player != null && Vector2.Distance(transform.position, Player.transform.position) < playerDetectionRange),
        new ActionNode(() => ChaseTargetPlayer())
        }),

        // 3. Chase target germ if in detection range
        new SequenceNode(new List<BTNode>
        {
        new ConditionNode(() => _targetGerm != null && Vector2.Distance(transform.position, _targetGerm.transform.position) < germDetectionRange),
        new ActionNode(() => ChaseTargetGerm())
        }),

        // 4. Search and wander if needed
        new SequenceNode(new List<BTNode>
        {
        new ConditionNode(() => SearchForGerm()),
        new ActionNode(() => WanderWhileSearching())
        }),

        // 5. Patrol
        new SequenceNode(new List<BTNode>
        {
        new ConditionNode(() => true),
        new ActionNode(() => Patrol())
        })
    });

    }

    void Update()
    { 
        if(Visuals){
        Vector3 lineEndPoint = transform.position + transform.right * 5;
        Debug.DrawLine(transform.position, lineEndPoint, Color.red);
        if (_currentState == _states.Searching)
        {
            GetComponent<SpriteRenderer>().color = Color.yellow;
        }

        if (_currentState == _states.Chasing)
        {
            GetComponent<SpriteRenderer>().color = Color.black;
        }

        if (_currentState == _states.Attacking)
        {
            GetComponent<SpriteRenderer>().color = Color.grey;
        }

        }

        behaviourTree.Execute();
        if (_targetGerm != null && !_targetGerm.activeInHierarchy) _targetGerm = null;      
    }

    void ChaseTargetPlayer()
    {
        _currentState = _states.Chasing;
        transform.position = Vector2.MoveTowards(transform.position, Player.transform.position, moveSpeed * Time.deltaTime);
    }

    void ChaseTargetGerm()
    {
        _currentState = _states.Chasing;
        transform.position = Vector2.MoveTowards(transform.position, _targetGerm.transform.position, moveSpeed * Time.deltaTime);
    }

    void Patrol()
    {
        if (Vector2.Distance(transform.position, _patrolTarget) < 1f || _patrolTimer <= 0)
        {
            _patrolTarget = (Vector2)transform.position + new Vector2(Random.Range(-5f, 5f), Random.Range(-5f, 5f));
            _patrolTimer = Random.Range(2f, 5f);
        }
        transform.position = Vector2.MoveTowards(transform.position, _patrolTarget, patrolSpeed * Time.deltaTime);
        _patrolTimer -= Time.deltaTime;
    }

    bool SearchForGerm()
    {
        if (_spawner == null || _spawner._AgermOB == null) return false;

        _currentState = _states.Searching;
        _searchTimer -= Time.deltaTime;

        if (_searchTimer <= 0)
        {
            GameObject nearestGerm = null;
            float closestDist = germDetectionRange;

            foreach (GameObject germ in _spawner._AgermOB)
            {
                if (germ != null && germ.activeInHierarchy)
                {
                    float distance = Vector2.Distance(transform.position, germ.transform.position);
                    if (distance < closestDist)
                    {
                        closestDist = distance;
                        nearestGerm = germ;
                    }
                }
            }

            _targetGerm = nearestGerm;
            _searchTimer = searchDelay;
            return _targetGerm != null;
        }
        return _targetGerm != null;
    }

    void Attack(GameObject _target){
        _currentState = _states.Attacking;
        AttackHandler handler = this.gameObject.GetComponent<AttackHandler>();
        if(handler != null) handler.TryUseAttack(_target, "Enemy");
    }
    void WanderWhileSearching()
    {
        Vector2 wanderDirection = ((Vector2)transform.position + Random.insideUnitCircle * wanderStrength) - (Vector2)transform.position;
        transform.position += (Vector3)(wanderDirection.normalized * moveSpeed * Time.deltaTime * 0.5f);
    }
}