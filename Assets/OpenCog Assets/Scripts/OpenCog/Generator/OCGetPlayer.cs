using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// creates a 4x4x4 block that can serve as a Player.
/// </summary>

public class OCGetPlayer : MonoBehaviour
{
    public static int width = 4;
    public static byte[, ,] map;

    private static Mesh mesh;
    private static List<Vector3> vert = new List<Vector3>();
    private static List<int> tris = new List<int>();
    private static List<Vector2> uv = new List<Vector2>();
    private static MeshCollider meshcollider;
    private static MeshRenderer meshrenderer;
    private static Material sharedMaterial;
    public static void Create(Vector3 v)
    {
        Create(v.x, v.y, v.z);
    }
    /// <summary>
    /// gets the position info of the player from Minecraft and use it as its position for unity 3d;
    /// </summary>
    /// <param name="a">x global coordinate of the player</param>
    /// <param name="b">y global coordinate of the player</param>
    /// <param name="c">z global coordinate of the player</param>
    /// <remarks>you can change the width value to change the size of the blocks</remarks>
   private static void Create(float a, float b, float c)
    {
        GameObject go = new GameObject();
        go.transform.position = new Vector3(a, b, c);
        meshcollider = go.AddComponent<MeshCollider>();
        meshrenderer = go.AddComponent<MeshRenderer>();
        sharedMaterial = new Material(Shader.Find("Diffuse"));
        //sharedMaterial.color = Color.green;
        meshrenderer.sharedMaterial = sharedMaterial;
        meshrenderer.renderer.sharedMaterial.mainTexture = Resources.Load("wood") as Texture;
        map = new byte[width, width, width];
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < width; z++)
            {
                for (int h = 0; h < width; h++)
                {
                    map[x, h, z] = 1;

                }

            }
        }


        mesh = new Mesh();
       go.AddComponent<MeshFilter>().mesh = mesh;

        Regenerate();

    }
    public static bool IsTransparent(int x, int y, int z)
    {
        if ((x < 0) || (y < 0) || (z < 0) || (x >= width) || (y >= width) || (z >= width))
            return true;
        return map[x, y, z] == 0;
    }

    public static void DrawFace(Vector3 start, Vector3 offset1, Vector3 offset2, byte block)
    {
        int index = vert.Count;
        vert.Add(start);
        vert.Add(start + offset1);
        vert.Add(start + offset2);
        vert.Add(start + offset2 + offset1);
        uv.Add(new Vector2(0, 0));
        uv.Add(new Vector2(0, 1));
        uv.Add(new Vector2(1, 0));
        uv.Add(new Vector2(1, 1));


        tris.Add(index + 0);
        tris.Add(index + 1);
        tris.Add(index + 2);

        tris.Add(index + 3);
        tris.Add(index + 2);
        tris.Add(index + 1);

    }


    public static void Regenerate()
    {
        vert.Clear();
        tris.Clear();
        uv.Clear();
        mesh.triangles = tris.ToArray();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < width; y++)
            {
                for (int z = 0; z < width; z++)
                {
                    byte block = map[x, y, z];
                    if (block == 0)
                        continue;

                    DrawBrick(x, y, z, block);
                }
            }
        }

        mesh.vertices = vert.ToArray();
        mesh.triangles = tris.ToArray();
        mesh.RecalculateNormals();
        meshcollider.sharedMesh = null;
        meshcollider.sharedMesh = mesh;
        mesh.uv = uv.ToArray();

    }
    public static void DrawBrick(int x, int y, int z, byte block)
    {
        Vector3 start = new Vector3(x, y, z);
        Vector3 offset1, offset2;
        if (IsTransparent(x, y - 1, z))
        {
            offset1 = Vector3.left;
            offset2 = Vector3.back;
            DrawFace(start + Vector3.right, offset1, offset2, block);
        }
        if (IsTransparent(x, y + 1, z))
        {
            offset1 = Vector3.right;
            offset2 = Vector3.back;
            DrawFace(start + Vector3.up, offset1, offset2, block);
        }
        if (IsTransparent(x - 1, y, z))
        {
            offset1 = Vector3.up;
            offset2 = Vector3.back;
            DrawFace(start, offset1, offset2, block);
        }
        if (IsTransparent(x + 1, y, z))
        {
            offset1 = Vector3.down;
            offset2 = Vector3.back;
            DrawFace(start + Vector3.right + Vector3.up, offset1, offset2, block);
        }
        if (IsTransparent(x, y, z - 1))
        {
            offset1 = Vector3.left;
            offset2 = Vector3.up;
            DrawFace(start + Vector3.right + Vector3.back, offset1, offset2, block);
        }
        if (IsTransparent(x, y, z + 1))
        {
            offset1 = Vector3.right;
            offset2 = Vector3.up;
            DrawFace(start, offset1, offset2, block);
        }

    }

}
