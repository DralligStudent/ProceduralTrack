﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour
{
    public int meshSize = 20;

    Mesh mesh;

    Vector3[] verts;

    int[] tris;

	// Use this for initialization
	void Start ()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        CreateShape();
        UpdateMesh();
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    void CreateShape()
    {
        float ct = Time.deltaTime;
        verts = new Vector3[(meshSize + 1) * (meshSize + 1)];
        float r = Random.value;
        int i = 0;
        for (int z = 0; z <= meshSize * 10; z+=10)
        {
            for (int x = 0; x <= meshSize * 10; x+=10)
            {
                
                //float y1 = Mathf.PerlinNoise(x * 0.001f, z * 0.001f) * 10f;
                float y2 = Mathf.PerlinNoise(x * 0.05f, z* 0.05f) * (r * 30.0f);
                float y3 = Mathf.PerlinNoise(x/10 * .3f, z/10 * .3f) * 4f;



                //loat ny = ((y1 * y3) + y2) / 3;
                //float ny = y1 * y2 * y3 / 3;
                float ny = y2 * y3 / 2;
                verts[i] = new Vector3(x, ny, z);
                i++;
            }
        }

        tris = new int[meshSize * meshSize * 6];

        int cVert = 0;
        int cTris = 0;

        for (int z = 0; z < meshSize; z++)
        {
            for (int x = 0; x < meshSize; x++)
            {
                tris[cTris + 0] = cVert + 0;
                tris[cTris + 1] = cVert+ meshSize + 1;
                tris[cTris + 2] = cVert + 1;

                tris[cTris + 3] = cVert + 1;
                tris[cTris + 4] = cVert + meshSize + 1;
                tris[cTris + 5] = cVert + meshSize + 2;

                cVert++;
                cTris += 6;
            }
            cVert++;

        }
    }


    void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = verts;
        mesh.triangles = tris;

        mesh.RecalculateNormals();
    }

    /*private void OnDrawGizmos()
    {
        if (verts == null)
            return;

        for (int i = 0; i < verts.Length; i++)
        {
            Gizmos.DrawSphere(verts[i], 0.1f);
        }
    }*/
}