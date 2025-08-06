using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using System.Collections.Generic;

public class OptionMenuManager : MonoBehaviour {
    public static OptionMenuManager Instance { get; private set; }
    public GameObject optionMenuPrefab;
    private GameObject currentMenu;
    private GameObject closeOverlay;

    void Awake() {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    public class OptionData {
        public string label;
        public UnityAction callback;
        public OptionData(string label, UnityAction callback) {
            this.label = label;
            this.callback = callback;
        }
    }

    public void ShowMenu(Vector3 worldPos, List<OptionData> options) {
        HideMenu();
        CreateOverlay();
        CreateMenu();
        PositionMenu(worldPos);
        PopulateMenu(options);
    }

    public void HideMenu() {
        if (currentMenu != null) Destroy(currentMenu);
        if (closeOverlay != null) Destroy(closeOverlay);
        currentMenu = null;
        closeOverlay = null;
    }

    private void CreateOverlay() {
        Canvas canvas = GetComponent<Canvas>();
        closeOverlay = new GameObject("OptionMenuOverlay", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        closeOverlay.transform.SetParent(canvas.transform, false);
        RectTransform rt = closeOverlay.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        Image img = closeOverlay.GetComponent<Image>();
        img.color = new Color(0f, 0f, 0f, 0f);
        Button btn = closeOverlay.GetComponent<Button>();
        btn.onClick.AddListener(HideMenu);
    }

    private void CreateMenu() {
        Canvas canvas = GetComponent<Canvas>();
        currentMenu = Instantiate(optionMenuPrefab, canvas.transform);
    }

    private void PositionMenu(Vector3 worldPos) {
        Canvas canvas = GetComponent<Canvas>();
        RectTransform canvasRect = canvas.transform as RectTransform;
        RectTransform menuRect = currentMenu.GetComponent<RectTransform>();
        menuRect.pivot = new Vector2(0f, 0.5f);
        Vector2 screenPoint = Camera.main.WorldToScreenPoint(worldPos);
        Vector3 worldPoint;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(canvasRect, screenPoint, null, out worldPoint);
        float gapWorld = 16f / canvas.scaleFactor;
        menuRect.position = worldPoint + new Vector3(gapWorld, 0f, 0f);
    }

    private void PopulateMenu(List<OptionData> options) {
        RectTransform menuRect = currentMenu.GetComponent<RectTransform>();
        Transform template = menuRect.Find("TemplateButton");
        if (template == null) return;
        GameObject templateButton = template.gameObject;
        foreach (var opt in options) {
            GameObject btnObj = Instantiate(templateButton, menuRect);
            btnObj.SetActive(true);
            TMP_Text tmp = btnObj.GetComponentInChildren<TMP_Text>();
            tmp.text = opt.label;
            Button btn = btnObj.GetComponent<Button>();
            btn.onClick.AddListener(() => { opt.callback.Invoke(); HideMenu(); });
        }
        templateButton.SetActive(false);
    }
}