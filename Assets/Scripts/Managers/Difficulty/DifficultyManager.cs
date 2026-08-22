using UnityEngine;

public enum Difficulty
{
    Easy,
    Normal,
    Hard,
    Hardcore
}

public class DifficultyManager : MonoBehaviour
{
    [SerializeField] DifficultySetting difficulty_EASY;
    [SerializeField] DifficultySetting difficulty_NORMAL;
    [SerializeField] DifficultySetting difficulty_HARD;
    [SerializeField] DifficultySetting difficulty_HARDCORE;

    public DifficultySetting currentDifficultySetting;
    public Difficulty currentDifficulty { get; private set; } = Difficulty.Normal;

    private float _startingDifficultyModifier;
    private float _currentDifficultyModifier;

    void Awake() 
    { ApplySettings(); }

    public void SetDifficulty(Difficulty difficulty)
    {
        currentDifficulty = difficulty;
        ApplySettings();
    }

    private void ApplySettings()
    {
        switch (currentDifficulty)
        {
            case Difficulty.Easy:
                currentDifficultySettings = difficulty_EASY;
                break;
            case Difficulty.Normal:
                currentDifficultySettings = difficulty_NORMAL;
                break;
            case Difficulty.Hard:
                currentDifficultySettings = difficulty_HARD;
                break;
            case Difficulty.Hardcore:
                currentDifficultySettings = difficulty_HARDCORE;
                break;
            default:
                currentDifficultySettings = difficulty_NORMAL;
                break;
        }
    }
}
