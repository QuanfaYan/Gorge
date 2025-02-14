using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    
    [SerializeField] GameObject[] foodPrefabs;
    [SerializeField] float spawnDelay = 1;
    [SerializeField] float spawnInterval = 1;
    [SerializeField] float spawnPosY = 20.0f;
    [SerializeField] float spawnPosXRange = 10.0f;
    [SerializeField] float spawnForceY = -0.5f;
    [SerializeField] float spawnForceXRangeMin = 0.5f;
    [SerializeField] float spawnForceXRangeMax = 1.0f;
    [SerializeField] float spawnForcePower = 1.0f;
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI livesText;
    [SerializeField] TextMeshProUGUI highScoreText;
    [SerializeField] TextMeshProUGUI capacityText;
    [SerializeField] Slider capacity;
    [SerializeField] Image capacityFillImg;
    [SerializeField] int score = 0;
    [SerializeField] int lives = 3;
    [SerializeField] bool isPaused = false;
    [SerializeField] float specialEffectMultiplier = 1/3f;
    [SerializeField] int highScore = 0;
    Color32 sliderGreen = new Color32(129, 220, 50, 255);
    Color32 sliderOrange = new Color32(220, 125, 33, 255);
    Color32 sliderRed = new Color32(225, 24, 0, 255);
    AudioSource audioPlayer;
    AudioSource bgm;
    PlayerControler playControler;
    public GameObject gameoverMenu;
    public GameObject pauseMenu;

    // Start is called before the first frame update
    void Start()
    {
        GameStart();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    void GameStart()
    {
        highScoreText = GameObject.Find("HighScore").GetComponent<TextMeshProUGUI>();
        scoreText = GameObject.Find("Scores").GetComponent<TextMeshProUGUI>();
        livesText = GameObject.Find("Lives").GetComponent<TextMeshProUGUI>();
        capacityText = GameObject.Find("Capacity Text").GetComponent<TextMeshProUGUI>();
        capacity = GameObject.Find("Capacity").GetComponent<Slider>();
        capacityFillImg = capacity.fillRect.GetComponent<Image>();
        playControler = FindAnyObjectByType<PlayerControler>();
        audioPlayer = GetComponent<AudioSource>();
        bgm = GameObject.Find("Background").GetComponent<AudioSource>();
        highScore = PlayerPrefs.GetInt("highscore", 0);
        highScoreText.SetText(highScore.ToString());
        bgm.Play();
        InvokeRepeating("SpawnFood", spawnDelay, spawnInterval);
    }

    void SpawnFood()
    {   
        int i = Random.Range(0, foodPrefabs.Length * (int)(1f / specialEffectMultiplier));
        i = i >= foodPrefabs.Length ? 0 : i;
        GameObject tempFood = Instantiate(foodPrefabs[i], GetRandomPos(), foodPrefabs[i].transform.rotation);
        tempFood.GetComponent<Rigidbody2D>().AddForce(GetRandomForce().normalized * spawnForcePower, ForceMode2D.Impulse);
    }

    Vector3 GetRandomPos()
    {
        return new Vector3(Random.Range(-spawnPosXRange, spawnPosXRange), spawnPosY, 0);
    }

    Vector2 GetRandomForce()
    {
        float randomX = Random.Range(spawnForceXRangeMin, spawnForceXRangeMax) * (Random.value < 0.5f ? -1 : 1);
        return new Vector2(randomX, spawnForceY);
    }

    public void UpdateScore(int mod)
    {
        score += mod;
        scoreText.SetText("Score: " + score);
        if(score > highScore)
        {
            highScore = score;
            highScoreText.SetText(highScore.ToString());
            PlayerPrefs.SetInt("highscore",score);
            PlayerPrefs.Save();
        }
    }

    public void UpdateLife(int mod)
    {
        lives += mod;
        livesText.SetText("Lives: " + lives);
        if(lives <= 0)
        {
            GameOver();
        }
    }

    public void UpdateCapacity(int val)
    {
        capacity.value = val;
        if(val < playControler.capacity/2)
        {
            capacityFillImg.color = sliderGreen;
        }
        else if(val < playControler.capacity)
        {
            capacityFillImg.color = sliderOrange;
        }
        else
        {
            capacityFillImg.color = sliderRed;
        }
        capacityText.SetText("capacity:" + val + "/" + playControler.capacity);
    }

    public void GameOver()
    {
        Debug.Log("GameOver!");
        gameoverMenu.SetActive(true);
        Time.timeScale = 0;
        audioPlayer.Play();
        bgm.Pause();
    }

    public void RestartGame()
    {
        gameoverMenu.SetActive(false);
        Time.timeScale = 1;
        SceneManager.LoadScene("GameView");
    }

    public void Resume()
    {
        Time.timeScale = 1;
        pauseMenu.SetActive(false);
        bgm.Play();
        isPaused = false;
    }

    public void Pause()
    {
        Time.timeScale = 0;
        pauseMenu.SetActive(true);
        bgm.Pause();
        isPaused = true;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
