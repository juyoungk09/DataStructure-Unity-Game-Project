using UnityEngine;

[System.Serializable]
public class MyDialogueLine
{
    public string speakerName;
    public Sprite portrait;
    [TextArea] public string sentence;
}
