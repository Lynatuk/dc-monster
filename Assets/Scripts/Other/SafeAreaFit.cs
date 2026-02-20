using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class SafeAreaFit : MonoBehaviour
{
    [SerializeField] bool isCentered;

    DeviceOrientation orientation = DeviceOrientation.Unknown;
    RectTransform rectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();

        if (!isCentered)
            InvokeRepeating(nameof(SafeArea), 0, 1f);
        else
            InvokeRepeating(nameof(CenteredSafeArea), 0, 1f);
    }

    void Update()
    {
        //Debug.Log(Input.deviceOrientation.);
        //if (Input.deviceOrientation != orientation)
        //{
        //    Debug.Log("Changed orientation :flushed: \n From " + Input.deviceOrientation + " to " + orientation);
        //    orientation = Input.deviceOrientation;
        //    SafeArea();
        //}
    }

    // Определение Safe Area, в котором никакие элементы телефона не перекроют контент. Например: вырез для камеры.
    void SafeArea()
    {
        Rect safeArea = Screen.safeArea;
        Vector2 anchorMin = safeArea.position;
        Vector2 anchorMax = safeArea.position + safeArea.size;

        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;

        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
    }

    public void DebugSafeArea(bool _isSafeArea, bool _isCentered)
    {
        RectTransform rect = GetComponent<RectTransform>();
        Rect safeArea;
        Vector2 anchorMin = new Vector2(), anchorMax = new Vector2();

        if (_isSafeArea)
        {
            safeArea = Screen.safeArea;

            if (!_isCentered)
            {
                anchorMin = safeArea.position;
                anchorMax = safeArea.position + safeArea.size;
            }
            else
            {
                anchorMin = new Vector2(Screen.currentResolution.width - safeArea.size.x, 0);
                anchorMax = safeArea.size;
            }
        }
        else
        {
            safeArea = new Rect(new Vector2(), new Vector2(Screen.currentResolution.width, Screen.currentResolution.height));
            anchorMin = safeArea.position;
            anchorMax = safeArea.position + safeArea.size;
        }

        anchorMin.x /= Screen.currentResolution.width;
        anchorMin.y /= Screen.currentResolution.height;
        anchorMax.x /= Screen.currentResolution.width;
        anchorMax.y /= Screen.currentResolution.height;

        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
    }

    void CenteredSafeArea()
    {
        Rect safeArea = Screen.safeArea;
        Vector2 anchorMin = new Vector2(Screen.width - safeArea.size.x, 0);
        Vector2 anchorMax = safeArea.size;

        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;

        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
    }
}
