using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class DialogueUI : MonoBehaviour
{
    public GameObject panel;
    public TMP_Text dialogueText;

    private Coroutine typingCoroutine;

    public void ShowDialogue(List<string> lines)
    {
        panel.SetActive(true);

        if(typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeLines(lines));
    }

    IEnumerator TypeLines(List<string> lines)
    {
        foreach(var line in lines)
        {
            dialogueText.text = "";

            // 한 글자씩 타이핑 효과
            foreach(char c in line)
            {
                dialogueText.text += c;
                yield return new WaitForSeconds(0.02f);
            }

            // 클릭 또는 스페이스로 다음 줄
            bool next = false;
            while(!next)
            {
                if(Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
                    next = true;
                yield return null;
            }
        }

        panel.SetActive(false);
    }
}
