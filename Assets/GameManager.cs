using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject stonePrefab;
    public Transform spawnPoint;

    public int stonesPerTeam = 4;
    public int totalRounds = 2;

    private int currentRound = 1;
    private int stonesThrown = 0;
    private int currentTeam = 0;

    private GameObject currentStone;
    public int totalStonesPerTeam = 4;

    

    void Start()
    {
        StartRound();
    }
    void StartRound()
    {
        Debug.Log("Starting Round " + currentRound);

        stonesThrown = 0;
        currentTeam = 0;

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

        stonesThrown++;
        currentTeam = 1 - currentTeam;
    }

    void EndRound()
    {
        Debug.Log("Round " + currentRound + " Complete");

        currentRound++;

        if (currentRound > totalRounds)
        {
            EndGame();
        }
        else
        {
            Invoke(nameof(StartRound), 2f);
        }
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
