using UnityEngine;

public class DifficultyButton : MonoBehaviour
{
    [SerializeField] Difficulty difficultyToLoad;
	
	private void LoadDifficulty()
	{ GameManager.GetInstance().GetDifficultyManager().GetComponent<DifficultySelector>().SetDifficulty(difficultyToLoad); }
}
