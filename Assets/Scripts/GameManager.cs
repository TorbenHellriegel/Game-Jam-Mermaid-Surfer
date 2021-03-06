using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System;

public class GameManager : MonoBehaviour
{
    [Header("Segment Management")]
    public int spawnedSegments;
    public ControlSpawnedObstacles lastSegment;
    public GameObject nextSectionSegment;
    public GameObject[] segmentPrefab;
    private float respawnSpeed = 60.0f / 30.0f;
    [Header("Character Management")]
    public GameObject[] characters;
    public PlayerController player;
    public SharkController shark;
    [Header("GUI Management")]
    public GameObject gameOverScreen;
    public GameObject finalScoreTextgo;
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI distanceTraveledText;
    public GameObject pauseMenu;
    public GameObject playerStatsDisplay;
    public int finalScore = 0;
    [Header("Game Management")]
    public bool isGameOver = false;
    public bool isGameOverScreen = false;
    public float lastTimeScale;
    private System.Random rnd;
    private DistanceTracker distanceTracker;
    public int difficulty;
    private int maxDifficulty = 6;

    // Start is called before the first frame update
    void Start()
    {
        shark = GameManager.FindObjectOfType<SharkController>();
        distanceTracker = FindObjectOfType<DistanceTracker>();

        rnd = new System.Random();
        isGameOver = false;
        spawnedSegments = 0;

        switch (PlayerPrefs.GetString("Difficulty", "Medium"))
        {
            case "Easy":
                Time.timeScale = 0.9f;
                difficulty = 1;
                break;
            case "Medium":
                Time.timeScale = 1;
                difficulty = 3;
                break;
            case "Hard":
                Time.timeScale = 1.2f;
                difficulty = 5;
                break;
            default:
                Time.timeScale = 1;
                difficulty = 3;
                break;
        }

        // Spawn the selected character
        int CharacterIndex = PlayerPrefs.GetInt("Character");
        characters[CharacterIndex].SetActive(true);
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

        // Spawn the first 5 segments
        for (int i = 0; i < 5; i++)
        {
            int index = rnd.Next(0, segmentPrefab.Length);
            lastSegment = Instantiate(segmentPrefab[index], new Vector3(0, 0, 160 + 60*i), segmentPrefab[index].gameObject.transform.rotation).GetComponent<ControlSpawnedObstacles>();
            lastSegment.SpawnObstacles(difficulty);
            spawnedSegments++;
        }
        // Spawn a new segment every second
        InvokeRepeating("SpawnSegment", respawnSpeed, respawnSpeed);
    }

    // Update is called once per frame
    void Update()
    {
        if(!isGameOver)
        {
            finalScore = FindObjectOfType<ScoreTracker>().score;
        }
        
    }

    public void IncreaseDifficulty()
    {
        if(difficulty < maxDifficulty)
        {
            difficulty++;
        }
        else
        {
            Time.timeScale += 0.02f;
        }
    }

    // Spawns a random segment
    void SpawnSegment()
    {
        if (spawnedSegments % 10 == 0)
        {
            Instantiate(nextSectionSegment, new Vector3(0, 0, 400), nextSectionSegment.gameObject.transform.rotation);
            IncreaseDifficulty();
        }
        else
        {
            int index = rnd.Next(0, segmentPrefab.Length);
            lastSegment = Instantiate(segmentPrefab[index], new Vector3(0, 0, 400), segmentPrefab[index].gameObject.transform.rotation).GetComponent<ControlSpawnedObstacles>();
            lastSegment.SpawnObstacles(difficulty);
        }
        spawnedSegments++;
    }

    public void GameOver()
    {
        CancelInvoke(nameof(SpawnSegment));
        shark.EndOfGameMovement();
        Invoke(nameof(GameOverScreen), 2.3f);
    }

    void GameOverScreen()
    {
        isGameOverScreen = true;
        gameOverScreen.SetActive(true);
        finalScoreText = finalScoreTextgo.GetComponent<TextMeshProUGUI>();
        finalScoreText.text = "Final Score: " + finalScore;
        distanceTraveledText.text = string.Format("Distance: {0:#0.0} m", distanceTracker.distanceUnit);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void PauseGame()
    {
        lastTimeScale = Time.timeScale;
        Time.timeScale = 0;
        pauseMenu.SetActive(true);
    }

    public void ContinueGame()
    {
        Time.timeScale = lastTimeScale;
        pauseMenu.SetActive(false);
    }
}
