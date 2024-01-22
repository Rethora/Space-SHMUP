using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // enables the loading & reloading of scenes

public class Main : MonoBehaviour
{
    static private Main S; // private Singleton for Main
    static private Dictionary<eWeaponType, WeaponDefinition> WEAP_DICT;

    [Header("Inscribed")]
    public bool spawnEnemies = true;
    public GameObject[] prefabEnemies; // array of Enemy prefabs
    public float enemySpawnPerSecond = 0.5f; // # enemies spawned/second
    public float enemyInsetDefault = 1.5f; // Inset from the sides
    public float gameRestartDelay = 2f;
    public GameObject prefabPowerUp;
    public WeaponDefinition[] weaponDefinitions;
    public eWeaponType[] powerUpFrequency = new eWeaponType[]
    {
        eWeaponType.blaster, eWeaponType.blaster, eWeaponType.spread, eWeaponType.shield
    };

    private BoundsCheck boundsCheck;

    void Awake()
    {
        S = this;
        // set bndCheck to reference the boundsCheck component on this 
        // GameObject
        boundsCheck = GetComponent<BoundsCheck>();

        // Invoke SpawnEnemy() once (in 2 seconds, based on default values)
        Invoke(nameof(SpawnEnemy), 1f / enemySpawnPerSecond);

        WEAP_DICT = new Dictionary<eWeaponType, WeaponDefinition>();
        foreach (WeaponDefinition def in weaponDefinitions)
        {
            WEAP_DICT[def.type] = def;
        }
    }

    public void SpawnEnemy()
    {
        // If spawnEnemies is flase, skip to the next invoke of SpawnEnemy()
        if (!spawnEnemies)
        {
            Invoke(nameof(SpawnEnemy), 1f / enemySpawnPerSecond);
            return;
        }

        int ndx = Random.Range(0, prefabEnemies.Length);
        GameObject go = Instantiate<GameObject>(prefabEnemies[ndx]);

        // position the enemy above the screen with a random x position
        float enemyInset = enemyInsetDefault;
        if (go.GetComponent<BoundsCheck>() != null)
        {
            enemyInset = Mathf.Abs(go.GetComponent<BoundsCheck>().radius);
        }

        // set the initial position for the spawned Enemy
        Vector3 pos = Vector3.zero;
        float xMin = -boundsCheck.camWidth + enemyInset;
        float xMax = boundsCheck.camWidth - enemyInset;
        pos.x = Random.Range(xMin, xMax);
        pos.y = boundsCheck.camHeight + enemyInset;
        go.transform.position = pos;

        // Invoke SpawnEnemy() again
        Invoke(nameof(SpawnEnemy), 1f / enemySpawnPerSecond);
    }

    void DelayedRestart()
    {
        // invoke the restart() method in gameRestartDelay seconds
        Invoke(nameof(Restart), gameRestartDelay);
    }

    void Restart()
    {
        // reload __Scene_0 to restart the game
        // "__Scene_0" below starts with 2 underscores and ends with a zero
        SceneManager.LoadScene("__Scene_0");
    }

    static public void HERO_DIED()
    {
        S.DelayedRestart();
    }

    static public WeaponDefinition GET_WEAPON_DEFINITION(eWeaponType wt)
    {
        if (WEAP_DICT.ContainsKey(wt))
        {
            return WEAP_DICT[wt];
        }

        // will return 'none' as default value
        return new WeaponDefinition();
    }

    static public void SHIP_DESTROYED(Enemy e)
    {
        if (Random.value <= e.powerUpDropChance)
        {
            int ndx = Random.Range(0, S.powerUpFrequency.Length);
            eWeaponType pUpType = S.powerUpFrequency[ndx];

            GameObject go = Instantiate<GameObject>(S.prefabPowerUp);
            PowerUp pUp = go.GetComponent<PowerUp>();
            pUp.SetType(pUpType);

            pUp.transform.position = e.transform.position;
        }
    }

}
