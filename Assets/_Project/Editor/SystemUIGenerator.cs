#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.UI;
using Project.UI;
using RTLTMPro;
using Project.Data;
using TMPro;
using System.Reflection;
using UnityEngine.Events;

namespace Project.EditorScripts
{
    public class SystemUIGenerator : EditorWindow
    {
        private const string ARABIC_FONT_PATH = "Assets/RTLTMPro/Fonts/segoeui SDF Arabic.asset";
        private static TMP_FontAsset _arabicFont;

        private static readonly Color AccentBlue = new Color(0.20f, 0.50f, 0.90f);
        private static readonly Color AccentGreen = new Color(0.18f, 0.70f, 0.42f);
        private static readonly Color AccentOrange = new Color(0.90f, 0.55f, 0.15f);
        private static readonly Color AccentRed = new Color(0.85f, 0.25f, 0.25f);
        private static readonly Color BgDark = new Color(0.12f, 0.12f, 0.15f);
        private static readonly Color BgMedium = new Color(0.18f, 0.18f, 0.22f);
        private static readonly Color BgLight = new Color(0.24f, 0.24f, 0.28f);
        private static readonly Color BgCard = new Color(0.10f, 0.10f, 0.12f, 0.75f);

        [MenuItem("Tools/Project FF/Recreate System UI")]
        public static void RecreateUI()
        {
            Selection.activeObject = null;

            Canvas[] canvases = Object.FindObjectsOfType<Canvas>();
            foreach (var c in canvases)
            {
                if (c.name.Contains("SystemUI")) DestroyImmediate(c.gameObject);
            }

            _arabicFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(ARABIC_FONT_PATH);

            GameObject canvasGo = new GameObject("SystemUI_Generated", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            Canvas canvas = canvasGo.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;

            CanvasScaler scaler = canvasGo.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;

            MenuManager menu = canvasGo.AddComponent<MenuManager>();

            GameObject roleGo = GenerateRoleSelection(canvasGo.transform);
            GameObject ageGo = GenerateAgeEntry(canvasGo.transform);
            GameObject gateGo = GenerateParentGate(canvasGo.transform);
            GameObject dashGo = GenerateDashboard(canvasGo.transform);
            GameObject lockGo = GenerateSessionLock(canvasGo.transform);

            SetField(menu, "_roleSelectionPanel", roleGo);
            SetField(menu, "_ageEntryPanel", ageGo);
            SetField(menu, "_parentGatePanel", gateGo);
            SetField(menu, "_parentDashboardPanel", dashGo);
            SetField(menu, "_sessionLockPanel", lockGo);

            EditorUtility.SetDirty(menu);
            Debug.Log("<color=green>SUCCESS: UI regenerated with mobile layout + Arabic font!</color>");
        }

        private static GameObject GenerateRoleSelection(Transform parent)
        {
            GameObject panel = CreatePanel(parent, "RoleSelectionPanel", new Color(0.16f, 0.28f, 0.42f));
            RoleSelectionUI script = panel.AddComponent<RoleSelectionUI>();

            SetField(script, "_titleText", CreateText(panel.transform, "TitleText", "Who are you?",
                new Vector2(0, 500), 64, new Vector2(800, 150)));

            SetField(script, "_parentButton", CreateButton(panel.transform, "ParentButton", "Parent",
                new Vector2(0, 150), new Vector2(500, 180), 48, AccentBlue));
            SetField(script, "_childButton", CreateButton(panel.transform, "ChildButton", "Child",
                new Vector2(0, -100), new Vector2(500, 180), 48, AccentGreen));
            return panel;
        }

        private static GameObject GenerateAgeEntry(Transform parent)
        {
            GameObject panel = CreatePanel(parent, "AgeEntryPanel", new Color(0.38f, 0.26f, 0.14f));
            AgeEntryUI script = panel.AddComponent<AgeEntryUI>();

            SetField(script, "_titleText", CreateText(panel.transform, "TitleText", "How old are you?",
                new Vector2(0, 550), 64, new Vector2(800, 150)));

            int[] ages = { 3, 4, 5, 6, 7, 8 };
            float[] cols = { -250f, 0f, 250f };
            float[] rows = { 200f, -50f };

            Color[] ageColors = { AccentOrange, new Color(0.85f, 0.40f, 0.60f), AccentBlue,
                                  AccentGreen, new Color(0.70f, 0.40f, 0.85f), new Color(0.90f, 0.60f, 0.20f) };

            for (int i = 0; i < ages.Length; i++)
            {
                int age = ages[i];
                int row = i / 3;
                int col = i % 3;
                Button btn = CreateButton(panel.transform, age.ToString(), age.ToString(),
                    new Vector2(cols[col], rows[row]), new Vector2(200, 200), 64, ageColors[i]);

                string fieldName = $"_btnAge{age}";
                var field = typeof(AgeEntryUI).GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
                if (field != null) field.SetValue(script, btn);
            }

            SetField(script, "_sceneConfig", FindAsset<GameSceneConfig>("GameSceneConfig"));
            return panel;
        }

        private static GameObject GenerateParentGate(Transform parent)
        {
            GameObject panel = CreatePanel(parent, "ParentGatePanel", BgDark);
            ParentGateUI script = panel.AddComponent<ParentGateUI>();

            GameObject keypadObj = CreateSubPanel(panel.transform, "PinEntryPanel", BgMedium,
                new Vector2(0, 0), new Vector2(750, 1200));

            PinKeypadUI keypad = keypadObj.AddComponent<PinKeypadUI>();
            PinValidationView pinVal = keypadObj.AddComponent<PinValidationView>();

            SetField(keypad, "_pinDisplayText", CreateText(keypadObj.transform, "PinDisplay", "****",
                new Vector2(0, 450), 80, new Vector2(500, 120)));
            SetField(pinVal, "_errorText", CreateText(keypadObj.transform, "ErrorText", "Wrong PIN",
                new Vector2(0, 350), 36, new Vector2(500, 80)));
            SetField(pinVal, "_keypad", keypad);

            float startY = 180f;
            float rowSpacing = -180f;
            float colSpacing = 200f;

            for (int i = 1; i <= 9; i++)
            {
                int digit = i;
                int row = (i - 1) / 3;
                int col = (i - 1) % 3;
                float x = (col - 1) * colSpacing;
                float y = startY + row * rowSpacing;
                Button btn = CreateButton(keypadObj.transform, $"Btn{i}", i.ToString(),
                    new Vector2(x, y), new Vector2(160, 140), 56, BgLight);
                UnityEventTools.AddIntPersistentListener(btn.onClick, new UnityAction<int>(keypad.OnDigitPressed), digit);
            }

            Button zeroBtn = CreateButton(keypadObj.transform, "Btn0", "0",
                new Vector2(0, startY + 3 * rowSpacing), new Vector2(160, 140), 56, BgLight);
            UnityEventTools.AddIntPersistentListener(zeroBtn.onClick, new UnityAction<int>(keypad.OnDigitPressed), 0);

            Button cancelBtn = CreateButton(keypadObj.transform, "Cancel", "Cancel",
                new Vector2(0, -500), new Vector2(350, 100), 40, AccentRed);
            UnityEventTools.AddPersistentListener(cancelBtn.onClick, new UnityAction(script.OnCancelPressed));

            SetField(script, "_pinEntryPanel", keypadObj);
            SetField(script, "_pinValidation", pinVal);
            return panel;
        }

        private static GameObject GenerateDashboard(Transform parent)
        {
            GameObject panel = CreatePanel(parent, "ParentDashboardPanel", new Color(0.15f, 0.35f, 0.22f));
            ParentDashboardUI script = panel.AddComponent<ParentDashboardUI>();

            SetField(script, "_titleText", CreateText(panel.transform, "TitleText", "Dashboard",
                new Vector2(0, 820), 56, new Vector2(800, 120)));
            SetField(script, "_sceneConfig", FindAsset<GameSceneConfig>("GameSceneConfig"));
            ChildProgressData progressData = FindAsset<ChildProgressData>("ChildProgressData");
            SetField(script, "_childProgressData", progressData);

            // Parent Controls Panel — top area
            GameObject controlsObj = CreateSubPanel(panel.transform, "ParentControlsPanel", BgCard,
                new Vector2(0, 400), new Vector2(900, 600));
            ParentControlsPanel controls = controlsObj.AddComponent<ParentControlsPanel>();

            SetField(controls, "_timeLimitLabel", CreateText(controlsObj.transform, "TimeLabel", "Time Limit",
                new Vector2(0, 220), 32, new Vector2(500, 60)));
            SetField(controls, "_timeLimitSlider", CreateSlider(controlsObj.transform, "Slider",
                new Vector2(0, 150), new Vector2(700, 50)));
            SetField(controls, "_under6Button", CreateButton(controlsObj.transform, "Under6Btn", "Under 6",
                new Vector2(-200, 30), new Vector2(360, 100), 32, BgLight));
            SetField(controls, "_ages6PlusButton", CreateButton(controlsObj.transform, "Ages6Btn", "Ages 6+",
                new Vector2(200, 30), new Vector2(360, 100), 32, BgLight));
            SetField(controls, "_resetDataButton", CreateButton(controlsObj.transform, "ResetBtn", "Reset Data",
                new Vector2(-200, -100), new Vector2(360, 100), 32, AccentRed));
            SetField(controls, "_changePinButton", CreateButton(controlsObj.transform, "ChangePinBtn", "Change PIN",
                new Vector2(200, -100), new Vector2(360, 100), 32, AccentOrange));
            SetField(controls, "_currentPinLabel", CreateText(controlsObj.transform, "PinLabel", "Current PIN",
                new Vector2(0, -210), 32, new Vector2(400, 50)));

            GameObject inputObj = new GameObject("InputField");
            inputObj.transform.SetParent(controlsObj.transform, false);
            RectTransform inputRt = inputObj.AddComponent<RectTransform>();
            inputRt.anchoredPosition = new Vector2(0, -260);
            inputRt.sizeDelta = new Vector2(300, 70);
            TextMeshProUGUI inputText = inputObj.AddComponent<TextMeshProUGUI>();
            inputText.fontSize = 36;
            inputText.alignment = TextAlignmentOptions.Center;
            inputText.color = Color.white;
            TMP_InputField inputField = inputObj.AddComponent<TMP_InputField>();
            inputField.textComponent = inputText;
            inputField.characterLimit = 4;
            inputField.contentType = TMP_InputField.ContentType.IntegerNumber;
            SetField(controls, "_pinInput", inputField);
            SetField(script, "_parentControlsPanel", controls);

            // Stats widget — left
            GameObject catStatsObj = CreateSubPanel(panel.transform, "CategoryStatsWidget", BgCard,
                new Vector2(-240, -150), new Vector2(440, 420));
            CreateText(catStatsObj.transform, "EduLabel", "Edu", new Vector2(0, 140), 28, new Vector2(400, 60), Color.white, false);
            GameObject eduBar = CreateSubPanel(catStatsObj.transform, "EduBar", AccentBlue, new Vector2(0, 90), new Vector2(380, 40));
            eduBar.GetComponent<Image>().type = Image.Type.Filled;
            eduBar.GetComponent<Image>().fillMethod = Image.FillMethod.Horizontal;
            CreateText(catStatsObj.transform, "PedLabel", "Ped", new Vector2(0, 10), 28, new Vector2(400, 60), Color.white, false);
            GameObject pedBar = CreateSubPanel(catStatsObj.transform, "PedBar", AccentGreen, new Vector2(0, -40), new Vector2(380, 40));
            pedBar.GetComponent<Image>().type = Image.Type.Filled;
            pedBar.GetComponent<Image>().fillMethod = Image.FillMethod.Horizontal;
            CreateText(catStatsObj.transform, "EntLabel", "Ent", new Vector2(0, -120), 28, new Vector2(400, 60), Color.white, false);
            GameObject entBar = CreateSubPanel(catStatsObj.transform, "EntBar", AccentOrange, new Vector2(0, -170), new Vector2(380, 40));
            entBar.GetComponent<Image>().type = Image.Type.Filled;
            entBar.GetComponent<Image>().fillMethod = Image.FillMethod.Horizontal;
            CategoryStatsWidget catWidget = catStatsObj.AddComponent<CategoryStatsWidget>();
            SetField(catWidget, "_childProgressData", progressData);
            SetField(script, "_categoryStatsWidget", catWidget);

            // Session history — right
            GameObject seshHistObj = CreateSubPanel(panel.transform, "SessionHistoryWidget", BgCard,
                new Vector2(240, -150), new Vector2(440, 420));
            ScrollRect sr = seshHistObj.AddComponent<ScrollRect>();
            GameObject content = CreateSubPanel(seshHistObj.transform, "Content", Color.clear,
                new Vector2(0, 0), new Vector2(400, 400));
            sr.content = content.GetComponent<RectTransform>();
            sr.horizontal = false;
            sr.vertical = true;
            SessionHistoryWidget seshWidget = seshHistObj.AddComponent<SessionHistoryWidget>();
            SetField(seshWidget, "_childProgressData", progressData);
            SetField(script, "_sessionHistoryWidget", seshWidget);

            // Strength/weakness — bottom area
            GameObject strWeakObj = CreateSubPanel(panel.transform, "StrengthWeaknessIndicator", BgCard,
                new Vector2(0, -450), new Vector2(920, 120));
            StrengthWeaknessIndicator sw = strWeakObj.AddComponent<StrengthWeaknessIndicator>();
            RTLTextMeshPro indicatorText = CreateText(strWeakObj.transform, "IndicatorText", "Play some games to see your progress!",
                new Vector2(0, 0), 32, new Vector2(880, 100));
            SetField(sw, "_indicatorText", indicatorText);
            SetField(sw, "_childProgressData", progressData);
            SetField(script, "_strengthWeaknessIndicator", sw);

            // Insight text
            SetField(script, "_insightText", CreateText(panel.transform, "InsightText", "Insights...",
                new Vector2(0, -600), 32, new Vector2(900, 150)));

            // Bottom buttons
            Button backBtn = CreateButton(panel.transform, "BackButton", "Back",
                new Vector2(-230, -780), new Vector2(400, 120), 40, BgLight);
            Button playBtn = CreateButton(panel.transform, "PlayModeButton", "Play Mode",
                new Vector2(230, -780), new Vector2(400, 120), 40, AccentGreen);

            SetField(script, "_backButton", backBtn.gameObject);
            SetField(script, "_playModeButton", playBtn.gameObject);

            UnityEventTools.AddPersistentListener(backBtn.onClick, new UnityAction(script.OnBackToGamePressed));
            UnityEventTools.AddPersistentListener(playBtn.onClick, new UnityAction(script.SwitchToPlayerMode));

            return panel;
        }

        private static GameObject GenerateSessionLock(Transform parent)
        {
            GameObject panel = CreatePanel(parent, "SessionLockPanel", new Color(0.40f, 0.12f, 0.12f));
            SessionLockScreenUI script = panel.AddComponent<SessionLockScreenUI>();

            SetField(script, "_titleText", CreateText(panel.transform, "TitleText", "Play time is up for today!\nAsk your parent to unlock more time.",
                new Vector2(0, 150), 52, new Vector2(900, 250)));
            SetField(script, "_askParentButton", CreateButton(panel.transform, "AskParentButton", "Ask Parent",
                new Vector2(0, -150), new Vector2(500, 150), 48, AccentGreen));
            return panel;
        }

        // ─── Helpers ───

        private static GameObject CreatePanel(Transform parent, string name, Color color)
        {
            GameObject go = new GameObject(name, typeof(RectTransform), typeof(Image));
            go.transform.SetParent(parent, false);
            RectTransform rt = go.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
            go.GetComponent<Image>().color = color;
            return go;
        }

        private static GameObject CreateSubPanel(Transform parent, string name, Color color, Vector2 pos, Vector2 size)
        {
            GameObject go = new GameObject(name, typeof(RectTransform), typeof(Image));
            go.transform.SetParent(parent, false);
            RectTransform rt = go.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = pos;
            rt.sizeDelta = size;
            go.GetComponent<Image>().color = color;
            return go;
        }

        private static Button CreateButton(Transform parent, string name, string text, Vector2 pos, Vector2 size, float fontSize = 36, Color? buttonColor = null)
        {
            Color bg = buttonColor ?? BgLight;

            GameObject go = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
            go.transform.SetParent(parent, false);
            RectTransform rt = go.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = pos;
            rt.sizeDelta = size;

            Image img = go.GetComponent<Image>();
            img.color = bg;

            Button btn = go.GetComponent<Button>();
            ColorBlock cb = btn.colors;
            cb.normalColor = bg;
            cb.highlightedColor = bg * 1.2f;
            cb.pressedColor = bg * 0.8f;
            cb.selectedColor = bg;
            btn.colors = cb;

            CreateText(go.transform, "Text", text, Vector2.zero, fontSize, size, Color.white);
            return btn;
        }

        private static Slider CreateSlider(Transform parent, string name, Vector2 pos, Vector2 size)
        {
            GameObject go = new GameObject(name, typeof(RectTransform), typeof(Slider));
            go.transform.SetParent(parent, false);
            RectTransform rt = go.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = pos;
            rt.sizeDelta = size;

            Slider slider = go.GetComponent<Slider>();
            slider.minValue = 5f;
            slider.maxValue = 120f;
            slider.wholeNumbers = true;
            slider.value = 30f;

            // Background Track
            GameObject bg = new GameObject("Background", typeof(RectTransform), typeof(Image));
            bg.transform.SetParent(go.transform, false);
            RectTransform bgRt = bg.GetComponent<RectTransform>();
            bgRt.anchorMin = new Vector2(0, 0.25f);
            bgRt.anchorMax = new Vector2(1, 0.75f);
            bgRt.offsetMin = Vector2.zero;
            bgRt.offsetMax = Vector2.zero;
            bg.GetComponent<Image>().color = BgDark;

            // Fill Area
            GameObject fillArea = new GameObject("Fill Area", typeof(RectTransform));
            fillArea.transform.SetParent(go.transform, false);
            RectTransform fillAreaRt = fillArea.GetComponent<RectTransform>();
            fillAreaRt.anchorMin = new Vector2(0, 0.25f);
            fillAreaRt.anchorMax = new Vector2(1, 0.75f);
            fillAreaRt.offsetMin = new Vector2(10, 0);
            fillAreaRt.offsetMax = new Vector2(-10, 0);

            // Fill Bar
            GameObject fill = new GameObject("Fill", typeof(RectTransform), typeof(Image));
            fill.transform.SetParent(fillArea.transform, false);
            RectTransform fillRt = fill.GetComponent<RectTransform>();
            fillRt.anchorMin = Vector2.zero;
            fillRt.anchorMax = Vector2.one;
            fillRt.offsetMin = Vector2.zero;
            fillRt.offsetMax = Vector2.zero;
            fill.GetComponent<Image>().color = AccentGreen;

            slider.fillRect = fillRt;
            slider.targetGraphic = bg.GetComponent<Image>();
            return slider;
        }

        private static RTLTextMeshPro CreateText(Transform parent, string name, string text, Vector2 pos, float fontSize = 40, Vector2? size = null, Color? color = null, bool wordWrap = true)
        {
            Vector2 textSize = size ?? new Vector2(800, 100);
            Color textColor = color ?? Color.white;

            GameObject go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            RectTransform rt = go.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = pos;
            rt.sizeDelta = textSize;

            RTLTextMeshPro tmp = go.AddComponent<RTLTextMeshPro>();
            tmp.text = text;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.fontSize = fontSize;
            tmp.color = textColor;
            tmp.enableWordWrapping = wordWrap;

            if (_arabicFont != null)
                tmp.font = _arabicFont;

            return tmp;
        }

        private static void SetField(object target, string fieldName, object value)
        {
            var field = target.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
            if (field != null && value != null) field.SetValue(target, value);
        }

        private static T FindAsset<T>(string name) where T : ScriptableObject
        {
            string[] guids = AssetDatabase.FindAssets("t:" + typeof(T).Name);
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (path.Contains(name)) return AssetDatabase.LoadAssetAtPath<T>(path);
            }
            return null;
        }
    }
}
#endif
