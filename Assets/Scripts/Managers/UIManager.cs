using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{

    [SerializeField] private GameObject menuCanvas;
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private GameObject gameStats;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text highScoreText;
    [SerializeField] private TMP_Text gunPowerUpTimerText;
    [SerializeField] private GameObject nukePowerUpPanel;
    [SerializeField] private GameObject nukeSprite;

    bool isSubscribedToGameEvents = false;
    bool isSubscribedToPlayerEvents = false;

    Vector2 gunPowerUpSpacing = new Vector2(0f, 40f);

    private void OnEnable()
    {
        if (isSubscribedToGameEvents == false)
            SubscribeToGameEvents();
    }

    private void OnDisable()
    {
        UnsubscribeToAllEvents();
        isSubscribedToGameEvents = false;
    }

    private void OnGameStart()
    {
        menuCanvas.SetActive(false);
        gameOverScreen.SetActive(false);
        gameStats.SetActive(true);
    }

    private void OnGameEnd()
    {
        gameOverScreen?.SetActive(true);
        gameStats.SetActive(false);
    }

    void Update()
    {
        if (!isSubscribedToPlayerEvents)
        {
            SubscribeToPlayerEvents();
        }
    }

    void SubscribeToGameEvents()
    {
        GameManager.GetInstance().GetScoreManager().OnScoreUpdated.AddListener(UpdateScore);
        GameManager.GetInstance().GetScoreManager().OnHighScoreUpdated.AddListener(UpdateHighScore);

        GameManager.GetInstance().OnGameStart.AddListener(OnGameStart);
        GameManager.GetInstance().OnGameEnd.AddListener(OnGameEnd);

        isSubscribedToGameEvents = true;
    }

    void UnsubscribeToAllEvents()
    {
        GameManager.GetInstance().GetScoreManager().OnScoreUpdated.RemoveListener(UpdateScore);
        GameManager.GetInstance().GetScoreManager().OnHighScoreUpdated.RemoveListener(UpdateHighScore);

        GameManager.GetInstance().OnGameStart.RemoveListener(OnGameStart);
        GameManager.GetInstance().OnGameEnd.RemoveListener(OnGameEnd);

        isSubscribedToGameEvents = false;
    }

    public void SubscribeToPlayerEvents()
    {
        if (GameManager.GetInstance().GetPlayer() != null)
        {
            SubscribePlayerHealth();
            SubscribePlayerPowerUpTimer();
            SubscribePlayerNukeEvents();
            GameManager.GetInstance().GetPlayer().OnDefeated.AddListener(UnsubscribeToPlayerEvents);

            isSubscribedToPlayerEvents = true;
        }
    }

    public void UnsubscribeToPlayerEvents()
    {
        if (GameManager.GetInstance().GetPlayer() != null)
        {
            GameManager.GetInstance().GetPlayer().health.OnHealthUpdate -= UpdateHealth;
            GameManager.GetInstance().GetPlayer().GetPickupBehaviorController().GetGunPowerUpBehavior().OnPowerUpTimerChange -= UpdateGunPowerUpTimerText;
            GameManager.GetInstance().GetPlayer().GetPickupBehaviorController().GetNukeBehavior().OnCollectNuke -= IncrementNukeList;
            GameManager.GetInstance().GetPlayer().GetPickupBehaviorController().GetNukeBehavior().OnUseNuke -= DecrementNukeList;
        }
        isSubscribedToPlayerEvents = false;
    }

    public void SubscribePlayerHealth()
    {
        GameManager.GetInstance().GetPlayer().health.OnHealthUpdate += UpdateHealth;
    }

    public void SubscribePlayerPowerUpTimer()
    {
        GameManager.GetInstance().GetPlayer().GetPickupBehaviorController().GetGunPowerUpBehavior().OnPowerUpTimerChange += UpdateGunPowerUpTimerText;
    }

    public void SubscribePlayerNukeEvents()
    {
        GameManager.GetInstance().GetPlayer().GetPickupBehaviorController().GetNukeBehavior().OnCollectNuke += IncrementNukeList;
        GameManager.GetInstance().GetPlayer().GetPickupBehaviorController().GetNukeBehavior().OnUseNuke += DecrementNukeList;
    }

    void UpdateHealth(float health)
    {
        healthText.text = health.ToString("0.0");
    }

    void UpdateScore(int score)
    {
        scoreText.text = score.ToString();
    }

    void UpdateHighScore(int highScore)
    {
        highScoreText.text = $"High Score: {highScore}";
    }

    void UpdateGunPowerUpTimerText(bool isActive, float time, Vector2 position)
    {
        Debug.Log(gunPowerUpTimerText);

        if (isActive)
        {
            gunPowerUpTimerText.gameObject.SetActive(true);
            gunPowerUpTimerText.gameObject.transform.position = position + gunPowerUpSpacing;
            gunPowerUpTimerText.text = time.ToString("0.0");
        } 
        else
        {
            gunPowerUpTimerText.gameObject.SetActive(false);
        }
    }

    void IncrementNukeList()
    {
        Instantiate(nukeSprite, nukePowerUpPanel.transform);
    }

    void DecrementNukeList()
    {
        int lastNukeIndex = nukePowerUpPanel.transform.childCount - 1;

        if (lastNukeIndex >= 0)
        {
            Destroy(nukePowerUpPanel.transform.GetChild(lastNukeIndex).gameObject);
        }
    }
}
