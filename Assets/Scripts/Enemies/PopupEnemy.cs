using UnityEngine;

public class PopupEnemy : Enemy
{
    public string scamMessage;

    protected override void StartAttack()
    {
        base.StartAttack();
        Scam();
    }

    private void Scam()
    {
        Debug.Log(scamMessage);
    }
}
