using UnityEngine;

public class ChestPositionManager : MonoBehaviour
{
    public Transform[] chestPositions; // CheckPosition1~5 넣기

    public static ChestPositionManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public Transform GetChestPosition(int index)
    {
        return chestPositions[index];
    }
}