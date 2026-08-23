using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(DifficultySelector))]
public class DifficultyManager : MonoBehaviour
{
	[SerializeField] private int difficultyValue = 0;
	private float difficultyModifier;
	
	private DifficultySelector selector;
	public UnityEvent<DifficultySetting> OnDifficultySettingUpdated;
	
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        selector = GetComponent<DifficultySelector>();
		
		if (OnDifficultySettingUpdated == null)
			OnDifficultySettingUpdated = new UnityEvent<DifficultySetting>();
		//OnDifficultySettingUpdated.AddListener(ApplyDifficultySettings);
		GameManager.GetInstance().OnGameStart.AddListener(ApplyDifficultySettings);
    }

    // Update is called once per frame
    void Update()
    {
        //TODO: increment difficulty value over time
    }
	
	private void ApplyDifficultySettings()
	{
		Debug.Log("Apply settings here!");
	}
			
	
	public int GetDifficultyValue()
	{ return difficultyValue; }
	
	private void ResetDifficultyValue()
	{ difficultyValue = 0; }
	
	public float GetDifficultyModifier()
	{ return this.difficultyModifier; }
	
}
