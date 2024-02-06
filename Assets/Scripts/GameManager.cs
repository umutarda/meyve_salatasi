using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager
{
    private GameData gameData;
    private FruitRound currentFRound;
    private Blade[] blades;
    private Transform[] spawnedFruits;

    private bool roundExecuting;
    private enum GameState { INIT, WAIT_FOR_ROUND_START_DELAY, BRING_QUESTION, GAME_ROUND, GAME_END }
    private GameState currentGameState;

    public Transform[] SpawnedFruits => spawnedFruits;
    public FruitRound CurrentFRound => currentFRound;

    public GameManager(GameData _gameData)
    {
        gameData = _gameData;
        if (Questions.Empty)
        {
            Questions.Read(gameData.QuestionsAsset.text);
        }

        gameData.QuestionViewer.showOptions = gameData.ChoiceImages == null || gameData.ChoiceImages.Length == 0;
        Questions.Reset(gameData.ChoiceImages == null || gameData.ChoiceImages.Length == 0);
        currentGameState = GameState.INIT;

    }
    public void GameStateMachine()
    {
        switch (currentGameState)
        {
            case GameState.INIT:

                gameData.QuestionViewer.question = null;
                blades = new Blade[gameData.PlayerData.Length];
                for (int i = 0; i < gameData.PlayerData.Length; i++)
                    blades[i] = gameData.PlayerData[i].blade;

                currentGameState = GameState.WAIT_FOR_ROUND_START_DELAY;

                break;
            case GameState.WAIT_FOR_ROUND_START_DELAY:
                if (gameData.RoundStartDelay != -1)
                    currentGameState = GameState.BRING_QUESTION;
                break;
            case GameState.BRING_QUESTION:

                if (currentFRound == null || currentFRound.IsSuccess)
                {
                    if (Questions.HasNextQuestion)
                        MoveNextQuestion();

                    else
                    {
                        currentGameState = GameState.GAME_END;
                        break;
                    }

                }

                else
                {
                    Question q = currentFRound.Question;
                    currentFRound.FruitMarks = GetAMarkSequence(currentFRound.FruitCount, q.OptionCount, q.CorrectOptionIndex);
                }

                
                gameData.QuestionViewer.question = currentFRound.Question;
                gameData.StartCoroutine(RoundNumerator());
                currentFRound.IsSuccess = false;
                currentGameState = GameState.GAME_ROUND;

                break;
            case GameState.GAME_ROUND:

                if (!roundExecuting)
                {
                    for (int i = 0; i < gameData.PlayerData.Length; i++)
                        gameData.PlayerData[i].combo = 0;

                    currentGameState = GameState.BRING_QUESTION;
                }

                else
                {
                    for (int i = 0; i < blades.Length; i++)
                        blades[i].ComboReset();

                    if (Input.touchSupported && blades.Length == 2)
                    {
                        bool isLeft = false, isRight = false;
                        int touchIndex = 0;
                        Touch[] touches = Input.touches;


                        while (!(isLeft && isRight) && touchIndex < touches.Length)
                        {
                            Touch t = touches[touchIndex++];
                            bool leftTouch = t.position.x / Screen.width < 0.5f;

                            if (!isLeft && leftTouch)
                            {
                                blades[0].BladeWithTouch(t.phase != TouchPhase.Ended, t.position);
                                isLeft = true;
                            }

                            else if (!isRight && !leftTouch)
                            {
                                blades[1].BladeWithTouch(t.phase != TouchPhase.Ended, t.position);
                                isRight = true;
                            }


                        }


                    }

                    else
                    {
                        bool isDown = blades[0].CurrentBladeState() == Blade.BladeState.WAIT ? Input.GetMouseButtonDown(0) : !Input.GetMouseButtonUp(0);

                        blades[0].BladeWithTouch(isDown, Input.mousePosition);
                    }

                }

                break;
            case GameState.GAME_END:

                int[] totalQuestionSuccess = new int[gameData.PlayerData.Length];
                int[] score = new int[gameData.PlayerData.Length];
                for (int i = 0; i < gameData.PlayerData.Length; i++)
                {
                    PlayerData playerData = gameData.PlayerData[i];
                    score[i] = playerData.score;
                    totalQuestionSuccess[i] = playerData.succeedQuestionCount;
                    playerData.endText = totalQuestionSuccess[i] + "/" + Questions.Count + " soruyu doğru cevapladın.";
                }

                if (gameData.PlayerData.Length == 2)
                {

                    if (totalQuestionSuccess[0] < totalQuestionSuccess[1])
                    {
                        gameData.PlayerData[0].endText = "KAYBETTİN!\n" + gameData.PlayerData[0].endText;
                        gameData.PlayerData[1].endText = "KAZANDIN!\n" + gameData.PlayerData[1].endText;
                    }

                    else if (totalQuestionSuccess[0] == totalQuestionSuccess[1])
                    {
                        if (score[0] < score[1])
                        {
                            gameData.PlayerData[0].endText = "KAYBETTİN!\n" + gameData.PlayerData[0].endText;
                            gameData.PlayerData[1].endText = "KAZANDIN!\n" + gameData.PlayerData[1].endText;
                        }

                        else if (score[0] == score[1])
                        {
                            gameData.PlayerData[0].endText = "BERABERE!\n" + gameData.PlayerData[0].endText;
                            gameData.PlayerData[1].endText = "BERABERE!\n" + gameData.PlayerData[1].endText;
                        }

                        else
                        {
                            gameData.PlayerData[0].endText = "KAZANDIN!\n" + gameData.PlayerData[0].endText;
                            gameData.PlayerData[1].endText = "KAYBETTİN!\n" + gameData.PlayerData[1].endText;
                        }
                    }

                    else
                    {
                        gameData.PlayerData[0].endText = "KAZANDIN!\n" + gameData.PlayerData[0].endText;
                        gameData.PlayerData[1].endText = "KAYBETTİN!\n" + gameData.PlayerData[1].endText;
                    }

                    
                }

                gameData.StartCoroutine(ReturnToMenu());

                break;
        }
    }


    private void MoveNextQuestion()
    {
        Question q = Questions.NextQuestion;

        int optionCount = q.OptionCount;
        float qRatio = ((Questions.Index + 1) * 1.0f) / Questions.Count;

        int fruitCount = Mathf.Max(optionCount, Mathf.RoundToInt(gameData.MaxFruitCount * qRatio));
        float targetDelay = Mathf.Lerp(gameData.MinMaxDelay.x, gameData.MinMaxDelay.y, 1 - qRatio);
        Vector2 delayInterval = qRatio < 0.5f ? new Vector2(targetDelay, gameData.MinMaxDelay.y) : new Vector2(gameData.MinMaxDelay.x, targetDelay);

        char correctOptionChar = (char)(65 + q.CorrectOptionIndex);

        char[] markSequence = GetAMarkSequence(fruitCount, optionCount, q.CorrectOptionIndex);

        currentFRound = new FruitRound(fruitCount, correctOptionChar, markSequence, delayInterval, q);
    }

    private char[] GetAMarkSequence(int fruitCount, int optionCount, int correctOptionIndex)
    {
        char[] markSequence = new char[fruitCount];
        //markSequence[Random.Range(0,fruitCount)] = correctOptionChar;
        int correctCounter = 0;
        int correctCount = fruitCount > optionCount ? fruitCount / 2 : 1;
        int falseOptionsIndexer = 0;
        char[] falseOptions = new char[optionCount - 1];

        for (int i = 0; i < optionCount; i++)
        {
            if (i < correctOptionIndex)
            {
                falseOptions[i] = (char)(65 + i);
            }

            else if (i == correctOptionIndex)
            {
                continue;
            }

            else
            {
                falseOptions[i - 1] = (char)(65 + i);
            }
        }

        for (int i = 0; i < fruitCount; i++)
        {
            if ((int)(markSequence[i]) == 0)
            {
                if (Random.value >= gameData.BombPrevalence)
                {
                    if (correctCounter < correctCount)
                    {
                        markSequence[i] = (char)(65 + correctOptionIndex);
                        correctCounter++;
                    }

                    else
                    {
                        if (falseOptionsIndexer < falseOptions.Length) 
                        {
                            markSequence[i] = falseOptions[falseOptionsIndexer];
                            falseOptionsIndexer++;
                        }

                        else 
                        {
                            markSequence[i] = falseOptions[Random.Range(0, falseOptions.Length)];
                        }
                        
                    }


                }

                else markSequence[i] = '-';
            }



        }

        for (int i = 0; i < fruitCount; i++)
        {
            int randPlace = Random.Range(i, fruitCount);
            char temp = markSequence[i];
            markSequence[i] = markSequence[randPlace];
            markSequence[randPlace] = temp;

        }



        return markSequence;

    }
    private IEnumerator RoundNumerator()
    {
        roundExecuting = true;

        int zDepth = 0;
        char[] fruitMarks = currentFRound.FruitMarks;
        int fruitCount = currentFRound.FruitCount;

        spawnedFruits = new Transform[gameData.PlayerData.Length * fruitCount];

        Transform spawnedFruitHolder = gameData.SpawnedFruitHolder;
        for (int i = 0; i < spawnedFruitHolder.childCount; i++)
        {
            GameObject.Destroy(spawnedFruitHolder.GetChild(i).gameObject);
        }
        for (int i = 0; i < fruitCount; i++)
        {
            GameObject spawnedFruit = GameObject.Instantiate(fruitMarks[i] == '-' ? gameData.BombPrefab : gameData.FruitPrefabs[Random.Range(0, gameData.FruitPrefabs.Length)], spawnedFruitHolder);
            spawnedFruit.SetActive(false);

            FruitData fruitData = spawnedFruit.GetComponent<FruitData>();


            if (gameData.ChoiceImages.Length > 0)
            {
                fruitData.image = gameData.ChoiceImages[fruitMarks[i] - 65];
                fruitData.mark = fruitMarks[i];
                fruitData.markVisibile = false;
            }

            else
            {
                fruitData.mark = fruitMarks[i];
                fruitData.markVisibile = fruitMarks[i] != '-';
            }


            spawnedFruits[i] = spawnedFruit.transform;

            Vector2 xSpawnPositionRange = gameData.MinMaxSpawnPosX;
            float xSpawnPosition = Random.Range(xSpawnPositionRange.x, xSpawnPositionRange.y);
            spawnedFruit.transform.position = new Vector3(xSpawnPosition, gameData.SpawnPosY, zDepth);
            zDepth += gameData.DistanceBetweenLayers;



            if (gameData.PlayerData.Length == 2)
            {
                GameObject copy = GameObject.Instantiate(spawnedFruit, spawnedFruitHolder);
                spawnedFruits[i + fruitCount] = copy.transform;
                copy.transform.position += Vector3.right * gameData.XDistanceBetweenPlayers;
                copy.name = spawnedFruit.name;
                copy.SetActive(false);
            }

        }
        for (int i = gameData.RoundStartDelay; i > 0; i--)
        {
            gameData.QuestionViewer.counter = i;
            SFXManager.Instance.PlaySFX("countdown");
            yield return new WaitForSecondsRealtime(1);
        }
        gameData.QuestionViewer.counter = 0;
        SFXManager.Instance.PlaySFX("start");

        for (int i = 0; i < fruitCount; i++)
        {
            Vector2 delayRange = currentFRound.DelayRange;
            float delay = Random.Range(delayRange.x, delayRange.y);
            yield return new WaitForSeconds(delay);

            Transform currentFruit = spawnedFruits[i];
            currentFruit.gameObject.SetActive(true);

            Rigidbody rb = currentFruit.GetComponent<Rigidbody>();

            Vector2 xSpeedRange = gameData.MinMaxSpeedX;
            Vector2 ySpeedRange = gameData.MinMaxSpeedY;
            Vector2 xSpawnPositionRange = gameData.MinMaxSpawnPosX;

            float xVelDirection = Mathf.Sign(0.5f * (xSpawnPositionRange.x + xSpawnPositionRange.y) - currentFruit.position.x);
            rb.velocity = Vector3.right * xVelDirection * Random.Range(xSpeedRange.x, xSpeedRange.y) +
                           Vector3.up * Random.Range(ySpeedRange.x, ySpeedRange.y);


            rb.angularVelocity = (Vector3.right - Vector3.forward) * Mathf.PI * xVelDirection * rb.velocity.magnitude * gameData.AngularLinearVelRatio;


            if (gameData.PlayerData.Length == 2)
            {
                Transform copy = spawnedFruits[i + fruitCount];
                copy.gameObject.SetActive(true);
                Rigidbody copyRB = copy.GetComponent<Rigidbody>();

                copyRB.velocity = rb.velocity;
                copyRB.angularVelocity = rb.angularVelocity;

            }

        }

        yield return new WaitForSecondsRealtime(gameData.RoundEndDelay);
        roundExecuting = false;
    }
    private IEnumerator ReturnToMenu()
    {
        yield return new WaitForSecondsRealtime(5);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex-1);

    }


}
