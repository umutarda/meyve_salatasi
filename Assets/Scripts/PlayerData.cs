using UnityEngine;
using TMPro;

public class PlayerData : MonoBehaviour
{
    [SerializeField] private TMP_Text _endText;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text comboText;
    [SerializeField] private GameObject bladeTrailPrefab;
    [SerializeField] private Camera raycastCamera;
    [SerializeField] private UIParticle _wrongParticle;

    private Blade _blade;
    private bool[] questionSuccessStates;
    private int _score;
    private int _combo;
    public Blade blade => _blade;
    public UIParticle wrongParticle => _wrongParticle;
    public Vector3 comboTextPosition 
    {
        set=> comboText.transform.position = value;
    }
    public int succeedQuestionCount 
    {
        get 
        {
            int no=0;
            for (int i=0; i<questionSuccessStates.Length; i++) 
            {
                if(questionSuccessStates[i]) no++;
            }
            return no;
        }
    }
    public int hasSucceedAtIndex 
    {
        set => questionSuccessStates[value] = true;
    }
    public int score 
    {
        get => _score;

        set 
        {
            _score = Mathf.Max(value,0);
            scoreText.text = "" + _score;
        }
    }
    public string endText 
    {
        get=> _endText.text;
        set 
        {
            _endText.gameObject.SetActive(true);
            _endText.text = value;
        }
    }
    public int combo 
    {
        get => _combo;
        set
        {   
            _combo = Mathf.Max(value,0);
            comboText.gameObject.SetActive(_combo>0);
            if(_combo>0) comboText.text = "×" + _combo;
                
        } 
    }
   

    void Start() 
    {
        _blade = new Blade(BladeGenericData.Instance,this,bladeTrailPrefab,raycastCamera);
        questionSuccessStates = new bool[Questions.Count];
    } 

}
