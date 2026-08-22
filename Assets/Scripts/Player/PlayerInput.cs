using UnityEngine;

[RequireComponent (typeof(Player))]
public class PlayerInput : MonoBehaviour
{
    private Player player;

    private float horizontalInput;
    private float verticalInput;
    private Vector2 lookTarget;
    private bool shootInput;
    private bool nukeInput;
    private bool startedShootingPowerUp;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        lookTarget = Input.mousePosition;

        ShootInput();
        player.Move(new Vector2(horizontalInput, verticalInput), lookTarget);
    }

    void ShootInput()
    {
        if (player.GetGunPowerUpBehavior().HasGunPowerUp())
        {
            
            if (!startedShootingPowerUp && Input.GetMouseButton(0))
            {
                player.GetGunPowerUpBehavior().StartShootCoroutine();
                startedShootingPowerUp = true;
            }
            if (Input.GetMouseButtonUp(0))
            {
                player.GetGunPowerUpBehavior().StopShootCoroutine();
                startedShootingPowerUp = false;
            }
        }
        else
        {
            shootInput = Input.GetMouseButtonDown(0);

            if (shootInput) player.Shoot();
        }
        nukeInput = Input.GetMouseButtonDown(1);

        if (nukeInput) player.UseNukePickup();
    }
}
