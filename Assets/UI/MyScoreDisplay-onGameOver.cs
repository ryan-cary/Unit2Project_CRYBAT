using TMPro;
using UnityEngine;

public class myScoreDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text myLabel;
    [SerializeField] private string myFormat = "YOUR SCORE: {0}";

    private ScoreManager myScoreManager;

    private void Awake()
    {
        if (myLabel == null)
        {
            myLabel = GetComponent<TMP_Text>();
        }
    }

    private void OnEnable()
    {
        myTrySubscribe();
    }

    private void Start()
    {
        myTrySubscribe();
    }

    private void OnDisable()
    {
        if (myScoreManager != null)
        {
            myScoreManager.OnScoreUpdated.RemoveListener(mySetScore);
            myScoreManager = null;
        }
    }

    private void myTrySubscribe()
    {
        if (myScoreManager != null)
        {
            return;
        }

        myScoreManager = GameManager.GetInstance()?.GetScoreManager();
        if (myScoreManager == null)
        {
            return;
        }

        myScoreManager.OnScoreUpdated.AddListener(mySetScore);
        mySetScore(myScoreManager.GetScore());
    }

    private void mySetScore(int myScore)
    {
        if (myLabel != null)
        {
            myLabel.text = string.Format(myFormat, myScore);
        }
    }
}