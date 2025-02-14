using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerControler : MonoBehaviour
{
    [SerializeField] float horizontalInput;
    [SerializeField] float speed = 10.0f;
    [SerializeField] float maxRangeX = 6.0f;
    [SerializeField] float minRangeX = -6.0f;
    [SerializeField] Vector3 hugeScale = new Vector3(1.0f, 1.0f, 1.0f);
    [SerializeField] float fastRatio = 1.25f;
    [SerializeField] float decelerateRatio = 0.8f;
    [SerializeField] float effectDuration = 8.0f;
    public bool isInhale = false;
    public int capacity = 10;
    [SerializeField] int currentCount = 0;
    [SerializeField] int decelerateCount = 0;
    [SerializeField] float maxAngle = 45f;
    public GameObject trampoline;
    public AudioClip inhaleSound;
    public AudioClip getLifeSound;
    public AudioClip digestSound;
    private Animator playerAnimator;
    private AudioSource soundPlayer;
    private SpriteRenderer windSp;
    private Rigidbody2D playerRb;
    private GameManager gameManager;
    private ParticleSystem particle;
    private Vector2 newPosition;
    private Coroutine getHugeCoroutine = null;
    private Coroutine getFastCoroutine = null;
    private Coroutine getInhaleCoroutine = null;
    private Coroutine getTrampolineCoroutine = null;
    private Vector3 normalScale = new Vector3(0.8f, 0.8f, 0.8f);
    private float normalSpeed;
    private Animator heartAnimator;
    // Start is called before the first frame update
    void Start()
    {
        GameStart();
    }   

    void GameStart()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        windSp = GameObject.Find("Wind").GetComponent<SpriteRenderer>();
        heartAnimator = GameObject.Find("Heart").GetComponent<Animator>();
        //trampoline = GameObject.Find("Trampoline");
        //spRender = GetComponent<SpriteRenderer>();
        playerRb = GetComponent<Rigidbody2D>();
        playerAnimator = GetComponent<Animator>();
        particle = GetComponent<ParticleSystem>();
        soundPlayer = GetComponent<AudioSource>();
        normalSpeed = speed;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        newPosition = playerRb.position + Vector2.right * horizontalInput * speed * Time.deltaTime;
       
        if (newPosition.x > maxRangeX)
        {
            newPosition.x = maxRangeX;
        }   
        else if(newPosition.x < minRangeX)
        {
            newPosition.x = minRangeX;
        }

        if(horizontalInput > 0)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else if(horizontalInput < 0)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }

        playerRb.MovePosition(newPosition);
       
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.CompareTag("Food"))
        {
            Vector2 direction = (collision.gameObject.transform.position - transform.position).normalized;
            float collisionAngle = 0f;
            // looking right
            if (transform.rotation == Quaternion.Euler(0, 0, 0))
            {
                collisionAngle = Vector2.Angle(direction, new Vector2(1,1));
            }
            else
            {
                collisionAngle = Vector2.Angle(direction, new Vector2(-1, 1));
            }

            if (collisionAngle > maxAngle)
                return;
            string spriteName = collision.gameObject.GetComponent<SpriteRenderer>().sprite.name;
            Debug.Log("Eat food " + spriteName + " Angle:" + collisionAngle);
            if(!isInhale)
            {
                playerAnimator.SetBool("isEatting", false);
            }
            currentCount++;
            switch (spriteName)
            {
                case "apple":
                    soundPlayer.Play();
                    break;

                case "banana":
                    // get huge temporary
                    if (getHugeCoroutine != null)
                    {
                        StopCoroutine(getHugeCoroutine);
                    }
                    soundPlayer.Play();
                    getHugeCoroutine = StartCoroutine("GetHuge");
                    break;

                case "orange":
                    // get fast temporary
                    if (getFastCoroutine != null)
                    {
                        StopCoroutine(getFastCoroutine);
                        speed /= fastRatio;
                    }
                    soundPlayer.Play();
                    getFastCoroutine = StartCoroutine("GetFast");
                    break;

                case "strawberry":
                    soundPlayer.PlayOneShot(getLifeSound);
                    heartAnimator.Play("Showof",-1,0);
                    gameManager.UpdateLife(1);
                    break;
                case "pear":
                    soundPlayer.PlayOneShot(digestSound);
                    speed /= Mathf.Pow(decelerateRatio, decelerateCount);
                    currentCount = 0;
                    decelerateCount = 0;
                    break;
                case "blueberry":
                    if(getTrampolineCoroutine != null)
                    {
                        StopCoroutine(getTrampolineCoroutine);
                    }
                    soundPlayer.Play();
                    getTrampolineCoroutine = StartCoroutine("GetTrampoline");
                    break;
                case "grape":
                    // get inhale
                    if (getInhaleCoroutine != null)
                    {
                        StopCoroutine(getInhaleCoroutine);
                       soundPlayer.Play();
                    }
                    else 
                    {
                        soundPlayer.PlayOneShot(inhaleSound);
                    }
                    getInhaleCoroutine = StartCoroutine("GetInhale");
                    break;
                default:
                    break;
            }
            Destroy(collision.gameObject);
            if(currentCount != 0 && currentCount% capacity == 0)
            {
                Decelerate();
            }
            gameManager.UpdateCapacity(currentCount);
            gameManager.UpdateScore(1);
        }
        else
        {
            Debug.Log("Hit the wall");
        }
    }
    
    /*
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Food"))
        {
            playerAnimator.SetTrigger("Eat");
            Debug.Log("Ready to Eat food");
        }

    }*/

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Food"))
        {
            playerAnimator.SetBool("isEatting", true);
        }
    }
    /*
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Food"))
        {
            playerAnimator.SetBool("isEatting", false);
        }
    }*/
    IEnumerator GetHuge()
    {
        gameObject.transform.localScale = hugeScale;
        yield return new WaitForSeconds(effectDuration);
        gameObject.transform.localScale = normalScale;
        getHugeCoroutine = null;
    }

    IEnumerator GetFast()
    {
        speed *= fastRatio;
        particle.Play();
        yield return new WaitForSeconds(effectDuration);
        speed /= fastRatio;
        particle.Stop();
        getFastCoroutine = null;
    }

    IEnumerator GetInhale()
    {
        windSp.enabled = true;
        isInhale = true;
        yield return new WaitForSeconds(effectDuration);
        isInhale = false;
        getInhaleCoroutine = null;
        windSp.enabled = false;
    }

    IEnumerator GetTrampoline()
    {
        trampoline.SetActive(true);
        yield return new WaitForSeconds(effectDuration);
        trampoline.SetActive(false);
        getTrampolineCoroutine = null;
    }

    public void Decelerate()
    {
        decelerateCount++;
        speed *= decelerateRatio;
    }
}
