using UnityEngine;
using System.Collections.Generic;

public class ChestPositionManager : MonoBehaviour
{
    private List<Transform> chestPositions = new List<Transform>();

    public static ChestPositionManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            InitializePositions();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializePositions()
    {
        // Clear existing positions
        chestPositions.Clear();
        
        // Get all direct children as positions
        foreach (Transform child in transform)
        {
            chestPositions.Add(child);
        }

        // Log the number of positions found for debugging
        Debug.Log($"Initialized {chestPositions.Count} chest positions");
    }

    public Transform GetChestPosition(int index)
    {
        if (index < 0 || index >= chestPositions.Count)
        {
            Debug.LogError($"Invalid chest position index: {index}. Total positions: {chestPositions.Count}");
            return null;
        }
        return chestPositions[index];
    }

    // Optional: Add a method to get a random position
    public Transform GetRandomChestPosition()
    {
        if (chestPositions.Count == 0)
        {
            Debug.LogWarning("No chest positions available");
            return null;
        }
        return chestPositions[Random.Range(0, chestPositions.Count)];
    }

    // Optional: Add gizmos to visualize the positions in the editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        foreach (Transform pos in transform)
        {
            Gizmos.DrawWireSphere(pos.position, 0.5f);
        }
    }
}