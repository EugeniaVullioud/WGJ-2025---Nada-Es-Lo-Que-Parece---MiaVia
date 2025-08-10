#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
public class DecisionGraphEditor : EditorWindow
{
    private Vector2 scrollPosition;
    private List<DecisionData> decisions = new();
    private Dictionary<DecisionData, Vector2> positions = new();
    private Dictionary<DecisionData, Vector2> nodeScrolls = new(); // per-node scroll

    private bool isDragging = false;

    [MenuItem("Tools/Decision Graph")]
    public static void Open()
    {
        GetWindow<DecisionGraphEditor>("Decision Graph");
    }

    private void OnEnable()
    {
        LoadAllDecisions();
        EditorApplication.update += OnEditorUpdate; // Controlled updates

    }

    private void OnDisable()
    {
        EditorApplication.update -= OnEditorUpdate;
    }
    private void OnEditorUpdate()
    {
        // Only repaint when dragging
        if (isDragging) Repaint();
    }
    private void LoadAllDecisions()
    {
        string[] guids = AssetDatabase.FindAssets("t:DecisionData");
        decisions.Clear();
        foreach (var guid in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var decision = AssetDatabase.LoadAssetAtPath<DecisionData>(path);
            decisions.Add(decision);

            if (!positions.ContainsKey(decision))
                positions[decision] = new Vector2(Random.Range(50, 500), Random.Range(50, 500));

            if (!nodeScrolls.ContainsKey(decision))
                nodeScrolls[decision] = Vector2.zero;
        }
        Repaint();
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Reload Decisions"))
            LoadAllDecisions();

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        BeginWindows();

        for (int i = 0; i < decisions.Count; i++)
        {
            var decision = decisions[i];
            Rect updatedRect = GUI.Window(i,
                new Rect(positions[decision].x, positions[decision].y, 250, 200),
                id =>
                {
                    nodeScrolls[decision] = EditorGUILayout.BeginScrollView(nodeScrolls[decision]);
                    EditorGUILayout.LabelField(decision.name, EditorStyles.boldLabel);
                    EditorGUILayout.LabelField(decision.prompt, EditorStyles.wordWrappedLabel);
                    EditorGUILayout.Space();

                    foreach (var choice in decision.choices)
                    {
                        EditorGUILayout.LabelField($"- {choice.choiceText}");
                        foreach (var consequence in choice.consequences)
                        {
                            EditorGUILayout.LabelField($"    -> {consequence.name}");
                        }
                    }

                    EditorGUILayout.EndScrollView();

                    // Detect if user is dragging a window
                    if (Event.current.type == EventType.MouseDrag)
                        isDragging = true;
                    else if (Event.current.type == EventType.MouseUp)
                        isDragging = false;

                    GUI.DragWindow();
                },
                decision.name);

            positions[decision] = updatedRect.position;
        }

        EndWindows();
        EditorGUILayout.EndScrollView();
    }
}
#endif