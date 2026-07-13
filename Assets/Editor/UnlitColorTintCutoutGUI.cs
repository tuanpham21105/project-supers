// Đặt file này vào thư mục Assets/Editor/
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public class UnlitColorTintCutoutGUI : ShaderGUI
{
    private enum Mode
    {
        Opaque = 0,
        Cutout = 1,
        Transparent = 2
    }

    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        Material material = materialEditor.target as Material;

        MaterialProperty color   = FindProperty("_Color", properties);
        MaterialProperty mainTex = FindProperty("_MainTex", properties);
        MaterialProperty mode    = FindProperty("_Mode", properties);
        MaterialProperty cutoff  = FindProperty("_Cutoff", properties);

        materialEditor.ShaderProperty(color, "Color");
        materialEditor.TexturePropertySingleLine(new GUIContent("Texture"), mainTex);
        materialEditor.ShaderProperty(mode, "Rendering Mode");

        Mode currentMode = (Mode)mode.floatValue;

        if (currentMode == Mode.Cutout)
        {
            materialEditor.ShaderProperty(cutoff, "Alpha Cutoff");
        }

        EditorGUI.BeginChangeCheck();
        if (EditorGUI.EndChangeCheck() || GUI.changed)
        {
            ApplyMode(material, currentMode);
        }

        EditorGUILayout.Space();

        // ─── Render Queue — cho phép override thủ công giống Standard shader ───
        EditorGUI.BeginChangeCheck();
        int renderQueue = EditorGUILayout.IntField("Render Queue", material.renderQueue);
        if (EditorGUI.EndChangeCheck())
        {
            material.renderQueue = renderQueue;
        }
    }

    private void ApplyMode(Material material, Mode mode)
    {
        switch (mode)
        {
            case Mode.Opaque:
                material.SetFloat("_SrcBlend", (float)BlendMode.One);
                material.SetFloat("_DstBlend", (float)BlendMode.Zero);
                material.SetFloat("_ZWrite", 1f);
                material.DisableKeyword("_ALPHATEST_ON");
                material.DisableKeyword("_ALPHABLEND_ON");
                material.renderQueue = (int)RenderQueue.Geometry;
                material.SetOverrideTag("RenderType", "Opaque");
                break;

            case Mode.Cutout:
                material.SetFloat("_SrcBlend", (float)BlendMode.One);
                material.SetFloat("_DstBlend", (float)BlendMode.Zero);
                material.SetFloat("_ZWrite", 1f);
                material.EnableKeyword("_ALPHATEST_ON");
                material.DisableKeyword("_ALPHABLEND_ON");
                material.renderQueue = (int)RenderQueue.AlphaTest;
                material.SetOverrideTag("RenderType", "TransparentCutout");
                break;

            case Mode.Transparent:
                material.SetFloat("_SrcBlend", (float)BlendMode.SrcAlpha);
                material.SetFloat("_DstBlend", (float)BlendMode.OneMinusSrcAlpha);
                material.SetFloat("_ZWrite", 0f);
                material.DisableKeyword("_ALPHATEST_ON");
                material.EnableKeyword("_ALPHABLEND_ON");
                material.renderQueue = (int)RenderQueue.Transparent;
                material.SetOverrideTag("RenderType", "Transparent");
                break;
        }
    }

    public override void AssignNewShaderToMaterial(Material material, Shader oldShader, Shader newShader)
    {
        base.AssignNewShaderToMaterial(material, oldShader, newShader);

        Mode mode = (Mode)material.GetFloat("_Mode");
        ApplyMode(material, mode);
    }
}
