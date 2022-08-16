using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DepthCharge : MonoBehaviour
{
    private GameManager gameManager;
    private GameObject player;
    public int points;
    public int damage;
    private int delay;
    public AudioClip detonation;
    private AudioSource depchargeAudio;
    public ParticleSystem explosionParticle;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        player = GameObject.Find("Player");
        depchargeAudio = GetComponent<AudioSource>();
        this.GetComponent<AudioSource>().enabled = true;
    }
    // Update is called once per frame
    void Update()
    {
        StartCoroutine(DepthChargeTimer());
    }
    public void PlayExplosionParticle()
    {
        explosionParticle.Play();
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Torpedo"))
        {
            PlayExplosionParticle();
            depchargeAudio.PlayOneShot(detonation, 1);
            collision.gameObject.SetActive(false);
            gameObject.SetActive(false);
            gameManager.UpdateScore(points);
        }
        if (collision.gameObject.CompareTag("Player"))
        {// Collision with Depth Charges
            PlayExplosionParticle();
            gameObject.SetActive(false);
            player.GetComponent<PlayerController>().UpdateHealth(0, damage);
        }
    }

    public IEnumerator DepthChargeTimer()
    {
        delay = Random.Range(0, 4);
        yield return new WaitForSeconds(delay);
        depchargeAudio.PlayOneShot(detonation, 1);
        yield return new WaitForSeconds(2);
        PlayExplosionParticle();
        gameObject.SetActive(false);

    }
}
