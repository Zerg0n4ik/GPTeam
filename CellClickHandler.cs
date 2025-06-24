using UnityEngine;

public class CellClickHandler : MonoBehaviour
{
    void OnMouseDown()
    {
        if (ConstructionManager.Instance != null)
        {
            ConstructionManager.Instance.HandleCellClick(gameObject);
        }
        else
        {
            Debug.LogError("ConstructionManager.Instance is null!");
        }
    }
}