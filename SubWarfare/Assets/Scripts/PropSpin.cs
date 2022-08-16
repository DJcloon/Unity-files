using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropSpin : MonoBehaviour
{
    private GameManager gameManager;
    [SerializeField] private float propellerSpeed = 1000;

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
            transform.Rotate(Vector3.forward, Time.deltaTime * propellerSpeed);
        }
    }
}
