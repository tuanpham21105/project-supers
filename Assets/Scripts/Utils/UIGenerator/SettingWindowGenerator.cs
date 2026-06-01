using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public static class SettingWindowGenerator
{
    [MenuItem("Tools/UI/Create Setting Window")]
    public static void Create()
    {
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();

        if (canvas == null)
        {
            Debug.LogError("Canvas not found");
            return;
        }

        GameObject window = new("SettingWindow");
        window.transform.SetParent(canvas.transform, false);

        RectTransform windowRT = window.AddComponent<RectTransform>();

        windowRT.anchorMin = Vector2.zero;
        windowRT.anchorMax = Vector2.one;
        windowRT.offsetMin = Vector2.zero;
        windowRT.offsetMax = Vector2.zero;

        Image bg = window.AddComponent<Image>();
        bg.color = new Color(.15f, .15f, .15f, .95f);

        CreateScrollView(window.transform);

        Selection.activeGameObject = window;
    }

    private static void CreateScrollView(Transform parent)
    {
        GameObject scrollView = new("ScrollView");
        scrollView.transform.SetParent(parent, false);

        RectTransform rt = scrollView.AddComponent<RectTransform>();

        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        Image image = scrollView.AddComponent<Image>();
        image.color = new Color(0, 0, 0, .1f);

        ScrollRect scrollRect = scrollView.AddComponent<ScrollRect>();

        // Viewport

        GameObject viewport = new("Viewport");
        viewport.transform.SetParent(scrollView.transform, false);

        RectTransform viewportRT =
            viewport.AddComponent<RectTransform>();

        viewportRT.anchorMin = Vector2.zero;
        viewportRT.anchorMax = Vector2.one;
        viewportRT.offsetMin = Vector2.zero;
        viewportRT.offsetMax = Vector2.zero;

        Image viewportImage =
            viewport.AddComponent<Image>();

        viewportImage.color = new Color(1,1,1,.01f);

        Mask mask = viewport.AddComponent<Mask>();
        mask.showMaskGraphic = false;

        // Content

        GameObject content = new("Content");
        content.transform.SetParent(viewport.transform, false);

        RectTransform contentRT =
            content.AddComponent<RectTransform>();

        contentRT.anchorMin = new Vector2(0, 1);
        contentRT.anchorMax = new Vector2(1, 1);
        contentRT.pivot = new Vector2(.5f, 1);

        VerticalLayoutGroup layout =
            content.AddComponent<VerticalLayoutGroup>();

        layout.spacing = 10;
        layout.padding = new RectOffset(20, 20, 20, 20);

        layout.childControlWidth = true;
        layout.childControlHeight = true;

        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = false;

        ContentSizeFitter fitter =
            content.AddComponent<ContentSizeFitter>();

        fitter.verticalFit =
            ContentSizeFitter.FitMode.PreferredSize;

        scrollRect.viewport = viewportRT;
        scrollRect.content = contentRT;
        scrollRect.horizontal = false;

        CreateKeybindSection(content.transform);

        // Scrollbar

        GameObject scrollbarGO = new("Scrollbar Vertical");
        scrollbarGO.transform.SetParent(scrollView.transform, false);

        RectTransform sbRT =
            scrollbarGO.AddComponent<RectTransform>();

        sbRT.anchorMin = new Vector2(1, 0);
        sbRT.anchorMax = new Vector2(1, 1);
        sbRT.pivot = new Vector2(1, .5f);

        sbRT.sizeDelta = new Vector2(20, 0);

        Image sbImage =
            scrollbarGO.AddComponent<Image>();

        sbImage.color = new Color(.3f,.3f,.3f,1);

        Scrollbar scrollbar =
            scrollbarGO.AddComponent<Scrollbar>();

        scrollRect.verticalScrollbar = scrollbar;
    }

    private static void CreateKeybindSection(
        Transform content)
    {
        CreateHeader(content, "Keybinds");

        CreateKeybindRow(
            content,
            "Move Forward");

        CreateKeybindRow(
            content,
            "Move Backward");

        CreateKeybindRow(
            content,
            "Jump");
    }

    private static void CreateHeader(
        Transform parent,
        string text)
    {
        GameObject header = new($"Header - {text}");
        header.transform.SetParent(parent, false);

        Image image = header.AddComponent<Image>();

        image.color = new Color(.25f,.25f,.25f,1);

        LayoutElement le =
            header.AddComponent<LayoutElement>();

        le.preferredHeight = 60;

        GameObject label = new("Text");
        label.transform.SetParent(header.transform, false);

        TMP_Text tmp =
            label.AddComponent<TextMeshProUGUI>();

        tmp.text = text;
        tmp.fontSize = 28;
        tmp.alignment = TextAlignmentOptions.MidlineLeft;

        RectTransform rt =
            label.GetComponent<RectTransform>();

        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = new Vector2(15, 0);
        rt.offsetMax = Vector2.zero;
    }

    private static void CreateKeybindRow(
        Transform parent,
        string settingName)
    {
        GameObject row = new(settingName);
        row.transform.SetParent(parent, false);

        HorizontalLayoutGroup layout =
            row.AddComponent<HorizontalLayoutGroup>();

        layout.spacing = 10;
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = false;

        LayoutElement rowLayout =
            row.AddComponent<LayoutElement>();

        rowLayout.preferredHeight = 60;

        // Name

        GameObject name = new("Name");

        name.transform.SetParent(row.transform, false);

        LayoutElement nameLayout =
            name.AddComponent<LayoutElement>();

        nameLayout.preferredWidth = 250;

        TMP_Text nameText =
            name.AddComponent<TextMeshProUGUI>();

        nameText.text = settingName;
        nameText.alignment =
            TextAlignmentOptions.MidlineLeft;

        // Controls

        GameObject controls = new("Controls");
        controls.transform.SetParent(row.transform, false);

        HorizontalLayoutGroup controlsLayout =
            controls.AddComponent<HorizontalLayoutGroup>();

        controlsLayout.spacing = 10;

        CreateDropdown(
            controls.transform,
            "Click",
            new[]
            {
                "Click",
                "Double Click"
            });

        CreateDropdown(
            controls.transform,
            "Trigger",
            new[]
            {
                "Hold",
                "Press Once",
                "Any",
                "Toggle"
            });

        CreateButton(
            controls.transform,
            "Press Key");
    }

    private static void CreateDropdown(
        Transform parent,
        string name,
        string[] options)
    {
        GameObject go = new(name);
        go.transform.SetParent(parent, false);

        TMP_Dropdown dropdown =
            go.AddComponent<TMP_Dropdown>();

        dropdown.options.Clear();

        foreach (string option in options)
        {
            dropdown.options.Add(
                new TMP_Dropdown.OptionData(option));
        }

        LayoutElement le =
            go.AddComponent<LayoutElement>();

        le.preferredWidth = 180;
    }

    private static void CreateButton(
        Transform parent,
        string text)
    {
        GameObject buttonGO = new(text);

        buttonGO.transform.SetParent(parent, false);

        Image image =
            buttonGO.AddComponent<Image>();

        image.color = Color.white;

        Button button =
            buttonGO.AddComponent<Button>();

        LayoutElement le =
            buttonGO.AddComponent<LayoutElement>();

        le.preferredWidth = 160;

        GameObject label = new("Text");

        label.transform.SetParent(
            buttonGO.transform,
            false);

        TMP_Text tmp =
            label.AddComponent<TextMeshProUGUI>();

        tmp.text = text;
        tmp.alignment =
            TextAlignmentOptions.Center;

        RectTransform rt =
            label.GetComponent<RectTransform>();

        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }
}