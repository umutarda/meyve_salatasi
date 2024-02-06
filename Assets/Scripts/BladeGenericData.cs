using UnityEngine;

public class BladeGenericData : MonoBehaviour
{
    [SerializeField] private float minCuttingVelocity = .001f;
	[SerializeField] private float lowPerpRatio = -0.2f;
	[SerializeField] private float highPerpRatio = 2.2f;

    public float MinCuttingVelocity => minCuttingVelocity;
    public float LowPerpRatio => lowPerpRatio;
    public float HighPerpRatio => highPerpRatio;

    public static BladeGenericData Instance;

    void Awake()=> Instance = this;
}
