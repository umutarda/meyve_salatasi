using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;


public class FruitData : MonoBehaviour
{
    public bool explode 
    {
        set 
        {
            if(value) 
            {
                GameObject explosion = transform.Find("Explosion").gameObject;
                explosion.transform.SetParent(null);
				explosion.SetActive(true);
				GameObject.Destroy(explosion,1.2f);
            }
        }
    }

    public bool correctParticle 
    {
        set
        {
            if(value) 
            {
                GameObject correctParticle = transform.Find("CorrectParticle").gameObject;
                correctParticle.transform.SetParent(null);
				correctParticle.SetActive(true);
				GameObject.Destroy(correctParticle,2);
            }
        }
    }

    public bool colliderEnabled 
    {
        set => transform.GetComponent<SphereCollider>().enabled = value;
    }

    public Rigidbody firstBody => transform.GetChild(0).GetComponent<Rigidbody>();
    public Rigidbody secondBody => transform.GetChild(1).GetComponent<Rigidbody>();


   public char mark 
   {
        get => transform.Find("Mark").GetComponent<TMP_Text>().text[0];
        set => transform.Find("Mark").GetComponent<TMP_Text>().text = "" + value;
   
   }

   public Sprite image 
   {
        get => transform.Find("Sprite").GetComponent<SpriteRenderer>().sprite;
        set => transform.Find("Sprite").GetComponent<SpriteRenderer>().sprite = value;
   }

   public bool markVisibile 
   {
        set=> transform.Find("Mark").GetComponent<TMP_Text>().enabled = value;
   }
}
