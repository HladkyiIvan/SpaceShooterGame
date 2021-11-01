using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Bullet BulletPrefab;
    public Vector2 Movement;
    public GameObject Burst;
    public SimpleFlash flashEffect;
    public float ShootingDelay = 1f;
    public int EnemyHealth;
    public int Score;

    private float nextBulletSpawnTime;
    private bool startMovement = false;
    private SpriteRenderer sprite;

    // Start is called before the first frame update
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (startMovement)
        {
            if (sprite.tag == "Enemy1")
            {
                transform.position += (Vector3)Movement * Time.deltaTime;
            }
            else if (sprite.tag == "Enemy2")
            {
                transform.position += new Vector3(Movement.x * Time.deltaTime, Movement.y * Mathf.Sin(transform.position.x * 3) * 3 * Time.deltaTime, 0);
                transform.Rotate(new Vector3(0, 0, 360 * Time.deltaTime));
            }
            else if (sprite.tag == "Asteroid")
            {
                transform.position += new Vector3(Movement.x * Mathf.Sin(transform.position.y * 2) * Time.deltaTime, Movement.y * Time.deltaTime, 0);
                transform.Rotate(new Vector3(0, 0, 360 * Time.deltaTime));
            }

            if (sprite.tag != "Asteroid")
            {
                if (Time.time > nextBulletSpawnTime)
                {
                    Bullet bullet = Instantiate<Bullet>(BulletPrefab, transform.position + new Vector3(Random.Range(-0.05f, 0.05f), 0, 3), new Quaternion(0, 0, 180, 0), null);
                    nextBulletSpawnTime += ShootingDelay;
                }
            }
        }
    }

    private void OnBecameInvisible()
    {
        GameObject.Destroy(gameObject);
    }

    private void OnBecameVisible()
    {
        startMovement = true;
        nextBulletSpawnTime = Time.time;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "LaserBeam")
        {
            Destroy();
        }
    }

    public void RegisterDamage(int damage)
    {
        EnemyHealth -= damage;

        if (EnemyHealth > 0)
        {
            flashEffect.Flash();
        }
        else
        {
            Destroy();
        }
    }

    public void Destroy()
    {
        Instantiate<GameObject>(Burst, new Vector3(transform.position.x, transform.position.y, 2), Quaternion.identity, null);
        GameObject.Destroy(gameObject);
        HUD.Instance.AddScore(Score);
    }
}
