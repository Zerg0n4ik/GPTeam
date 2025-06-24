using UnityEngine;
using UnityEngine.UI;

public class RoundButtonHandler : MonoBehaviour
{
    private Button button;

    private void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(StartNewRound);
    }

    private void StartNewRound()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.StartRound();
        }
        else
        {
            Debug.LogError("GameManager instance not found!");
        }
    }
}