using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoundsCheck))]
public class PowerUp : MonoBehaviour
{
    [Header("Inscribed")]
    [Tooltip("x holds a min value and y a max value for a Random.Range() call.")]
    public Vector2 rotMinMax = new Vector2(15, 90);
    [Tooltip("x holds a min value and y a max value for a Random.Range() call.")]
    public Vector2 driftMinMax = new Vector2(0.25f, 2);
    public float lifeTime = 10f;
    public float fadeTime = 4f;

    [Header("Dynamic")]
    public eWeaponType type;
    public GameObject cube;
    public TextMesh letter;
    public Vector3 rotPerSecond;
    public float birthTime;

    private Rigidbody rb;
    private BoundsCheck boundsCheck;
    private Material cubeMat;

    void Awake()
    {
        cube = transform.GetChild(0).gameObject;
        letter = GetComponent<TextMesh>();
        rb = GetComponent<Rigidbody>();
        boundsCheck = GetComponent<BoundsCheck>();
        cubeMat = cube.GetComponent<Renderer>().material;

        Vector3 vel = Random.onUnitSphere;

        vel.z = 0;
        vel.Normalize();

        vel *= Random.Range(driftMinMax.x, driftMinMax.y);
        rb.velocity = vel;

        transform.rotation = Quaternion.identity;

        rotPerSecond = new Vector3(Random.Range(rotMinMax[0], rotMinMax[1]), Random.Range(rotMinMax[0], rotMinMax[1]));

        birthTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        cube.transform.rotation = Quaternion.Euler(rotPerSecond * Time.time);

        float u = (Time.time - birthTime - lifeTime) / fadeTime;
        if (u > 1)
        {
            Destroy(this.gameObject);
            return;
        }

        if (u > 0)
        {
            Color c = cubeMat.color;
            c.a = 1f - u;
            cubeMat.color = c;

            c = letter.color;
            c.a = 1f - (u * 0.5f);
            letter.color = c;
        }

        if (!boundsCheck.onScreen)
        {
            Destroy(gameObject);
        }
    }

    /*    public eWeaponType type
        {
            get { return type; }
            set { SetType(value); }
        }*/

    public void SetType(eWeaponType wt)
    {
        WeaponDefinition def = Main.GET_WEAPON_DEFINITION(wt);
        cubeMat.color = def.powerUpColor;
        letter.text = def.letter;
        type = wt;
    }

    public void AbsorbedBy(GameObject target)
    {
        Destroy(this.gameObject);
    }
}
