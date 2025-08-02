using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class EnemyBehavior : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject player;
    public float moveSpeed;
    public float health;
    public float dmg;
    public float atkspd;
    private float lastAtkTime;
    public float atkRad;

    NavMeshAgent agent;
    void Start() 
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        lastAtkTime =Time.time;
        player = GameManager.Instance.player;
    }

    // Update is called once per frame
    void Update()
    {
        //Vector2 direction = (Vector2)(player.transform.position-transform.position).normalized;
        //transform.position += (Vector3)direction * moveSpeed * Time.deltaTime;
        
        agent.SetDestination(player.transform.position);
        //Debug.Log(Vector3.Distance(transform.position, player.transform.position));
        if (Vector3.Distance(transform.position, player.transform.position) < atkRad) 
        { 
            if (Time.time - lastAtkTime > atkspd)
            {
                lastAtkTime = Time.time;
                player.GetComponent<PlayerBehavior>().AlterHealth(-dmg);
            }

        }
        if (health <= 0)
        {
            Destroy(gameObject);
        }

    }

    public void AlterHealth(float diff)
    {
        health += diff;
    }


}
