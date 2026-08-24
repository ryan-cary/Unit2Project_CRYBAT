using UnityEngine;

public class ExploderEnemy : Enemy
{
    protected override void Attack()
    {
        target.GetComponent<IDamageable>().GetDamage(attackDamage);
        Defeated();
    }
}
