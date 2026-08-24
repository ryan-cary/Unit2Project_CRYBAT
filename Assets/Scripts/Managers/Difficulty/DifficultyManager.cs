using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(DifficultySelector))]
public class DifficultyManager : MonoBehaviour
{
	[SerializeField] private float baseDifficultyValue = 5;
	[SerializeField] private float difficultyValue;
	private float difficultyModifier;
	
	private DifficultySelector selector;
	
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
		difficultyValue = baseDifficultyValue;
		
        selector = GetComponent<DifficultySelector>();
		GameManager.GetInstance().OnGameStart.AddListener(ApplyDifficultySettings);
		GameManager.GetInstance().OnGameStart.AddListener(ResetDifficultyValue);
    }

    // Update is called once per frame
    void Update()
    {
        difficultyValue += Time.deltaTime * 0.1f * selector.GetCurrentDifficultySetting().baseDifficultyModifier;
    }
	
	private void ApplyDifficultySettings()
	{
		GameManager.GetInstance().GetPlayer().DifficultyOverride(selector.GetCurrentDifficultySetting());
		GameManager.GetInstance().GetPickupSpawner().DifficultyOverride(selector.GetCurrentDifficultySetting());
		GameManager.GetInstance().GetEnemySpawner().DifficultyOverride(selector.GetCurrentDifficultySetting());
	}
			
	
	public float GetDifficultyValue()
	{ return difficultyValue; }
	
	private void ResetDifficultyValue()
	{ difficultyValue = baseDifficultyValue; }
	
	public float GetDifficultyModifier()
	{ return this.difficultyModifier; }
	
}
