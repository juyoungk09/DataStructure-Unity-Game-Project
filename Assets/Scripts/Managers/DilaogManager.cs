// using UnityEngine;
// using System.Collections;

// public class DialogueManager : MonoBehaviour
// {
//     public DialogueUI ui;
//     public Player player;

//     public void PlayDialogue(DialogueData dialogue)
//     {
//         if (dialogue == null) return;

//         if(dialogue.blockPlayerMovement)
//             player.canMove = false;

//         ui.ShowDialogue(dialogue.lines);

//         StartCoroutine(ReenableMovement(dialogue.lines.Count));
//     }

//     IEnumerator ReenableMovement(int linesCount)
//     {
//         yield return new WaitForSeconds(linesCount * 2f);
//         player.canMove = true;
//     }
// }
