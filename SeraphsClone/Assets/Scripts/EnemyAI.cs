using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public EnemySO enemySO;

    public float health;
    public float height;

    GameObject player;
    GameObject bullet_parent;
    GameObject childAppearence;
    SpriteRenderer[] spriteRenders;

    float distanceToPlayer;
    bool canShoot = true;

    void Start()
    {
        player = GameObject.Find("Player");
        bullet_parent = GameObject.Find("Bullets");
        childAppearence = Instantiate(enemySO.EnemyAppearence, transform);

        health = enemySO.healthMax;
        height = Random.Range( enemySO.height -0.5f, enemySO.height +0.5f);
        spriteRenders = GetComponentsInChildren<SpriteRenderer>();
    }


    void Update()
    {
        foreach (SpriteRenderer spriteRender in spriteRenders) {
            spriteRender.color = new Color(spriteRender.color.r, spriteRender.color.g, spriteRender.color.b,Mathf.Pow(health/enemySO.healthMax,0.7f));
        }
        if (health < 0) {
            Destroy(gameObject);
        }

        Debug.Log("DistancePlayer:"+ distanceToPlayer+";Distance:"+ enemySO.distance + "; CanShoot:"+ canShoot);
        if (distanceToPlayer <= enemySO.distance && canShoot) {
            StartCoroutine(ShootCooldown());
            GameObject bullet = Instantiate(enemySO.bullet_prefab, transform.position, Quaternion.Euler(new Vector3(0, 0, transform.rotation.eulerAngles.z -90)), bullet_parent.transform);
            bullet.GetComponent<Rigidbody2D>().AddRelativeForce(new Vector2(2, 0), ForceMode2D.Impulse);
            Destroy(bullet,10);
        }


        rotateTowardsPlayer();
        move();
    }

    void rotateTowardsPlayer() {
        float angle = Mathf.Atan2(player.transform.position.y - transform.position.y, player.transform.position.x - transform.position.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0,0,angle +90));
    }

    private void move() {
        distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
        float moveDistance = Mathf.Pow((distanceToPlayer - height + 2) * 0.017f, 3);
        
        if (transform.position.y > height) {
            transform.position = new Vector3(transform.position.x, transform.position.y - moveDistance, 0);
        } else if (transform.position.y < height - 1) {
            transform.position = new Vector3(transform.position.x, transform.position.y + moveDistance, 0);
        }

        if (player.transform.position.x > transform.position.x) {
            transform.position = new Vector3(transform.position.x + moveDistance, transform.position.y, 0);
        } else {
            transform.position = new Vector3(transform.position.x - moveDistance, transform.position.y, 0);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Bullet")) {
            health -= 20;
        }
    }

    IEnumerator ShootCooldown() {
        canShoot = false;
        yield return new WaitForSeconds(enemySO.cooldown);
        canShoot = true;
    }
}
