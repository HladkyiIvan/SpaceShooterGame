using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float Speed = 1;

    private SpriteRenderer sprite;

    // Start is called before the first frame update
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (sprite.tag == "Bullet")
        {
            transform.position += new Vector3(0, Speed * Time.deltaTime);
        }
        else if (sprite.tag == "EnemyBullet1" || sprite.tag == "EnemyBullet2")
        {
            transform.position -= new Vector3(0, Speed * Time.deltaTime);

            if (transform.tag == "EnemyBullet2")
            {
                transform.Rotate(new Vector3(0, 0, -360 * Time.deltaTime));
            }
        }
    }

    private void OnBecameInvisible()
    {
        GameObject.Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (sprite.tag == "Bullet")
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.RegisterDamage(1);
                GameObject.Destroy(gameObject);
            }
        }
        else if (sprite.tag == "EnemyBullet1" || sprite.tag == "EnemyBullet2")
        {
            SpaceShip spaceShip = collision.gameObject.GetComponent<SpaceShip>();
            if (spaceShip != null)
            {
                if (!spaceShip.IsInvincible)
                {
                    spaceShip.RegisterDamage(1);
                }
                GameObject.Destroy(gameObject);
            }
        }
    }
}
