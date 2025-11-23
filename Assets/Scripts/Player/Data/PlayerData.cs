using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Data/Player Data")]
public class PlayerData : ScriptableObject
{
    [Header("Movement")]
    public float anaMoveSpeed = 8f;
    public float tapieMoveSpeed = 4f;

    [Header("Attack")]
    public float anaDamage = 5f;
    public float anaBackAttackMultiplier = 1.5f;

    [Header("Defense")]
    public float tapieDefense = 20f;
    public int tapieShieldHits = 3;

    [Header("Skills")]
    public float anaDashDistance = 4f;
    public float anaDashInvincibleTime = 0.3f;
}
