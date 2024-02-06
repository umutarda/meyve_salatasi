using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MenuAnimation : MonoBehaviour
{
    [SerializeField] private Transform initialPositionHolder;
    [SerializeField] private Transform finalPositionHolder;
    [SerializeField] private Ease animationEase;
    [SerializeField] private float duration;
    

    public void DOAnimation(out float _duration) 
    {
        Vector3 initPos = initialPositionHolder.position;
        Vector3 finalPos = finalPositionHolder.position;

        transform.position = initPos;
        transform.DOMove(finalPos,duration).SetEase(animationEase);
        _duration = duration;
    }

}
