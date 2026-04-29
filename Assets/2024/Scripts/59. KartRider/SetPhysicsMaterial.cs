using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KartRider
{
    public class SetPhysicsMaterial : MonoBehaviour
    {
        [SerializeField] PhysicsMaterial targetMaterial;

        private void Start()
        {
            ApplyMaterialToMeshColliders();
        }

        public void ApplyMaterialToMeshColliders()
        {
            if (targetMaterial == null) return;

            // ���� ������Ʈ�� �ڽ� ������Ʈ�� ��� Mesh Collider �˻�
            MeshCollider[] meshColliders = GetComponentsInChildren<MeshCollider>();

            // �˻��� ��� Mesh Collider�� Physic Material ����
            foreach (var collider in meshColliders)
            {
                collider.material = targetMaterial;
            }
        }
    }
}
