using UnityEngine;

public class CellClickHandler : MonoBehaviour
{
    void OnMouseDown()
    {
        ConstructionManager.Instance.HandleCellClick(gameObject);
    }
}