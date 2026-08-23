using UnityEngine;

public class ExploderEnemy : Enemy
{
	protected override void Start()
	{ this.defeatScore = 15; }
	
    protected override void Attack()
    {
        target.GetComponent<IDamageable>().GetDamage(attackDamage);
        Defeated();
    }
}
