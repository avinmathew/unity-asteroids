using UnityEngine;

public class Warp : MonoBehaviour
{
    private Rigidbody2D _rigidbody;
    private Bounds _screenBounds;
    private IWarpable _warpable;

    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();

        // Convert the screen-space coordinates to world-space coordinates and encapsulate those
        // points in the bounds.
        // We encapsulate two points: one for the top-left corner (min) and one for the
        // bottom-right corner (max).
        _screenBounds = new Bounds();
        _screenBounds.Encapsulate(Camera.main.ScreenToWorldPoint(Vector3.zero));
        _screenBounds.Encapsulate(Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0f)));

        _warpable = GetComponent<IWarpable>();
    }

    void FixedUpdate()
    {
        // Check if player is beyond the bounds and change position to other side
        // Offset screen bounds by 0.5 because the position of the player is in the centre so
        // this ensures player is entirely off the screen before warping to the other side
        //
        // Remove trails when warping otherwise trails end up across the screen. For some reason
        // TrailRenderer.Clear() doesn't work, so disable and then enable
        if (_rigidbody.position.x > _screenBounds.max.x + 0.5f)
        {
            _warpable.OnWarpEnter();
            _rigidbody.position = new Vector2(_screenBounds.min.x - 0.5f, _rigidbody.position.y);
            _warpable.OnWarpExit();
        }
        else if (_rigidbody.position.x < _screenBounds.min.x - 0.5f)
        {
            _warpable.OnWarpEnter();
            _rigidbody.position = new Vector2(_screenBounds.max.x + 0.5f, _rigidbody.position.y);
            _warpable.OnWarpExit();
        }
        else if (_rigidbody.position.y > _screenBounds.max.y + 0.5f)
        {
            _warpable.OnWarpEnter();
            _rigidbody.position = new Vector2(_rigidbody.position.x, _screenBounds.min.y - 0.5f);
            _warpable.OnWarpExit();
        }
        else if (_rigidbody.position.y < _screenBounds.min.y - 0.5f)
        {
            _warpable.OnWarpEnter();
            _rigidbody.position = new Vector2(_rigidbody.position.x, _screenBounds.max.y + 0.5f);
            _warpable.OnWarpExit();
        }
    }
}
