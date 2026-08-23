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
    [SerializeField] DifficultySetting difficulty_EASY;
    [SerializeField] DifficultySetting difficulty_NORMAL;
    [SerializeField] DifficultySetting difficulty_HARD;
    [SerializeField] DifficultySetting difficulty_HARDCORE;

    public DifficultySetting currentDifficultySetting;
    public Difficulty currentDifficulty { get; private set; } = Difficulty.Normal;

    void Awake() 
    { ApplySettings(); }
	
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
		GameManager.GetInstance().GetDifficultyManager().onDifficultySettingUpdated?.Invoke();
		Debug.Log("Settings updated!");
    }
}
