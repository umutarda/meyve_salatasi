using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class QuestionViewer : MonoBehaviour
{
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private TMP_Text counterText;
    [SerializeField] private TMP_Text optionsText;

    public bool showOptions = true;
    public bool markCorrectAnswer = false;
    public Question question
    {
        set
        {
            if (value != null)
            {
                descriptionText.transform.parent.GetComponent<Image>().enabled = true;
                descriptionText.transform.parent.Find("Mask").gameObject.SetActive(true);

                descriptionText.text = value.Description;


                if (showOptions)
                {
                    optionsText.text = "";
                    string[] options = value.Options;
                    
                    for (int i = 0; i < options.Length; i++) 
                    {
                        if(markCorrectAnswer && i == value.CorrectOptionIndex)
                        {
                            optionsText.text += "<color=\"red\">";
                            optionsText.text += ((char)(65 + i)).ToString() + ") " + options[i] + "\n";
                            optionsText.text += "</color>";
                        }   
                            
                        else 
                        {
                            optionsText.text += ((char)(65 + i)).ToString() + ") " + options[i] + "\n";
                        }
                       
                    }
                       
                }

            }

            else
            {
                descriptionText.text = "";
                counter = 0;
                if(showOptions)
                    optionsText.text = "";
                descriptionText.transform.parent.GetComponent<Image>().enabled = false;
                descriptionText.transform.parent.Find("Mask").gameObject.SetActive(false);

            }


        }
    }
    public int counter
    {
        set => counterText.text = value > 0 ? "" + value : "";
    }

}
