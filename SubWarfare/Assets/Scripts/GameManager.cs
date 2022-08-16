using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject enemySubPrefabs, 
                      enemyShipPrefabs, 
                      minePrefabs, 
                      supplyCratePrefabs, 
                      titleScreen, 
                      gameOverScreen, 
                      controlsScreen, 
                      player;
    public GameObject[] rockPrefabs;
    public Button restartButton, 
                  playerControlButton, 
                  exitButton;
    public TextMeshProUGUI scoreText;
    public bool isGameActive;
    public int enemyCount, 
               score, 
               gameDifficulty = 1;
    [SerializeField] private int waveCount = 1;
    //Game Bounds
    public float xBounds = 95,
                 zBounds = 95,
                 yLowerBounds = -50,
                 yUpperBounds = .5f;
    // Start the game, remove title screen, reset score, and adjust spawnRate based on difficulty
    public void StartGame(int difficulty)
    {
        isGameActive = true;
        Debug.Log("The Game has Started");
        titleScreen.SetActive(false);
        gameDifficulty = difficulty;
        score = 0;
        SpawnEnemyWave(waveCount, difficulty);
        GenerateSubmergedEnvironment(waveCount, difficulty);
        UpdateScore(0);
    }
    // Constant game tracker, generate new wave upon termination of all enemy sub, redistribute obstacles
    void Update()
    {
        enemyCount = GameObject.FindGameObjectsWithTag("EnemySubs").Length;
        GameObject[] obstacle = GameObject.FindGameObjectsWithTag("Rocks");
        if (enemyCount == 0 && isGameActive)
        {
            Destroy(GameObject.Find("Rocks"));
            GenerateSubmergedEnvironment(waveCount, gameDifficulty);
            SpawnEnemyWave(waveCount, gameDifficulty);
            Debug.Log("The wave " + waveCount + " has Started");
        }
    }
    //Generate random position in the general game area
    Vector3 GenerateSubmergedPosition()
    {
        float xPos = Random.Range(-xBounds, xBounds);
        float zPos = Random.Range(-zBounds, zBounds);
        float yPos = Random.Range(yLowerBounds, yUpperBounds);
        return new Vector3(xPos, yPos, zPos);
    }
    //Generate random position in the top of the game area
    Vector3 GenerateSufacePosition()
    {
        float xPos = Random.Range(-xBounds, xBounds);
        float zPos = Random.Range(-zBounds, zBounds);
        return new Vector3(xPos, 0, zPos);
    }
    //Generate random position in the bottom of the game area
    Vector3 GenerateGroundPosition()
    {
        float xPos = Random.Range(-xBounds, xBounds);
        float zPos = Random.Range(-zBounds, zBounds);
        return new Vector3(xPos, -50, zPos);
    }
    // Generate a wave of enemies
    void SpawnEnemyWave(int enemiesToSpawn, int difficulty)
    {
        // spawn surface ships and mines based on difficulty
        for (int i = 0; i < difficulty; i++)
        {   
            //new mine every wave
            Instantiate(minePrefabs, GenerateSubmergedPosition(), minePrefabs.transform.rotation);
            if (i % 2 == 0)//new ship on even waves only 
            { 
                Instantiate(enemyShipPrefabs, GenerateSufacePosition(), enemyShipPrefabs.transform.rotation); 
            }
        }
        Instantiate(supplyCratePrefabs, GenerateSubmergedPosition(), supplyCratePrefabs.transform.rotation);

        // Spawn number of enemy based on wave number
        for (int i = 0; i < enemiesToSpawn; i++)
        {
            GameObject enemy = Instantiate(enemySubPrefabs, GenerateSubmergedPosition(), enemySubPrefabs.transform.rotation);
            enemy.GetComponent<EnemySub>().speed += (waveCount * (0.1f * gameDifficulty));
        }
        waveCount++;
    }
    //Generate obstacles on bottom of game area
    void GenerateSubmergedEnvironment(int waveCount, int difficulty) 
    {
        int obstacleCount = 20;
        if (obstacleCount < 30) 
        { 
            obstacleCount += waveCount * difficulty; 
        }

        for (int i = 0;  i < obstacleCount; i++) 
        {
            int index = Random.Range(0, rockPrefabs.Length);
            Instantiate(rockPrefabs[index], GenerateGroundPosition(), transform.rotation);
           rockPrefabs[index].transform.localScale = new Vector3(Random.Range(1, 5), Random.Range(1, 5), Random.Range(1, 5));
        }
    }
    // Update and display score
    public void UpdateScore(int scoreToAdd)
    {
        score += scoreToAdd;
        scoreText.text = "Score: " + score;
    }
    // Display GameOver Screen
    public void GameOver()
    {
        gameOverScreen.SetActive(true);
        isGameActive = false;
    }
    // Restart Game
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void ControlsMenu()
    {
        titleScreen.SetActive(false);
        controlsScreen.SetActive(true);
    }
    public void MainMenu()
    {
        controlsScreen.SetActive(false);
        titleScreen.SetActive(true);
    }
}
