using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(DifficultySelector))]
public class DifficultyManager : MonoBehaviour
{
	[SerializeField] private int difficultyValue = 0;
	private float difficultyModifier;
	
	private DifficultySelector selector;
	public UnityEvent onDifficultySettingUpdated;
	
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        selector = GetComponent<DifficultySelector>();
		
		if (onDifficultySettingUpdated == null)
			onDifficultySettingUpdated = new UnityEvent();
		onDifficultySettingUpdated.AddListener(ApplyDifficultySettings);
		
    }

    // Update is called once per frame
    void Update()
    {
        //TODO: increment difficulty value over time
    }
	
	private void ApplyDifficultySettings()
	{
		DifficultySetting settingToApply = selector.GetCurrentDifficultySetting();
		difficultyModifier = settingToApply.baseDifficultyModifier;
		//TODO: apply player health override
		//TODO: modify spawn rate
	}
			
	
	public int GetDifficultyValue()
	{ return difficultyValue; }
	
	private void ResetDifficultyValue()
	{ difficultyValue = 0; }
	
	public float GetDifficultyModifier()
	{ return this.difficultyModifier; }
	
}
