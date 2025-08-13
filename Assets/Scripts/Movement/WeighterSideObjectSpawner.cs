using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WeighterSideObjectSpawner : MonoBehaviour
{
    [System.Serializable]
    public class SpawnablePrefab
    {
        public MovingFadingObject prefab;
        public bool isFunctional;
        public string id;
    }

    [Header("Prefabs")]
    public List<SpawnablePrefab> spawnablePrefabs;
    public List<SpawnablePrefab> spawnableFunctionalPrefabs;

    [Header("Settings")]
    public int poolSizePerPrefab = 5;
    public float decorativeSpawnInterval = 0.5f;
    public float functionalMinSpacing = 15f; // distance in Z between functionals
    public float functionalLaneBlockTime = 2f; // seconds decoratives avoid functional lanes

    public float spawnDistance = 50f;
    public float exitDistance = -5f;
    public float fixedSpeed = 8f;
    public float startDistanceFromCenter = 10f;
    public float endDistanceFromCenter = 40f;
    public float heightOffsetX = -10f;
    public float tiltAngle = 3f;
    public float tiltAnglex = 90f;          // Tilt per lane
    public float tiltAngley = 180f;          // Tilt per lane
    public float growthScale = 1.5f;
    [Range(0f, 1f)] public float growthStartPercent = 0.7f;

    private Camera _cam;
    private Dictionary<MovingFadingObject, ComponentPool<MovingFadingObject>> _pools = new();
    private Queue<SpawnablePrefab> _functionalQueue = new();

    private float _lastFunctionalZ = float.MinValue;

    private Dictionary<int, float> _laneBlockUntil = new() { { -1, 0f }, { 1, 0f } };

    private List<Coroutine> _spawnCoroutines = new();

    private Dictionary<int, float> _lastFunctionalSpawnTime = new Dictionary<int, float> { { -1, 0f }, { 1, 0f } };
    private Dictionary<int, float> _lastDecorativeSpawnTime = new Dictionary<int, float> { { -1, 0f }, { 1, 0f } };

    private Coroutine _functionalCoroutine;

    public float functionalMinTimeSpacing = 2f;
    public float decorativeMinTimeSpacing = 1f;

    private void Awake()
    {
        _cam = Camera.main;

    }
    private void Start()
    {
        
        foreach (var sp in spawnablePrefabs)
        {
            var pool = new ComponentPool<MovingFadingObject>(sp.prefab, poolSizePerPrefab, transform);
            _pools[sp.prefab] = pool;
        }
       
        StartSpawning();
    }
    public void SetPrefabs(List<DecisionData> sequence)
    {
        spawnableFunctionalPrefabs = new List<SpawnablePrefab>();
        foreach (var dec in sequence)
        {
            var c = new SpawnablePrefab() { prefab = dec.prefab, isFunctional =true, id = dec.decisionId };
            spawnableFunctionalPrefabs.Add(c);
            // Ensure the prefab is added to the pool
            if (!_pools.ContainsKey(c.prefab))
            {
                var pool = new ComponentPool<MovingFadingObject>(c.prefab, poolSizePerPrefab, transform);
                _pools[c.prefab] = pool;
            }
        }
    }
   
    /// <summary>
    /// Triggers the next functional prefab in sequence to be spawned.
    /// </summary>
    public void TriggerFunctionalSpawn()
    {

        if (spawnableFunctionalPrefabs == null || spawnableFunctionalPrefabs.Count <= 0) return;

        if (next == null)
        {
            //Debug.Log("No functional prefabs assigned.");
            return;
        }
        _functionalQueue.Enqueue(new SpawnablePrefab() { prefab = next.prefab, isFunctional = true, id = next.decisionId });
      

    }

    private IEnumerator DecorativeLoop(int laneSide)
    {
        /*
        var decorativePrefabs = spawnablePrefabs;

        while (true)
        {
            if (decorativePrefabs.Count > 0)
            {
                var chosen = decorativePrefabs[Random.Range(0, decorativePrefabs.Count)];
                SpawnFromPrefab(chosen, laneSide);
            }

            yield return new WaitForSeconds(decorativeSpawnInterval);
        }*/

        /*
        var decorativePrefabs = spawnablePrefabs.Where(sp => !sp.isFunctional).ToList();

        while (true)
        {
            // If functionals queued, block decorative spawn on this lane
            if (_functionalQueue.Count == 0 && Time.time >= _laneBlockUntil[laneSide])
            {
                if (decorativePrefabs.Count > 0)
                {
                    var chosen = decorativePrefabs[Random.Range(0, decorativePrefabs.Count)];
                    SpawnFromPrefab(chosen, laneSide);
                    _lastFunctionalSpawnTime[laneSide] = _cam.transform.position.z + spawnDistance;
                }
            }
            else
            {
                // Either functionals waiting or lane blocked — skip decorative spawn
            }

            yield return new WaitForSeconds(decorativeSpawnInterval);
        }*/
        var decorativePrefabs = spawnablePrefabs.Where(sp => !sp.isFunctional).ToList();

        while (spawnablePrefabs!= null && spawnablePrefabs.Count>0)
        {
            bool laneBlocked = Time.time < _laneBlockUntil[laneSide];
            bool functionalsQueued = _functionalQueue.Count > 0;

            if (!functionalsQueued && !laneBlocked)
            {
                var chosen = decorativePrefabs[Random.Range(0, decorativePrefabs.Count)];
                SpawnFromPrefab(chosen, laneSide);
                // decorative spawns already controlled by spawnInterval so no need for extra delay here
            }

            yield return new WaitForSeconds(decorativeSpawnInterval);
        }
    }
    private IEnumerator FunctionalLoop()
    {
        while (true)
        {
            if (_functionalQueue.Count > 0)
            {

                /* 
                var spawnZ = _cam.transform.position.z + spawnDistance; // where objects spawn in Z
                bool canSpawnLeft = (spawnZ - _lastFunctionalSpawnZ[-1]) >= functionalMinSpacing
                    && (spawnZ - _lastDecorativeSpawnZ[-1]) >= functionalMinSpacing;
                bool canSpawnRight = (spawnZ - _lastFunctionalSpawnZ[1]) >= functionalMinSpacing
                                    && (spawnZ - _lastDecorativeSpawnZ[1]) >= functionalMinSpacing; */

                /*
                float distSinceLast = Mathf.Abs(_cam.transform.position.z - _lastFunctionalZ);
                if (distSinceLast >= functionalMinSpacing)
                {
                    // Always spawn in both lanes at the same time? Or only one?
                    var prefab = _functionalQueue.Dequeue();

                    var side = Random.Range(0, 2);
                    if (side == 0 )SpawnFromPrefab(prefab, -1);
                    else SpawnFromPrefab(prefab, 1);

                    // Block both lanes from decorative spawn for a short time
                    _laneBlockUntil[-1] = Time.time + functionalLaneBlockTime;
                    _laneBlockUntil[1] = Time.time + functionalLaneBlockTime;

                    _lastFunctionalZ = _cam.transform.position.z;
                }
                */

                /*
                // Spawn on left lane if spacing allows
                if (canSpawnLeft && _functionalQueue.Count > 0)
                {
                    var prefab = _functionalQueue.Dequeue();
                    SpawnFromPrefab(prefab, -1);
                    _lastFunctionalSpawnZ[-1] = spawnZ;

                    // Block lane for decoratives
                    _laneBlockUntil[-1] = Time.time + functionalLaneBlockTime;
                }

                // Spawn on right lane if spacing allows
                else if (canSpawnRight && _functionalQueue.Count > 0)
                {
                    var prefab = _functionalQueue.Dequeue();
                    SpawnFromPrefab(prefab, 1);
                    _lastFunctionalSpawnZ[1] = spawnZ;

                    // Block lane for decoratives
                    _laneBlockUntil[1] = Time.time + functionalLaneBlockTime;
                }*/

                /*
                if (canSpawnLeft)
                {
                    var prefab = _functionalQueue.Dequeue();
                    SpawnFromPrefab(prefab, -1);
                    _lastFunctionalSpawnZ[-1] = spawnZ;
                    _laneBlockUntil[-1] = Time.time + functionalLaneBlockTime;
                    Debug.Log($"Dequeueing prefab: {prefab.prefab.name} for lane left");
                    Debug.Log($"Queue count: {_functionalQueue.Count}");

                }
                else if (canSpawnRight)
                {
                    var prefab = _functionalQueue.Dequeue();
                    SpawnFromPrefab(prefab, 1);
                    _lastFunctionalSpawnZ[1] = spawnZ;
                    _laneBlockUntil[1] = Time.time + functionalLaneBlockTime;
                    Debug.Log($"Dequeueing prefab: {prefab.prefab.name} for lane right");
                    Debug.Log($"Queue count: {_functionalQueue.Count}");

                }

                Debug.Log($"Can left: {canSpawnLeft} Can right: {canSpawnRight}");
                */

                float now = Time.time;
                bool canSpawnLeft = (now - _lastFunctionalSpawnTime[-1]) >= functionalMinTimeSpacing;
                bool canSpawnRight = (now - _lastFunctionalSpawnTime[1]) >= functionalMinTimeSpacing;

                if (!canSpawnLeft && !canSpawnRight && spawnablePrefabs.Count ==0)
                {
                    var randdom = Random.Range(0, 2);
                    if (randdom == 0) canSpawnLeft = true;
                    else canSpawnRight = true;
                }
                if (canSpawnLeft)
                {
                    var prefab = _functionalQueue.Dequeue();

                    queued = false;
                    if (!_pools.ContainsKey(prefab.prefab))
                    {
                        var pool = new ComponentPool<MovingFadingObject>(prefab.prefab, poolSizePerPrefab, transform);
                        _pools[prefab.prefab] = pool;
                    }
                    SpawnFromPrefab(prefab, -1);
                    _lastFunctionalSpawnTime[-1] = now;
                    _laneBlockUntil[-1] = now + functionalLaneBlockTime;
                }
                else if (canSpawnRight)
                {
                    var prefab = _functionalQueue.Dequeue();
                    queued = false;

                    if (!_pools.ContainsKey(prefab.prefab))
                    {
                        var pool = new ComponentPool<MovingFadingObject>(prefab.prefab, poolSizePerPrefab, transform);
                        _pools[prefab.prefab] = pool;
                    }
                    SpawnFromPrefab(prefab, 1);
                    _lastFunctionalSpawnTime[1] = now;
                    _laneBlockUntil[1] = now + functionalLaneBlockTime;
                }

            }
            yield return null; // check every frame
        }
    }
    Vector3 StartPosition(int laneSide)
    {
        float startX = _cam.transform.position.x + startDistanceFromCenter * laneSide;
        float startY = 0f;
        float startZ = _cam.transform.position.z + spawnDistance;

        Vector3 startPos = new Vector3(startX, startY, startZ);

        return startPos;
    }

    private void SpawnFromPrefab(SpawnablePrefab chosen, int laneSide)
    {
       
        Vector3 startPos = StartPosition(laneSide);
        // Update last decorative spawn Z if this is a decorative prefab

        /*
        if (chosen.isFunctional)
        {
            _lastFunctionalSpawnZ[laneSide] = startPos.z;
        }
        else
        {
            _lastDecorativeSpawnZ[laneSide] = startPos.z;
        }*/
        float endX = laneSide * endDistanceFromCenter;
        float endY = startPos.y - heightOffsetX;
        float endZ = _cam.transform.position.z + exitDistance;

        Vector3 endPos = new Vector3(endX, endY, endZ);

        float travelTime = Vector3.Distance(startPos, endPos) / fixedSpeed;

        var obj = _pools[chosen.prefab].Get();
        obj.InitMovement(startPos, endPos, travelTime, o => ReturnToPool(chosen.prefab, o), growthScale, growthStartPercent, paused);

        // Apply tilt
        float appliedTilt = laneSide > 0 ? -tiltAngle : tiltAngle;
        obj.transform.rotation = Quaternion.Euler(tiltAnglex, tiltAngley, -appliedTilt);
    }

    private void ReturnToPool(MovingFadingObject prefab, MovingFadingObject obj)
    {
        obj.ResetObject();
        _pools[prefab].Return(obj);
    }

    public void StartSpawning()
    {
        if (spawnablePrefabs != null && spawnablePrefabs.Count > 0) _spawnCoroutines.Add(StartCoroutine(DecorativeLoop(1)));
        if (spawnablePrefabs != null && spawnablePrefabs.Count > 0) _spawnCoroutines.Add(StartCoroutine(DecorativeLoop(-1)));
        _functionalCoroutine = StartCoroutine(FunctionalLoop());
        _spawnCoroutines.Add(_functionalCoroutine);
    }
    public void PauseFunctionalLoop()
    {
        if (_functionalCoroutine != null)
        {
            //Debug.Log("pause spawning");

            StopCoroutine(_functionalCoroutine);
            _spawnCoroutines.Remove(_functionalCoroutine);
            _functionalCoroutine = null;
        }
    }
    public void ResumeFunctionalLoop()
    {

        if (_functionalCoroutine == null)
        {
           // Debug.Log("resume spawning");

            _functionalCoroutine = StartCoroutine(FunctionalLoop());
            _spawnCoroutines.Add(_functionalCoroutine);
        }
    }
    public void PauseSpawning()
    {
        foreach (var c in _spawnCoroutines) StopCoroutine(c);
        PauseFunctionalLoop();
        _spawnCoroutines.Clear();
    }

    public void ResumeSpawning()
    {
        StartSpawning();
        ResumeFunctionalLoop();
    }
    DecisionData next;
    DecisionData nextWhenPaused;
    bool queued = false;
    void StoreNext(DecisionData data)
    {
        next = data;
        queued = true;
    }

    bool paused = false;
    private void OnEnable()
    {
        EventBus.Subscribe<EnterDecisionMakingState>((e) => { nextWhenPaused = next; paused = true; });
       
        EventBus.Subscribe<LostGame>((e) => { PauseSpawning(); queued = false; _functionalQueue.Clear(); next = null; _lastFunctionalSpawnTime = new Dictionary<int, float> { { -1, 0f }, { 1, 0f } }; });
        EventBus.Subscribe<WonGame>((e) => { PauseSpawning(); queued = false; _functionalQueue.Clear(); next = null; _lastFunctionalSpawnTime = new Dictionary<int, float> { { -1, 0f }, { 1, 0f } }; });
        EventBus.Subscribe<PlayAgain>((e) => { ResumeSpawning(); queued = false; _functionalQueue.Clear(); next = null; _lastFunctionalSpawnTime = new Dictionary<int, float> { { -1, 0f }, { 1, 0f } }; });
        EventBus.Subscribe<QueueNextDecision>((e) => { StoreNext(e.NextDecision); });
        EventBus.Subscribe<SpawnNextDecision>((e) => {TriggerFunctionalSpawn(); });
        EventBus.Subscribe<FinishDecisionMaking>((e) => { paused = false; });
    }
   
    private void OnDisable()
    {
        EventBus.Unsubscribe<EnterDecisionMakingState>((e) => { nextWhenPaused = next; paused = true; });
       

        EventBus.Unsubscribe<FinishDecisionMaking>((e) => { paused = false; });
       EventBus.Unsubscribe<SpawnNextDecision>((e) => { TriggerFunctionalSpawn(); });
        EventBus.Unsubscribe<LostGame>((e) => { PauseSpawning(); queued = false; _functionalQueue.Clear(); next = null; _lastFunctionalSpawnTime = new Dictionary<int, float> { { -1, 0f }, { 1, 0f } }; });
        EventBus.Unsubscribe<WonGame>((e) => { PauseSpawning(); queued = false; _functionalQueue.Clear(); next = null; _lastFunctionalSpawnTime = new Dictionary<int, float> { { -1, 0f }, { 1, 0f } }; });
        EventBus.Unsubscribe<PlayAgain>((e) => { ResumeSpawning(); queued = false; _functionalQueue.Clear(); next = null; _lastFunctionalSpawnTime = new Dictionary<int, float> { { -1, 0f }, { 1, 0f } }; });
        EventBus.Unsubscribe<QueueNextDecision>((e) => { StoreNext(e.NextDecision); });

    }

}
