using System.Collections;
using UnityEngine;

public class AsteroidSpawner : MonoBehaviour
{
    public Asteroid[] AsteroidPrefabs;
    public float SpawnRate = 1f;
    public float SpawnDistance = 15f;
    public float SpawnAmount = 0f;
    public float TrajectoryVariance = 15f;

    public System.DateTime SpawningStartedAt;

    private Bounds _screenBounds;
    private float startNonRandomSpawn = 20;

    private void Awake()
    {
        _screenBounds = new Bounds();
        _screenBounds.Encapsulate(Camera.main.ScreenToWorldPoint(Vector3.zero));
        _screenBounds.Encapsulate(Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0f)));
    }

    void Start()
    {
        SpawningStartedAt = System.DateTime.Now;
        InvokeRepeating(nameof(Spawn), SpawnRate, SpawnRate);
    }

    void Spawn()
    {
        float rnd = Random.value;
        System.TimeSpan sinceStart = System.DateTime.Now.Subtract(SpawningStartedAt);
        // Increase the amount of random spawn each minute
        SpawnAmount = (float)sinceStart.TotalSeconds / 60;
        // 1% chance to spawn from a specific direction
        if (sinceStart.TotalSeconds > startNonRandomSpawn && rnd >= 0 && rnd < 0.01)
        {
            SpawnFromRight(Random.Range(3, 5));
        }
        else if (sinceStart.TotalSeconds > startNonRandomSpawn && rnd >= 0.01 && rnd < 0.02)
        {
            SpawnFromLeft(Random.Range(3, 5));
        }
        else if (sinceStart.TotalSeconds > startNonRandomSpawn && rnd >= 0.02 && rnd < 0.03)
        {
            SpawnFromTop(Random.Range(4, 8));
        }
        else if (sinceStart.TotalSeconds > startNonRandomSpawn && rnd >= 0.03 && rnd < 0.04)
        {
            SpawnFromBottom(Random.Range(4, 8));
        }
        else
        {
            SpawnRandom();
        }
    }

    void SpawnRandom()
    {
        int amount = Mathf.CeilToInt(SpawnAmount);
        for (int i = 0; i < amount; i++)
        {
            // Create a random asteroid with random size and trajectory
            Vector3 direction = Random.insideUnitCircle.normalized * SpawnDistance;
            Vector3 position = transform.position + direction;

            float variance = Random.Range(-TrajectoryVariance, TrajectoryVariance);
            Quaternion rotation = Quaternion.AngleAxis(variance, Vector3.forward);

            Asteroid prefab = AsteroidPrefabs[Random.Range(0, AsteroidPrefabs.Length)];
            Asteroid asteroid = Instantiate(prefab, position, rotation);

            float size = Random.Range(asteroid.MinSize, asteroid.MaxSize);
            asteroid.SetDimensions(size);

            asteroid.SetTrajectory(rotation * -direction);
        }
    }

    IEnumerator SpawnFromRight(int numberOfAsteroids = 5)
    {
        float minY = _screenBounds.min.y;
        float maxY = _screenBounds.max.y;
        float asteroidSize = 1;
        for (int i = 0; i < numberOfAsteroids; i++)
        {
            float offsetY = (maxY - minY) / numberOfAsteroids * i + minY + asteroidSize;
            Vector3 position = new Vector3(_screenBounds.min.x - asteroidSize, offsetY);

            float variance = Random.Range(-TrajectoryVariance, TrajectoryVariance);
            Quaternion rotation = Quaternion.AngleAxis(variance, Vector3.forward);

            Asteroid prefab = AsteroidPrefabs[Random.Range(0, AsteroidPrefabs.Length)];
            Asteroid asteroid = Instantiate(prefab, position, rotation);

            asteroid.SetDimensions(asteroidSize);
            asteroid.SetTrajectory(Vector3.right * SpawnDistance);
        }
        return null;
    }

    void SpawnFromLeft(int numberOfAsteroids = 5)
    {
        float minY = _screenBounds.min.y;
        float maxY = _screenBounds.max.y;
        float asteroidSize = 1;
        for (int i = 0; i < numberOfAsteroids; i++)
        {
            float offsetY = (maxY - minY) / numberOfAsteroids * i + minY + asteroidSize;
            Vector3 position = new Vector3(_screenBounds.max.x + asteroidSize, offsetY);

            float variance = Random.Range(-TrajectoryVariance, TrajectoryVariance);
            Quaternion rotation = Quaternion.AngleAxis(variance, Vector3.forward);

            Asteroid prefab = AsteroidPrefabs[Random.Range(0, AsteroidPrefabs.Length)];
            Asteroid asteroid = Instantiate(prefab, position, rotation);

            asteroid.SetDimensions(asteroidSize);
            asteroid.SetTrajectory(Vector3.left * SpawnDistance);
        }
    }

    void SpawnFromTop(int numberOfAsteroids = 8)
    {
        float minX = _screenBounds.min.x;
        float maxX = _screenBounds.max.x;
        float asteroidSize = 1;
        
        for (int i = 0; i < numberOfAsteroids; i++)
        {
            float offsetX = (maxX - minX) / numberOfAsteroids * i + minX + asteroidSize;
            Vector3 position = new Vector3(offsetX, _screenBounds.max.y + asteroidSize);

            float variance = Random.Range(-TrajectoryVariance, TrajectoryVariance);
            Quaternion rotation = Quaternion.AngleAxis(variance, Vector3.forward);

            Asteroid prefab = AsteroidPrefabs[Random.Range(0, AsteroidPrefabs.Length)];
            Asteroid asteroid = Instantiate(prefab, position, rotation);

            asteroid.SetDimensions(asteroidSize);
            asteroid.SetTrajectory(Vector3.down * SpawnDistance);
        }
    }

    void SpawnFromBottom(int numberOfAsteroids = 8)
    {
        float minX = _screenBounds.min.x;
        float maxX = _screenBounds.max.x;
        float asteroidSize = 1;
        for (int i = 0; i < numberOfAsteroids; i++)
        {
            float offsetX = (maxX - minX) / numberOfAsteroids * i + minX + asteroidSize;
            Vector3 position = new Vector3(offsetX, _screenBounds.min.y - asteroidSize);

            float variance = Random.Range(-TrajectoryVariance, TrajectoryVariance);
            Quaternion rotation = Quaternion.AngleAxis(variance, Vector3.forward);

            Asteroid prefab = AsteroidPrefabs[Random.Range(0, AsteroidPrefabs.Length)];
            Asteroid asteroid = Instantiate(prefab, position, rotation);

            asteroid.SetDimensions(asteroidSize);
            asteroid.SetTrajectory(Vector3.up * SpawnDistance);
        }
    }
}
 