using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindControl : MonoBehaviour
{
    private GameObject player;
    private PlayerControler playerControler;
    private Animator playerAnimator;
    [SerializeField] float inhalePower = 0.5f;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Pelican");
        playerControler = player.GetComponent<PlayerControler>();
        playerAnimator = player.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Food") && playerControler.isInhale)
        {
            Vector2 inhaleDirection = player.transform.position - collision.gameObject.transform.position;
            Vector2 inhaleForce = inhaleDirection.normalized * inhalePower / Mathf.Pow(inhaleDirection.magnitude,2);
            collision.gameObject.GetComponent<Rigidbody2D>().AddForce(inhaleForce, ForceMode2D.Impulse);
        }
    }
}
