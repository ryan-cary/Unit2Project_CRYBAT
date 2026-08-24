using UnityEngine;

public class ExploderEnemy : Enemy
{
	protected override void Start()
	{ 
		base.Start();
		this.defeatScore = 15; 
	}
	
    protected override void Attack()
    {
        target.GetComponent<IDamageable>().GetDamage(attackDamage);
        Defeated();
    }
}
