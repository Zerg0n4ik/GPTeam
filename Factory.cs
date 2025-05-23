using UnityEngine;

public class Factory : MonoBehaviour
{
    public int level = 1;
    public int health = 1;
    public int maxInsurance = 3;
    
    public void Upgrade()
    {
        level++;
    }
    
    public void Insure()
    {
        if (health < maxInsurance) health++;
    }
    
    public void TakeDamage()
    {
        health--;
        if (health <= 0) Destroy(gameObject);
    }
    
    public int GetIncome()
    {
        return 5 + (level - 1);
    }
}