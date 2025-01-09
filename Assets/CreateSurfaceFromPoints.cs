using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class CreateSurfaceFromPoints : MonoBehaviour
{
    [SerializeField] private bool autoUpdate = true;

    // 使用SerializeField使私有变量在Inspector中可见
    [SerializeField] private List<Vector3> _points = new List<Vector3>();

    // 属性，用于在修改points时自动更新mesh
    public List<Vector3> points
    {
        get => _points;
        set
        {
            _points = value;
            if (autoUpdate) CreateSurface();
        }
    }

    private Mesh mesh;
    private MeshFilter meshFilter;

    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        mesh = new Mesh();
        meshFilter.mesh = mesh;
        CreateSurface();
    }

    // 添加这个方法使Unity在Inspector中的值改变时调用
    private void OnValidate()
    {
        if (Application.isPlaying && autoUpdate)
        {
            CreateSurface();
        }
    }

    public void CreateSurface()
    {
        if (_points == null || _points.Count < 3)
        {
            if (mesh != null) mesh.Clear();
            return;
        }

        if (meshFilter == null)
        {
            meshFilter = GetComponent<MeshFilter>();
            mesh = new Mesh();
            meshFilter.mesh = mesh;
        }

        // 1. 找到主平面
        Vector3 centroid = Vector3.zero;
        foreach (var point in _points)
            centroid += point;
        centroid /= _points.Count;

        // 2. 计算协方差矩阵
        Matrix4x4 covariance = new Matrix4x4();
        foreach (var point in _points)
        {
            Vector3 diff = point - centroid;
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    covariance[i, j] += diff[i] * diff[j];
        }

        // 3. 找到主方向（这里简化处理，假设y轴为法线）
        Vector3 normal = Vector3.up;

        // 4. 投影到2D平面
        List<Vector2> points2D = new List<Vector2>();
        foreach (var point in _points)
        {
            Vector3 diff = point - centroid;
            points2D.Add(new Vector2(Vector3.Dot(diff, Vector3.right),
                                   Vector3.Dot(diff, Vector3.forward)));
        }

        // 5. 创建三角形
        List<int> triangles = new List<int>();
        for (int i = 1; i < _points.Count - 1; i++)
        {
            triangles.Add(0);
            triangles.Add(i);
            triangles.Add(i + 1);
        }

        // 6. 更新mesh
        mesh.Clear();
        mesh.vertices = _points.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }

    // 提供一个方法用于在运行时添加点
    public void AddPoint(Vector3 point)
    {
        _points.Add(point);
        if (autoUpdate) CreateSurface();
    }

    // 清除所有点
    public void ClearPoints()
    {
        _points.Clear();
        if (mesh != null) mesh.Clear();
    }

    // 可选：在Scene视图中显示点
    private void OnDrawGizmos()
    {
        if (_points == null) return;

        Gizmos.color = Color.yellow;
        foreach (Vector3 point in _points)
        {
            Gizmos.DrawSphere(point, 0.1f);
        }
    }
}