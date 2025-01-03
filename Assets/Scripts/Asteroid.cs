using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    public AudioClip BigAsteroidDestroyedClip;
    public AudioClip SmallAsteroidDestroyedClip;

    public float Size = 1f;
    public float MinSize = 0.5f;
    public float MaxSize = 1.5f;
    public float Speed = 5f;

    private Rigidbody2D _rigidbody;
    private GameManager _gameManager;
    private ParticleSystem _explosion;
    private AudioSource _audioSource;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _gameManager = FindAnyObjectByType<GameManager>();
        _explosion = FindObjectsByType<ParticleSystem>(FindObjectsSortMode.None).First(x => x.name == "Asteroid Explosion");
        _audioSource = FindAnyObjectByType<AudioSource>();
    }

    public void SetDimensions(float size)
    {
        Size = size;

        // Rotate and resize sprite so it looks unique
        transform.eulerAngles = new Vector3(0f, 0f, Random.value * 360f);
        transform.localScale = new Vector3(size, size);

        _rigidbody.mass = size;
    }

    public void SetTrajectory(Vector3 direction)
    {
        _rigidbody.AddForce(direction * Speed);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Bullet")
        {
            AudioClip asteriodDestroyedClip = SmallAsteroidDestroyedClip;
            // If it can be split and not be too small
            // Assumes we always split into two
            if ((Size * 0.5f) >= MinSize)
            {
                Split();
                Split();
                asteriodDestroyedClip = BigAsteroidDestroyedClip;
            }
            Explode(asteriodDestroyedClip);
            _gameManager.AsteroidDestroyed(this);
            
            Destroy(gameObject);
        }
    }

    // Destroy object when moving off screen
    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }

    void Explode(AudioClip asteriodDestroyedClip)
    {
        _explosion.transform.position = transform.position;
        _explosion.Play();
        _audioSource.PlayOneShot(asteriodDestroyedClip);
    }

    void Split()
    {
        Vector2 position = transform.position;
        position += Random.insideUnitCircle * 0.5f;

        Asteroid split = Instantiate(this, position, transform.rotation);
        float splitSize = this.Size * 0.5f;
        split.SetDimensions(splitSize);
        split.SetTrajectory(Random.insideUnitCircle.normalized * Speed);
    }
}
