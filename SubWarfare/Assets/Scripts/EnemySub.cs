using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySub : MonoBehaviour
{
    private GameManager gameManager;
    public GameObject mine, torpedoPrefab, player;
    public List<Transform> waypoints;
    private Vector3 plyr;
    private int currentWaypointIndex = 0;
    private const int points = 15, reloadTime = 7;
    public float speed, distanceToPlayer; 
    private float timer;
    public AudioClip torpedoHit, deployMine;
    private AudioSource enemyAudio;
    public ParticleSystem explosionParticle;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        enemyAudio = GetComponent<AudioSource>();
        player = GameObject.Find("Player");


    }
    // Update is called once per frame
    void Update()
    {
        Bounds();
        if (gameManager.isGameActive) 
        {
            CheckDistance();

            if (distanceToPlayer < 50 && distanceToPlayer >= 30)
            {
                Debug.Log("Detected by Surface Vessel");
                Patrol();
                DeployMine(1);
            }
            else if (distanceToPlayer < 30)
            {
                ChasePlayer();
                FireTorpedo();
            }
            else
            {
                Patrol();
                DeployMine(.5f);
            }
        }
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
    public void CheckDistance()
    {
        distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        plyr = player.transform.position;
    }
    public void ChasePlayer()
    {

        plyr.y = transform.position.y;
        transform.LookAt(plyr);
        transform.position = Vector3.MoveTowards(
           transform.position, plyr, speed * Time.deltaTime);
    }
    void DeployMine(float timerDelay)
    {
         timer += Time.deltaTime;
        if (timer > reloadTime/timerDelay)
        {
            Vector3 offset = transform.position + new Vector3(0, -5, 0);
            Instantiate(mine, offset, mine.transform.rotation);
            enemyAudio.PlayOneShot(deployMine, 3);
            timer = 0f;
        }
    }
    void FireTorpedo()
    {
        timer += Time.deltaTime;
        if (timer > reloadTime)
        {
            torpedoPrefab = ObjectPooler.SharedInstance.GetPooledTorpedos();
            if (torpedoPrefab != null)
            {
                torpedoPrefab.SetActive(true);
                torpedoPrefab.transform.SetPositionAndRotation(transform.position + new Vector3(0, 0, 1.5f), transform.rotation.normalized);
            }
        timer = 0f;
    }
}
        public void PlayExplosionParticle()
    {
        explosionParticle.Play();
    }
    void Bounds()
    {
        if (transform.position.x > gameManager.xBounds)
        {
            transform.position = new Vector3(gameManager.xBounds, transform.position.y, transform.position.z);
        }
        if (transform.position.x < -gameManager.xBounds)
        {
            transform.position = new Vector3(-gameManager.xBounds, transform.position.y, transform.position.z);
        }
        if (transform.position.y > gameManager.yUpperBounds)
        {
            transform.position = new Vector3(transform.position.x, gameManager.yUpperBounds, transform.position.z);
        }
        if (transform.position.y < gameManager.yLowerBounds)
        {
            transform.position = new Vector3(transform.position.x, gameManager.yLowerBounds, transform.position.z);
        }
        if (transform.position.z > gameManager.zBounds)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, gameManager.zBounds);
        }
        if (transform.position.z < -gameManager.zBounds)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, -gameManager.zBounds);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Torpedo")) 
        {
            PlayExplosionParticle();
            collision.gameObject.SetActive(false);
            enemyAudio.PlayOneShot(torpedoHit, 1);
            Destroy(gameObject, 2f);
            gameManager.UpdateScore(points);
        }
    }
}


