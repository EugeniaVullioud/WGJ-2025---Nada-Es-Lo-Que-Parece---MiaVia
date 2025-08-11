using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideObjectSpawner : MonoBehaviour
{
    public List<MovingFadingObject> prefabs; // Multiple prefabs
    public int poolSize = 10;
    public float spawnInterval = 0.5f;
    public float spawnDistance = 50f; // Far in front
    public float exitDistance = -5f;  // Behind the camera
    public float fixedSpeed = 8f;
    public float heightOffsetX = -10f;    // How far from center the lane is
    public float tiltAngle = 3f;          // Tilt per lane
    public float tiltAnglex = 90f;          // Tilt per lane
    public float tiltAngley = 180f;          // Tilt per lane

    public float startDistanceFromCenter = 10f; // How close to center you want spawn (Z)
    public float endDistanceFromCenter = 40f;   // How far from center you want object to move to (Z)

    public float growthScale = 1.5f; // Max size multiplier
    [Range(0f, 1f)]
    public float growthStartPercent = 0.7f; // Start growing after 70% of travel

    private Camera _cam;
    private List<ComponentPool<MovingFadingObject>> _pools = new();
    private List<Coroutine> _spawnCoroutines = new();

    private void Start()
    {
        _cam = Camera.main;

        // Create a separate pool for each prefab
        foreach (var prefab in prefabs)
        {
            var pool = new ComponentPool<MovingFadingObject>(prefab, poolSize, transform);
            _pools.Add(pool);
        }
        // Start spawn loops for both sides

        ResumeSpawning();
       
    }

    private IEnumerator SpawnLoop(int laneSide)
    {
        while (true)
        {
            Spawn(laneSide);
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void Spawn(int laneSide)
    {
        // Randomly select a prefab and corresponding pool
        int prefabIndex = Random.Range(0, _pools.Count);
        var pool = _pools[prefabIndex];
        var obj = pool.Get();

        // Save which pool this object came from
        obj.poolIndex = prefabIndex;

        // Compute start and end positions
        float startX = _cam.transform.position.x + startDistanceFromCenter * laneSide;
        float startY = 0f;
        float startZ = _cam.transform.position.z + spawnDistance;

        Vector3 startPos = new Vector3(startX, startY, startZ);

        float endX = laneSide * endDistanceFromCenter;
        float endY = startY - heightOffsetX;
        float endZ = _cam.transform.position.z + exitDistance;

        Vector3 endPos = new Vector3(endX, endY, endZ);

        // Compute travel time
        float travelTime = Vector3.Distance(startPos, endPos) / fixedSpeed;

        // Initialize movement and tilt
        obj.InitMovement(startPos, endPos, travelTime, ReturnToPool, growthScale, growthStartPercent);

        float appliedTilt = laneSide > 0 ? -tiltAngle : tiltAngle;
        obj.transform.rotation = Quaternion.Euler(tiltAnglex, tiltAngley, -appliedTilt);
    }

    private void ReturnToPool(MovingFadingObject obj)
    {
        obj.ResetObject();

        // Return to the correct pool using the stored index
        if (obj.poolIndex >= 0 && obj.poolIndex < _pools.Count)
        {
            _pools[obj.poolIndex].Return(obj);
        }
        else
        {
            Debug.LogWarning("Invalid pool index on returned object.");
        }
    }


    public void PauseSpawning()
    {
        foreach (var coroutine in _spawnCoroutines)
        {
            StopCoroutine(coroutine);
        }
        _spawnCoroutines.Clear();
    }

    public void ResumeSpawning()
    {
        _spawnCoroutines.Add(StartCoroutine(SpawnLoop(-1)));
        _spawnCoroutines.Add(StartCoroutine(SpawnLoop(1)));
    }

    private void OnEnable()
    {
        EventBus.Subscribe<EnterDecisionMakingState>((e) => { PauseSpawning(); });
        EventBus.Subscribe<FinishDecisionMaking>((e) => { ResumeSpawning(); });
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<EnterDecisionMakingState>((e) => { PauseSpawning(); });
        EventBus.Unsubscribe<FinishDecisionMaking>((e) => { ResumeSpawning(); });
    }
}
