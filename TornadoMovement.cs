using UnityEngine;

public class TornadoMovement : MonoBehaviour
{
    public float moveSpeed = 2f;
    public Transform[] waypoints; // Массив клеток для перемещения
    private int currentWaypoint = 0;

    void Update()
    {
        if (waypoints.Length == 0) return;

        // Плавное перемещение к текущей клетке
        transform.position = Vector3.MoveTowards(
            transform.position,
            waypoints[currentWaypoint].position + Vector3.up * 2f,
            moveSpeed * Time.deltaTime
        );

        // Переключение на следующую клетку
        if (Vector3.Distance(transform.position, waypoints[currentWaypoint].position) < 0.1f)
        {
            currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
        }
    }
}