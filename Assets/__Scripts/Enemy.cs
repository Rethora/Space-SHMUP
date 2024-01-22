using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Inscribed")]
    public float speed = 10f; // the movement speed is 10m/s
    public float fireRate = 0.3f; // seconds/shot (unused)
    public float health = 10f; // damage needed to destroy this enemy
    public int score = 100; // points earned for destroying this
    public float powerUpDropChance = 1f; // chance to drop a PowerUp

    protected bool calledShipDestroyed = false;
    protected BoundsCheck bndCheck;

    void Awake()
    {
        bndCheck = GetComponent<BoundsCheck>();
    }

    // This is a Property: A method that acts like a field
    public Vector3 pos
    {
        get
        {
            return this.transform.position;
        }
        set
        {
            this.transform.position = value;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Move();

        // Check whether this Enemy has gone off the bottom of the screen
        if (bndCheck.LocIs(BoundsCheck.eScreenLocs.offDown))
        {
            Destroy(gameObject);
        }
    }

    public virtual void Move()
    {
        Vector3 tempPos = pos;
        tempPos.y -= speed * Time.deltaTime;
        pos = tempPos;
    }

    void OnCollisionEnter(Collision collision)
    {
        GameObject otherGO = collision.gameObject;

        ProjectileHero p = otherGO.GetComponent<ProjectileHero>();
        if (p != null)
        {
            if (bndCheck.onScreen)
            {
                health -= Main.GET_WEAPON_DEFINITION(p.type).damageOnHit;
                if (health <= 0)
                {
                    if (!calledShipDestroyed)
                    {
                        calledShipDestroyed = true;
                        Main.SHIP_DESTROYED(this);
                    }
                    Destroy(this.gameObject); // Destroy this Enemy GameObject
                }
            }

            Destroy(otherGO); // Destroy the Projectile
        }
        else
        {
            print("Enemy hit by non-ProjectileHero: " + otherGO.name);
        }
    }
}
