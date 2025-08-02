using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.Universal;

public class PlayerBehavior : MonoBehaviour
{
    // Start is called before the first frame update
    private List<GameObject> enemies = new List<GameObject>();
    public GameObject EnemiesHolder;
    public float moveSpeed;
    public float health;
    public float dmg;
    public float atkspd;
    private float lastAtkTime;
    public float atkRad;
    NavMeshAgent agent;
    private bool JustWon = false;
    private Vector3 oriPos;
    public float maxHealth;
    public int reversal;
    public float luck;
    public int enemyCount;
    public float startRoundTime;
    public float stopRoundTime;
    public float luckmult;
    public GameObject ReachLight;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        lastAtkTime = atkspd + 1;
        oriPos = transform.localPosition;
        maxHealth = health;
    }

    // Update is called once per frame
    void Update()
    {
        
        //Vector2 direction = (Vector2)(player.transform.position-transform.position).normalized;
        //transform.position += (Vector3)direction * moveSpeed * Time.deltaTime;
        enemies.Clear();
        agent.speed = moveSpeed;
        foreach(Transform child in EnemiesHolder.transform)
        {

            enemies.Add(child.gameObject);
        }
        if (enemies.Count > 0)
        {
            float mindist = 100f;
            GameObject enemy = enemies[0]; //potential out of bounds issues
            Vector3 pos = Vector3.zero;
            foreach (GameObject go in enemies)
            {
                if (Vector3.Distance(go.transform.position, transform.position) < mindist)
                {
                    mindist = Vector3.Distance(go.transform.position, transform.position);
                    enemy = go;
                    pos = go.transform.position;
                }
            }
            agent.SetDestination(pos);
            if (Vector3.Distance(transform.position, pos) <= atkRad)
            {
                if ((Time.time - lastAtkTime)*atkspd > 1)
                {
                    lastAtkTime = Time.time;
                    enemy.GetComponent<EnemyBehavior>().AlterHealth(-dmg);
                }
            }
        }
        else if (!JustWon)
        {
            stopRoundTime = Time.time;
            JustWon = true;
            setPlayerActive(false);

            //gameObject.GetComponentInChildren<SpriteRenderer>().enabled = false;
            GameManager.Instance.moveNavMesh(true);
            transform.localPosition = oriPos;
            GameManager.Instance.setOptions();
        }
    }


    public void AlterHealth(float diff)
    {
        health += diff; 
        if (health <= 0)
        {
            GameManager.Instance.showEnd(false);
        }
    }
    public void updateJustWon(bool val){ 
        JustWon = val;  
    }
    public void setPlayerActive(bool active)
    {
        gameObject.active = active;
    }

     
    public void statChange(string stat, float value)
    {
        if (stat == "Health")
        {
            maxHealth *= value;
            health = maxHealth;
        }
        if (stat == "Damage")
        {
            dmg *= value;
        }
        if (stat == "Speed")
        {
            atkspd *= value;
            moveSpeed *= value;
        }
        if (stat == "Reach")
            atkRad *= value;
            ReachLight.GetComponent<Light2D>().pointLightOuterRadius = atkRad;
        if (stat == "Luck")
        {
            luckmult *= value;
        }

        if (stat == "Reversal")
        {
            reversal += Mathf.RoundToInt(value) ;
        }

    }

    public void updateLuck()
    {
        luck =  ((enemyCount / 100f) + (health / maxHealth) + (8 - (stopRoundTime - startRoundTime)) + GameManager.Instance.lvl)*luckmult;
    }

}
