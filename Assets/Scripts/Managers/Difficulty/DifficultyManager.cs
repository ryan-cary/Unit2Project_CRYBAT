using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
	[SerializeField] private int difficultyValue;
	
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
	
	public int GetDifficultyValue()
	{ return difficultyValue; }
}
