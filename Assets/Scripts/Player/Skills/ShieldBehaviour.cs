using UnityEngine;

public class ShieldBehavior : MonoBehaviour
{
    public Player player;
    public int index;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("EnemyAttack"))
        {
            Destroy(other.gameObject); // 공격 제거

            // 쉴드 단계 감소
            if (player.shield > 0)
            {
                player.shield--;
                Destroy(this.gameObject);       // 맞은 쉴드 제거
                player.activeShields[index] = null; // 배열에서 제거
            }
        }
    }
}
