using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System;

public class GameManager : MonoBehaviour
{
    private float respawnSpeed = 60.0f/30.0f;
    private int difficulty;
    private int maxDifficulty = 6;

    [Header("Segment Management")]
    public int spawnedSegments;
    public ControlSpawnedObstacles lastSegment;
    public GameObject nextSectionSegment;
    [Space]
    public GameObject[] segmentPrefabs;
    [Header("Character Management")]
    public GameObject[] characters;
    public PlayerController player;
    public SharkController shark;
    [Header("GUI Management")]
    public GameObject gameOverScreen;
    public GameObject finalScoreTextgo;
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI distanceTraveledText;
    public int finalScore = 0;
    [Header("Game Management")]
    public bool isGameOver = false;
    public bool isGameOverScreen = false;
    private System.Random rnd;
    private DistanceTracker distanceTracker;

    // Start is called before the first frame update
    void Start()
    {
        shark = GameManager.FindObjectOfType<SharkController>();
        distanceTracker = FindObjectOfType<DistanceTracker>();

        rnd = new System.Random();
        Time.timeScale = 1;
        isGameOver = false;
        spawnedSegments = 0;
        difficulty = 1;

        // Spawn the selected character
        int CharacterIndex = PlayerPrefs.GetInt("Character");
        characters[CharacterIndex].SetActive(true);
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

        // Spawn the first 5 segments
        for (int i = 0; i < 5; i++)
        {
            int index = rnd.Next(0, segmentPrefabs.Length);
            lastSegment = Instantiate(segmentPrefabs[index], new Vector3(0, 0, 160 + 60*i), segmentPrefabs[index].gameObject.transform.rotation).GetComponent<ControlSpawnedObstacles>();
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
            finalScore = player.score;
        }
        
        if(Input.anyKey && isGameOverScreen)
        {
            RestartGame();
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
            int index = rnd.Next(0, segmentPrefabs.Length);
            lastSegment = Instantiate(segmentPrefabs[index], new Vector3(0, 0, 400), segmentPrefabs[index].gameObject.transform.rotation).GetComponent<ControlSpawnedObstacles>();
            lastSegment.SpawnObstacles(difficulty);
        }
        spawnedSegments++;
    }

    public void GameOver()
    {
        isGameOver = true;
        CancelInvoke(nameof(SpawnSegment));
        shark.EndOfGameMovement();
        Invoke("GameOverScreen", 2.1f);
    }

    void GameOverScreen()
    {
        isGameOverScreen = true;
        gameOverScreen.SetActive(true);
        finalScoreText = finalScoreTextgo.GetComponent<TextMeshProUGUI>();
        finalScoreText.text = "Final Score: " + finalScore;
        distanceTraveledText.text = string.Format("Distance: {0:#0.0} m", distanceTracker.distanceUnit);
    }

    void RestartGame()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
