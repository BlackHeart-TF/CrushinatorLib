using System.Collections;
using System.Collections.Generic;
using System.IO;                            // Added for file read function
using System;                               // Added for array resize, could possibly use a list for this
using UnityEngine;
using LibOpenCrush;

// Original starting point for coding this is from Aaron Hibberd's YouTube Channel
// Unity Tutorial - Mesh from Code Nov 6, 2015
// buildMesh.cs

/// <summary>
/// SMF File load test, Evo 1 only.
/// </summary>
public class SMFtest : MonoBehaviour {

    const string defaultDir = ".\\Assets\\Evo Content\\";           // Point to our Evo Content folder!



    public Vector3 vertLeftTopFront = new Vector3(-1, 1, 1);
    public Vector3 vertRightTopFront = new Vector3(1, 1, 1);
    public Vector3 vertRightTopBack = new Vector3(1, 1, -1);
    public Vector3 vertLeftTopBack = new Vector3(-1, 1, -1);

    private float waitN = 3f;
    private float waitD = 3f;
    public int shapeN = 0;

    // Use this for initialization
    void Start () {
        //string filename = "31X10_08L.SMF";                          // This will eventually be the texture file read from the SMF file
        //string filepath = defaultDir + "MODELS\\" + filename;       // Use content from the EVOContent Directory
        var pod = new POD("C:\\Users\\Noire\\Downloads\\4x4EvoR_Patch_013\\TRUCK.POD");
        Metadata blazer = pod.FileTree["Models/BLAZERLT.SMF"];
        LoadSimpleModelFile(blazer, pod.GetFileStream(blazer));

        MeshFilter mf = GetComponent<MeshFilter>();
        Mesh mesh = mf.mesh;

        //Vertices//
        Vector3[] vertices = new Vector3[]
        {
            //front face//
            vertLeftTopFront,//left top front, 0
            vertRightTopFront,//right top front, 1
            new Vector3(-1,-1,1),//left bottom front, 2
            new Vector3(1,-1,1),//right bottom front, 3
 
            //back face//
            vertRightTopBack,//right top back, 4
            vertLeftTopBack,//left top back, 5
            new Vector3(1,-1,-1),//right bottom back, 6
            new Vector3(-1,-1,-1),//left bottom back, 7
 
            //left face//
            vertLeftTopBack,//left top back, 8
            vertLeftTopFront,//left top front, 9
            new Vector3(-1,-1,-1),//left bottom back, 10
            new Vector3(-1,-1,1),//left bottom front, 11
 
            //right face//
            vertRightTopFront,//right top front, 12
            vertRightTopBack,//right top back, 13
            new Vector3(1,-1,1),//right bottom front, 14
            new Vector3(1,-1,-1),//right bottom back, 15
 
            //top face//
            vertLeftTopBack,//left top back, 16
            vertRightTopBack,//right top back, 17
            vertLeftTopFront,//left top front, 18
            vertRightTopFront,//right top front, 19
 
            //bottom face//
            new Vector3(-1,-1,1),//left bottom front, 20
            new Vector3(1,-1,1),//right bottom front, 21
            new Vector3(-1,-1,-1),//left bottom back, 22
            new Vector3(1,-1,-1)//right bottom back, 23
 
        };

        //Triangles// 3 points, clockwise determines which side is visible
        int[] triangles = new int[]
        {
            //front face//
            0,2,3,//first triangle
            3,1,0,//second triangle
 
            //back face//
            4,6,7,//first triangle
            7,5,4,//second triangle
 
            //left face//
            8,10,11,//first triangle
            11,9,8,//second triangle
 
            //right face//
            12,14,15,//first triangle
            15,13,12,//second triangle
 
            //top face//
            16,18,19,//first triangle
            19,17,16,//second triangle
 
            //bottom face//
            20,22,23,//first triangle
            23,21,20//second triangle
        };

        //UVs//
        Vector2[] uvs = new Vector2[]
        {
            //front face// 0,0 is bottom left, 1,1 is top right//
            new Vector2(0,1),
            new Vector2(0,0),
            new Vector2(1,1),
            new Vector2(1,0),

            new Vector2(0,1),
            new Vector2(0,0),
            new Vector2(1,1),
            new Vector2(1,0),

            new Vector2(0,1),
            new Vector2(0,0),
            new Vector2(1,1),
            new Vector2(1,0),

            new Vector2(0,1),
            new Vector2(0,0),
            new Vector2(1,1),
            new Vector2(1,0),

            new Vector2(0,1),
            new Vector2(0,0),
            new Vector2(1,1),
            new Vector2(1,0),

            new Vector2(0,1),
            new Vector2(0,0),
            new Vector2(1,1),
            new Vector2(1,0)
        };

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        
        mesh.RecalculateNormals();
    }
	
	// Update is called once per frame
	void Update () {
        if (waitN > 0f)
        {
            waitN -= Time.deltaTime;
        }
        else
        {
            waitN = waitD;
            shapeN++;
            if (shapeN > 3)
            {
                shapeN = 0;
            }

        }

        //morph to cube//
        if (shapeN == 0)
        {
            vertLeftTopFront = Vector3.Lerp(vertLeftTopFront, new Vector3(-1, 1, 1), Time.deltaTime);
            vertRightTopFront = Vector3.Lerp(vertRightTopFront, new Vector3(1, 1, 1), Time.deltaTime);
            vertRightTopBack = Vector3.Lerp(vertRightTopBack, new Vector3(1, 1, -1), Time.deltaTime);
            vertLeftTopBack = Vector3.Lerp(vertLeftTopBack, new Vector3(-1, 1, -1), Time.deltaTime);
        }

        //morph to pyramid//
        if (shapeN == 1)
        {
            vertLeftTopFront = Vector3.Lerp(vertLeftTopFront, new Vector3(0, 1, 0), Time.deltaTime);
            vertRightTopFront = Vector3.Lerp(vertRightTopFront, new Vector3(0, 1, 0), Time.deltaTime);
            vertRightTopBack = Vector3.Lerp(vertRightTopBack, new Vector3(0, 1, 0), Time.deltaTime);
            vertLeftTopBack = Vector3.Lerp(vertLeftTopBack, new Vector3(0, 1, 0), Time.deltaTime);
        }

        //morph to ramp//
        if (shapeN == 2)
        {
            vertLeftTopFront = Vector3.Lerp(vertLeftTopFront, new Vector3(-1, -1, 2), Time.deltaTime);
            vertRightTopFront = Vector3.Lerp(vertRightTopFront, new Vector3(1, -1, 2), Time.deltaTime);
            vertRightTopBack = Vector3.Lerp(vertRightTopBack, new Vector3(1, 0.5f, -1), Time.deltaTime);
            vertLeftTopBack = Vector3.Lerp(vertLeftTopBack, new Vector3(-1, 0.5f, -1), Time.deltaTime);
        }

        //morph to roof//
        if (shapeN == 3)
        {
            vertLeftTopFront = Vector3.Lerp(vertLeftTopFront, new Vector3(-1, 0.2f, 0), Time.deltaTime);
            vertRightTopFront = Vector3.Lerp(vertRightTopFront, new Vector3(1, 0.2f, 0), Time.deltaTime);
            vertRightTopBack = Vector3.Lerp(vertRightTopBack, new Vector3(1, 0.2f, 0), Time.deltaTime);
            vertLeftTopBack = Vector3.Lerp(vertLeftTopBack, new Vector3(-1, 0.2f, 0), Time.deltaTime);
        }

        //Start(); // Adjust this for realtime deformation
    }

    void LoadSimpleModelFile(Metadata metadata,Stream stream)
    {
        try                                                     // Attempt to read colortable (768 bytes, Adobe color table used for 256-color raw files)
        {
            StreamReader file = new StreamReader(stream);

            string[] numbers = file.ReadLine().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            Debug.Log(numbers[0]);      // String ID model type

            numbers = file.ReadLine().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            Debug.Log(numbers[0]);      // Version number (EVO1 uses Version4)

            numbers = file.ReadLine().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            int partCount = Convert.ToInt32(numbers[0]);       // Total number of parts in the file.
            Debug.Log(numbers[0]);      // Part count
            
            numbers = file.ReadLine().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            Debug.Log(numbers[0]);      // autoDetailFlag
            Debug.Log(numbers[1]);      // detailPixelHeight

            String[] partName = new String[partCount];
            for (int i = 0; i < partCount; i++)
            {
                numbers = file.ReadLine().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                partName[i] = numbers[0];   // part name

                numbers = file.ReadLine().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                // Status? (Always 1
                numbers = file.ReadLine().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                // Part Version (Always 1)
                numbers = file.ReadLine().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                // &vertexCount,&frameCount,&faceCount,&dummy
                numbers = file.ReadLine().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                // &kd, &ks, &power, &transarent,  		&envMap,    TEXTURE FILE NAME

                for(int j=0; j < 30; j++)       // Change to vertex count
                {
                    numbers = file.ReadLine().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    /*&vertexList[i].x,
			&vertexList[i].y,
			&vertexList[i].z,
			&vertexList[i].nx,
			&vertexList[i].ny,
			&vertexList[i].nz,
			&vertexList[i].u,
			&vertexList[i].v*/
                }

                for (int j = 0; j < 30; j++)       // Change to face count
                {
                    numbers = file.ReadLine().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    /*faceList[i * 3 + 0] = v1;
		faceList[i * 3 + 1] = v2;
		faceList[i * 3 + 2] = v3;*/
                }       // Look back for next part E

            }

            
        }
        catch (IOException e)
        {
            string debugStr = "ERROR: Failed to load SMF: " + metadata.Name + "!";
            Debug.Log(debugStr);
            Debug.Log(e);
        }
    }
}
