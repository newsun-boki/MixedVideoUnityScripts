using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class SurfaceGenerator : MonoBehaviour
{
    [SerializeField] private Material surfaceMaterial;
    public List<Vector3> points = new List<Vector3>();
    private Mesh mesh;
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;

    private void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        mesh = new Mesh();
        meshFilter.mesh = mesh;

        if (surfaceMaterial != null)
            meshRenderer.material = surfaceMaterial;

        GenerateTestPoints();
    }

    private void GenerateTestPoints()
    {
        float range = 5;
        for (int i = 0; i < 100; i++) {
            points.Add(new Vector3(
                    Random.Range(-range, range),
                    Random.Range(-range, range),
                    Random.Range(-range, range)
                ));
        }
        GenerateSurface();
    }

    public void AddPoint(Vector3 point)
    {
        points.Add(point);
        GenerateSurface();
    }

    public void ClearPoints()
    {
        points.Clear();
        mesh.Clear();
    }

    private class Edge
    {
        public int from;
        public int to;
        public float weight;

        public Edge(int from, int to, float weight)
        {
            this.from = from;
            this.to = to;
            this.weight = weight;
        }
    }

    public void GenerateSurface()
    {
        if (points.Count < 3) return;

        // 构建最小生成树
        List<Edge> mst = BuildMinimumSpanningTree();

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector3> normals = new List<Vector3>();

        // 使用最小生成树的边生成三角形
        for (int i = 0; i < mst.Count - 1; i++)
        {
            Edge e1 = mst[i];
            Edge e2 = mst[i + 1];

            // 找到共同的顶点
            int commonVertex = -1;
            int otherVertex1 = -1;
            int otherVertex2 = -1;

            if (e1.to == e2.from)
            {
                commonVertex = e1.to;
                otherVertex1 = e1.from;
                otherVertex2 = e2.to;
            }
            else if (e1.to == e2.to)
            {
                commonVertex = e1.to;
                otherVertex1 = e1.from;
                otherVertex2 = e2.from;
            }
            else if (e1.from == e2.from)
            {
                commonVertex = e1.from;
                otherVertex1 = e1.to;
                otherVertex2 = e2.to;
            }
            else if (e1.from == e2.to)
            {
                commonVertex = e1.from;
                otherVertex1 = e1.to;
                otherVertex2 = e2.from;
            }

            if (commonVertex != -1)
            {
                // 添加三角形
                vertices.Add(points[commonVertex]);
                vertices.Add(points[otherVertex1]);
                vertices.Add(points[otherVertex2]);

                int baseIndex = vertices.Count - 3;
                triangles.Add(baseIndex);
                triangles.Add(baseIndex + 1);
                triangles.Add(baseIndex + 2);

                // 计算法线
                Vector3 normal = Vector3.Cross(
                    points[otherVertex1] - points[commonVertex],
                    points[otherVertex2] - points[commonVertex]
                ).normalized;

                normals.Add(normal);
                normals.Add(normal);
                normals.Add(normal);
            }
        }

        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.normals = normals.ToArray();
        mesh.RecalculateBounds();
    }

    private List<Edge> BuildMinimumSpanningTree()
    {
        List<Edge> mst = new List<Edge>();
        HashSet<int> visited = new HashSet<int>();
        List<Edge> edges = new List<Edge>();

        // 构建所有可能的边
        for (int i = 0; i < points.Count; i++)
        {
            for (int j = i + 1; j < points.Count; j++)
            {
                float distance = Vector3.Distance(points[i], points[j]);
                edges.Add(new Edge(i, j, distance));
            }
        }

        // 从第一个点开始
        visited.Add(0);

        // Prim算法
        while (visited.Count < points.Count)
        {
            Edge minEdge = null;
            float minWeight = float.MaxValue;

            foreach (Edge edge in edges)
            {
                if ((visited.Contains(edge.from) && !visited.Contains(edge.to)) ||
                    (visited.Contains(edge.to) && !visited.Contains(edge.from)))
                {
                    if (edge.weight < minWeight)
                    {
                        minWeight = edge.weight;
                        minEdge = edge;
                    }
                }
            }

            if (minEdge != null)
            {
                mst.Add(minEdge);
                visited.Add(visited.Contains(minEdge.from) ? minEdge.to : minEdge.from);
            }
            else
            {
                break; // 图不连通
            }
        }

        return mst;
    }
}