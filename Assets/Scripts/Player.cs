using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    public AudioSource AudioSource;
    public AudioClip BulletShootClip;
    public AudioClip ThrustClip;
    public AudioClip PlayerDiedClip;

    public Bullet BulletPrefab;
    public float ThurstSpeed = 1f;
    public float TurnSpeed = 2f;

    public ParticleSystem Explosion;
    public TrailRenderer TrailRenderer;
    public SpriteRenderer Shield;

    private GameManager _gameManager;
    private Rigidbody2D _rigidbody;
    
    private Bounds _screenBounds;

    private float _thrusting;
    private float _turnDirection;
    private int _invulnerabilityTime = 3; // seconds


    public void Respawn(bool showShield)
    {
        // Reset position to centre of screen
        transform.position = Vector3.zero;
        
        StartCoroutine(Invulnerable(showShield, _invulnerabilityTime, _invulnerabilityTime * 2));
    }

    // Ignore collisions with asteroids and show flashing shield
    IEnumerator Invulnerable(bool showShield, float flashDuration, int numberOfFlashes)
    {
        gameObject.layer = LayerMask.NameToLayer("Ignore Collisions");
        Shield.enabled = showShield;

        Color shieldColor = Shield.color;
        Color flashColor = new Color(shieldColor.r, shieldColor.g, shieldColor.b, 0);
        float elapsedFlashTime = 0;
        float elapsedFlashPercentage;

        while (elapsedFlashTime < flashDuration)
        {
            elapsedFlashTime += Time.deltaTime;
            elapsedFlashPercentage = elapsedFlashTime / flashDuration;

            if (elapsedFlashPercentage > 1)
            {
                elapsedFlashPercentage = 1;
            }

            float pingPongPercentage = Mathf.PingPong(elapsedFlashPercentage * 2 * numberOfFlashes, 1);
            Shield.color = Color.Lerp(shieldColor, flashColor, pingPongPercentage);

            yield return null;
        }
        gameObject.layer = LayerMask.NameToLayer("Player");
        Shield.enabled = false;
    }

    void Awake()
    {
        _gameManager = FindAnyObjectByType<GameManager>();
        _rigidbody = GetComponent<Rigidbody2D>();

        // Convert the screen-space coordinates to world-space coordinates and encapsulate those
        // points in the bounds.
        // We encapsulate two points: one for the top-left corner (min) and one for the
        // bottom-right corner (max).
        _screenBounds = new Bounds();
        _screenBounds.Encapsulate(Camera.main.ScreenToWorldPoint(Vector3.zero));
        _screenBounds.Encapsulate(Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0f)));
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            _thrusting = 1;
        }
        else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            _thrusting = -1;
        }
        else
        {
            _thrusting = 0;
        }

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            _turnDirection = 1;
        }
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            _turnDirection = -1;
        }
        else
        {
            _turnDirection = 0;
        }

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }

    void FixedUpdate()
    {
        bool moving = false;
        if (_thrusting == 1)
        {
            _rigidbody.AddForce(transform.up * ThurstSpeed);
            moving = true;
            
        }
        else if (_thrusting == -1)
        {
            _rigidbody.AddForce(-transform.up * ThurstSpeed);
            moving = true;
        }

        if (_turnDirection != 0) {
            _rigidbody.AddTorque(_turnDirection * TurnSpeed);
            moving = true;
        }

        if (moving && !AudioSource.isPlaying)
        {
            AudioSource.PlayOneShot(ThrustClip);
        }

        // Check if player is beyond the bounds and change position to other side
        // Offset screen bounds by 0.5 because the position of the player is in the centre so
        // this ensures player is entirely off the screen before warping to the other side
        //
        // Remove trails when warping otherwise trails end up across the screen. For some reason
        // TrailRenderer.Clear() doesn't work, so disable and then enable
        if (_rigidbody.position.x > _screenBounds.max.x + 0.5f)
        {
            TrailRenderer.enabled = false;
            _rigidbody.position = new Vector2(_screenBounds.min.x - 0.5f, _rigidbody.position.y);
            StartCoroutine(EnableTrails());
        }
        else if (_rigidbody.position.x < _screenBounds.min.x - 0.5f)
        {
            TrailRenderer.enabled = false;
            _rigidbody.position = new Vector2(_screenBounds.max.x + 0.5f, _rigidbody.position.y);
            StartCoroutine(EnableTrails());
        }
        else if (_rigidbody.position.y > _screenBounds.max.y + 0.5f)
        {
            TrailRenderer.enabled = false;
            _rigidbody.position = new Vector2(_rigidbody.position.x, _screenBounds.min.y - 0.5f);
            StartCoroutine(EnableTrails());
        }
        else if (_rigidbody.position.y < _screenBounds.min.y - 0.5f)
        {
            TrailRenderer.enabled = false;
            _rigidbody.position = new Vector2(_rigidbody.position.x, _screenBounds.max.y + 0.5f);
            StartCoroutine(EnableTrails());
        }
    }

    IEnumerator EnableTrails()
    {
        yield return new WaitForSeconds(TrailRenderer.time);
        TrailRenderer.enabled = true;
    }

    void Shoot()
    {
        Bullet bullet = Instantiate(BulletPrefab, transform.position, transform.rotation);
        bullet.Project(transform.up);
        AudioSource.PlayOneShot(BulletShootClip);
    }

    void Explode()
    {
        Explosion.transform.position = transform.position;
        Explosion.Play();
        AudioSource.PlayOneShot(PlayerDiedClip);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Asteroid")
        {
            _rigidbody.linearVelocity = Vector3.zero;
            _rigidbody.angularVelocity = 0f;

            Explode();
            _gameManager.PlayerDied();
        }
    }
}
