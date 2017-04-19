using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace Common.Components.Meshs
{
    public class GenerateMesh : SingletonPersistent<GenerateMesh>
    {
        private int _number;
        public Mesh Generate(Mesh mesh, List<KeyValuePair<Vector3, Vector2>> posAndSize)
        {
            int number = posAndSize.Count;
            if (number <= 0) return null;
            if (_number != number)
            {
                if (mesh)
                    UnityEngine.Object.Destroy(mesh);
                mesh = new Mesh();
                _number = number;
            }
            if (!mesh)
                mesh = new Mesh();
            //int particleNum = number;//:  = QualityManager.quality > Quality.Medium ? numberOfParticles : numberOfParticles / 2;

            Vector3[] verts = new Vector3[4 * number];
            Vector2[] uvs = new Vector2[4 * number];
            //Vector2[] uvs2 = new Vector2[4 * number];
            Vector3[] normals = new Vector3[4 * number];

            int[] tris = new int[2 * 3 * number];

            Vector3 position = Vector3.zero;
            Vector2 size = Vector2.zero;
            for (int i = 0; i < number; i++)
            {
                KeyValuePair<Vector3, Vector2> kvp = posAndSize[i];
                int i4 = i * 4;
                int i6 = i * 6;
                position = kvp.Key;
                size = kvp.Value;

                verts[i4 + 0] = new Vector3(position.x - size.x, 0, position.z + size.y);
                verts[i4 + 1] = new Vector3(position.x + size.x, 0, position.z + size.y);
                verts[i4 + 2] = new Vector3(position.x - size.x, 0, position.z - size.y);
                verts[i4 + 3] = new Vector3(position.x + size.x, 0, position.z - size.y);

                normals[i4 + 0] = Vector3.up;
                normals[i4 + 1] = Vector3.up;
                normals[i4 + 2] = Vector3.up;
                normals[i4 + 3] = Vector3.up;

                uvs[i4 + 0] = new Vector2(0.0f, 0.0f);
                uvs[i4 + 1] = new Vector2(1.0f, 0.0f);
                uvs[i4 + 2] = new Vector2(0.0f, 1.0f);
                uvs[i4 + 3] = new Vector2(1.0f, 1.0f);

                //Vector2 tc1 = new Vector2(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
                //uvs2[i4 + 0] = new Vector2(tc1.x, tc1.y);
                //uvs2[i4 + 1] = new Vector2(tc1.x, tc1.y); ;
                //uvs2[i4 + 2] = new Vector2(tc1.x, tc1.y); ;
                //uvs2[i4 + 3] = new Vector2(tc1.x, tc1.y); ;

                tris[i6 + 0] = i4 + 0;
                tris[i6 + 1] = i4 + 1;
                tris[i6 + 2] = i4 + 2;
                tris[i6 + 3] = i4 + 1;
                tris[i6 + 4] = i4 + 3;
                tris[i6 + 5] = i4 + 2;
            }

            mesh.vertices = verts;
            mesh.triangles = tris;
            mesh.normals = normals;
            mesh.uv = uvs;
            //mesh.uv2 = uvs2;
            mesh.RecalculateBounds();

            return mesh;
        }
    }
}