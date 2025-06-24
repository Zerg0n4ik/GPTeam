using UnityEngine;

public class Factory : MonoBehaviour
{
    public int level = 1;
    public int health = 1;
    
    [Header("Settings")]
    public int maxLevel = 3;
    public int maxInsurance = 3;
    public int baseIncome = 5;
    public int incomePerLevel = 1;
    public int healthPerInsurance = 1;

    public void Upgrade()
    {
        if (level < maxLevel)
        {
            level++;
        }
    }
    
    public void Insure()
    {
        if (health < maxInsurance)
        {
            health += healthPerInsurance;
            if (health > maxInsurance) health = maxInsurance;
        }
    }
    
    public void TakeDamage()
    {
        if (this == null || gameObject == null) return;
        
        health--;
        Debug.Log($"Factory took damage! Health: {health}");
        
        if (health <= 0)
        {
            Debug.Log("Factory destroyed!");
            AudioManager.Instance.PlaySound("explosion");
            Destroy(gameObject); // Изменил DestroyImmediate на Destroy
        }
    }
    
    public int GetIncome()
    {
        return baseIncome + (level - 1) * incomePerLevel;
    }
}