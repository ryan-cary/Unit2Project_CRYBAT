using TMPro;
using UnityEngine;

public class HighScoreDisplay : MonoBehaviour
{
    [SerializeField] TMP_Text label;
    [SerializeField] string format = "High Score: {0}";

    ScoreManager scoreManager;

    void Awake()
    {
        if (label == null)
            label = GetComponent<TMP_Text>();
    }

    void OnEnable()
    {
        SetHighScore(PlayerPrefs.GetInt("HighScore", 0));
        TrySubscribe();
    }

    void Start()
    {
        TrySubscribe();
    }

    void OnDisable()
    {
        if (scoreManager != null)
        {
            scoreManager.OnHighScoreUpdated.RemoveListener(SetHighScore);
            scoreManager = null;
        }
    }

    void TrySubscribe()
    {
        if (scoreManager != null)
            return;

        scoreManager = GameManager.GetInstance()?.GetScoreManager();
        if (scoreManager == null)
            return;

        scoreManager.OnHighScoreUpdated.AddListener(SetHighScore);
    }

    void SetHighScore(int highScore)
    {
        if (label != null)
            label.text = string.Format(format, highScore);
    }
}
