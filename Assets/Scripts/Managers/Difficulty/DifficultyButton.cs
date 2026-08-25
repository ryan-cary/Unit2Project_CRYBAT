using UnityEngine;

[RequireComponent(typeof(UnityEngine.UI.Button))]
public class DifficultyButton : MonoBehaviour
{
    [SerializeField] Difficulty difficultyToLoad;
	
	void Start()
	{ this.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(LoadDifficulty); }
	
	private void LoadDifficulty()
	{ GameManager.GetInstance().GetDifficultyManager().GetComponent<DifficultySelector>().SetDifficulty(difficultyToLoad); }
}
