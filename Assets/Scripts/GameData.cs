using TMPro;
using UnityEngine;

public class GameData : MonoBehaviour
{
    [SerializeField] private PlayerData[] playerData;
    [SerializeField] private GameObject[] fruitPrefabs;
    [SerializeField] private Sprite[] choiceImages;
    [SerializeField] private GameObject bombPrefab;
    [SerializeField] private QuestionViewer questionViewer;
    [SerializeField] private Transform spawnedFruitHolder;
    [SerializeField] private int maxFruitCount;
    [SerializeField] private int roundStartDelay;
    [SerializeField] private int roundEndDelay;
    [SerializeField] private float spawnPosY;
    [SerializeField] private float bombPrevalence;
    [SerializeField] private float bombDestructionRange;
    [SerializeField] private float comboMaxDelayError;
    [SerializeField] private int distanceBetweenLayers;
    [SerializeField] private int xDistanceBetweenPlayers;
    [SerializeField] private float angularLinearVelRatio = 0.2f;
    [SerializeField] private Vector2 comboTextOffset = new Vector2(0.3f,.9f);
    [SerializeField] private Vector2 minMaxDelay = new Vector2(0.3f,.9f);
    [SerializeField] private Vector2 minMaxSpawnPosX = new Vector2(-7,7);
    [SerializeField] private Vector2 minMaxSpeedX = new Vector2(0.3f,1.8f);
    [SerializeField] private Vector2 minMaxSpeedY = new Vector2(10,13.5f);
    [SerializeField] private TextAsset questionsAsset;

    public Transform SpawnedFruitHolder => spawnedFruitHolder;
    public PlayerData[] PlayerData => playerData;
    public GameObject[] FruitPrefabs => fruitPrefabs;
    public GameObject BombPrefab => bombPrefab;
    public QuestionViewer QuestionViewer => questionViewer;
    public int MaxFruitCount => maxFruitCount;
    public int RoundStartDelay => roundStartDelay;
    public int RoundEndDelay => roundEndDelay;
    public float SpawnPosY => spawnPosY;
    public float BombPrevalence => bombPrevalence;
    public float BombDestructionRange => bombDestructionRange;
    public float ComboMaxDelayError => comboMaxDelayError;
    public int DistanceBetweenLayers => distanceBetweenLayers;
    public int XDistanceBetweenPlayers=> xDistanceBetweenPlayers;
    public float AngularLinearVelRatio => angularLinearVelRatio;
    public TextAsset QuestionsAsset => questionsAsset;
    public Vector2 ComboTextOffset => comboTextOffset;
    public Vector2 MinMaxDelay => minMaxDelay;
    public Vector2 MinMaxSpawnPosX => minMaxSpawnPosX;
    public Vector2 MinMaxSpeedX => minMaxSpeedX;
    public Vector2 MinMaxSpeedY => minMaxSpeedY;
    public Sprite[] ChoiceImages => choiceImages;

    public static GameData Instance;

    private GameManager gameManager;
    public GameManager GameManager => gameManager;
    



    void Awake() 
    {
        Instance = this;
        //roundStartDelay = -1;
        gameManager = new GameManager (this);
    }

    void Update() => gameManager.GameStateMachine();

    public void SetRoundStartDelay(Transform sender) 
    {
        roundStartDelay = int.Parse(sender.GetComponent<TMP_Text>().text);
        sender.parent.gameObject.SetActive(false);
    }

    public void MoveNextQuestionDebug() 
    {
        QuestionViewer.markCorrectAnswer = true;
        
        if(Questions.HasNextQuestion) 
            QuestionViewer.question = Questions.NextQuestion;
        else 
        {
            questionViewer.question = null;
            Questions.Reset(choiceImages == null || choiceImages.Length == 0);
        } 
            
    }

    
    
}
