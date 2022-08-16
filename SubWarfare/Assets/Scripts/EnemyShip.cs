using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShip : MonoBehaviour
{
    private GameManager gameManager;
    public GameObject depthCharge, player;
    public List<Transform> waypoints;
    private Vector3 plyr;
    private int currentWaypointIndex = 0;
    private const int points = 25;
    public float speed, distanceToPlayer;
    private float timer = 0f;
    private readonly float reloadTime = 5f;
    public AudioClip torpedoHit, depthChargeDeploy;
    private AudioSource enemyAudio;
    public ParticleSystem explosionParticle;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        enemyAudio = GetComponent<AudioSource>();
        player = GameObject.Find("Player");
    }

    // 
    void Update()
    {
        if (gameManager.isGameActive)
        {
            CheckDistance();

            if (distanceToPlayer < 50 && distanceToPlayer >= 30)
            {
                Debug.Log("Detected by Surface Vessel");
                Patrol();
                DeployCharge(1);
            }
            else if (distanceToPlayer < 30)
            {
                ChasePlayer();
                DeployCharge(2);
            }
            else 
            { 
                Patrol(); 
            }
        }
    }

    // release depth charge
    void DeployCharge(int timerdelay) 
    {
        Vector3 offset = transform.position + new Vector3(0,-3,0);
        timer += Time.deltaTime;
        if (timer > reloadTime/timerdelay)
        {
            enemyAudio.PlayOneShot(depthChargeDeploy, 1);
            depthCharge = ObjectPooler.SharedInstance.GetPooledDepthCharges();
            if (depthCharge != null)
            {
                depthCharge.SetActive(true);
                depthCharge.transform.position = offset;
                timer = 0f;
            }
        }
    }
    
    public void CheckDistance() 
    {
        distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        plyr = player.transform.position;
    }

    // patrol along four points at random
    public void Patrol()
    {
        Transform wp = waypoints[currentWaypointIndex];
        wp.position = new Vector3(wp.transform.position.x, transform.position.y, wp.transform.position.z);

        if (Vector3.Distance(transform.position, wp.position) < 0.01f)
        {
            currentWaypointIndex = Random.Range(0, waypoints.Count); ;
        }
        else
        {
            transform.LookAt(wp);
            transform.position = Vector3.MoveTowards(
               transform.position, wp.position, speed * Time.deltaTime);
        }
    }
    public void ChasePlayer() 
    {
       
       plyr.y = transform.position.y;
        transform.LookAt(plyr);
        transform.position = Vector3.MoveTowards(
           transform.position, plyr, speed * Time.deltaTime);
    }
    public void PlayExplosionParticle()
    {
        explosionParticle.Play();
    }
    // Collisions
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Torpedo"))
        {
            enemyAudio.PlayOneShot(torpedoHit, 1);
            collision.gameObject.SetActive(false);
            PlayExplosionParticle();
            Destroy(gameObject, 2f);
            gameManager.UpdateScore(points);
        }
    }
}
