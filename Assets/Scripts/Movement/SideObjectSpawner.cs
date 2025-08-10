using System.Collections.Generic;
using UnityEngine;

public class SideObjectSpawner : MonoBehaviour
{
    public MovingFadingObject prefab;
    public int poolSize = 10;
    public float spawnInterval = 0.5f;
    public float spawnDistance = 50f; // Far in front
    public float exitDistance = -5f;  // Behind the camera
    public float minSpeed = 8f;
    public float maxSpeed = 12f;
    public float heightOffsetX = -10f;    // How far from center the lane is
    public float tiltAngle = 3f;    // How far from center the lane is

    public float startDistanceFromCenter = 10f; // How close to center you want spawn (Z)
    public float endDistanceFromCenter = 40f;   // How far from center you want object to move to (Z)

    private ComponentPool<MovingFadingObject> _pool;
    private Camera _cam;

    public float growthScale = 1.5f; // Max size multiplier
    [Range(0f, 1f)]
    public float growthStartPercent = 0.7f; // Start growing after 70% of travel

    private List<Coroutine> _spawnCoroutines = new();

    private void Start()
    {
        _pool = new ComponentPool<MovingFadingObject>(prefab, poolSize, transform);
        _cam = Camera.main;

        StartCoroutine(SpawnLoop(-1)); // Left lane
        StartCoroutine(SpawnLoop(1));  // Right lane
    }

    private System.Collections.IEnumerator SpawnLoop(int laneSide)
    {
        while (true)
        {
            Spawn(laneSide);
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void Spawn(int laneSide)
    {

        float startX = _cam.transform.position.x + startDistanceFromCenter* laneSide;  // Start near center, +/- 1 unit randomly
        float startY = 0f;
        float startZ = _cam.transform.position.z + spawnDistance; // e.g. +50

        Vector3 startPos = new Vector3(startX, startY, startZ);

        // Controlled horizontal offset for the end position (e.g. 5 units to side)
        float endX = laneSide * endDistanceFromCenter; // laneOffsetX is how far you want from center at end
        float endY = startY - heightOffsetX;      // slight downward offset if you want
        float endZ = _cam.transform.position.z + exitDistance;    // e.g. 0 or negative

        Vector3 endPos = new Vector3(endX, endY, endZ);

        float speed = Random.Range(minSpeed, maxSpeed);
        float travelTime = Vector3.Distance(startPos, endPos) / speed;

        var obj = _pool.Get();
        obj.InitMovement(startPos, endPos, travelTime, ReturnToPool, growthScale, growthStartPercent);

        // Apply tilt based on side
        float appliedTilt = laneSide > 0 ? -tiltAngle : tiltAngle;
        obj.transform.rotation = Quaternion.Euler(-90, 0f, appliedTilt);
    }

    private void ReturnToPool(MovingFadingObject obj)
    {
        obj.ResetObject();

        _pool.Return(obj);
    }


}
