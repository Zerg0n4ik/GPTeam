using UnityEngine;

public class ConstructionManager : MonoBehaviour
{
    public enum Mode { None, Build, Upgrade, Insure }
    public Mode currentMode = Mode.None;
    public GameObject factoryPrefab;
    public int buildCost = 5;
    public int upgradeCost = 5;
    public int insureCost = 1;

    public void SetBuildMode() => currentMode = Mode.Build;
    public void SetUpgradeMode() => currentMode = Mode.Upgrade;
    public void SetInsureMode() => currentMode = Mode.Insure;

    public void HandleCellClick(GameObject cell)
    {
        switch (currentMode)
        {
            case Mode.Build:
                if (cell.transform.childCount == 0 && GameManager.Instance.SpendCoins(buildCost))
                {
                    Instantiate(factoryPrefab, cell.transform.position, Quaternion.identity, cell.transform);
                }
                break;
                
            case Mode.Upgrade:
                if (cell.transform.childCount > 0 && GameManager.Instance.SpendCoins(upgradeCost))
                {
                    cell.GetComponentInChildren<Factory>().Upgrade();
                }
                break;
                
            case Mode.Insure:
                if (cell.transform.childCount > 0 && GameManager.Instance.SpendCoins(insureCost))
                {
                    cell.GetComponentInChildren<Factory>().Insure();
                }
                break;
        }
        
        currentMode = Mode.None;
    }
}