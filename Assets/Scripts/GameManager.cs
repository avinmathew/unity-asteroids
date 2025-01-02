using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public UnityEngine.UI.Image Life1;
    public UnityEngine.UI.Image Life2;
    public UnityEngine.UI.Image Life3;
    public TextMeshProUGUI MoveInstructions;
    public TextMeshProUGUI ShootInstructions;
    public TextMeshProUGUI ScoreText;
    public TextMeshProUGUI GameOverText;
    public TextMeshProUGUI PlayAgainText;

    public Player Player;
    
    public int Lives = 1;
    public float RespawnTime = 3.0f; // seconds
    public int Score = 0;

    private bool ShowMoveInstructions = true;
    private bool ShowShootInstructions = true;
    private bool GamePlaying = false;

    void Start()
    {
        RestartGame();
    }

    public void AsteroidDestroyed(Asteroid asteroid)
    {
        if (asteroid.Size < 0.75f)
        {
            Score += 100;
        }
        else if (asteroid.Size < 1f)
        {
            Score += 50;
        }
        else if (asteroid.Size < 1.25f)
        {
            Score += 25;
        }
        else if (asteroid.Size < 1.5f)
        {
            Score += 10;
        }
        ScoreText.text = Score.ToString();
    }

    public void PlayerDied()
    {
        Player.gameObject.SetActive(false);

        Lives--;
        if (Lives == 2)
        {
            Life3.enabled = false;
        }
        else if (Lives == 1)
        {
            Life2.enabled = false;
        }
        else if (Lives == 0)
        {
            Life1.enabled = false;
        }

        if (Lives == 0)
        {
            GameOver();
        }
        else
        {
            Invoke(nameof(Respawn), RespawnTime);
        }
    }

    void Respawn()
    {
        Player.gameObject.SetActive(true);
        Player.Respawn(showShield: true);
    }

    void GameOver()
    {
        GameOverText.enabled = true;
        PlayAgainText.enabled = true;
        GamePlaying = false;
    }

    private void Update()
    {
        if (ShowMoveInstructions)
        {
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D)
                || Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.RightArrow))
            {
                ShowMoveInstructions = false;
                MoveInstructions.enabled = false;
            }
        }

        if (ShowShootInstructions)
        {
            if (Input.GetKey(KeyCode.Space) || Input.GetMouseButtonDown(0))
            {
                ShowShootInstructions = false;
                ShootInstructions.enabled = false;
            }
        }

        if (Input.GetKey(KeyCode.Escape))
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
            Application.Quit();
        }

        if (GamePlaying == false)
        {
            if (Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter))
            {
                RestartGame();
            }
        }
    }

    void RestartGame()
    {
        Lives = 3;
        Life1.enabled = true;
        Life2.enabled = true;
        Life3.enabled = true;
        Score = 0;
        ScoreText.text = Score.ToString();
        GameOverText.enabled = false;
        PlayAgainText.enabled = false;

        // Clear all asteroids
        foreach (var a in FindObjectsByType<Asteroid>(FindObjectsSortMode.None))
        {
            Destroy(a.gameObject);
        }

        GamePlaying = true;
        Player.Respawn(showShield: false);
    }
}
