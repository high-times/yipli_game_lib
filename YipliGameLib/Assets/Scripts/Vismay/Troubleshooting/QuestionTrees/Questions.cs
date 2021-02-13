using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Questions/QuestionList")]
public class Questions : ScriptableObject
{
    [SerializeField] List<QuestionModel> questions;

    [System.Serializable]
    public class QuestionModel
    {
        public int id;
        public string question;
        public string[] solutions;
        public string[] choices;
    }
}
