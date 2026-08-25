using UnityEngine;
using TMPro;

public class DifficultyButton : MonoBehaviour
{
    [SerializeField] Difficulty desiredDifficulty;
	[SerializeField] TMP_Text difficultyText;
	
	public void SelectDifficulty()
	{ GameManager.GetInstance().GetDifficultyManager().GetComponent<DifficultySelector>().SetDifficulty(desiredDifficulty); }
	
	public void UpdateText()
	{ difficultyText.text = $"Selected: {desiredDifficulty}"; }
}
