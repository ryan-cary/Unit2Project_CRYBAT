using UnityEngine;

public class DifficultyButton : MonoBehaviour
{
    [SerializeField] Difficulty desiredDifficulty;
	
	public void SelectDifficulty()
	{ GameManager.GetInstance().GetDifficultyManager().GetComponent<DifficultySelector>().SetDifficulty(desiredDifficulty); }
}
