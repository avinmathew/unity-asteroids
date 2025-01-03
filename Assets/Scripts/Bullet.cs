using UnityEngine;

public class Bullet : MonoBehaviour, IWarpable
{
    public float Speed = 500.0f;
    public float Lifetime = 0.6f; // in seconds

    private Rigidbody2D _rigidbody;

    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    public void Project(Vector2 direction)
    {
        _rigidbody.AddForce(direction * Speed);

        Destroy(this.gameObject, this.Lifetime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(this.gameObject);
    }

    public void OnWarpEnter()
    {

    }

    public void OnWarpExit()
    {

    }
}
