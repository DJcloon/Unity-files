using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerController : MonoBehaviour
{
    private GameManager gameManager;
    public GameObject mine, depthCharge, torpedoPrefab;
    //Audio Variables
    [SerializeField] private AudioSource playerAudio;
    public AudioClip objectCollision, enemyCollision;
    //movement variables
    [SerializeField] private float speed = 5, riseSpeed = 5, turnSpeed = 5;
    private Rigidbody playerRb;
    //fire control variable
    public  bool isLoaded;
    public int numTorpedoLoaded = 0, numTorpedoInventory;
    private const int maxTorpedoLoad = 4, maxTorpedoInvetory = 20;
    private readonly float reloadTime = 2f;
    public TextMeshProUGUI torpedoText, inventoryText;
    float timer = 0f;
    //Player Stat variables
    public TextMeshProUGUI healthText;
    private readonly int maxHealth = 100;
    public int currentHealth;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        playerRb = GetComponent<Rigidbody>();
        UpdateHealth(100, 0);
        currentHealth = maxHealth;
        numTorpedoInventory = maxTorpedoInvetory;

    }
    // Update is called once per frame
    void Update()
    {
        PlayerBounds();
        // Automatic Forward Motion unless player enters shift
        UpdateHealth(0, 0);
        TorpedoReloader();
        //fire torpedo if player presses space and has torpedo loaded
        if (Input.GetKeyDown(KeyCode.Space) && isLoaded && gameManager.isGameActive)
        {
            FireTorpedo();
        }
    }
    void FixedUpdate()
    {
        if (gameManager.isGameActive)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                float reverseSpeed = speed / -3;
                playerRb.velocity = (reverseSpeed * transform.forward);
            }
            else
            {
                playerRb.velocity = (speed * transform.forward);
            }
        }
        // Rotates based on horizontal input else level off rotation
        if (gameManager.isGameActive)
        {
            transform.Translate(-Input.GetAxis("Vertical") * riseSpeed * Time.fixedDeltaTime * Vector3.up);
            transform.Rotate(Input.GetAxis("Horizontal") * Time.fixedDeltaTime * turnSpeed * transform.up);
        }   
    }
    // Fire Torpedo
    void FireTorpedo() 
    {
        torpedoPrefab = ObjectPooler.SharedInstance.GetPooledTorpedos();
        if (torpedoPrefab != null)
        {
            torpedoPrefab.SetActive(true);
            torpedoPrefab.transform.SetPositionAndRotation(transform.position + new Vector3(0,0,1.5f), transform.rotation.normalized);
        }
        UpdateTorpedoReadOut(0,1);
    }
    //Keep player in the Bounds
    void PlayerBounds()
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
    // Timed Auto Reloader for Torpedo
    void TorpedoReloader()
    {
        if (numTorpedoLoaded < maxTorpedoLoad && gameManager.isGameActive)
            {
                timer += Time.deltaTime;
                if (timer > reloadTime)
                {
                    UpdateTorpedoReadOut(1,0);
                    timer = 0f;
                }
            }
        if (numTorpedoLoaded >= 1)
        {
            isLoaded = true;
        }
        else if (numTorpedoLoaded < 1)
        {
            isLoaded = false;
        }
    }
    // Update and Dislpay number of loaded torpedos
    void UpdateTorpedoReadOut()
    {
        if (numTorpedoInventory == 0)
        {
            inventoryText.text = "No Torpedos Remaining";
            torpedoText.text = " Collect Supply Crate to reload Torpedos";
        }
        else
        {
            torpedoText.text = numTorpedoLoaded + " - Torpedos Loaded";
            inventoryText.text = numTorpedoInventory + " - Torpedos Remaining";
        }
    }
    void UpdateTorpedoReadOut(int loaded, int fired) 
    {
        numTorpedoLoaded += loaded;
        numTorpedoLoaded -= fired;
        numTorpedoInventory -= fired;
        UpdateTorpedoReadOut();
    }
    void UpdateTorpedoReadOut(int suppy) 
    {
        int plannedReload = numTorpedoInventory + suppy;
        if (plannedReload <= maxTorpedoInvetory) 
        {
            numTorpedoInventory += suppy;
        } else if (plannedReload > maxTorpedoInvetory)
        {
            numTorpedoInventory += maxTorpedoInvetory;
        }
        UpdateTorpedoReadOut();
    }
    // Update and Dislpay Health
    public void UpdateHealth(int heal, int damage) 
    {
        currentHealth += heal;
        if (currentHealth > 100) 
        {
            currentHealth = 100;
        }
        currentHealth -= damage;
        if (currentHealth < 1)
        {
            Debug.Log("Player has suffered too much damage to continue");
            gameManager.GameOver();
        }
        if (currentHealth > 85)
        {
            healthText.color = Color.green;
        }
        else if (currentHealth > 50 && currentHealth <= 85)
        {
            healthText.color = Color.yellow;
        }
        else if (currentHealth <= 50 && currentHealth > 0)
        {
            healthText.color = Color.red;
        }
        else
        {
            healthText.color = Color.black;
        }
            healthText.text = "Health: " + currentHealth;
    }
    // Collision
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Supply Crate"))
        {
            // Collection of Supply Crates
            Destroy(collision.gameObject);
            UpdateTorpedoReadOut(5);
            UpdateHealth(50,0);
        }
        if (collision.gameObject.CompareTag("Ground")) 
        {// Ground collision damage over time
            timer += Time.deltaTime;
            playerAudio.PlayOneShot(objectCollision, 2);
            if (timer > 1)
            {
                UpdateHealth(0, 1);
                timer = 0f;
            }
        }
        if (collision.gameObject.CompareTag("Rocks")) 
        {// Collision on rocks damage
            playerAudio.PlayOneShot(objectCollision, 2);
            UpdateHealth(0,5);
        }
        if (collision.gameObject.CompareTag("EnemySubs"))
        {// Collision damage
            playerAudio.PlayOneShot(enemyCollision, 2);
            UpdateHealth(0, 5);
        }
    }
}

