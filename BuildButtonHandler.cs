using UnityEngine;
using UnityEngine.UI;

public class BuildButtonHandler : MonoBehaviour
{
    private Button buildButton;

    private void Start()
    {
        buildButton = GetComponent<Button>();
        buildButton.onClick.AddListener(SetBuildMode);
    }

    private void SetBuildMode()
    {
        if (ConstructionManager.Instance != null)
        {
            ConstructionManager.Instance.SetBuildMode();
            Debug.Log("Build mode activated"); // Для отладки
        }
        else
        {
            Debug.LogError("ConstructionManager instance not found!");
        }
    }
}