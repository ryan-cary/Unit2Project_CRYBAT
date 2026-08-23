using UnityEngine;

[RequireComponent (typeof(Player))]
public class PlayerInput : MonoBehaviour
{
    private Player player;

    private float horizontalInput;
    private float verticalInput;
    private Vector2 lookTarget;
    private bool shootInput;
    private bool stopShootInput;
    private bool nukeInput;

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
        shootInput = Input.GetMouseButtonDown(0);
        stopShootInput = Input.GetMouseButtonUp(0);
        nukeInput = Input.GetMouseButtonDown(1);

        if (player.HasGunPowerUp())
        {
            player.GetGunPowerUpBehavior().Shoot(shootInput, stopShootInput);
        }
        else
        {
            if (shootInput) player.Shoot();
        }

        if (nukeInput) player.GetNukeBehavior().Use();
    }
}
