#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
public class VertexDebugger : MonoBehaviour
{
    public bool ShowLabels = true;
    public bool ShowUVs = true;
    public float labelSize = 0.02f;

    public enum LabelMode
    {
        RectangularExtremes,
        RadialExtremes
    }

    public LabelMode labelMode = LabelMode.RectangularExtremes;
    public int maxRoundLabels = 8; // Max labels for round mode

    void OnDrawGizmos()
    {
        if (!ShowLabels) return;
        var mesh = GetComponent<MeshFilter>()?.sharedMesh;
        if (mesh == null) return;

        Vector3[] verts = mesh.vertices;
        Vector2[] uvs = mesh.uv;
        Matrix4x4 localToWorld = transform.localToWorldMatrix;

        float lineHeight = labelSize;
        Dictionary<Vector3, List<int>> vertexGroups = new Dictionary<Vector3, List<int>>();
        HashSet<int> drawnIndices = new HashSet<int>();
        Vector3 boundsCenter = localToWorld.MultiplyPoint3x4(mesh.bounds.center);

        // Group vertices by local position
        for (int i = 0; i < verts.Length; i++)
        {
            Vector3 local = verts[i];
            if (!vertexGroups.ContainsKey(local))
                vertexGroups[local] = new List<int>();

            vertexGroups[local].Add(i);
        }

        // foreach (var group in vertexGroups)
        // {
        //     Vector3 local = group.Key;
        //     List<int> indices = group.Value;
        //     Vector3 worldPos = localToWorld.MultiplyPoint3x4(local);

        //     // Use first index for labeling
        //     int i = indices[0];
        //     string label = $"V{i}";
        //     if (indices.Count > 1)
        //         label += $" x{indices.Count}";

        //     label += $"\nLocal: {RoundVec3(local)}";

        //     if (ShowUVs && i < uvs.Length)
        //         label += $"\nUV: {RoundVec2(uvs[i])}";

        //     Handles.Label(worldPos + Vector3.up * lineHeight * 0, label);
        // }

        List<Vector3> extremePoints = new List<Vector3>();

        if (labelMode == LabelMode.RectangularExtremes)
        {
            // Get axis-aligned extremes
            Vector3 min = verts[0];
            Vector3 max = verts[0];
            foreach (var v in verts)
            {
                min = Vector3.Min(min, v);
                max = Vector3.Max(max, v);
            }

            extremePoints = new List<Vector3>
            {
                min,
                max,
                new Vector3(min.x, min.y, max.z),
                new Vector3(min.x, max.y, min.z),
                new Vector3(max.x, min.y, min.z),
                new Vector3(max.x, max.y, max.z),
                new Vector3(min.x, max.y, max.z),
                new Vector3(max.x, min.y, max.z),
                new Vector3(max.x, max.y, min.z),
            };
        }
        else if (labelMode == LabelMode.RadialExtremes)
        {
            Vector3 center = Vector3.zero;
            foreach (var v in verts)
                center += v;
            center /= verts.Length;

            // Get vertices farthest from center
            SortedList<float, Vector3> furthest = new SortedList<float, Vector3>(new DuplicateKeyComparer<float>());
            foreach (var v in verts)
            {
                float d = (v - center).sqrMagnitude;
                if (!furthest.ContainsKey(d))
                    furthest.Add(d, v);
            }

            var reversed = new List<KeyValuePair<float, Vector3>>(furthest);
            reversed.Reverse();

            int count = 0;
            foreach (var pair in reversed)
            {
                extremePoints.Add(pair.Value);
                if (++count >= maxRoundLabels)
                    break;
            }

        }

        // Draw labels at extreme points
        foreach (var group in vertexGroups)
        {
            Vector3 local = group.Key;
            if (!extremePoints.Contains(local)) continue;

            List<int> indices = group.Value;
            Vector3 worldPos = localToWorld.MultiplyPoint3x4(local);

            int i = indices[0];
            string label = $"V{i}";
            if (indices.Count > 1)
                label += $" x{indices.Count}";
            label += $"\nLocal: {RoundVec3(local)}";

            if (ShowUVs && i < uvs.Length)
                label += $"\nUV: {RoundVec2(uvs[i])}";

            Handles.Label(worldPos + Vector3.up * lineHeight * 0, label);
        }

        // Show total vertex count near center
        Handles.Label(boundsCenter + Vector3.up * labelSize * 5, $"Total Vertices: {verts.Length}");
    }

    private string RoundVec3(Vector3 v)
    {
        return $"{v.x:F3}, {v.y:F3}, {v.z:F3}";
    }

    private string RoundVec2(Vector2 v)
    {
        return $"{v.x:F3}, {v.y:F3}";
    }    
}

public class DuplicateKeyComparer<TKey> : IComparer<TKey> where TKey : System.IComparable
{
    public int Compare(TKey x, TKey y)
    {
        int result = y.CompareTo(x);
        return result == 0 ? 1 : result;
    }
}
#endif