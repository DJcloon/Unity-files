using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine : MonoBehaviour
{
    private GameManager gameManager;
    private GameObject player;

    public int damage;
    private const int points = 5;
    public AudioClip detonation;
    private AudioSource mineAudio;
    public ParticleSystem explosionParticle;

    // Start is called before the first frame update
    private void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        player = GameObject.Find("Player");
        mineAudio = GetComponent<AudioSource>();

    }
    public void PlayExplosionParticle()
    {
        explosionParticle.Play();
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Torpedo"))
        {
            mineAudio.PlayOneShot(detonation, 1);
            PlayExplosionParticle();
            Destroy(gameObject, 1f);
            collision.gameObject.SetActive(false);
            gameManager.UpdateScore(points);
        }
        if (collision.gameObject.CompareTag("Player"))
        {// Collision with mines 
            mineAudio.PlayOneShot(detonation, 1);
            PlayExplosionParticle();
            Destroy(gameObject, 1f);
            player.GetComponent<PlayerController>().UpdateHealth(0, damage);
        }
    }
}
