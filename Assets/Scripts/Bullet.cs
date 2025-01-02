using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float Speed = 500.0f;
    public float Lifetime = 1.0f; // in seconds

    private Rigidbody2D _rigidBody;

    void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
    }

    public void Project(Vector2 direction)
    {
        _rigidBody.AddForce(direction * Speed);

        Destroy(this.gameObject, this.Lifetime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(this.gameObject);
    }
}
