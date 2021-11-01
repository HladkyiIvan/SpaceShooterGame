using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUD : MonoBehaviour
{
    public static HUD Instance;

    public GameObject LivesPanel;
    public Text ScoreText;
    public Image EnergyBar;
    public GameObject EndGamePanel;
    public TextMeshProUGUI EndGameText;
    public TextMeshProUGUI EndGameScoreText;

    private int gameScore;

    public void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        gameScore = 0;

        foreach (Transform child in LivesPanel.transform)
        {
            child.gameObject.SetActive(true);
        }

        EndGamePanel.SetActive(false);
    }

    public void AddScore(int score)
    {
        gameScore += score;
        ScoreText.text = "Score: " + gameScore;
    }

    public void UpdateHPBar(int health, bool changeDirection)
    {
        LivesPanel.transform.GetChild(health).gameObject.SetActive(changeDirection);
    }

    public void UpdateEnergyBar(float fill)
    {
        EnergyBar.fillAmount = fill;
    }

    public void ShowLoseMessage()
    {
        EndGameText.text = "It's time to retreat, captain!";
        EndGameScoreText.text += gameScore;
        EndGamePanel.SetActive(true);
    }

    public void ShowWinMessage()
    {
        if (!EndGamePanel.activeSelf)
        {
            EndGameText.text = "Great job, captain!";
            EndGameScoreText.text += gameScore;
            EndGamePanel.SetActive(true);
        }
    }

    public void RestartPressed()
    {
        SpaceShip.Instance.Restart();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
