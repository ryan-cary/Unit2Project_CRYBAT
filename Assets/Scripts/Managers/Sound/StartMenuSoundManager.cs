using UnityEngine;
using UnityEngine.UI;

public class StartMenuSoundManager : MonoBehaviour
{
    [Header("Music")]
    [SerializeField] private AudioClip startMenuClip;
    [SerializeField, Range(0f, 1f)] private float startMenuVolume = 0.4f;

    [Header("UI")]
    [SerializeField] private AudioClip buttonHoverClip;
    [SerializeField, Range(0f, 1f)] private float buttonHoverVolume = 0.7f;
    [SerializeField] private AudioClip buttonClickClip;
    [SerializeField, Range(0f, 1f)] private float buttonClickVolume = 0.7f;

    SoundManager soundManager;

    void Awake()
    {
        soundManager = GetComponentInParent<SoundManager>();
    }

    void Start()
    {
        PlayStartMenuMusic();
        BindButtonSounds();
    }

    public void PlayStartMenuMusic()
    {
        soundManager?.PlayMusic(startMenuClip, startMenuVolume, true);
    }

    public void PlayButtonHover()
    {
        soundManager?.PlaySfx(buttonHoverClip, buttonHoverVolume);
    }

    public void PlayButtonClick()
    {
        soundManager?.PlaySfx(buttonClickClip, buttonClickVolume);
    }

    void BindButtonSounds()
    {
        Button[] buttons = FindObjectsByType<Button>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (Button button in buttons)
        {
            if (button.gameObject.name == "SoundToggleButton")
                continue;

            if (button.GetComponent<UIButtonSound>() == null)
                button.gameObject.AddComponent<UIButtonSound>();
        }
    }
}
