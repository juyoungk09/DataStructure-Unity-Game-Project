using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [Header("UI")]
    public GameObject dialoguePanel;
    public TMP_Text nameText;
    public TMP_Text dialogueText;
    public Image portraitImage;
    public GameObject nextButton;  // ★ 버튼

    private MyDialogueLine[] currentLines;
    private int index = 0;

    void Awake()
{
    Instance = this;
    
    // Add click listener for the next button
    if (nextButton != null)
    {
        Button button = nextButton.GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(ShowNextLine);
        }
        else
        {
            Debug.LogError("NextButton is missing Button component!");
        }
    }
    else
    {
        Debug.LogError("NextButton reference is missing in the inspector!");
    }
}

    // 대화 시작
    public void StartDialogue(MyDialogueLine[] lines)
    {
        currentLines = lines;
        index = 0;

        dialoguePanel.SetActive(true);
        nextButton.SetActive(true); // ★ 버튼 활성화

        ShowNextLine();
    }

    public void ShowNextLine()
    {
        if (currentLines == null || index >= currentLines.Length)
        {
            EndDialogue();
            return;
        }

        MyDialogueLine line = currentLines[index];
        if (line == null)
        {
            Debug.LogError($"Dialogue line at index {index} is null!");
            index++;
            ShowNextLine(); // Skip to next line
            return;
        }

        nameText.text = line.speakerName ?? "";
        dialogueText.text = line.sentence ?? "";

        if (portraitImage != null)
        {
            if (line.portrait != null)
            {
                portraitImage.sprite = line.portrait;
                portraitImage.gameObject.SetActive(true);
            }
            else
            {
                portraitImage.gameObject.SetActive(false);
            }
        }

        index++;
    }

    public void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        nextButton.SetActive(false); // ★ 대화 끝나면 버튼 숨김
    }
}
