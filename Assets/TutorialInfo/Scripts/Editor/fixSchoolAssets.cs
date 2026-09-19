using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class Fix2DMaterials : EditorWindow
{
    const string Bad2DShaderPrefix = "Universal Render Pipeline/2D/";

    Material replacement;
    bool trimToSubmeshCount = true;
    bool replaceNullSlots = true;

    [MenuItem("Tools/Fix 2D Materials")]
    static void Open() => GetWindow<Fix2DMaterials>("Fix 2D Materials");

    void OnGUI()
    {
        EditorGUILayout.HelpBox(
            "Replaces every material slot using a URP 2D shader (and optionally empty slots) " +
            "with the replacement material, and trims material arrays to the mesh's submesh count. " +
            "Applies to all open scenes. Undo is supported.",
            MessageType.Info);

        replacement = (Material)EditorGUILayout.ObjectField("Replacement Material", replacement, typeof(Material), false);
        trimToSubmeshCount = EditorGUILayout.Toggle("Trim to Submesh Count", trimToSubmeshCount);
        replaceNullSlots = EditorGUILayout.Toggle("Replace Empty Slots", replaceNullSlots);

        EditorGUILayout.Space();

        if (GUILayout.Button("Report Only"))
            Run(apply: false);

        using (new EditorGUI.DisabledScope(replacement == null))
        {
            if (GUILayout.Button("Fix Open Scenes"))
                Run(apply: true);
        }
    }

    static Mesh GetMesh(Renderer r)
    {
        if (r is SkinnedMeshRenderer smr) return smr.sharedMesh;
        if (r is MeshRenderer)
        {
            var mf = r.GetComponent<MeshFilter>();
            return mf != null ? mf.sharedMesh : null;
        }
        return null;
    }

    static bool Is2DMaterial(Material m) =>
        m != null && m.shader != null && m.shader.name.StartsWith(Bad2DShaderPrefix);

    void Run(bool apply)
    {
        var renderers = FindObjectsByType<Renderer>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        int fixedCount = 0, slotsReplaced = 0, slotsTrimmed = 0;

        Undo.SetCurrentGroupName("Fix 2D Materials");
        int undoGroup = Undo.GetCurrentGroup();

        foreach (var r in renderers)
        {
            var mesh = GetMesh(r);
            if (mesh == null) continue;

            var mats = r.sharedMaterials.ToList();
            bool changed = false;

            for (int i = 0; i < mats.Count; i++)
            {
                bool bad = Is2DMaterial(mats[i]) || (replaceNullSlots && mats[i] == null);
                if (!bad) continue;

                slotsReplaced++;
                changed = true;
                if (apply) mats[i] = replacement;
            }

            int subCount = Mathf.Max(1, mesh.subMeshCount);
            if (trimToSubmeshCount && mats.Count > subCount)
            {
                slotsTrimmed += mats.Count - subCount;
                changed = true;
                if (apply) mats = mats.Take(subCount).ToList();
            }

            if (!changed) continue;
            fixedCount++;

            if (!apply)
            {
                Debug.Log($"[Fix2DMaterials] Would fix '{r.name}' ({mats.Count} slots, {mesh.subMeshCount} submeshes)", r);
                continue;
            }

            Undo.RecordObject(r, "Fix 2D Materials");
            r.sharedMaterials = mats.ToArray();
            if (PrefabUtility.IsPartOfPrefabInstance(r))
                PrefabUtility.RecordPrefabInstancePropertyModifications(r);
            EditorUtility.SetDirty(r);
            EditorSceneManager.MarkSceneDirty(r.gameObject.scene);
        }

        Undo.CollapseUndoOperations(undoGroup);

        string verb = apply ? "Fixed" : "Found";
        Debug.Log($"[Fix2DMaterials] {verb} {fixedCount} renderers: {slotsReplaced} slots replaced, {slotsTrimmed} extra slots trimmed.");
    }
}