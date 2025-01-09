using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class CreateSurfaceFromPoints : MonoBehaviour
{
    [SerializeField] private bool autoUpdate = true;

    // ʹ��SerializeFieldʹ˽�б�����Inspector�пɼ�
    [SerializeField] private List<Vector3> _points = new List<Vector3>();

    // ���ԣ��������޸�pointsʱ�Զ�����mesh
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

    // ����������ʹUnity��Inspector�е�ֵ�ı�ʱ����
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

        // 1. �ҵ���ƽ��
        Vector3 centroid = Vector3.zero;
        foreach (var point in _points)
            centroid += point;
        centroid /= _points.Count;

        // 2. ����Э�������
        Matrix4x4 covariance = new Matrix4x4();
        foreach (var point in _points)
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
        foreach (var point in _points)
        {
            Vector3 diff = point - centroid;
            points2D.Add(new Vector2(Vector3.Dot(diff, Vector3.right),
                                   Vector3.Dot(diff, Vector3.forward)));
        }

        // 5. ����������
        List<int> triangles = new List<int>();
        for (int i = 1; i < _points.Count - 1; i++)
        {
            triangles.Add(0);
            triangles.Add(i);
            triangles.Add(i + 1);
        }

        // 6. ����mesh
        mesh.Clear();
        mesh.vertices = _points.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }

    // �ṩһ����������������ʱ��ӵ�
    public void AddPoint(Vector3 point)
    {
        _points.Add(point);
        if (autoUpdate) CreateSurface();
    }

    // ������е�
    public void ClearPoints()
    {
        _points.Clear();
        if (mesh != null) mesh.Clear();
    }

    // ��ѡ����Scene��ͼ����ʾ��
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