using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private float respawnSpeed = 60.0f/30.0f;

    public GameObject[] segmentPrefabs;
    public GameObject[] characters;
    public SharkController shark;

    public bool isGameOver = false;

    // Start is called before the first frame update
    void Start()
    {
        shark = GameManager.FindObjectOfType<SharkController>();

        isGameOver = false;
        // Spawn the selected character
        int CharacterIndex = PlayerPrefs.GetInt("Character");
        characters[CharacterIndex].SetActive(true);

        // Spawn the first 5 segments
        for (int i = 0; i < 5; i++)
        {
            int index = Random.Range(0, segmentPrefabs.Length);
            Instantiate(segmentPrefabs[index], new Vector3(0, 0, 160 + 60*i), segmentPrefabs[index].gameObject.transform.rotation);
        }
        // Spawn a new segment every second
        InvokeRepeating("SpawnSegment", respawnSpeed, respawnSpeed);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Spawns a random segment
    void SpawnSegment()
    {
        int index = Random.Range(0, segmentPrefabs.Length);
        Instantiate(segmentPrefabs[index], new Vector3(0, 0, 400), segmentPrefabs[index].gameObject.transform.rotation);
    }

    public void GameOver()
    {
        isGameOver = true;
        CancelInvoke(nameof(SpawnSegment));
        shark.EndOfGameMovement();
        StartCoroutine(RestartGame());
    }

    IEnumerator RestartGame()
    {
        yield return new WaitForSeconds(5.0f);
        SceneManager.LoadScene("MainMenu");
    }
}
