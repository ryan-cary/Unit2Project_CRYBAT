using UnityEngine;

public class Weapon
{

    public static int weaponCount = 0;
    private string name;
    private float damage;
    private float bulletSpeed;

    public Weapon(string _name, float _damage, float _bulletSpeed = 10)
    {
        name = _name;
        damage = _damage;
        bulletSpeed = _bulletSpeed;
        weaponCount++;
    }

    public Weapon()
    {
        weaponCount++;
    }

    public void Shoot(Bullet _bullet, PlayableObject _shooter, string _targetTag)
    {
        Bullet bullet = GameObject.Instantiate(_bullet, _shooter.transform.position, _shooter.transform.rotation);
        bullet.SetBullet(damage, _targetTag, bulletSpeed);
        GameObject.Destroy(bullet.gameObject, GameManager.GetInstance().GetBulletLifetime());
    }

    public void Shoot(Bullet _bullet, PlayableObject _shooter, string _targetTag,  float angle)
    {
        Bullet bullet = GameObject.Instantiate(_bullet, _shooter.transform.position, Quaternion.AngleAxis(angle, Vector3.forward));
        bullet.SetBullet(damage, _targetTag, bulletSpeed);
        GameObject.Destroy(bullet.gameObject, GameManager.GetInstance().GetBulletLifetime());
    }

    public float GetDamage()
    {
        return damage;
    }
}
