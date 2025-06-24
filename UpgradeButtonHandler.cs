using UnityEngine;
using UnityEngine.UI;

public class UpgradeButtonHandler : MonoBehaviour
{
    private Button upgradeButton;

    private void Start()
    {
        upgradeButton = GetComponent<Button>();
        upgradeButton.onClick.AddListener(SetUpgradeMode);
    }

    private void SetUpgradeMode()
    {
        if (ConstructionManager.Instance != null)
        {
            ConstructionManager.Instance.SetUpgradeMode();
            Debug.Log("Upgrade mode activated");
        }
        else
        {
            Debug.LogError("ConstructionManager instance not found!");
        }
    }
}