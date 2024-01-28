using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Question", menuName = "QuestionData", order = 1)]
public class QuizDataScriotable : ScriptableObject
{
    public List<QuestionData> questions;
}
