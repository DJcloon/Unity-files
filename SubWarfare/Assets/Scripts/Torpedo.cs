using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Torpedo : MonoBehaviour
{

    private GameManager gameManager;
    private readonly float speed = 50.0f;
    public int damage;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(speed * Time.deltaTime * Vector3.forward);
        // Destroy object when outside of gmae area
        if (transform.position.x > gameManager.xBounds) 
        { 
            gameObject.SetActive(false); 
        }
        if (transform.position.z > gameManager.zBounds) 
        {
            gameObject.SetActive(false);
        }
        if (transform.position.x < -gameManager.xBounds) 
        {
            gameObject.SetActive(false);
        }
        if (transform.position.z < -gameManager.zBounds) 
        {
            gameObject.SetActive(false);
        }
    }
}
