using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]
public class ClickController : MonoBehaviour
{
    public delegate void PlayerDelegate();
    public static event PlayerDelegate OnPlayerDied;
    public static event PlayerDelegate OnPlayerScored;
    
    public float tapForce = 10;
    public float tiltSmooth = 5;
    public Vector3 startPos;



    public AudioSource flapAudio;
    public AudioClip[] flapClips;
    public int randomFlapSound;
    public AudioSource scoreAudio;
    public AudioClip[] scoreClips;
    public int randomScoreSound;
    public AudioSource dieAudio;
    public AudioClip[] dieClips;
    public int randomDieSound;

    


    Rigidbody2D rigidbody;
    Quaternion downRotation;
    Quaternion forwardRotation;

    GameManager game;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        downRotation = Quaternion.Euler(0,0,-90);
        forwardRotation = Quaternion.Euler(0,0,35);
        game = GameManager.Instance;
        rigidbody.simulated = false;
        flapClips = Resources.LoadAll<AudioClip>("FlapSounds");
        scoreClips = Resources.LoadAll<AudioClip>("ScoreSounds");
        dieClips = Resources.LoadAll<AudioClip>("CrashSounds");
    }

    void OnEnable() {
        GameManager.OnGameStarted += OnGameStarted;
        GameManager.OnGameOverConfirmed += OnGameOverConfirmed;    
    }
    void OnDisable() {
        GameManager.OnGameStarted -= OnGameStarted;
        GameManager.OnGameOverConfirmed -= OnGameOverConfirmed;    
        
    }
    void OnGameStarted(){
        rigidbody.velocity = Vector3.zero;
        rigidbody.simulated = true;
    }
    void OnGameOverConfirmed(){
        transform.localPosition = startPos;
        transform.rotation = Quaternion.identity;
    }



    // Update is called once per frame
    void Update()
    {
        if(game.GameOver) return;
        if(Input.GetMouseButtonDown(0)){
            randomFlapSound = Random.Range(0,2);
            flapAudio.PlayOneShot(flapClips[randomFlapSound]);
            
            flapAudio.Play();
            transform.rotation=forwardRotation;
            rigidbody.velocity= Vector3.zero;
            rigidbody.AddForce(Vector2.up * tapForce,ForceMode2D.Force);
        }


        transform.rotation= Quaternion.Lerp(transform.rotation,downRotation,tiltSmooth * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D collider) {
        if(collider.gameObject.tag == "ScoreZone"){
            //register score event
            OnPlayerScored(); // event sent to GameManager
            //play sound if not lazy
            randomScoreSound = Random.Range(0,2);
            scoreAudio.PlayOneShot(scoreClips[randomScoreSound]);

        }
        if(collider.gameObject.tag == "DeadZone"){
            rigidbody.simulated = false;
            //register dead event
            OnPlayerDied(); // Event sent to GameManager
            randomDieSound = Random.Range(0,3);
            dieAudio.PlayOneShot(dieClips[randomDieSound]);
            
            

        }
    }
}
