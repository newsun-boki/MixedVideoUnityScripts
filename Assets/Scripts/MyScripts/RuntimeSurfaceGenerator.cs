using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class SurfaceFromPoints : MonoBehaviour
{
    public List<Vector3> points = new List<Vector3>();
    private Mesh mesh;
    private MeshFilter meshFilter;

    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        mesh = new Mesh();
        meshFilter.mesh = mesh;

        // ���Ե�
        GenerateTestPoints();
        CreateSurface();
    }

    void GenerateTestPoints()
    {
        // ����һЩ���Ե�
        for (int i = 0; i < 10; i++)
        {
            float angle = i * Mathf.PI * 2f / 10;
            float x = Mathf.Cos(angle) * 2;
            float z = Mathf.Sin(angle) * 2;
            float y = Random.Range(-0.5f, 0.5f);
            points.Add(new Vector3(x, y, z));
        }
    }

    public void CreateSurface()
    {
        if (points.Count < 3) return;

        // 1. �ҵ���ƽ��
        Vector3 centroid = Vector3.zero;
        foreach (var point in points)
            centroid += point;
        centroid /= points.Count;

        // 2. ����Э�������
        Matrix4x4 covariance = new Matrix4x4();
        foreach (var point in points)
        {
            Vector3 diff = point - centroid;
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    covariance[i, j] += diff[i] * diff[j];
        }

        // 3. �ҵ�����������򻯴�������y��Ϊ���ߣ�
        Vector3 normal = Vector3.up;

        // 4. ͶӰ��2Dƽ��
        List<Vector2> points2D = new List<Vector2>();
        foreach (var point in points)
        {
            Vector3 diff = point - centroid;
            points2D.Add(new Vector2(Vector3.Dot(diff, Vector3.right),
                                   Vector3.Dot(diff, Vector3.forward)));
        }

        // 5. ���������Σ��򻯵�Delaunay�����ʷ֣�
        List<int> triangles = new List<int>();
        for (int i = 1; i < points.Count - 1; i++)
        {
            triangles.Add(0);
            triangles.Add(i);
            triangles.Add(i + 1);
        }

        // 6. ����mesh
        mesh.Clear();
        mesh.vertices = points.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }

    public void AddPoint(Vector3 point)
    {
        points.Add(point);
        CreateSurface();
    }

    public void ClearPoints()
    {
        points.Clear();
        mesh.Clear();
    }
}