using UnityEngine;
using UnityEngine.UI;

public class HalfScreenWidthCamera : MonoBehaviour
{
    // The desired half-screen width in world units
    [SerializeField] private float halfScreenWidth = 10f;
    [SerializeField] private Image cameraBG;
    private Camera _camera;
    private RectTransform canvasRT;

    private void Awake()
    {
        // Get the reference to the camera component
        _camera = GetComponent<Camera>();
        canvasRT = FindObjectOfType<Canvas>().GetComponent<RectTransform>();
    }

    private void Start()
    {
        
        Adjust();

    }

    public void Adjust() 
    {
        if (!_camera || !canvasRT) return;

        // Calculate the orthographic size based on the desired half-screen width and the screen's aspect ratio
        float orthoSize = halfScreenWidth / _camera.aspect * 0.5f;

        // Set the camera's orthographic size
        _camera.orthographicSize = orthoSize;

        
        float imageRatio = 0.55f * canvasRT.sizeDelta.x / cameraBG.rectTransform.sizeDelta.x;
        cameraBG.rectTransform.sizeDelta *= imageRatio;
    }
}
