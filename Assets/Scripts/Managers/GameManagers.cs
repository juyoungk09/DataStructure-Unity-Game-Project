using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI; 
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; } // 전역 접근용
    public Player player;
    public CameraController cameraController;
    public StageController stageController;
    public DialogueTrigger dialogueTrigger;
    public TMP_Text Skill1Text;
    public TMP_Text Skill2Text;
    public TMP_Text GameText;
    public TMP_Text GoldText;
    public Image AnASlotImage;
    public Image TapieSlotImage;
    public Color CheckColor = new Color(1f, 0.5f, 0.5f, 0.7f);
    public int maxHP;
    public int playerHP = 10;
    public int gold;
    public int stageCount = 0;
    public int[,] enemyCount = new int[,]
    {
        {4,5,6,0,9},
        {6,5,6,0,1},
        {6,6,6,0,1},
        {4,7,7,0,9},
        {6,7,7,0,1}
    };
    public bool isAnA = false;
    public bool isNotinCircle = true;
    public bool canModeChange = true;
    void Awake()
    {
        
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // 씬 전환 시 파괴 X
        AudioManager.Instance.PlayBackgroundMusic();
        if(dialogueTrigger != null) {
            dialogueTrigger.StartPrologue();
        }
    }
    void Update() {
        SkillManager.UpdateCooldowns();
        UpdateGameUI();
    }
    public void AddGold(int amount) {
        gold += amount;
    }
    public void UpdateGameUI() 
    {
        if(!isNotinCircle) {
            if(isAnA) {
                AnASlotImage.color = CheckColor;
                TapieSlotImage.color = Color.white;
            }
            else {
                TapieSlotImage.color = CheckColor;
                AnASlotImage.color = Color.white;
            }
        }
        else {
            AnASlotImage.color = Color.white;
            TapieSlotImage.color = Color.white;
        }
        if (Skill1Text != null)
        {
            if (GameManager.Instance.isNotinCircle)
            {
                Skill1Text.text = "스킬: 동아리 미가입";
            }
            else if (GameManager.Instance.isAnA)
            {
                float cd = SkillManager.GetCooldown(SkillManager.SkillType.AnASkill1);
                Skill1Text.text = cd > 0 ? 
                    $"<color=red>쿨타임: {cd:F1}초</color>" : 
                    "<color=green>스킬1: Z(급습)</color>";
            }
            else
            {
                float cd = SkillManager.GetCooldown(SkillManager.SkillType.TapieSkill1);
                Skill1Text.text = cd > 0 ? 
                    $"<color=red>쿨타임: {cd:F1}초</color>" : 
                    "<color=green>스킬1: Z(실드)</color>";
            }
        }

        if (Skill2Text != null)
        {
            if (GameManager.Instance.isNotinCircle)
            {
                Skill2Text.text = "스킬: 동아리 미가입";
            }
            else if (GameManager.Instance.isAnA)
            {
                float cd = SkillManager.GetCooldown(SkillManager.SkillType.AnASkill2);
                Skill2Text.text = cd > 0 ? 
                    $"<color=red>쿨타임: {cd:F1}초</color>" : 
                    "<color=green>스킬2: Ctrl(무적 대쉬)</color>";
            }
            else
            {
                float cd = SkillManager.GetCooldown(SkillManager.SkillType.TapieSkill2);
                Skill2Text.text = cd > 0 ? 
                    $"<color=red>쿨타임: {cd:F1}초</color>" : 
                    "<color=green>스킬2: Ctrl(랜덤 버프)</color>";
            }
        }
        GoldText.text = $"{gold}";
    }
    public int getRound() {
        return stageCount/5+1;   
    }
    public int getStage() {
        return stageCount%5+1;
    }
    public void OnEnemyDead()
    {
        enemyCount[getRound()-1, getStage()-1]--;

        if (enemyCount[getRound()-1, getStage()-1] <= 0)
        {
            StageClear();
        }
    }
    public void StageClear()
    {
        Debug.Log("스테이지 클리어!");
        if(stageCount==0) dialogueTrigger.StartChapter1();
        // EnemyManager.Instance.SpawnStage(stageCount);
        // UIManager.Instance.UpdateStageText(getRound(), getStage());
        stageController.EndRound();
        if(getStage() != 4){
            RewardManager.Instance.SpawnReward();
            StartCoroutine(StageClearText());
        }
    }
    
    public void StageUp() {
        stageCount++;
        Camera.main.GetComponent<CameraController>()?.UpdateCameraPosition();
        if(stageCount == 5) {
            isNotinCircle = false;
            isAnA=false;
        }
        if(stageCount == 10) {
            isNotinCircle = false;
            canModeChange = true;
        }
        if (getStage() == 4) {
            StageClear();
        }
        else {
            StartCoroutine(ShowRoundStageText());
        }
    }
    
    private IEnumerator ShowRoundStageText() {
        if (GameText != null) {
            GameText.text = $"Round: {getRound()}-{getStage()}";
            GameText.gameObject.SetActive(true);
            
            // Fade in
            float duration = 0.5f;
            float elapsed = 0f;
            Color startColor = new Color(GameText.color.r, GameText.color.g, GameText.color.b, 0);
            Color endColor = new Color(GameText.color.r, GameText.color.g, GameText.color.b, 1);
            
            // Fade in
            while (elapsed < duration) {
                GameText.color = Color.Lerp(startColor, endColor, elapsed / duration);
                elapsed += Time.deltaTime;
                yield return null;
            }
            GameText.color = endColor;
            
            // Wait for 1 second
            yield return new WaitForSeconds(1f);
            
            // Fade out
            elapsed = 0f;
            while (elapsed < duration) {
                GameText.color = Color.Lerp(endColor, startColor, elapsed / duration);
                elapsed += Time.deltaTime;
                yield return null;
            }
            GameText.color = startColor;
            GameText.gameObject.SetActive(false);
        }
    }
    public void ShowMessage(string str) {
        GameText.text = str;
    }
    private IEnumerator StageClearText() {
        if (GameText != null) {
            GameText.text = "Stage Clear!";
            GameText.gameObject.SetActive(true);
            
            // Fade in
            float duration = 0.5f;
            float elapsed = 0f;
            Color startColor = new Color(GameText.color.r, GameText.color.g, GameText.color.b, 0);
            Color endColor = new Color(GameText.color.r, GameText.color.g, GameText.color.b, 1);
            
            // Fade in
            while (elapsed < duration) {
                GameText.color = Color.Lerp(startColor, endColor, elapsed / duration);
                elapsed += Time.deltaTime;
                yield return null;
            }
            GameText.color = endColor;
            
            // Wait for 1 second
            yield return new WaitForSeconds(1f);
            
            // Fade out
            elapsed = 0f;
            while (elapsed < duration) {
                GameText.color = Color.Lerp(endColor, startColor, elapsed / duration);
                elapsed += Time.deltaTime;
                yield return null;
            }
            GameText.color = startColor;
            GameText.gameObject.SetActive(false);
        }
    }
}
