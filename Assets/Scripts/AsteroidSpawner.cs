using System.Drawing;
using UnityEngine;

public class AsteroidSpawner : MonoBehaviour
{
    public Asteroid[] AsteroidPrefabs;
    public float SpawnRate = 1f;
    public float SpawnDistance = 15f;
    public float trajectoryVariance = 15f;
    public int SpawnAmount = 1;
    void Start()
    {
        InvokeRepeating(nameof(Spawn), SpawnRate, SpawnRate);
    }

    void Spawn()
    {
        for (int i = 0; i < SpawnAmount; i++)
        {
            // Create a random asteroid with random size and trajectory
            Vector3 direction = Random.insideUnitCircle.normalized * SpawnDistance;
            Vector3 position = transform.position + direction;
            
            float variance = Random.Range(-trajectoryVariance, trajectoryVariance);
            Quaternion rotation = Quaternion.AngleAxis(variance, Vector3.forward);
            
            Asteroid prefab = AsteroidPrefabs[Random.Range(0, AsteroidPrefabs.Length)];
            Asteroid asteroid = Instantiate(prefab, position, rotation);

            float size = Random.Range(asteroid.MinSize, asteroid.MaxSize);
            asteroid.SetDimensions(size);

            asteroid.SetTrajectory(rotation * -direction);
        }
    }
}
