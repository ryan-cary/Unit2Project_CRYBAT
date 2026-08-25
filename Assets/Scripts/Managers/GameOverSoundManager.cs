using UnityEngine;

public class GameOverSoundManager : MonoBehaviour
{
    [Header("Music")]
    [SerializeField] private AudioClip gameOverClip;
    [SerializeField, Range(0f, 1f)] private float gameOverVolume = 0.4f;

    SoundManager soundManager;

    void Awake()
    {
        soundManager = GetComponentInParent<SoundManager>();
    }

    void OnEnable()
    {
        if (GameManager.GetInstance() != null)
            GameManager.GetInstance().OnGameEnd.AddListener(PlayGameOverMusic);
    }

    void OnDisable()
    {
        if (GameManager.GetInstance() != null)
            GameManager.GetInstance().OnGameEnd.RemoveListener(PlayGameOverMusic);
    }

    public void PlayGameOverMusic()
    {
        soundManager?.PlayMusic(gameOverClip, gameOverVolume, false);
    }
}
