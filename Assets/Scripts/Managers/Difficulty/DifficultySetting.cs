using UnityEngine;

[CreateAssetMenu(fileName = "difficulty_settings", menuName = "Difficulty Settings", order = 0)]
public class DifficultySetting : ScriptableObject
{
    [Header("ID")]
    new public string name = "missingDifficultyName";
    public int difficultyCode = 1;

    [Header("Settings")]
    public float baseDifficultyModifier = 1.0f;
    public float enemySpawnRate = 5.0f;
	public float playerHealthOverride = 100.0f;
}
