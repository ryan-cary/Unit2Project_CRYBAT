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
                currentDifficultySetting = difficulty_EASY;
                break;
            case Difficulty.Normal:
                currentDifficultySetting = difficulty_NORMAL;
                break;
            case Difficulty.Hard:
                currentDifficultySetting = difficulty_HARD;
                break;
            case Difficulty.Hardcore:
                currentDifficultySetting = difficulty_HARDCORE;
                break;
            default:
                currentDifficultySetting = difficulty_NORMAL;
                break;
        }
    }
}
