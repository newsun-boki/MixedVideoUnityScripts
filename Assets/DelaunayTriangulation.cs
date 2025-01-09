using UnityEngine;
using System.Collections.Generic;

public class DelaunayTriangulation
{
    private List<Vector2> points;
    private List<Triangle> triangles;

    public class Triangle
    {
        public Vector2 p1, p2, p3;

        public Triangle(Vector2 point1, Vector2 point2, Vector2 point3)
        {
            p1 = point1;
            p2 = point2;
            p3 = point3;
        }

        public bool ContainsPoint(Vector2 point)
        {
            float alpha = ((p2.y - p3.y) * (point.x - p3.x) + (p3.x - p2.x) * (point.y - p3.y)) /
                         ((p2.y - p3.y) * (p1.x - p3.x) + (p3.x - p2.x) * (p1.y - p3.y));
            float beta = ((p3.y - p1.y) * (point.x - p3.x) + (p1.x - p3.x) * (point.y - p3.y)) /
                        ((p2.y - p3.y) * (p1.x - p3.x) + (p3.x - p2.x) * (p1.y - p3.y));
            float gamma = 1.0f - alpha - beta;

            return alpha > 0f && beta > 0f && gamma > 0f;
        }

        public bool IsPointInCircumcircle(Vector2 point)
        {
            float ax = p1.x - point.x;
            float ay = p1.y - point.y;
            float bx = p2.x - point.x;
            float by = p2.y - point.y;
            float cx = p3.x - point.x;
            float cy = p3.y - point.y;

            float det = (ax * ax + ay * ay) * (bx * cy - cx * by) -
                       (bx * bx + by * by) * (ax * cy - cx * ay) +
                       (cx * cx + cy * cy) * (ax * by - bx * ay);

            return det > 0;
        }
    }

    public DelaunayTriangulation(List<Vector2> inputPoints)
    {
        points = new List<Vector2>(inputPoints);
        triangles = new List<Triangle>();
        Triangulate();
    }

    private void Triangulate()
    {
        // 创建一个超级三角形
        float minX = float.MaxValue, minY = float.MaxValue;
        float maxX = float.MinValue, maxY = float.MinValue;

        foreach (Vector2 point in points)
        {
            minX = Mathf.Min(minX, point.x);
            minY = Mathf.Min(minY, point.y);
            maxX = Mathf.Max(maxX, point.x);
            maxY = Mathf.Max(maxY, point.y);
        }

        float dx = maxX - minX;
        float dy = maxY - minY;
        float dmax = Mathf.Max(dx, dy);
        float xmid = (minX + maxX) / 2f;
        float ymid = (minY + maxY) / 2f;

        Vector2 p1 = new Vector2(xmid - 20 * dmax, ymid - dmax);
        Vector2 p2 = new Vector2(xmid, ymid + 20 * dmax);
        Vector2 p3 = new Vector2(xmid + 20 * dmax, ymid - dmax);

        triangles.Add(new Triangle(p1, p2, p3));

        // 添加所有点
        foreach (Vector2 point in points)
        {
            List<Triangle> badTriangles = new List<Triangle>();

            // 找出所有包含当前点的三角形
            foreach (Triangle triangle in triangles)
            {
                if (triangle.IsPointInCircumcircle(point))
                {
                    badTriangles.Add(triangle);
                }
            }

            // 找出多边形边界
            List<Edge> polygon = new List<Edge>();
            foreach (Triangle triangle in badTriangles)
            {
                Edge e1 = new Edge(triangle.p1, triangle.p2);
                Edge e2 = new Edge(triangle.p2, triangle.p3);
                Edge e3 = new Edge(triangle.p3, triangle.p1);

                if (!IsSharedEdge(e1, badTriangles, triangle)) polygon.Add(e1);
                if (!IsSharedEdge(e2, badTriangles, triangle)) polygon.Add(e2);
                if (!IsSharedEdge(e3, badTriangles, triangle)) polygon.Add(e3);
            }

            // 移除不好的三角形
            foreach (Triangle triangle in badTriangles)
            {
                triangles.Remove(triangle);
            }

            // 重新三角剖分多边形
            foreach (Edge edge in polygon)
            {
                triangles.Add(new Triangle(edge.p1, edge.p2, point));
            }
        }

        // 移除包含超级三角形顶点的三角形
        triangles.RemoveAll(t =>
            IsPointEqual(t.p1, p1) || IsPointEqual(t.p1, p2) || IsPointEqual(t.p1, p3) ||
            IsPointEqual(t.p2, p1) || IsPointEqual(t.p2, p2) || IsPointEqual(t.p2, p3) ||
            IsPointEqual(t.p3, p1) || IsPointEqual(t.p3, p2) || IsPointEqual(t.p3, p3));
    }

    private struct Edge
    {
        public Vector2 p1, p2;

        public Edge(Vector2 point1, Vector2 point2)
        {
            p1 = point1;
            p2 = point2;
        }
    }

    private bool IsSharedEdge(Edge edge, List<Triangle> triangles, Triangle exclude)
    {
        foreach (Triangle triangle in triangles)
        {
            if (triangle == exclude) continue;

            if ((IsPointEqual(edge.p1, triangle.p1) && IsPointEqual(edge.p2, triangle.p2)) ||
                (IsPointEqual(edge.p1, triangle.p2) && IsPointEqual(edge.p2, triangle.p3)) ||
                (IsPointEqual(edge.p1, triangle.p3) && IsPointEqual(edge.p2, triangle.p1)) ||
                (IsPointEqual(edge.p2, triangle.p1) && IsPointEqual(edge.p1, triangle.p2)) ||
                (IsPointEqual(edge.p2, triangle.p2) && IsPointEqual(edge.p1, triangle.p3)) ||
                (IsPointEqual(edge.p2, triangle.p3) && IsPointEqual(edge.p1, triangle.p1)))
            {
                return true;
            }
        }
        return false;
    }

    private bool IsPointEqual(Vector2 p1, Vector2 p2)
    {
        return Vector2.Distance(p1, p2) < 0.0001f;
    }

    public List<Triangle> GetTriangles()
    {
        return triangles;
    }
}