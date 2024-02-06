using System.Xml;
using System;
using UnityEngine;

public static class Questions 
{
    private static int currentIndex;
    private static Question[] questions;

    public static bool HasNextQuestion => currentIndex < questions.Length;
    public static int Count=>questions.Length;
    public static int Index =>currentIndex-1;
    public static Question NextQuestion => questions[currentIndex++];
    public static bool Empty=>questions == null;
    public static void Read(string rawXMLText) 
    {
        XmlDocument questionsDoc = new XmlDocument();
        questionsDoc.LoadXml(rawXMLText);


        questions = new Question[questionsDoc.FirstChild.NextSibling.ChildNodes.Count];
        int optionCount = Int32.Parse(questionsDoc.FirstChild.NextSibling.Attributes["optionCount"].Value);

        XmlNode q = questionsDoc.FirstChild.NextSibling.FirstChild; //The first question
        int questionCounter = 0;

        while(q != null) 
        {
            XmlNode descriptionNode = q.FirstChild;
            string description = descriptionNode.InnerText.Trim(); //The first qu of the question
            string type = descriptionNode.Attributes["type"].Value;
            string[] options = new string[optionCount];
            int correctOptionIndex = -1;

            XmlNode aChoice = descriptionNode.NextSibling;
            for (int i=0; i<optionCount; i++) 
            {
                options[i] = aChoice.InnerText.Trim();
                
                if (aChoice.Attributes["value"] != null && aChoice.Attributes["value"].Value == "true")  correctOptionIndex = i;
                   
                aChoice = aChoice.NextSibling;
            }

           
            questions[questionCounter++] = new Question(description, options, correctOptionIndex);
            q = q.NextSibling; //The next question
        }

    }  
   
    public static void Reset(bool mixChoices) 
    {

        currentIndex = 0;

        for (int i=0; i<questions.Length; i++) 
	    {
		    int randPlace =  UnityEngine.Random.Range(i,questions.Length);
            Question temp = questions[i];
            questions[i] = questions[randPlace];
            questions[randPlace] = temp;

            if(mixChoices)
                questions[i].MixChoices();

        }
    }  


}

public class Question 
{
    private string description;
    private string description_type;
    private string[] options;
    private int correctOptionIndex;

    public string Description => description;
    public string[] Options => options;
    public int CorrectOptionIndex => correctOptionIndex;
    public int OptionCount => options.Length;
    public Question (string _description, string[] _options, int _correctOptionIndex) 
    {
        description = _description;
        options = _options;
        correctOptionIndex = _correctOptionIndex;
    }

    public void MixChoices() 
    {
        string correctOption = options[correctOptionIndex];
        for (int i=0; i<OptionCount; i++) 
	    {
		    int randPlace =  UnityEngine.Random.Range(i,OptionCount);
            string temp = options[i];
            options[i] = options[randPlace];
            options[randPlace] = temp;

        }

        for (int i=0; i<OptionCount; i++) 
	    {
		    if (options[i] == correctOption) 
            {
                correctOptionIndex = i;
                break;
            }

        }
    }



}
