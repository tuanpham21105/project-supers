using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public static class AccountWindowsGenerator
{
    [MenuItem("Tools/Create Account Windows")]
    public static void Create()
    {
        Canvas canvas = Object.FindObjectOfType<Canvas>();

        if (canvas == null)
        {
            GameObject canvasGO = new GameObject("Canvas");
            canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            canvasGO.AddComponent<CanvasScaler>();
            canvasGO.AddComponent<GraphicRaycaster>();
        }

        CreateLoginWindow(canvas.transform);
        CreateSignUpWindow(canvas.transform);
        CreateAccountWindow(canvas.transform);
        CreateGuestWindow(canvas.transform);
    }

    static GameObject CreateWindow(Transform parent, string title)
    {
        GameObject window = new GameObject(title);

        window.transform.SetParent(parent, false);

        Image bg = window.AddComponent<Image>();
        bg.color = new Color(.15f, .15f, .15f, .95f);

        RectTransform rt = window.GetComponent<RectTransform>();

        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        // Title Bar

        GameObject titleBar = new GameObject("TitleBar");
        titleBar.transform.SetParent(window.transform, false);

        Image titleBg = titleBar.AddComponent<Image>();
        titleBg.color = new Color(.1f,.1f,.1f,1);

        RectTransform titleRt = titleBar.GetComponent<RectTransform>();

        titleRt.anchorMin = new Vector2(0, .85f);
        titleRt.anchorMax = new Vector2(1, 1);
        titleRt.offsetMin = Vector2.zero;
        titleRt.offsetMax = Vector2.zero;

        GameObject titleText = new GameObject("Title");

        titleText.transform.SetParent(titleBar.transform, false);

        TMP_Text tmp = titleText.AddComponent<TextMeshProUGUI>();

        tmp.text = title;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.fontSize = 36;

        RectTransform textRt = titleText.GetComponent<RectTransform>();

        textRt.anchorMin = Vector2.zero;
        textRt.anchorMax = Vector2.one;
        textRt.offsetMin = Vector2.zero;
        textRt.offsetMax = Vector2.zero;

        GameObject content = new GameObject("Content");

        content.transform.SetParent(window.transform, false);

        RectTransform contentRt = content.AddComponent<RectTransform>();

        contentRt.anchorMin = Vector2.zero;
        contentRt.anchorMax = new Vector2(1,.85f);
        contentRt.offsetMin = Vector2.zero;
        contentRt.offsetMax = Vector2.zero;

        VerticalLayoutGroup layout = content.AddComponent<VerticalLayoutGroup>();

        layout.padding = new RectOffset(30,30,30,30);
        layout.spacing = 15;
        layout.childControlHeight = true;
        layout.childControlWidth = true;

        return window;
    }

    static TMP_InputField CreateInput(Transform parent, string placeholder)
    {
        GameObject go = new GameObject(placeholder);

        go.transform.SetParent(parent,false);

        Image image = go.AddComponent<Image>();
        image.color = Color.white;

        TMP_InputField input = go.AddComponent<TMP_InputField>();

        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(go.transform,false);

        TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();

        input.textComponent = text;

        GameObject placeholderObj = new GameObject("Placeholder");
        placeholderObj.transform.SetParent(go.transform,false);

        TextMeshProUGUI placeholderText =
            placeholderObj.AddComponent<TextMeshProUGUI>();

        placeholderText.text = placeholder;
        placeholderText.color = Color.gray;

        input.placeholder = placeholderText;

        return input;
    }

    static Button CreateButton(Transform parent,string text)
    {
        GameObject go = new GameObject(text);

        go.transform.SetParent(parent,false);

        Image image = go.AddComponent<Image>();

        Button button = go.AddComponent<Button>();

        GameObject label = new GameObject("Text");

        label.transform.SetParent(go.transform,false);

        TextMeshProUGUI tmp = label.AddComponent<TextMeshProUGUI>();

        tmp.text = text;
        tmp.alignment = TextAlignmentOptions.Center;

        return button;
    }

    static void CreateLoginWindow(Transform parent)
    {
        GameObject window = CreateWindow(parent,"Login");

        Transform content =
            window.transform.Find("Content");

        CreateInput(content,"Username");
        CreateInput(content,"Password");

        CreateButton(content,"Login");
        CreateButton(content,"Sign Up");
        CreateButton(content,"Continue As Guest");
    }

    static void CreateSignUpWindow(Transform parent)
    {
        GameObject window = CreateWindow(parent,"Sign Up");

        window.SetActive(false);

        Transform content =
            window.transform.Find("Content");

        CreateInput(content,"Username");
        CreateInput(content,"Password");
        CreateInput(content,"Email");

        CreateButton(content,"Sign In");
        CreateButton(content,"Login");
        CreateButton(content,"Continue As Guest");
    }

    static void CreateAccountWindow(Transform parent)
    {
        GameObject window = CreateWindow(parent,"Account");

        window.SetActive(false);

        Transform content =
            window.transform.Find("Content");

        CreateLabel(content,"Username");
        CreateLabel(content,"Created Date");
        CreateLabel(content,"Email");
        CreateLabel(content,"********");

        CreateButton(content,"Edit");
        CreateButton(content,"Logout");

        GameObject editPanel =
            new GameObject("EditPanel");

        editPanel.transform.SetParent(content,false);

        VerticalLayoutGroup layout =
            editPanel.AddComponent<VerticalLayoutGroup>();

        CreateInput(editPanel.transform,"Username");
        CreateInput(editPanel.transform,"Password");
        CreateInput(editPanel.transform,"Email");

        CreateButton(editPanel.transform,"Save");
        CreateButton(editPanel.transform,"Cancel");

        editPanel.SetActive(false);
    }

    static void CreateGuestWindow(Transform parent)
    {
        GameObject window =
            CreateWindow(parent,"Guest Account");

        window.SetActive(false);

        Transform content =
            window.transform.Find("Content");

        CreateInput(content,"Guest Id");

        CreateButton(content,"Login");
        CreateButton(content,"Sign Up");
    }

    static void CreateLabel(
        Transform parent,
        string text)
    {
        GameObject go =
            new GameObject(text);

        go.transform.SetParent(parent,false);

        TextMeshProUGUI tmp =
            go.AddComponent<TextMeshProUGUI>();

        tmp.text = text;
        tmp.fontSize = 28;
    }
}