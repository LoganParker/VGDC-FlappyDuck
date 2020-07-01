using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public delegate void GameDelegate();
    public static event GameDelegate OnGameStarted;
    public static event GameDelegate OnGameOverConfirmed;
    public GameObject startPage;
    public GameObject gameOverPage;
    public GameObject countDownPage;
    public int Score { get{return score;} } 
    public Text scoreText;

    enum PageState{
        None,
        Start,
        GameOver,
        CountDown
    }
    int score = 0;
    bool gameOver = true;

    public bool GameOver { get{return gameOver;} }

    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
        
    }
    void OnEnable() {
        CountdownText.OnCountdownFinished += OnCountdownFinished;
        ClickController.OnPlayerDied += OnPlayerDied;      
        ClickController.OnPlayerScored += OnPlayerScored;  
    }
    void OnDisable() {
        CountdownText.OnCountdownFinished -= OnCountdownFinished;
        ClickController.OnPlayerDied -= OnPlayerDied;      
        ClickController.OnPlayerScored -= OnPlayerScored;
    }
    void OnCountdownFinished(){
        SetPageState(PageState.None);
        OnGameStarted();
        score = 0;
        gameOver = false;
    }
    void OnPlayerDied(){
        gameOver = true;
        int savedScore = PlayerPrefs.GetInt("HighScore");
        if(score>savedScore){
            PlayerPrefs.SetInt("HighScore",score);
        }
        SetPageState(PageState.GameOver);
    }
    void OnPlayerScored(){
        score++;
        scoreText.text = score.ToString();
    }
    // Update is called once per frame
    void SetPageState(PageState state)
    {
        switch(state){
            case PageState.None:
                
                startPage.SetActive(false);
                gameOverPage.SetActive(false);
                countDownPage.SetActive(false);
            
                break;
            case PageState.Start:
                startPage.SetActive(true);
                gameOverPage.SetActive(false);
                countDownPage.SetActive(false);
                break;
            case PageState.GameOver:
                startPage.SetActive(false);
                gameOverPage.SetActive(true);
                countDownPage.SetActive(false);
                break;
            case PageState.CountDown:
                startPage.SetActive(false);
                gameOverPage.SetActive(false);
                countDownPage.SetActive(true);
                break;
        }
    }
    public void ConfirmGameOver(){
        //Activated on Replay button
        OnGameOverConfirmed(); //event
        scoreText.text = "0";
        SetPageState(PageState.Start);
    }
    public void StartGame(){
        //activated on Play button
        SetPageState(PageState.CountDown);
    }



}
