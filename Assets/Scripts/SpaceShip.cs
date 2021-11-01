using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class SpaceShip : MonoBehaviour
{
    public static SpaceShip Instance;

    public Bullet BulletPrefab;
    public GameObject Burst;
    public GameObject LaserBeam;
    public float FlipSpeed = 0.05f;
    public float FlightSpeed = 1f;
    public float ShootingDelay = 0.5f;
    public float LaserReloadSpeed = 1f;
    public float LaserConsuptionSpeed = 1.5f;
    public float LaserReloadDelay = 3f;
    public int Score;
    public SimpleFlash flashEffect;
    public int Health = 3;
    public bool IsInvincible = false;

    private float nextBulletSpawnTime;
    private float laserStopTime;
    private float laserLevel;
    private SpriteRenderer sprite;

    public void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        //Cursor.visible = false;
        laserLevel = 1;
        sprite = GetComponent<SpriteRenderer>();
        nextBulletSpawnTime = Time.time;
        //transform.position = new Vector3();
    }

    // Update is called once per frame
    void Update()
    {
        // SpaceShip movement
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Bounds bounds = OrthographicBounds(Camera.main);

        if ((mousePosition.x > transform.position.x + (sprite.bounds.size.x / 2)) || (mousePosition.x < transform.position.x - (sprite.bounds.size.x / 2)))
        {
            if (transform.localScale.x > 0.8)
            {
                transform.localScale = new Vector3(transform.localScale.x - (FlipSpeed * Time.deltaTime), 1, 1);
            }
        }
        else
        {
            if (transform.localScale.x < 1)
            {
                transform.localScale = new Vector3(transform.localScale.x + (FlipSpeed * Time.deltaTime), 1, 1);
            }
        }

        // Laser shooting (super shit)
        Bounds laserBounds = LaserBeam.gameObject.GetComponent<SpriteRenderer>().bounds;

        if (Input.GetMouseButton(0) && laserLevel != 0)
        {
            LaserBeam.SetActive(true);
            if (laserBounds.max.y <= bounds.max.y + 0.15f)
            {
                LaserBeam.transform.localScale = new Vector3(LaserBeam.transform.localScale.x + 30f * Time.deltaTime, 0.5f, 1);
            }
            else
            {
                LaserBeam.transform.localScale = new Vector3(LaserBeam.transform.localScale.x - 30f * Time.deltaTime, 0.5f, 1);
            }

            laserLevel -= LaserConsuptionSpeed * Time.deltaTime;
            laserLevel = Mathf.Clamp(laserLevel, 0, 1);

            if (laserLevel == 0)
            {
                laserStopTime = Time.time;
            }

            HUD.Instance.UpdateEnergyBar(laserLevel);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            laserStopTime = Time.time;
        }
        else
        {
            if (LaserBeam.transform.localScale.x > 0.05)
            {
                LaserBeam.transform.localScale = new Vector3(LaserBeam.transform.localScale.x - 30f * Time.deltaTime, 0.5f, 1);
            }
            else
            {
                LaserBeam.SetActive(false);
            }

            if (laserLevel != 1 && Time.time > laserStopTime + LaserReloadDelay)
            {
                laserLevel += LaserReloadSpeed * Time.deltaTime;
                laserLevel = Mathf.Clamp(laserLevel, 0, 1);
                HUD.Instance.UpdateEnergyBar(laserLevel);
            }
        }

        transform.position += (mousePosition - transform.position) * FlightSpeed * Time.deltaTime;
        transform.position = new Vector3(
                            Mathf.Clamp(transform.position.x, bounds.min.x + sprite.bounds.size.x / 2, bounds.max.x - sprite.bounds.size.x / 2),
                            Mathf.Clamp(transform.position.y, bounds.min.y + sprite.bounds.size.y / 2, bounds.max.y - sprite.bounds.size.y / 2),
                            0);

        if (Time.time > nextBulletSpawnTime)
        {
            Bullet bullet = Instantiate<Bullet>(BulletPrefab, transform.position + new Vector3(Random.Range(-0.05f, 0.05f), 0, 3), Quaternion.identity, null);
            nextBulletSpawnTime += ShootingDelay;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsInvincible)
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                RegisterDamage(1);
                enemy.Destroy();
            }
        }

        if (collision.tag == "FireRatePowerUp")
        {
            ShootingDelay *= 0.7f;
            HUD.Instance.AddScore(20);
            GameObject.Destroy(collision.gameObject);
        }

        if (collision.tag == "HealthPowerUp")
        {
            if (Health + 1 <= 3)
            {
                HUD.Instance.UpdateHPBar(Health, true);
                Health += 1;
            }
            HUD.Instance.AddScore(20);
            GameObject.Destroy(collision.gameObject);
        }

        if (collision.tag == "EndScreenTrigger")
        {
            HUD.Instance.ShowWinMessage();
        }
    }

    public void RegisterDamage(int damage)
    {
        if (Health - damage >= 0)
        {
            Health -= damage;
            HUD.Instance.UpdateHPBar(Health, false);

            if (Health == 0)
            {
                Instantiate<GameObject>(Burst, transform.position, Quaternion.identity, null);
                GameObject.Destroy(gameObject);
                HUD.Instance.ShowLoseMessage();
            }
            else
            {
                flashEffect.Flash();
            }
        }
    }

    public static Bounds OrthographicBounds(Camera camera)
    {
        float screenAspect = (float)Screen.width / (float)Screen.height;
        float cameraHeight = camera.orthographicSize * 2;
        Bounds bounds = new Bounds(
            camera.transform.position,
            new Vector3(cameraHeight * screenAspect, cameraHeight, 0));
        return bounds;
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
