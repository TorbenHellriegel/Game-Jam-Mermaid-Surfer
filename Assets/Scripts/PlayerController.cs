using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerController : MonoBehaviour
{
    private Rigidbody playerRb;
    private bool gameOverTriggered;

    private PlayerSFX playerSFX;
    private AudioSource playerAudioSource;
    private GameManager gameManager;

    [Header("GUI Components")]
    public TextMeshProUGUI livesText;
    public TextMeshProUGUI scoreText;
    public Button jumpButton;
    public Button leftButton;
    public Button rightButton;
    [Space]
    public int lives;
    public int maxLives;
    public int score;
    [Header("Dive Variables")]
    public float speed;
    public float diveSpeed;
    private float diveTimer;
    public float timeBetweenDives = 1;
    [Header("Control Varaibles")]
    public int switchPosition;
    private Vector3[] position = new Vector3[] {new Vector3(-5, 0, 0), new Vector3(0, 0, 0), new Vector3(5, 0, 0)};
    public float floatSpeed;
    [Header("Camera Settings")]
    public float camSensitivity;
    [Header("Particle Systems")]
    public ParticleSystem rockCrash;
    public ParticleSystem pufferCrash;
    public ParticleSystem coinCollect;
    public ParticleSystem waterSplash;

    private void Awake()
    {
        // Was giving error in start so moved to awake
        playerRb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        playerRb = GetComponent<Rigidbody>();

        maxLives = 3;
        lives = maxLives;
        livesText.text = "Lives: " + lives + "/" + maxLives;
        score = 0;
        scoreText.text = "Score: " + score;
        gameOverTriggered = false;

        diveTimer = 0;
        switchPosition = 1;

        playerSFX = GetComponent<PlayerSFX>();
        playerAudioSource = GetComponent<AudioSource>();
        gameManager = FindObjectOfType<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        // Count to determine when the next dive is allowed
        diveTimer += Time.deltaTime;

        // Dive when the player presses space
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
        
        // Swich lanes left and right
        if ((Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)))
        {
            MoveLeft();
        }

        if ((Input.GetKeyDown(KeyCode.D) ||Input.GetKeyDown(KeyCode.RightArrow)))
        {
            MoveRight();   
        }

        // Check for gameOver
        if (lives < 1 && gameOverTriggered == false)
        {
            gameOverTriggered = true;
            GameOver();
        }
    }

    public void MoveLeft()
    {
        if (switchPosition > 0)
        {
            switchPosition--;
            playerRb.MovePosition(new Vector3(position[switchPosition].x, transform.position.y, transform.position.z));
        }
    }

    public void MoveRight()
    {
        if (switchPosition < 2)
        {
            switchPosition++;
            playerRb.MovePosition(new Vector3(position[switchPosition].x, transform.position.y, transform.position.z));
        }
    }

    public void Jump()
    {
        if (diveTimer > timeBetweenDives)
        {
            playerRb.AddForce(Vector3.down * diveSpeed, ForceMode.Impulse);
            Instantiate(waterSplash, transform.position,
            waterSplash.transform.rotation);
            playerSFX.PlayJumpAudio();
            diveTimer = 0;
        }

    }

    private void OnTriggerStay(Collider other)
    {
        // Bounce the player up if hes in the water
        if(other.CompareTag("Water"))
        {
            float updrift;
            if(transform.position.y > -0.5f)
            {
                updrift = -0.2f;
            }
            else
            {
                updrift = 1;
            }
            playerRb.AddForce(transform.up * floatSpeed * updrift);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Wall"))
        {
            playerAudioSource.PlayOneShot(playerSFX.rockCrash);
            Instantiate(rockCrash, transform.position,
            rockCrash.transform.rotation);
            LooseLives(-1);
        }
        if(other.CompareTag("Obstacle"))
        {
            playerAudioSource.PlayOneShot(playerSFX.pufferCrash);
            Instantiate(pufferCrash, transform.position,
            pufferCrash.transform.rotation);
            LooseLives(-1);
            Destroy(other.gameObject);
        }
        if(other.CompareTag("Coin"))
        {
            playerAudioSource.PlayOneShot(playerSFX.coinCollected);
            Instantiate(coinCollect, transform.position,
            coinCollect.transform.rotation);
            GainScore(10);
            Destroy(other.gameObject);
        }
        if(other.CompareTag("Section"))
        {
            GainLives(1);
        }
    }

    private void GainLives(int amount)
    {
        if(lives == maxLives)
        {
            maxLives++;
        }
        else
        {
            lives++;
        }
        livesText.text = "Lives: " + lives + "/" + maxLives;
    }

    private void LooseLives(int amount)
    {
        lives = Mathf.Min(lives + amount, maxLives);
        livesText.text = "Lives: " + lives + "/" + maxLives;
    }

    private void GainScore(int amount)
    {
        score += Mathf.RoundToInt(amount * Time.timeScale);
        scoreText.text = "Score: " + score;
    }

    private void GameOver()
    {
        // Do game over stuff
        Destroy(gameObject, 0.1f);
        gameManager.GameOver();
    }
}
