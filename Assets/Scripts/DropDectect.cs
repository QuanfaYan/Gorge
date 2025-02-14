using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropDectect : MonoBehaviour
{
    private GameManager gameManager;
    private AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    /*
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Food")) 
        {
            Debug.Log("Food Drop");
            Destroy(collision.gameObject);
            gameManager.UpdateLife();
        }
    }*/

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Food"))
        {
            Debug.Log("Food Drop");
            audioSource.Play();
            Destroy(collision.gameObject);
            gameManager.UpdateLife(-1);
        }
    }
}
