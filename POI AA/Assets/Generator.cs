using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Generator : NetworkBehaviour
{
    public GameObject[] enemies;
    private int randEnemy;
    private Vector3 Epos;
    private GameObject enemy;
    private float EspawnWaitIni = 3f;
    private float EspawnWaitMax = 2.5f;
    private float EspawnWaitMin = 1.25f;
    private float EspawnWait;
    bool dir;
    float dirChangeChance;

    private bool active;

    void Start ()
    {
        EspawnWait = Time.time + EspawnWaitIni;
        if (Random.value >= 0.5f)
            dir = !dir;
        dirChangeChance = 0.5f;
    }
	
	void Update ()
    {
        if (GameObject.Find("NetworkManager").GetComponent<CustomNetMan>().Players[0] != null &&
            GameObject.Find("NetworkManager").GetComponent<CustomNetMan>().Players[1] != null)
        {
            if (!active)
            {
                active = true;                
                EspawnWait = Time.time + EspawnWaitIni;
                EspawnWaitMax = 2.5f;
                EspawnWaitMin = 1.25f;
            }
        }
        else
        {
            if (active)
            {
                active = false;                
                foreach (Transform spawned in transform)
                    GameObject.Destroy(spawned.gameObject);
            }
        }

        if (active)
        {
            if (EspawnWait <= Time.time)
            {
                randEnemy = Random.Range(0, enemies.Length);
                Epos.x = Random.Range(-1.0f, 1.0f);

                enemy = Instantiate(enemies[randEnemy], transform.position + Epos, transform.rotation);
                enemy.GetComponent<Spike>().groundDir = dir;
                if (Random.value <= dirChangeChance)
                {
                    dir = !dir;
                    dirChangeChance = 0.5f;
                }
                else
                {
                    dirChangeChance = 1.0f;
                }
                enemy.transform.parent = transform;
                NetworkServer.Spawn(enemy);

                EspawnWait = Time.time + Random.Range(EspawnWaitMin, EspawnWaitMax);

                if (EspawnWaitMin > 0.25f)
                    EspawnWaitMin -= 0.01f;
                if (EspawnWaitMax > 0.5f)
                    EspawnWaitMax -= 0.02f;
            }
        }
    }
}
