#if UNITY_EDITOR
namespace Project.Editor
{
    using Project.UI;
    using RTLTMPro;
    using TMPro;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.UI;

    public static class UniversalMiniGameHUDSetupTool
    {
        private const string CANVAS_NAME = "MiniGameHUD_Canvas";
        private const string HUD_NAME = "UniversalMiniGameHUD";
        private const string ARABIC_FONT_PATH = "Assets/RTLTMPro/Fonts/segoeui SDF Arabic.asset";

        private static TMP_FontAsset _arabicFont;

        private static TMP_FontAsset GetArabicFont()
        {
            if (_arabicFont == null)
            {
                _arabicFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(ARABIC_FONT_PATH);
                if (_arabicFont == null)
                    Debug.LogWarning($"Arabic font not found at {ARABIC_FONT_PATH}");
            }
            return _arabicFont;
        }

        [MenuItem("Tools/Mini-Game HUD/Create Universal HUD", false, 10)]
        public static void CreateHUD()
        {
            GetArabicFont();

            Canvas canvas = GetOrCreateCanvas();
            GameObject hudObject = new GameObject(HUD_NAME, typeof(RectTransform));
            hudObject.transform.SetParent(canvas.transform, false);
            Undo.RegisterCreatedObjectUndo(hudObject, "Create HUD");

            UniversalMiniGameHUD hud = hudObject.AddComponent<UniversalMiniGameHUD>();

            RectTransform hudRect = hudObject.GetComponent<RectTransform>();
            hudRect.anchorMin = Vector2.zero;
            hudRect.anchorMax = Vector2.one;
            hudRect.offsetMin = Vector2.zero;
            hudRect.offsetMax = Vector2.zero;

            CreateScoreText(hudObject, hud);
            CreateTimerText(hudObject, hud);
            CreatePauseButton(hudObject, hud);
            CreatePauseOverlay(hudObject, hud);
            CreateSummaryOverlay(hudObject, hud);

            Selection.activeGameObject = hudObject;
            EditorUtility.SetDirty(canvas.gameObject);
            Debug.Log("Universal Mini-Game HUD created successfully!");
        }

        [MenuItem("Tools/Mini-Game HUD/Add HUD to Current Scene", true)]
        private static bool ValidateAddHUD()
        {
            return FindExistingHUD() == null;
        }

        [MenuItem("Tools/Mini-Game HUD/Add HUD to Current Scene", false, 11)]
        private static void AddHUDToScene()
        {
            if (FindExistingHUD() != null)
            {
                EditorUtility.DisplayDialog("HUD Exists", "A Universal Mini-Game HUD already exists in the scene.", "OK");
                return;
            }
            CreateHUD();
        }

        [MenuItem("Tools/Mini-Game HUD/Select HUD", false, 20)]
        private static void SelectHUD()
        {
            UniversalMiniGameHUD existing = FindExistingHUD();
            if (existing != null)
                Selection.activeGameObject = existing.gameObject;
            else
                EditorUtility.DisplayDialog("No HUD Found", "No UniversalMiniGameHUD found in the current scene.", "OK");
        }

        private static UniversalMiniGameHUD FindExistingHUD()
        {
            return Object.FindAnyObjectByType<UniversalMiniGameHUD>();
        }

        private static Canvas GetOrCreateCanvas()
        {
            Canvas canvas = Object.FindAnyObjectByType<Canvas>();
            if (canvas != null) return canvas;

            GameObject canvasGO = new GameObject(CANVAS_NAME, typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            Undo.RegisterCreatedObjectUndo(canvasGO, "Create Canvas");

            canvas = canvasGO.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            CanvasScaler scaler = canvasGO.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;

            if (Object.FindAnyObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
            {
                new GameObject("EventSystem", typeof(UnityEngine.EventSystems.EventSystem), typeof(UnityEngine.EventSystems.StandaloneInputModule));
            }

            return canvas;
        }

        private static GameObject CreateLabel(RectTransform parent, string name, string text, Vector2 anchoredPos, Vector2 sizeDelta)
        {
            GameObject labelGO = new GameObject(name, typeof(RTLTextMeshPro));
            labelGO.transform.SetParent(parent, false);
            RectTransform rt = labelGO.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 1f);
            rt.anchorMax = new Vector2(0.5f, 1f);
            rt.anchoredPosition = anchoredPos;
            rt.sizeDelta = sizeDelta;

            RTLTextMeshPro tmp = labelGO.GetComponent<RTLTextMeshPro>();
            tmp.text = text;
            tmp.fontSize = 36;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = Color.white;
            tmp.enableWordWrapping = false;
            if (_arabicFont != null)
                tmp.font = _arabicFont;

            return labelGO;
        }

        private static GameObject CreateButton(RectTransform parent, string name, string label, Vector2 anchoredPos, Vector2 sizeDelta)
        {
            GameObject btnGO = new GameObject(name, typeof(Image), typeof(Button));
            btnGO.transform.SetParent(parent, false);
            RectTransform rt = btnGO.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = anchoredPos;
            rt.sizeDelta = sizeDelta;

            Image img = btnGO.GetComponent<Image>();
            img.color = new Color(0.2f, 0.6f, 1f);

            Button btn = btnGO.GetComponent<Button>();
            ColorBlock colors = btn.colors;
            colors.normalColor = new Color(0.2f, 0.6f, 1f);
            colors.highlightedColor = new Color(0.3f, 0.7f, 1f);
            colors.pressedColor = new Color(0.1f, 0.5f, 0.9f);
            btn.colors = colors;

            GameObject textGO = new GameObject("Text", typeof(RTLTextMeshPro));
            textGO.transform.SetParent(rt, false);
            RectTransform textRT = textGO.GetComponent<RectTransform>();
            textRT.anchorMin = Vector2.zero;
            textRT.anchorMax = Vector2.one;
            textRT.offsetMin = Vector2.zero;
            textRT.offsetMax = Vector2.zero;

            RTLTextMeshPro tmp = textGO.GetComponent<RTLTextMeshPro>();
            tmp.text = label;
            tmp.fontSize = 28;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = Color.white;
            tmp.enableWordWrapping = false;
            if (_arabicFont != null)
                tmp.font = _arabicFont;

            return btnGO;
        }

        private static void CreateScoreText(GameObject root, UniversalMiniGameHUD hud)
        {
            GameObject go = CreateLabel(root.GetComponent<RectTransform>(), "ScoreText", "0", new Vector2(0, -60), new Vector2(300, 60));
            hud.SetScoreText(go.GetComponent<RTLTextMeshPro>());
            GameObject bestGo = CreateLabel(root.GetComponent<RectTransform>(), "BestScoreText", "0", new Vector2(0, -110), new Vector2(300, 40));
            bestGo.GetComponent<RTLTextMeshPro>().fontSize = 24;
            hud.SetBestScoreText(bestGo.GetComponent<RTLTextMeshPro>());
        }

        private static void CreateTimerText(GameObject root, UniversalMiniGameHUD hud)
        {
            GameObject go = CreateLabel(root.GetComponent<RectTransform>(), "TimerText", "0", new Vector2(0, -120), new Vector2(300, 50));
            go.GetComponent<RTLTextMeshPro>().fontSize = 28;
            hud.SetTimerText(go.GetComponent<RTLTextMeshPro>());
        }

        private static void CreatePauseButton(GameObject root, UniversalMiniGameHUD hud)
        {
            GameObject btnGO = new GameObject("PauseButton", typeof(Image), typeof(Button));
            btnGO.transform.SetParent(root.transform, false);
            RectTransform rt = btnGO.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(1f, 1f);
            rt.anchorMax = new Vector2(1f, 1f);
            rt.anchoredPosition = new Vector2(-60, -60);
            rt.sizeDelta = new Vector2(80, 80);

            Image img = btnGO.GetComponent<Image>();
            img.color = new Color(0, 0, 0, 0.3f);

            GameObject iconGO = new GameObject("Icon", typeof(RTLTextMeshPro));
            iconGO.transform.SetParent(rt, false);
            RectTransform iconRT = iconGO.GetComponent<RectTransform>();
            iconRT.anchorMin = Vector2.zero;
            iconRT.anchorMax = Vector2.one;
            iconRT.offsetMin = Vector2.zero;
            iconRT.offsetMax = Vector2.zero;

            RTLTextMeshPro icon = iconGO.GetComponent<RTLTextMeshPro>();
            icon.text = "| |";
            icon.fontSize = 36;
            icon.alignment = TextAlignmentOptions.Center;
            icon.color = Color.white;
            if (_arabicFont != null)
                icon.font = _arabicFont;

            Button btn = btnGO.GetComponent<Button>();
            ColorBlock colors = btn.colors;
            colors.normalColor = new Color(0, 0, 0, 0.3f);
            colors.highlightedColor = new Color(0, 0, 0, 0.5f);
            colors.pressedColor = new Color(0, 0, 0, 0.6f);
            btn.colors = colors;

            hud.SetPauseButton(btnGO.GetComponent<Button>());
        }

        private static void CreatePauseOverlay(GameObject root, UniversalMiniGameHUD hud)
        {
            GameObject overlay = new GameObject("PauseOverlay", typeof(Image));
            overlay.transform.SetParent(root.transform, false);
            RectTransform overlayRT = overlay.GetComponent<RectTransform>();
            overlayRT.anchorMin = Vector2.zero;
            overlayRT.anchorMax = Vector2.one;
            overlayRT.offsetMin = Vector2.zero;
            overlayRT.offsetMax = Vector2.zero;

            Image overlayImg = overlay.GetComponent<Image>();
            overlayImg.color = new Color(0, 0, 0, 0.7f);

            GameObject resumeBtn = CreateButton(overlayRT, "ResumeButton", "", Vector2.zero, new Vector2(300, 80));
            hud.SetPauseOverlay(overlay, resumeBtn.GetComponent<Button>());
            hud.SetResumeButtonText(resumeBtn.GetComponentInChildren<RTLTextMeshPro>());
        }

        private static void CreateSummaryOverlay(GameObject root, UniversalMiniGameHUD hud)
        {
            GameObject overlay = new GameObject("SummaryOverlay", typeof(Image));
            overlay.transform.SetParent(root.transform, false);
            RectTransform overlayRT = overlay.GetComponent<RectTransform>();
            overlayRT.anchorMin = Vector2.zero;
            overlayRT.anchorMax = Vector2.one;
            overlayRT.offsetMin = Vector2.zero;
            overlayRT.offsetMax = Vector2.zero;

            Image overlayImg = overlay.GetComponent<Image>();
            overlayImg.color = new Color(0, 0, 0, 0.7f);
            hud.SetSummaryOverlay(overlay);

            GameObject panel = new GameObject("SummaryPanel", typeof(Image));
            panel.transform.SetParent(overlayRT, false);
            RectTransform panelRT = panel.GetComponent<RectTransform>();
            panelRT.anchorMin = new Vector2(0.5f, 0.5f);
            panelRT.anchorMax = new Vector2(0.5f, 0.5f);
            panelRT.anchoredPosition = Vector2.zero;
            panelRT.sizeDelta = new Vector2(500, 520);

            Image panelImg = panel.GetComponent<Image>();
            panelImg.color = new Color(0.15f, 0.15f, 0.2f);

            GameObject titleLabel = CreateLabel(panelRT, "TitleLabel", "", Vector2.up * 80, new Vector2(400, 60));
            hud.SetSummaryTitleText(titleLabel.GetComponent<RTLTextMeshPro>());

            GameObject scoreLabel = CreateLabel(panelRT, "ScoreLabel", "", Vector2.up * 30, new Vector2(400, 50));
            hud.SetSummaryScoreText(scoreLabel.GetComponent<RTLTextMeshPro>());

            GameObject bestLabel = CreateLabel(panelRT, "BestScoreLabel", "", Vector2.down * 20, new Vector2(400, 50));
            bestLabel.GetComponent<RTLTextMeshPro>().fontSize = 28;
            hud.SetSummaryBestScoreText(bestLabel.GetComponent<RTLTextMeshPro>());

            GameObject coinsLabel = CreateLabel(panelRT, "CoinsLabel", "", Vector2.down * 70, new Vector2(400, 50));
            hud.SetSummaryCoinsText(coinsLabel.GetComponent<RTLTextMeshPro>());

            GameObject replayBtn = CreateButton(panelRT, "ReplayButton", "", Vector2.down * 140, new Vector2(300, 70));
            hud.SetReplayButton(replayBtn.GetComponent<Button>());
            hud.SetReplayButtonText(replayBtn.GetComponentInChildren<RTLTextMeshPro>());

            GameObject backBtn = CreateButton(panelRT, "BackToCityButton", "", Vector2.down * 210, new Vector2(300, 70));
            hud.SetBackToCityButton(backBtn.GetComponent<Button>());
            hud.SetBackToCityButtonText(backBtn.GetComponentInChildren<RTLTextMeshPro>());
            hud.SetSummaryPanel(panelRT);
        }
    }
}
#endif
