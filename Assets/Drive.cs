using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class Drive : MonoBehaviour {

	float speed = 20.0F;
    float rotationSpeed = 120.0F;
    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    public Slider healthBar;
    public float health = 100.0f;
    public bool isTestingProtect = false;

    void Start()
    {
        InvokeRepeating("UpdateHealth", 5, 0.5f);
    }

    void Update() {
        Vector3 healthBarPos = Camera.main.WorldToScreenPoint(this.transform.position);
        healthBar.value = (int)health;
        healthBar.transform.position = healthBarPos + new Vector3(0, 30, 0);

        float translation = Input.GetAxis("Vertical") * speed;
        float rotation = Input.GetAxis("Horizontal") * rotationSpeed;
        translation *= Time.deltaTime;
        rotation *= Time.deltaTime;
        transform.Translate(0, 0, translation);
        transform.Rotate(0, rotation, 0);

        if(Input.GetKeyDown("space"))
        {
            GameObject bullet = GameObject.Instantiate(bulletPrefab, bulletSpawn.transform.position, bulletSpawn.transform.rotation);
            bullet.GetComponent<Rigidbody>().AddForce(bullet.transform.forward*2000);
        }
    }

    void UpdateHealth()
    {
        if (health < 100)
            health++;
    }
    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "bullet")
        {
            Debug.Log("hit");
            if (isTestingProtect)
            {
                if (health > 10)
                    health -= 10;

                if (health < 20) 
                    health = 10;
            }
            else
            {
                health -= 10;
            }
        }
    }
}
