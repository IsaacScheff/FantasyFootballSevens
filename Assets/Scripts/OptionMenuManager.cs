using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using System.Collections.Generic;

public class OptionMenuManager : MonoBehaviour {
    public static OptionMenuManager Instance { get; private set; }
    [Tooltip("Prefab with a Panel + TemplateButton (with TMP_Text) inside")]
    public GameObject optionMenuPrefab;

    GameObject currentMenu;

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
        if (currentMenu != null)
            Destroy(currentMenu);

        Canvas rootCanvas = GetComponent<Canvas>();
        RectTransform canvasRect = rootCanvas.transform as RectTransform;

        currentMenu = Instantiate(optionMenuPrefab, rootCanvas.transform);
        RectTransform menuRect = currentMenu.GetComponent<RectTransform>();

        menuRect.pivot = new Vector2(-0.1f, 0.5f);

        Vector2 screenPoint = Camera.main.WorldToScreenPoint(worldPos);
        Vector3 worldPointOnCanvas;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(
            canvasRect,
            screenPoint,
            null,
            out worldPointOnCanvas
        );

        float gapPx = 16f;
        float gapWorld = gapPx / rootCanvas.scaleFactor;
        menuRect.position = worldPointOnCanvas + new Vector3(gapWorld, 0f, 0f);

        Transform templateT = menuRect.Find("TemplateButton");
        if (templateT == null) {
            Debug.LogError("OptionMenuManager: 'TemplateButton' not found!");
            return;
        }
        GameObject templateButton = templateT.gameObject;

        foreach (var opt in options) {
            GameObject btnObj = Instantiate(templateButton, menuRect);
            btnObj.SetActive(true);

            var tmp = btnObj.GetComponentInChildren<TMP_Text>();
            if (tmp != null)
                tmp.text = opt.label;
            else
                Debug.LogError("OptionMenuManager: No TMP_Text found!");

            var btn = btnObj.GetComponent<Button>();
            btn.onClick.AddListener(() => {
                opt.callback.Invoke();
                HideMenu();
            });
        }

        templateButton.SetActive(false);
    }

    public void HideMenu() {
        if (currentMenu != null) Destroy(currentMenu);
    }
}
