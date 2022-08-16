using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkySrcoll : MonoBehaviour
{
    private GameManager gameManager;
    [SerializeField] private float speed = 1;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager.isGameActive)
        {
            transform.Rotate(Vector3.right, Time.deltaTime * speed);
        }
    }
}
