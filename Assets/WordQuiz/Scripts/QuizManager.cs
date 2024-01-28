using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class QuizManager : MonoBehaviour
{
    public void OnResetButtonClick()
    {
        QuizManager.instance.ResetGameButton();
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public static QuizManager instance;

    [SerializeField] private GameObject gameover;
    [SerializeField]
    private QuizDataScriotable questionData;
    [SerializeField]
    private Image questionImage;
    [SerializeField]
    private WordData[] answerWordArray;
    [SerializeField]
    private WordData[] optionsWordArray;

    private char[] charArray = new char[12];
    private int currentAnswerIndex = 0;
    private bool correctAnswer = true;
    private List<int> selectedWordIndex;
    private int currentQuestionIndex = 0;
    private GameStatus gameStatus = GameStatus.Playing;
    private string answerWord;
    private int score = 0;

    private void Awake()
    {
        if (instance == null) instance = this;
        else
            Destroy(gameObject);

        selectedWordIndex = new List<int>();
    }

    private void Start()
    {
        SetQuestion();
    }
    private void SetQuestion()
    {


        currentAnswerIndex = 0;
        selectedWordIndex.Clear();

        if (questionData != null && questionData.questions != null && currentQuestionIndex < questionData.questions.Count)
        {
            if (questionImage != null)
            {
                questionImage.sprite = questionData.questions[currentQuestionIndex].questionImage;
            }
            else
            {
                Debug.LogError("questionImage is null. Make sure it is assigned in the Unity Editor.");
            }
        }

        answerWord = questionData.questions[currentQuestionIndex].answer;

        ResetQuestion();

        for (int i = 0; i < answerWord.Length; i++)
        {
            charArray[i] = char.ToUpper(answerWord[i]);
        }

        for (int i = answerWord.Length; i < optionsWordArray.Length; i++)
        {
            charArray[i] = (char)UnityEngine.Random.Range(65, 91);
        }

        charArray = ShuffleList.ShuffleListItems<char>(charArray.ToList()).ToArray();

        for (int i = 0; i < optionsWordArray.Length; i++)
        {
            optionsWordArray[i].SetChar(charArray[i]);
        }

        currentQuestionIndex++;
        gameStatus = GameStatus.Playing;
    }

    public WordData[] GetAnswerWordArray()
    {
        return answerWordArray;
    }

    public void SelectedOption(WordData wordData)
    {
        if (gameStatus == GameStatus.Next || currentAnswerIndex >= answerWord.Length) return;

        selectedWordIndex.Add(wordData.transform.GetSiblingIndex());
        answerWordArray[currentAnswerIndex].SetChar(wordData.charValue);
        wordData.gameObject.SetActive(false);
        currentAnswerIndex++;

        if (currentAnswerIndex >= answerWord.Length)
        {
            correctAnswer = true;

            for (int i = 0; i < answerWord.Length; i++)
            {
                if (char.ToUpper(answerWord[i]) != char.ToUpper(answerWordArray[i].charValue))
                {
                    correctAnswer = false;
                    break;
                }
            }

            if (correctAnswer)
            {
                gameStatus = GameStatus.Next;
                score += 50;

                Debug.Log("We have answeered correct" + score);

                if (currentQuestionIndex < questionData.questions.Count)
                {
                    Invoke("SetQuestion", 0.5f);
                }
                else
                {
                    gameover.SetActive(true);
                }
            }
            else if (!correctAnswer)
            {
                Debug.Log("We have not answeered correct");
            }
        }
    }

    private void ResetQuestion()
    {
        for (int i = 0; i < answerWordArray.Length; i++)
        {
            answerWordArray[i].gameObject.SetActive(true);
            answerWordArray[i].SetChar('_');
        }

        for (int i =  answerWord.Length; i < answerWordArray.Length; i++)
        {
            answerWordArray[i].gameObject.SetActive(false);
        }   

        for (int i = 0; i < optionsWordArray.Length; i++)
        {
           optionsWordArray[i].gameObject.SetActive(true);
        }
    }

    public void ResetGameButton()
    {
        currentQuestionIndex = 0;
        score = 0;
        gameStatus = GameStatus.Playing;
        gameover.SetActive(false);

        SetQuestion();
    }

    public void ResetLastWord()
    {
        if (selectedWordIndex.Count > 0)
        {
            int index = selectedWordIndex[selectedWordIndex.Count - 1];
            optionsWordArray[index].gameObject.SetActive(true);
            selectedWordIndex.RemoveAt(selectedWordIndex.Count - 1);
            currentAnswerIndex--;
            answerWordArray[currentAnswerIndex].SetChar('_');
        }
    }
}

[System.Serializable]

public class QuestionData
{
    public Sprite questionImage;
    public string answer;
}   

public enum GameStatus
{
    Playing,
    Next
}
