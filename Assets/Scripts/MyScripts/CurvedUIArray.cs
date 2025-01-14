using UnityEngine;

public class CurvedUIArray : MonoBehaviour
{
    public GameObject parentObject; // ����������ĸ�����
    public GameObject interactablePrefab;

    public int rows = 5; // ����
    public int cols = 10; // ����
    public float radius = 5f; // ����뾶
    public float verticalSpacing = 0.5f; // ��ֱ���
    public float horizontalSpacing = 0.5f; // ˮƽ���
    public float fixedSize = 0.1f; // �̶�������ߴ�
    void Start()
    {
        if (parentObject == null)
        {
            Debug.LogError("��ȷ�������ø�����");
            return;
        }

        // ��ȡ������ĵ�һ��������
        int childCount = parentObject.transform.childCount;
        if (childCount == 0)
        {
            Debug.LogError("������û���κε�һ�������壡");
            return;
        }

        // ȷ�������������㹻
        //if (rows * cols > childCount)
        //{
        //    Debug.LogError("������������������� UI ����");
        //    return;
        //}

        // ������ά����
        int childIndex = 0;
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                if (childIndex >= childCount) break; // �������������������ֹͣ

                // ��ȡ�������ֱ��������
                Transform originalChild = parentObject.transform.GetChild(childIndex);
                childIndex++;

                // ��¡������
                GameObject clonedChild = Instantiate(interactablePrefab, this.transform);

                // �����ά�����Ӧ�ľֲ�λ��
                float angle = (j - (cols - 1) / 2f) * horizontalSpacing / radius; // ˮƽ�Ƕ�
                float verticalOffset = (i - (rows - 1) / 2f) * verticalSpacing; // ��ֱƫ��

                Vector3 localPosition = new Vector3(
                    radius * Mathf.Sin(angle),   // x ����
                    verticalOffset,             // y ����
                    radius * Mathf.Cos(angle)   // z ����
                );

                // ���㷨����
                Vector3 localNormal = -localPosition.normalized;

                // ����ֲ���ת
                Quaternion localRotation = Quaternion.LookRotation(localNormal, Vector3.up) * Quaternion.Euler(0, -90, 0);

                // ���ÿ�¡������ľֲ�λ�ú���ת
                clonedChild.transform.localPosition = localPosition;
                clonedChild.transform.localRotation = localRotation;


                // ��ѡ��������С
                // ͳһ���ŵ��̶��ߴ�
                Renderer originalRenderer = GetRendererInChildren(originalChild);
                MeshFilter originalMeshFilter = GetMeshFilerInChildren(originalChild);
                Collider originalCollider = GetColliderInChildren(originalChild);
                clonedChild.name = originalChild.name;
                clonedChild.GetComponent<Renderer>().materials = originalRenderer.materials;
                clonedChild.GetComponent<MeshFilter>().mesh = originalMeshFilter.mesh;
                // clonedChild.GetComponent<BoxCollider>().center = clonedCollider.center;
                // clonedChild.GetComponent<BoxCollider>().size = clonedCollider.size;
                Collider clonedColliderComponent = clonedChild.GetComponent<Collider>();
                if (originalCollider != null && clonedColliderComponent != null)
                {
                    // 根据Collider类型进行相应处理
                    if (originalCollider is BoxCollider && clonedColliderComponent is BoxCollider)
                    {
                        BoxCollider origBox = (BoxCollider)originalCollider;
                        BoxCollider clonedBox = (BoxCollider)clonedColliderComponent;
                        clonedBox.center = origBox.center;
                        clonedBox.size = origBox.size;
                    }
                    else if (originalCollider is SphereCollider && clonedColliderComponent is SphereCollider)
                    {
                        SphereCollider origSphere = (SphereCollider)originalCollider;
                        SphereCollider clonedSphere = (SphereCollider)clonedColliderComponent;
                        clonedSphere.center = origSphere.center;
                        clonedSphere.radius = origSphere.radius;
                    }
                    else if (originalCollider is CapsuleCollider && clonedColliderComponent is CapsuleCollider)
                    {
                        CapsuleCollider origCapsule = (CapsuleCollider)originalCollider;
                        CapsuleCollider clonedCapsule = (CapsuleCollider)clonedColliderComponent;
                        clonedCapsule.center = origCapsule.center;
                        clonedCapsule.radius = origCapsule.radius;
                        clonedCapsule.height = origCapsule.height;
                        clonedCapsule.direction = origCapsule.direction;
                    }
                }
                if (originalRenderer != null)
                {
                    // ����ԭʼ�ߴ�
                    float originalSize = Mathf.Max(originalRenderer.bounds.size.x, originalRenderer.bounds.size.y, originalRenderer.bounds.size.z);
                    Debug.Log(originalSize);

                    // �������ű���
                    float scaleRatio = fixedSize / originalSize;

                    // ��������
                    clonedChild.transform.localScale = originalRenderer.gameObject.transform.localScale * scaleRatio;
                }
            }
        }
    }
    // �ݹ���� Renderer ���
    private Renderer GetRendererInChildren(Transform parent)
    {
        Renderer renderer = parent.GetComponent<Renderer>();
        if (renderer != null)
        {
            return renderer;
        }

        // �����ǰ����û�� Renderer����ݹ����������
        foreach (Transform child in parent)
        {
            renderer = GetRendererInChildren(child);
            if (renderer != null)
            {
                return renderer;
            }
        }

        // ���û���ҵ� Renderer������ null
        return null;
    }

    private MeshFilter GetMeshFilerInChildren(Transform parent)
    {
        MeshFilter meshFilter = parent.GetComponent<MeshFilter>();
        if (meshFilter != null)
        {
            return meshFilter;
        }

        // �����ǰ����û�� Renderer����ݹ����������
        foreach (Transform child in parent)
        {
            meshFilter = GetMeshFilerInChildren(child);
            if (meshFilter != null)
            {
                return meshFilter;
            }
        }

        // ���û���ҵ� Renderer������ null
        return null;
    }
    private Collider GetColliderInChildren(Transform parent)
    {
        Collider collider = parent.GetComponent<Collider>();
        if (collider != null)
        {
            return collider;
        }
        foreach (Transform child in parent)
        {
            collider = GetColliderInChildren(child);
            if (collider != null)
            {
                return collider;
            }
        }
        return null;
    }


}
