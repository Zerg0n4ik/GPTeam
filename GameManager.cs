using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int coins = 10;
    public TextMeshProUGUI coinsText;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Update()
    {
        coinsText.text = $"Coins: {coins}";
    }

    public bool SpendCoins(int amount)
    {
        if (coins >= amount)
        {
            coins -= amount;
            return true;
        }
        return false;
    }

    public void AddCoins(int amount)
    {
        coins += amount;
    }
}

public void StartRound()
{
    // Выбираем случайную линию (ряд или колонку)
    int lineIndex = Random.Range(0, gridSize * 2);
    bool isRow = lineIndex < gridSize;

    for (int i = 0; i < gridSize; i++)
    {
        int x = isRow ? lineIndex : i;
        int z = isRow ? i : lineIndex - gridSize;

        if (grid[x, z].transform.childCount > 0)
        {
            Factory factory = grid[x, z].GetComponentInChildren<Factory>();
            factory.TakeDamage();
        }
    }

    // Начисляем доход
    foreach (var cell in grid)
    {
        if (cell.transform.childCount > 0)
        {
            AddCoins(cell.GetComponentInChildren<Factory>().GetIncome());
        }
    }
}

