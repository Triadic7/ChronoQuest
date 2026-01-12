using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generic spawner to spawn objects at position.
/// </summary>
public class Spawner : MonoBehaviour
{
    /// <summary>
    /// Called when a prefab is instanced.
    /// </summary>
    public event Action<GameObject> OnObjectInstanced;

    /// <summary>
    /// The object to spawn.
    /// </summary>
    [SerializeField]
    private GameObject prefab;

    /// <summary>
    /// How long until another object spawns.
    /// </summary>
    [SerializeField]
    private float interval;

    /// <summary>
    /// Possible spawn locations.
    /// If empty, spawns at spawner position.
    /// </summary>
    [SerializeField]
    private List<Transform> spawnPoints = new List<Transform>();

    /// <summary>
    /// Should the spawner loop automatically.
    /// </summary>
    [SerializeField]
    private bool loop = true;

    /// <summary>
    /// If the prefabs should spawn between points.
    /// </summary>
    [SerializeField]
    private bool spawnBetweenPoints;

    /// <summary>
    /// Optional max spawn count. 
    /// -1 = infinite.
    /// </summary>
    [SerializeField]
    private int maxSpawns = -1;

    /// <summary>
    /// Is the spawner currently running.
    /// </summary>
    private bool isRunning;

    /// <summary>
    /// Spawn coroutine reference.
    /// </summary>
    private Coroutine spawnRoutine;

    /// <summary>
    /// All currently spawned objects.
    /// </summary>
    private readonly List<GameObject> spawnedObjects = new List<GameObject>();

    /// <summary>
    /// Starts spawning.
    /// </summary>
    public void StartSpawning()
    {
        if (this.isRunning)
        {
            return;
        }

        this.isRunning = true;
        this.spawnRoutine = StartCoroutine(this.SpawnLoop());
    }

    /// <summary>
    /// Spawns a single object immediately.
    /// </summary>
    public void SpawnOnce()
    {
        this.Spawn();
    }

    /// <summary>
    /// Destroys all objects spawned by this spawner.
    /// </summary>
    public void DestroyAllSpawned()
    {
        for (int i = this.spawnedObjects.Count - 1; i >= 0; i--)
        {
            if (this.spawnedObjects[i] != null)
            {
                Destroy(this.spawnedObjects[i]);
            }
        }

        this.spawnedObjects.Clear();
    }

    /// <summary>
    /// Stops spawning.
    /// </summary>
    public void StopSpawning()
    {
        if (!this.isRunning)
        {
            return;
        }

        this.isRunning = false;

        if (this.spawnRoutine != null)
        {
            StopCoroutine(this.spawnRoutine);
            this.spawnRoutine = null;
        }
    }

    /// <summary>
    /// Destroys all spawned objects and stops spawning.
    /// </summary>
    public void StopSpawningAndDestroyAll()
    {
        StopSpawning();

        for (int i = this.spawnedObjects.Count - 1; i >= 0; i--)
        {
            if (this.spawnedObjects[i] != null)
            {
                Destroy(this.spawnedObjects[i]);
            }
        }

        this.spawnedObjects.Clear();
    }

    /// <summary>
    /// Main spawn loop.
    /// </summary>
    private IEnumerator SpawnLoop()
    {
        while (this.isRunning)
        {
            this.Spawn();

            if (this.maxSpawns > 0 && this.spawnedObjects.Count >= this.maxSpawns)
            {
                this.StopSpawning();
                yield break;
            }

            if (!this.loop)
            {
                this.StopSpawning();
                yield break;
            }

            yield return new WaitForSeconds(this.interval);
        }
    }

    /// <summary>
    /// Spawns the prefab at a random spawn point.
    /// </summary>
    private void Spawn()
    {
        if (this.prefab == null)
        {
            Debug.LogWarning("Spawner has no prefab assigned.");
            return;
        }

        if (this.spawnBetweenPoints)
        {
            SpawnBetweenPoints();
            return;
        }

        Transform spawnPoint = GetRandomSpawnPoint();

        // Set spawn position to either spawnpoint, or spawner location.
        Vector3 position = spawnPoint != null ? spawnPoint.position : this.transform.position;

        // Set rotation on the prefab.
        Quaternion rotation = spawnPoint != null ? spawnPoint.rotation : Quaternion.identity;

        // Instance prefab and add to the list.
        GameObject instance = Instantiate(this.prefab, position, rotation);
        this.spawnedObjects.Add(instance);

        // Fire event.
        this.OnObjectInstanced?.Invoke(instance);
    }

    /// <summary>
    /// Spawns the prefab at a random position between two spawn points.
    /// Requires at least 2 spawn points.
    /// </summary>
    private void SpawnBetweenPoints()
    {
        if (this.prefab == null)
        {
            Debug.LogWarning("Spawner has no prefab assigned.");
            return;
        }

        if (this.spawnPoints == null || this.spawnPoints.Count < 2)
        {
            Debug.LogWarning("SpawnBetweenPoints requires at least 2 spawn points.");
            return;
        }

        // Pick two different points.
        Transform a = this.spawnPoints[UnityEngine.Random.Range(0, this.spawnPoints.Count)];
        Transform b = this.spawnPoints[UnityEngine.Random.Range(0, this.spawnPoints.Count)];

        if (a == b)
        {
            return;
        }

        // Random value.
        float t = UnityEngine.Random.value;

        // Get position and rotation.
        Vector3 position = Vector3.Lerp(a.position, b.position, t);
        Quaternion rotation = Quaternion.identity;

        // Instance prefab.
        GameObject instance = Instantiate(this.prefab, position, rotation);
        this.spawnedObjects.Add(instance);

        this.OnObjectInstanced?.Invoke(instance);
    }


    /// <summary>
    /// Returns a random spawn point or null if none exist.
    /// </summary>
    private Transform GetRandomSpawnPoint()
    {
        if (this.spawnPoints == null || this.spawnPoints.Count == 0)
        {
            return null;
        }

        int index = UnityEngine.Random.Range(0, this.spawnPoints.Count);
        return this.spawnPoints[index];
    }
}
