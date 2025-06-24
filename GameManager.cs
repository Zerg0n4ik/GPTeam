using UnityEngine;
using TMPro;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    [Header("Currency Settings")]
    public int startingCoins = 10;
    private int coins;
    private int totalCoinsEarned = 0;
    
    [Header("UI References")]
    public TextMeshProUGUI coinsText;
    public TextMeshProUGUI roundText;
    public GridManager gridManager;
    public TextMeshProUGUI finalScoreText;
    public GameObject gameOverPanel;
    
    private GameObject[,] grid;
    private int gridSize = 4;
    private int currentRound = 1;

    private void Awake()
    {
                AudioManager.Instance.PlayMusic(AudioManager.Instance.sounds.Find(s => s.name == "background_music").clip);  
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
        
        coins = startingCoins;
        totalCoinsEarned = startingCoins;
    }

    private void Start()
    {
        if (gridManager != null)
        {
            grid = gridManager.GetGrid();
            gridSize = gridManager.gridSize;
        }
        UpdateUI();
    }

    private void UpdateUI()
    {
        coinsText.text = $"Coins: {coins}";
        roundText.text = $"Round: {currentRound}";
    }
    
    public bool SpendCoins(int amount)
    {
        if (coins >= amount)
        {
            coins -= amount;
            UpdateUI();
            return true;
        }
        return false;
    }
    
    public void AddCoins(int amount)
    {
        coins += amount;
        totalCoinsEarned += amount;
        UpdateUI();
    }

    public void StartRound()
    {
        if (grid == null) return;
        StartCoroutine(RoundSequence());
    }

    private int GetCataclysmsCount()
    {
        if (currentRound >= 20) return 10;
        if (currentRound >= 18) return 8;
        if (currentRound >= 15) return 7;
        if (currentRound >= 12) return 6;
        if (currentRound >= 10) return 5;
        if (currentRound >= 7) return 4;
        if (currentRound >= 5) return 3;
        if (currentRound >= 3) return 2;
        return 1;
    }

    private IEnumerator RoundSequence()
    {
        Debug.Log($"Starting round {currentRound}");
        AudioManager.Instance.PlaySound("round_start");

        int cataclysmsCount = GetCataclysmsCount();
        Debug.Log($"Will execute {cataclysmsCount} cataclysms this round");

        for (int i = 0; i < cataclysmsCount; i++)
        {
            bool isFire = false;
            if (currentRound >= 3)
            {
                // После 3 раунда есть шанс на пожар (30%)
                isFire = Random.value < 0.3f;
            }

            if (isFire)
            {
                int startX = Random.Range(0, gridSize - 1);
                int startZ = Random.Range(0, gridSize - 1);
                Debug.Log($"Fire cataclysm #{i+1} at [{startX},{startZ}]");
                yield return CataclysmAnimation.Instance.PlayFireAnimation(grid, startX, startZ);
            }
            else
            {
                int lineIndex = Random.Range(0, gridSize * 2);
                bool isRow = lineIndex < gridSize;
                Debug.Log($"Tornado cataclysm #{i+1} at line {lineIndex} ({(isRow ? "row" : "column")})");
                yield return CataclysmAnimation.Instance.PlayTornadoAnimation(lineIndex, isRow, gridSize, grid);
            }

            // Небольшая пауза между катаклизмами
            if (i < cataclysmsCount - 1)
            {
                yield return new WaitForSeconds(1f);
            }
        }

        // Проверка на проигрыш
        if (currentRound >= 1 && GetFactoryCount() == 0)
        {
            GameOver();
            yield break;
        }

        // Начисление дохода
        int totalIncome = 0;
        foreach (var cell in grid)
        {
            if (cell.transform.childCount > 0)
            {
                Factory factory = cell.GetComponentInChildren<Factory>();
                if (factory != null)
                {
                    int income = factory.GetIncome();
                    totalIncome += income;
                }
            }
        }
        AddCoins(totalIncome);

        currentRound++;
        UpdateUI();
        AudioManager.Instance.PlaySound("storm_warning");
    }

    private int GetFactoryCount()
    {
        int count = 0;
        foreach (var cell in grid)
        {
            if (cell.transform.childCount > 0)
            {
                Factory factory = cell.GetComponentInChildren<Factory>();
                if (factory != null) count++;
            }
        }
        return count;
    }

    private void GameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            finalScoreText.text = $"Total Score: {totalCoinsEarned}";
        }
        Debug.Log($"Game Over! Final Score: {totalCoinsEarned}");
    }
}