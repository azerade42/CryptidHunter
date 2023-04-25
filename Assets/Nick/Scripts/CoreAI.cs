using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
public class CoreAI : MonoBehaviour
{
    // Handles each state of behavior the AI is in, they're like constants
    // Apply specific instructions based on the state and run them every frame
    public enum AIState
    {
        // Default state
        // Roams to randomly generated points OR
        // Roams to predetermined waypoints, depending on bool (_usingWaypoints)
        Passive,
        // In this state, the AI will actively seek out the player
        // Losing line of sight of the player for a long enough time
        // will cause the AI to go back to the "Passive" state.
        Hostile,

        // In this state, the AI will frantically run around in circles around the player
        Panic,

        // The AI stops in place and stands up
        Stop
    }

    // How long it has been since the AI has "seen" the player
    // Used to determine when the AI can lose track of the player
    private float _timeSinceSeenPlayer;

    // The NavMeshAgent allows the AI to move and to limit the areas it's allowed to enter
    private NavMeshAgent _navMeshAgent;

    [Tooltip("The current state of the AI")] [SerializeField]
    public AIState _AIState;
    [Tooltip("Which Unity layer will make the AI go into the Hostile state")]
    public LayerMask targetMask;
    [Tooltip("Which unity layer will block the AI's ability to \"see\"")]
    public LayerMask ObstructionMask;

    [Tooltip("If the AI can \"see\" the player on a specific frame")]
    public bool _canSeePlayer;
    public bool _isChasingPlayer = false;

    [Tooltip("When the AI is holding a position")]
    [SerializeField]
    private bool _isWaiting;

    [Tooltip("Determines if the AI waits when it reaches a random point or if it never stops moving")]
    [SerializeField]
    private bool _alwaysMoving;

    [Tooltip("Is random wandering enabled?")]
    [SerializeField]
    private bool _randomWander;

    [Tooltip("Is the AI chasing the player or fleeing?")]
    [SerializeField]
    private bool _fleeFromPlayer;

    [Tooltip("Is the wait time randomized?")]
    [SerializeField]
    private bool _randomWaitTime;

    [Tooltip("How long the AI will wait at each point if _alwaysMoving is FALSE")]
    [SerializeField]
    [Range(1, 15)]
    private float _waitTime;

    [Tooltip("Manipulates the NavMeshAgent's built in speed variable")]
    [SerializeField]
    private float _speed;

    [Tooltip("How fast the AI will move towards the player in the Hostile chasing state")]
    [SerializeField]
    private float _attackSpeed;

    [Tooltip("How fast the AI will move away the player in the Hostile flee state")]
    [SerializeField]
    private float _fleeSpeed;

    [Tooltip("Dictates how far away a randomly generated waypoint can spawn from the AI's current position")]
    [Range(0, 500)]
    public float walkRadius;

    [Tooltip("How far in the distance can the AI \"see\"? (field of view check)")]
    public float _FOVRadius = 20f;

    [Tooltip("How far around itself can the AI see? (field of view check)")]
    [Range(0, 360)]
    public float _FOVAngle = 90f;

    [Tooltip("How far in the distance can the AI \"hear\" or sense? (proximity check)")]
    public float _proximityRadius = 20f;

    [Tooltip("How far around itself can the AI \"hear\" or sense? (proximity check)")]
    [Range(0, 360)]
    public float _proximityAngle = 90f;

    [Tooltip("How far around does the AI move when it's panicing?")]
    [Range(1, 100)]
    public float _circleRadius;

    [Tooltip("How fast does the AI move when it's panicing?")]
    [Range(0, 3)]
    public float _circleSpeed;

    private float timeSpentCircling;
    private float currentCircleAngle;
    private bool circlingClockwise;
    private bool prevDir;

    [SerializeField] private AudioObject holdStill;

    [Tooltip("Holds the waypoints displayed in the scene. Only used if _randomWander is false")]
    private Transform[] _waypoints;

    [Tooltip("Determines which waypoint the AI goes to next")]
    private int _nextWaypoint = 0;

    [Tooltip("A reference to the player")]
    public GameObject _player;
    [Tooltip("WaypointManager script handles the position and look of the waypoints")]
    public WaypointManager _waypointManager;

    [Tooltip("Materials that get changed during the AI's state changes")]
    public Renderer [] _materialsToChange;
    public Material _enemyWanderingColor;
    public Material _enemySearchingColor;
    public Material _enemyAttackingColor;
    public Material _enemyFleeingColor;

    [Tooltip("Animator for the enemy")]
    public Animator anim;

    [SerializeField] private Rifle rifle;

    public void OnEnable()
    {
        EventManager.Instance.rifleHit += AIHit;
        EventManager.Instance.enemyDie += StopMoving;
        EventManager.Instance.talismanUsed += Panic;
    }

    public void OnDisable()
    {
        EventManager.Instance.rifleHit -= AIHit;
        EventManager.Instance.enemyDie -= StopMoving;
        EventManager.Instance.talismanUsed -= Panic;
    }

    public void Spawn(Talisman talisman)
    {
        Transform spawnPoint = talisman.spawnPoints[Random.Range(0, talisman.spawnPoints.Count)];
        transform.position = spawnPoint.position;

        if (EventManager.Instance.nearPlayer != null)
            EventManager.Instance.nearPlayer.Invoke();
    }

    private void Start()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _waypoints = _waypointManager._trackedWaypoints;

        StartCoroutine(CheckForPlayer());
    }
    private void Update()
    {
        anim.speed = _navMeshAgent.speed/5;
        
        switch (_AIState)
        {
            case AIState.Passive:

                _navMeshAgent.speed = _speed;

                if (_randomWander == true)
                {
                    Wander();
                    if (_canSeePlayer == true)
                    {
                        _AIState = AIState.Hostile;
                    }
                }
                else
                {
                    if (_navMeshAgent.remainingDistance < 2f && _canSeePlayer == false)
                    {
                        GotoNextPoint();
                    }
                    if (_canSeePlayer == true)
                    {
                        _AIState = AIState.Hostile;
                    }
                }

                TargetCheck(_proximityRadius, _proximityAngle);

                // ChangeRendererColors(_enemyWanderingColor);
                //anim.SetBool("isChasingPlayer", _isChasingPlayer);

                break;

            case AIState.Hostile:

                _navMeshAgent.acceleration = 8;

                if (_fleeFromPlayer == true)
                {
                    FleeFromPlayer();
                    
                    _navMeshAgent.speed = _fleeSpeed;
                    // ChangeRendererColors(_enemyFleeingColor);
                }
                else
                {
                    ChasePlayer();

                    _navMeshAgent.speed = _attackSpeed;
                    // anim.SetBool("isChasingPlayer", _isChasingPlayer);

                }
                if (_canSeePlayer == false)
                {
                    TargetCheck(_FOVRadius, _FOVAngle);
                }
                TargetCheck(_proximityRadius, _proximityAngle);
                break;

            case AIState.Panic:

                _navMeshAgent.speed = _circleSpeed * _circleRadius - _navMeshAgent.speed * _navMeshAgent.stoppingDistance;
                _navMeshAgent.acceleration = 2 * _navMeshAgent.speed;

                if (timeSpentCircling > 20f)
                {
                    Vocals.instance.Say(holdStill);
                    timeSpentCircling = 0f;
                    prevDir = circlingClockwise;
                    circlingClockwise = !circlingClockwise;
                }

                timeSpentCircling += Time.deltaTime;

                CirclePlayer(_circleRadius, _circleSpeed, circlingClockwise);

                break;

            case AIState.Stop:

                _navMeshAgent.speed = 0;
                
                _navMeshAgent.acceleration = 0;
                _navMeshAgent.destination = transform.position;

                break;

        }
    }

    // Wander around to random points
    private void Wander()
    {
        if (_navMeshAgent == null)
            return;
        
        if (_navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance && _isWaiting == false)
        {
            _navMeshAgent.SetDestination(RandomNavMeshLocation());
            _isWaiting = true;
            StartCoroutine(RandomWaitTimer());
        }
    }

    // Choose the random point on the navMesh
    public Vector3 RandomNavMeshLocation()
    {
        Vector3 finalPosition = Vector3.zero;
        Vector3 randomPosition = Random.insideUnitSphere * walkRadius;
        randomPosition += transform.position;
        if (NavMesh.SamplePosition(randomPosition, out NavMeshHit hit, walkRadius, 1))
        {
            finalPosition = hit.position;
        }
        return finalPosition;
    }

    // Move to the next waypoint
    void GotoNextPoint()
    {
        if (_waypoints.Length == 0)
            return;
        _navMeshAgent.destination = _waypoints[_nextWaypoint].position;
        _nextWaypoint = (_nextWaypoint + 1) % _waypoints.Length;
        _isWaiting = true;

        StartCoroutine(RandomWaitTimer());
    }

    // Check if the objects in the target mask are within the AI's field of view (seeing range)/ proximity (hearing range)
    private void TargetCheck(float radius, float angle)
    {
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, radius, targetMask);

        if (rangeChecks.Length > 0)
        {
            // rangeChecks[0] is the player if no other objects are detected in the targetMask
            // The player can be found in multiple objects by comparing the tag of each element in the
            // rangeChecks array to the "Player" tag or some other crazy multiplayer system
            Transform target = rangeChecks[0].transform;
            Vector3 directionToTarget = (target.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, ObstructionMask))
                {
                    _canSeePlayer = true;
                }
                else
                {
                    _canSeePlayer = false;
                }
            }
            else
            {
                _canSeePlayer = false;

            }
        }
        else if (_canSeePlayer == false)
        {
            _timeSinceSeenPlayer += Time.deltaTime;

            if (_timeSinceSeenPlayer >= 2f)
            {
                _isChasingPlayer = false;
                _AIState = AIState.Passive;
                _timeSinceSeenPlayer = 0;
            }
        }
    }

    // Field check over and over for 0.2 seconds
    private IEnumerator CheckForPlayer()
    {
        WaitForSeconds wait = new WaitForSeconds(0.2f);

        while (true)
        {
            yield return wait;
            TargetCheck(_FOVRadius, _FOVAngle);
        }

    }

    // Wait at waypoints
    IEnumerator RandomWaitTimer()
    {
        if (_alwaysMoving == false && _randomWaitTime == true)
        {
            _waitTime = Random.Range(1, 5);
            _navMeshAgent.speed = 0;
            yield return new WaitForSeconds(_waitTime);
        }
        else if (_alwaysMoving == false && _randomWaitTime == false)
        {
            _navMeshAgent.speed = 0;
            yield return new WaitForSeconds(_waitTime);
        }
        _navMeshAgent.speed = _speed;
        _isWaiting = false;
    }

    // Move towards the player
    private void ChasePlayer()
    {
        _isChasingPlayer = true;
        _navMeshAgent.destination = _player.transform.position;

        // Change the color of the enemy based on if they can see the player or are searching
        if (_canSeePlayer == true)
        {
            // ChangeRendererColors(_enemyAttackingColor);
        }
        else
        {
            // ChangeRendererColors(_enemySearchingColor);
        }
        TargetCheck(_FOVRadius, _FOVAngle);
    }

    // Move away from the player 
    public void FleeFromPlayer()
    {
        Vector3 runTo = transform.position + ((transform.position - _player.transform.position));
        float distance = Vector3.Distance(transform.position, _player.transform.position);
        if (distance < walkRadius)
        {
             _navMeshAgent.SetDestination(runTo);
        }
    }

    // Moves around the player in a circle pattern
    private void CirclePlayer(float radius, float speed, bool clockwise)
    {
        int dir = clockwise == false ? -1 : 1;
        dir = 1;
        currentCircleAngle += Time.deltaTime * speed;
        if (currentCircleAngle >= Mathf.PI * 2)
            currentCircleAngle = Mathf.PI * -2;
        Vector3 circlePos = new Vector3(Mathf.Sin(currentCircleAngle) * radius, 0f, Mathf.Cos(currentCircleAngle) * radius);
        _navMeshAgent.SetDestination(_player.transform.position + circlePos);
    }

    // Changes the color of the materials in _materialsToChange
    // Used for debugging state changes
    private void ChangeRendererColors(Material newMat)
    {
        foreach (Renderer r in _materialsToChange)
        {
            r.material = newMat;
        }
    }

    private void OnTriggerStay(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<NickPlayerController>().Damage(10f);
        }
    }

    private void AIHit()
    {
        
    }

    private void Panic()
    {
        StartCoroutine(RunBeforePanic());
    }

    IEnumerator RunBeforePanic()
    {
        _fleeFromPlayer = true;
        yield return new WaitForSeconds(3.0f);
        _fleeFromPlayer = false;
        _AIState = AIState.Panic;
    }

    private void StopMoving()
    {
        _AIState = AIState.Stop;
    }

    void OnDrawGizmos()
    {
        if (_navMeshAgent == null)
            return;
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(_navMeshAgent.destination, 1f);
    }
}

