using UnityEngine;
using UnityEngine.UI;

public class InsureButtonHandler : MonoBehaviour
{
    private Button insureButton;

    private void Start()
    {
        insureButton = GetComponent<Button>();
        insureButton.onClick.AddListener(SetInsureMode);
    }

    private void SetInsureMode()
    {
        if (ConstructionManager.Instance != null)
        {
            ConstructionManager.Instance.SetInsureMode();
            Debug.Log("Insure mode activated");
        }
        else
        {
            Debug.LogError("ConstructionManager instance not found!");
        }
    }
}