using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [Header("NPC Portraits (없을 수 있음)")]
    public Sprite npcMinGiImg;       // 민기
    public Sprite npcJiYeonImg;      // 지연 쌤
    public Sprite npcSeongSuImg;     // 성수 쌤
    public Sprite npcGoAhYoonImg;    // 매점 아저씨
    public Sprite npcBossImg;        // 보스 / 해커톤 망상체
    public Sprite npcSenpaiImg;      // 동아리 선배
    private void CheckManager()
    {
        if (DialogueManager.Instance == null)
            Debug.LogError("DialogueManager instance is null! 대화를 시작할 수 없습니다.");
    }

    public void StartPrologue()
    {
        CheckManager();

        MyDialogueLine[] dialogue = new MyDialogueLine[]
        {
            new MyDialogueLine { speakerName = "나레이션", portrait = null, sentence = "김민기는 평범한 마음으로 등교한다."},
            new MyDialogueLine { speakerName = "나레이션", portrait = null, sentence = "하지만 학기 시작과 동시에 각 학년의 수행 일정이 발표되며 교내 방송이 울린다." },
            new MyDialogueLine { speakerName = "교내 방송", portrait = null, sentence = "수행평가가 곧 시작됩니다. 교실 지역 난이도: 하"},
            new MyDialogueLine { speakerName = "교내 방송", portrait = null, sentence = "복도 지역 난이도: 중"},
            new MyDialogueLine { speakerName = "교내 방송", portrait = null, sentence = "체육관 및 해커톤 구역 난이도: 상"},
            new MyDialogueLine { speakerName = "나레이션", portrait = null, sentence = "본인의 동아리 능력에 맞춰 안전하게 진행 바랍니다." },
            new MyDialogueLine { speakerName = "나레이션", portrait = null, sentence = "프롤로그 끝" }
        };

        DialogueManager.Instance.StartDialogue(dialogue);
    }

    public void StartChapter1()
    {
        CheckManager();

        MyDialogueLine[] dialogue = new MyDialogueLine[]
        {
            new MyDialogueLine { speakerName = "나레이션", portrait = null, sentence = "챕터 1. 교실 — 복도는 학습체들이 가장 많이 출몰하는 기본 구역이다."},
            new MyDialogueLine { speakerName = "나레이션", portrait = null, sentence = "수학 문제와 사회 개념이 엉켜 만들어진 약한 학습체들이 나타난다." },
            new MyDialogueLine { speakerName = "민기", portrait = npcMinGiImg, sentence = "처음엔 어색하지만, 곧 내가 배운 것들을 익히며 작은 수행들을 완수해 나가자." },
            new MyDialogueLine { speakerName = "동아리 선배", portrait = npcSenpaiImg, sentence = "민기? 네 능력은 아직 안정되지 않았어."},
            new MyDialogueLine { speakerName = "동아리 선배", portrait = npcSenpaiImg, sentence = "동아리 능력 중 하나를 선택해 전직해야 수행 난이도를 감당할 수 있어."},
            new MyDialogueLine { speakerName = "나레이션", portrait = null, sentence = "TAPIE(기사 타입) 또는 AnA(암살자 타입) 중 하나를 전직으로 선택하여 나아가자." }
        };

        DialogueManager.Instance.StartDialogue(dialogue);
    }

    public void StartChapter2()
    {
        CheckManager();

        MyDialogueLine[] dialogue = new MyDialogueLine[]
        {
            new MyDialogueLine { speakerName = "나레이션", portrait = null, sentence = "챕터 2. 복도" },
            new MyDialogueLine { speakerName = "지연 쌤", portrait = npcJiYeonImg, sentence = "회로가 틀리면? 당연히 폭발하지.그러니까 정확히 연결하고, 그 전에 이 학습체들부터 정리하고 오도록." },
            new MyDialogueLine { speakerName = "나레이션", portrait = null, sentence = "교실 스테이지는 논리회로 조각으로 만들어진 공격형 몬스터들이 등장한다.\n기본적인 패턴을 익히고, 회피와 반격을 하면서 전투 수행을 진행한다." },
            new MyDialogueLine { speakerName = "나레이션", portrait = null, sentence = "보스전에서는 지연 쌤이 학생들에게 직접 공격을 하는 것이 아니라,\n본인이 만든 평가장치로 논리회로 보스가 등장한다.\n클리어 후 지연 쌤은 민기의 성장 잠재력을 인정하며 스킬 해금을 허용한다." }
        };

        DialogueManager.Instance.StartDialogue(dialogue);
    }

    public void StartChapter3()
    {
        CheckManager();

        MyDialogueLine[] dialogue = new MyDialogueLine[]
        {
            new MyDialogueLine { speakerName = "성수 쌤", portrait = npcSeongSuImg, sentence = "전자파는 눈에 보이지 않지만, 너희의 실수는 분명히 보이지." },
            new MyDialogueLine { speakerName = "나레이션", portrait = null, sentence = "체육관 전체에 전자파 충격, 회전형 파동, 순간 전류를 내뿜는 전자 생체들이 출몰한다.\n민기의 조작 난이도도 최고점에 이른다.\n전직 능력(TAPIE/AnA)도 본격적으로 빛을 발하기 시작한다." }
        };

        DialogueManager.Instance.StartDialogue(dialogue);
    }

    public void StartChapter4()
    {
        CheckManager();

        MyDialogueLine[] dialogue = new MyDialogueLine[]
        {
            new MyDialogueLine { speakerName = "매점 아저씨", portrait = npcGoAhYoonImg, sentence = "1학기 때는 많이들 쓰러지지… 음료는 충분히 챙겨가라." },
            new MyDialogueLine { speakerName = "나레이션", portrait = null, sentence = "몬스터를 잡아서 얻은 재화를 사용해 검은 카페인 음료(스탯 증가), 하얀 카페인 음료(공속 증가), 마시는 비타민(체력 회복) 등을 구매하며 민기는 다음 스테이지를 준비한다." }
        };

        DialogueManager.Instance.StartDialogue(dialogue);
    }

    public void StartChapter5()
    {
        CheckManager();

        MyDialogueLine[] dialogue = new MyDialogueLine[]
        {
            new MyDialogueLine { speakerName = "나레이션", portrait = null, sentence = "✦ 챕터 5. 최종 구역 — 해커톤 홀\n최종 스테이지는 선린 1학년의 절대 수행평가, '해커톤'이다." },
            new MyDialogueLine { speakerName = "보스", portrait = npcBossImg, sentence = "해커톤 참가자들의 피로·스트레스·자료·아이디어 충돌이 합쳐져 생성된 거대한 망상체가 등장한다." },
            new MyDialogueLine { speakerName = "나레이션", portrait = null, sentence = "보스를 쓰러뜨리면 해커톤 홀 전체가 안정되고, 심사위원들이 들어와 말한다.\n수고했다. 이제야 해커톤을 시작할 수 있겠구나." },
            new MyDialogueLine { speakerName = "나레이션", portrait = null, sentence = "민기와 동료들은 서로 얼굴을 바라보며 작은 미소를 짓는다.\n■ 엔딩 — '1학기 종료'\n해커톤 보스를 제압한 후 민기는 조용한 복도로 돌아온다.\n학교는 다시 평범하게 보이지만, 그는 이제 알게 된다.\n선린에서의 1학기는 단순한 수업이 아니라, 학생들이 성장하는 '과정' 그 자체라는 것을.\n화면은 천천히 어두워지며 1학기 스토리는 마무리된다." }
        };

        DialogueManager.Instance.StartDialogue(dialogue);
    }
}
