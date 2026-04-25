using UnityEngine;
using TMPro;
//For lists
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public GameObject stonePrefab;
    public Transform spawnPoint;

    public int stonesPerTeam = 4;
    public int totalRounds = 2;

    private int currentRound = 1;
    private int stonesThrown = 0;
    private int currentTeam = 0;
    private string[] teamName = {"Blue", "Red"};
    private int redScore = 0;
    private int blueScore = 0;

    private GameObject currentStone;
    public int totalStonesPerTeam = 4;

    //Scoring
    public GameObject button;
    public GameObject ring4;
    public GameObject ring8;
    public GameObject ring12;
    
    //UI
    public TextMeshProUGUI scoreDisplay;
    public TextMeshProUGUI currentEndDisplay;

    void Start()
    {
        StartRound();
    }
    void StartRound()
    {
        Debug.Log("Starting Round " + currentRound);

        stonesThrown = 0;
        currentTeam = 0;

        updateRoundDisplay();
        updateScoreDisplay();

        CleanupStones();

        SpawnNextStone();
    }

    void CleanupStones()
{
    StoneController[] stones = FindObjectsOfType<StoneController>();

    foreach (StoneController stone in stones)
    {
        Destroy(stone.gameObject);
    }
}
    public void SpawnNextStone()
    {
        if (stonesThrown >= stonesPerTeam * 2)
        {
            EndRound();
            return;
        }

        currentStone = Instantiate(stonePrefab, spawnPoint.position, spawnPoint.rotation);

        Renderer r = currentStone.GetComponentInChildren<Renderer>();
        if (r != null)
        {
            r.material.color = (currentTeam == 0) ? Color.red : Color.blue;
        }
        currentStone.tag = (currentTeam == 0) ? "RedStone" : "BlueStone";

        stonesThrown++;
        currentTeam = 1 - currentTeam;
        updateRoundDisplay();
    }

    void updateRoundDisplay()
    {
        currentEndDisplay.text = string.Format("Round {0}\nTeam {1}", (currentRound), (teamName[currentTeam]));
    }

    void updateScoreDisplay()
    {
        scoreDisplay.text = string.Format("Scores\n Red {0}\n Blue {1}", (redScore), (blueScore));
    }

    void EndRound()
    {
        Debug.Log("Round " + currentRound + " Complete");

        scoring();

        currentRound++;
        updateRoundDisplay();

        if (currentRound > totalRounds)
        {
            EndGame();
        }
        else
        {
            Invoke(nameof(StartRound), 2f);
        }
    }

    void scoring()
    {
        //Lists to store all Red stones and Blue stones
        List<StoneController> redStones = new List<StoneController>();
        List<StoneController> blueStones = new List<StoneController>();

        StoneController[] stones = FindObjectsOfType<StoneController>();
        
        //Seperate stones
        foreach (StoneController stone in stones)
        {
            if(stone.tag == "RedStone")
            {
                redStones.Add(stone);
            }
            else
            {
                blueStones.Add(stone);
            }
        }

        //Check closest stone
        
        
        
        updateScoreDisplay();
    }

    void EndGame()
    {
        Debug.Log("Game Over!");
    }

    public void OnStoneStopped()
    {
        Invoke(nameof(SpawnNextStone), 0.5f); // short delay before next turn
    }
}
