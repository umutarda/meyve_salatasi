using UnityEngine;
public class FruitRound 
{
    private int fruitCount;
    private char correctMark;
    private bool isSuccess;
    private Vector2 delayRange;
    private Question question;

    public int FruitCount => fruitCount;
    public char CorrectMark => correctMark;
    public Vector2 DelayRange => delayRange;
    public Question Question => question;
    public char[] FruitMarks;

    public bool IsSuccess 
    {
        get => isSuccess;
        set => isSuccess = value || isSuccess;

    }

    public FruitRound (int _fruitCount, char _correctMark ,char[] _fruitMarks, Vector2 _delayRange, Question _question)             
    {
        fruitCount = _fruitCount;
        correctMark = _correctMark;
        FruitMarks = _fruitMarks;
        delayRange = _delayRange;
        question = _question;
    }


}
