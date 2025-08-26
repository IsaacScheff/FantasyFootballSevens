using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasScaler))]
public class CameraViewportFitter : MonoBehaviour {
    public int left = 360;
    public int right = 360;
    public int top = 120;
    public int bottom = 220;
    public Camera targetCamera;

    CanvasScaler scaler;

    void Awake() {
        scaler = GetComponent<CanvasScaler>();
        if (targetCamera == null) targetCamera = Camera.main;
        Apply();
    }

    public void Apply() {
        if (targetCamera == null || scaler == null) return;
        float w = scaler.referenceResolution.x;
        float h = scaler.referenceResolution.y;
        float x = left / w;
        float y = bottom / h;
        float width = 1f - (left + right) / w;
        float height = 1f - (top + bottom) / h;
        targetCamera.rect = new Rect(x, y, width, height);
    }
}