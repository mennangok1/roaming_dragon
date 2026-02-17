using UnityEngine;
using System.Collections;

public class Room : MonoBehaviour
{
    [SerializeField] private GameObject[] enemies;
    [SerializeField] private float resetCooldown = 2f;

    private Vector3[] initialPosition;

    private void Awake() {
        //save the initial position of all enemies

        initialPosition = new Vector3[enemies.Length];
        for ( int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i] != null)
            {
                initialPosition[i] = enemies[i].transform.position; 
            }
        }
    }

    public void ActivateRoom(bool status)
    {
        ResetRoomRoutine(status);
    }

    private IEnumerator ResetRoomRoutine(bool status)
    {
        yield return new WaitForSeconds(resetCooldown);
        for ( int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i] != null)
            {
                enemies[i].SetActive(status);
                enemies[i].transform.position = initialPosition[i];
            }
        }
    }
}
