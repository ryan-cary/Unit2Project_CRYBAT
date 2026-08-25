using UnityEngine;

public enum Difficulty
{
    Easy,
    Normal,
    Hard,
    Hardcore
}

public class DifficultySelector : MonoBehaviour
{
	[Header("Debug")]
	[SerializeField] Difficulty defaultDifficulty = Difficulty.Normal;
	
	[Header("Difficulty Settings")]
    [SerializeField] DifficultySetting difficulty_EASY;
    [SerializeField] DifficultySetting difficulty_NORMAL;
    [SerializeField] DifficultySetting difficulty_HARD;
    [SerializeField] DifficultySetting difficulty_HARDCORE;

    public DifficultySetting currentDifficultySetting;
    public Difficulty currentDifficulty { get; private set; }

    void Awake() 
    { SetDifficulty(defaultDifficulty); }
	
	public DifficultySetting GetCurrentDifficultySetting()
	{ return currentDifficultySetting; }

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
				Debug.Log($"Now on {currentDifficulty}!");
                break;
            case Difficulty.Normal:
                currentDifficultySetting = difficulty_NORMAL;
				Debug.Log($"Now on {currentDifficulty}!");
                break;
            case Difficulty.Hard:
                currentDifficultySetting = difficulty_HARD;
				Debug.Log($"Now on {currentDifficulty}!");
                break;
            case Difficulty.Hardcore:
                currentDifficultySetting = difficulty_HARDCORE;
				Debug.Log($"Now on {currentDifficulty}!");
                break;
            default:
                currentDifficultySetting = difficulty_NORMAL;
				Debug.Log($"Now on {currentDifficulty}!");
                break;
        }
    }
}
