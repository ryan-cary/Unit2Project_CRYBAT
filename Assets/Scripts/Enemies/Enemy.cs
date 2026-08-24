using UnityEngine;
using UnityEngine.Events;
using System;

public class Enemy : PlayableObject
{
    [SerializeField] private EnemyType enemyType;	
    [SerializeField] ExplosionFragment explosionFragmentPrefab;
	
	[Header("Enemy Stats")]
	[SerializeField] protected float baseHealth = 100f;
    [SerializeField] protected float attackDamage = 10f;
    [SerializeField] protected float attackRange = 5;
    [SerializeField] protected float attackRate = 2f;
    [SerializeField] protected int defeatScore = 10;
	
	protected Transform target;
    protected float targetSpeed;
    protected float timer = 0;
    protected bool isAttacking;

    protected virtual void Start()
    {
        try
        {
            target = GameManager.GetInstance().GetPlayer().transform;
        }
        catch (Exception e)
        {
            Debug.Log("There is no player in the scene! Goodbye!");
            Destroy(gameObject);
        }
        targetSpeed = speed;
    }

    protected virtual void Update()
    {
        if (target == null)
        {
            return;
        }

        float distance = Vector2.Distance(transform.position, target.position);

        if (distance <= attackRange)
        {
            speed = 0;
            StartAttack();
        } else
        {
            speed = targetSpeed;
            StopAttack();
        }

        if (isAttacking)
        {
            Attack();
        }
        Move(target.position);
    }

    public void Move()
    {
        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }

    public override void Move(Vector2 direction)
    {
        direction.x -= transform.position.x;
        direction.y -= transform.position.y;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90; 
        transform.rotation = Quaternion.Euler(0, 0, angle);
        rb.linearVelocity = direction.normalized * speed;
    }

    public override void Shoot() { }

    protected virtual void StartAttack()
    {
        isAttacking = true;
    }

    protected virtual void StopAttack()
    {
        isAttacking = false;
    }

    protected virtual void Attack()
    {
        Debug.Log("Enemy is attacking");
    }

    public override void Defeated()
    {
        Explode();
        Destroy(gameObject);
        GameManager.GetInstance().GetScoreManager().IncrementScore(defeatScore);
        GameManager.GetInstance().OnEnemyDefeated(this);
    }

    protected virtual void Explode()
    {
        for (int i = 0; i < 4; i++)
        {
            ExplosionFragment fragment = GameObject.Instantiate(explosionFragmentPrefab, transform.position, Quaternion.identity);
            fragment.SetMoveDirection(new Vector2(i < 2 ? 1 : -1, i % 2 == 0 ? 1 : -1));
        }
    }

    public EnemyType GetEnemyType()
    {
        return enemyType;
    }

    public void SetEnemyType(EnemyType _enemyType)
    {
        enemyType = _enemyType;
    }

    public override void GetDamage(float damage)
    {
        health.DeductHealth(damage);

        if (health.GetHealth() == 0)
        {
            Defeated();
        }
    }
	
	public int GetDefeatScore()
	{ return this.defeatScore; }
}
