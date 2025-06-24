using UnityEngine;

public class ConstructionManager : MonoBehaviour
{
    public static ConstructionManager Instance;

    public enum Mode { None, Build, Upgrade, Insure }
    public Mode currentMode = Mode.None;
    
    [Header("Build Settings")]
    public GameObject factoryPrefab;
    public int buildCost = 5;
    
    [Header("Upgrade Settings")] 
    public int upgradeCost = 5;
    public int maxUpgradeLevel = 3;  // Можно изменить в инспекторе
    
    [Header("Insure Settings")]
    public int insureCost = 1;
    public int maxInsurance = 3;     // Можно изменить в инспекторе
    public int healthPerInsurance = 1; // Можно изменить в инспекторе

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void SetBuildMode() => currentMode = Mode.Build;
    public void SetUpgradeMode() => currentMode = Mode.Upgrade;
    public void SetInsureMode() => currentMode = Mode.Insure;
    public void CancelMode() => currentMode = Mode.None;

    public void HandleCellClick(GameObject cell)
    {
        switch (currentMode)
        {
            case Mode.Build:
                if (cell.transform.childCount == 0 && GameManager.Instance.SpendCoins(buildCost))
                {
                    AudioManager.Instance.PlaySound("build_factory");
                    Factory newFactory = Instantiate(factoryPrefab, cell.transform).GetComponent<Factory>();
                    newFactory.maxLevel = maxUpgradeLevel;
                    newFactory.maxInsurance = maxInsurance;
                    newFactory.healthPerInsurance = healthPerInsurance;
                }
                break;
                
            case Mode.Upgrade:
                if (cell.transform.childCount > 0)
                {
                    AudioManager.Instance.PlaySound("upgrade");
                    Factory factory = cell.GetComponentInChildren<Factory>();
                    if (factory != null && factory.level < factory.maxLevel && GameManager.Instance.SpendCoins(upgradeCost))
                    {
                        factory.Upgrade();
                    }
                }
                break;
                
            case Mode.Insure:
                if (cell.transform.childCount > 0)
                {
                    AudioManager.Instance.PlaySound("insurance");
                    Factory factory = cell.GetComponentInChildren<Factory>();
                    if (factory != null && factory.health < factory.maxInsurance && GameManager.Instance.SpendCoins(insureCost))
                    {
                        factory.Insure();
                    }
                }
                break;
        }
    }
}