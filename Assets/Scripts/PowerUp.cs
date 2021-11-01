using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    private int goUp = 1;
    private float changeDirectionTime;


    // Start is called before the first frame update
    void Start()
    {
        changeDirectionTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > changeDirectionTime)
        {
            goUp = -goUp;
            changeDirectionTime += 0.35f;
        }

        transform.position += new Vector3(0, goUp * Time.deltaTime, 0);
    }
}
