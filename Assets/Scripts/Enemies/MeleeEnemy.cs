using UnityEngine;

public class MeleeEnemy : Enemy
{

    [SerializeField] private GameObject fistObject;
    [SerializeField] private float punchTime = 1f;

    private float punchSpeed;
    private bool isPunching;

     new private void Start()
    {
        base.Start();
        punchSpeed = 2 * attackRange / punchTime;
        ResetFist();
    }

    protected override void StopAttack()
    {
        base.StopAttack();
        isPunching = false;
        ResetFist();
    }

    protected override void Attack()
    {
        if (!isPunching)
        {
            if (timer <= attackRate)
            {
                timer += Time.deltaTime;
            } 
            else
            {
                timer = 0;
                isPunching = true;
            }
        }
        else
        {
            if (timer <= punchTime)
            {
                if (timer <= punchTime / 2 && timer + Time.deltaTime > punchTime / 2)
                {
                    DealPunchDamage();
                } 
                else if (timer <= punchTime / 2)
                {
                    MoveFist(true);
                } 
                else
                {
                    MoveFist(false);
                }
                timer += Time.deltaTime;
            } else
            {
                timer = 0;
                isPunching = false;
                ResetFist();
            }
        }
    }

    void DealPunchDamage()
    {
        target.GetComponent<IDamageable>().GetDamage(attackDamage);
    }

    void MoveFist(bool isPunchingOut)
    {
        fistObject.transform.Translate((isPunchingOut ? 1 : -1) * Vector2.up * punchSpeed * Time.deltaTime);
    }

    void ResetFist()
    {
        fistObject.transform.localPosition = Vector3.zero;
    }

    public void SetupMeeleeEnemy(float desiredAttackRange, float desiredAttackRate)
    {
        attackRange = desiredAttackRange;
        attackRate = desiredAttackRate;
    }

}
