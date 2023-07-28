using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("Stats")]
    public float movementSpeed=10;
    public float jumpPower = 5;
    public float fireRate = 1;

    private Rigidbody2D playerRigidbody;
    private PlayerInputActions playerInputActions;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheckTransform;
    [SerializeField] private Camera cam;
    [SerializeField] private GameObject wand_gameObject;
    [SerializeField] private GameObject bullet_prefab;
    [SerializeField] private GameObject bullet_parent;
    [SerializeField] private Transform wand_end;
    Coroutine fireCorroutine = null;

    private void Awake() {
        playerRigidbody = GetComponent<Rigidbody2D>();
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        WandAimAndShoot();
    }

    void Move() {
        Vector2 inputVector = playerInputActions.Player.Move.ReadValue<Vector2>();
        float horizontal = inputVector.x * movementSpeed;
        playerRigidbody.velocity =  new Vector2 (horizontal, playerRigidbody.velocity.y);

        playerInputActions.Player.Jump.performed += Jump;
        playerInputActions.Player.Jump.started += Jump;
        playerInputActions.Player.Shoot.started += _ =>StartShoot();
        playerInputActions.Player.Shoot.canceled += _ => StopShoot();


    }

    public void Jump(InputAction.CallbackContext context) {
        bool grounded = IsGrounded();
        if (context.started && grounded ) {
            Debug.Log("Jump");
            playerRigidbody.AddForce(Vector3.up * jumpPower, ForceMode2D.Impulse);
            return;
        } 
        
        if (context.performed && !grounded) {
            Debug.Log("JumpImpulse");
            playerRigidbody.AddForce(Vector3.up * jumpPower/2, ForceMode2D.Impulse);
            return;
        }
    }

    void WandAimAndShoot() {
        Vector2 mousePos = cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        float angle = Mathf.Atan2(mousePos.y - wand_gameObject.transform.position.y, 
            mousePos.x - wand_gameObject.transform.position.x) * Mathf.Rad2Deg;
        wand_gameObject.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    private void StartShoot() {
        Debug.Log("StartShoot");
        if (fireCorroutine == null) {
            fireCorroutine = StartCoroutine(ShootingCooDown());
        }
    }

    private void StopShoot() {
        Debug.Log("StopShoot");
        if (fireCorroutine != null) {
            StopCoroutine(fireCorroutine);
            fireCorroutine = null;
        }
    }

    private void Shoot() {
        Debug.Log("Shooting");
        GameObject bullet = Instantiate(
            bullet_prefab,
            wand_end.position, 
            wand_end.rotation, 
            bullet_parent.transform);
        bullet.GetComponent<Rigidbody2D>().AddRelativeForce(new Vector2(10,0), ForceMode2D.Impulse);
        Destroy(bullet, 10);
    }

    public bool IsGrounded() {
        return Physics2D.CircleCast(groundCheckTransform.position, 0.1f, Vector2.down,0.1f,groundLayer);
    }

    IEnumerator ShootingCooDown() {
        while (true) {
            Shoot();
            yield return new WaitForSeconds(1 / fireRate);
        }
    }
 

}
