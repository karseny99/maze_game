using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class MazeGenerator : MonoBehaviour
{
    [Header("Maze Dimensions")]
    public int width = 21; 
    public int height = 21;

    [Header("Scaling")]
    [Range(1, 5)]
    public int passageSize = 2; 

    [Header("Prefabs")]
    public GameObject wallPrefab;
    public GameObject floorPrefab;
    public GameObject buttonPrefab;
    public GameObject doorPrefab;
    public GameObject lampPrefab;
    public Material ceilingMaterial;

    [Header("Gameplay Settings")]
    public int buttonsToSpawn = 2;
    [Header("Corner Lighting")]
    [Range(0f, 1f)]
    public float cornerLampChance = 0.5f;
    
    private int[,] maze;
    private List<Vector2Int> deadEnds = new List<Vector2Int>();

    [ContextMenu("Generate Maze Now")]
    public void GenerateMaze()
    {
        ClearMaze();
        deadEnds.Clear();

        maze = new int[width, height];
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                maze[x, y] = 0;

        Backtrack(1, 1);
        SpawnMaze();
    }

    void Backtrack(int x, int y)
    {
        maze[x, y] = 1;
        bool isDeadEnd = true;

        Vector2Int[] dirs = {
            new Vector2Int(0, 2), new Vector2Int(0, -2),
            new Vector2Int(2, 0), new Vector2Int(-2, 0)
        };
        
        for (int i = 0; i < dirs.Length; i++) {
            int rnd = Random.Range(i, dirs.Length);
            var temp = dirs[rnd];
            dirs[rnd] = dirs[i];
            dirs[i] = temp;
        }

        foreach (var dir in dirs)
        {
            int nx = x + dir.x;
            int ny = y + dir.y;

            if (nx > 0 && nx < width - 1 && ny > 0 && ny < height - 1 && maze[nx, ny] == 0)
            {
                isDeadEnd = false;
                maze[x + dir.x / 2, y + dir.y / 2] = 1;
                Backtrack(nx, ny);
            }
        }

        if (isDeadEnd) {
            deadEnds.Add(new Vector2Int(x, y));
        }
    }

    void SpawnMaze()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 basePos = new Vector3(x * passageSize, 0, y * passageSize);

                for (int ix = 0; ix < passageSize; ix++)
                {
                    for (int iy = 0; iy < passageSize; iy++)
                    {
                        Vector3 offsetPos = basePos + new Vector3(ix, 0, iy);

                        if (maze[x, y] == 1)
                        {
                            Instantiate(floorPrefab, offsetPos, Quaternion.identity, transform);

                            Vector3 roofPos = offsetPos + Vector3.up * 2.75f; 
                            GameObject roof = Instantiate(floorPrefab, roofPos, Quaternion.Euler(180, 0, 0), transform);
                            
                            Renderer rend = roof.GetComponent<Renderer>();
                            if (rend != null) {
                                rend.material = ceilingMaterial;
                            }
                        }
                        else
                        {
                            Instantiate(wallPrefab, offsetPos + Vector3.up * 1.5f, Quaternion.identity, transform);
                        }
                    }
                }

                if (maze[x, y] == 1 && IsCorner(x, y) && Random.value < cornerLampChance)
                {
                    float cOffset = (passageSize - 1) / 2f;
                    Vector3 lampPos = new Vector3(x * passageSize + cOffset, 0.0f, y * passageSize + cOffset);
                    
                    if (!(x == 1 && y == 1))
                    {
                        Instantiate(lampPrefab, lampPos, Quaternion.identity, transform);
                    }
                }
            }
        }

        float centerOffset = (passageSize - 1) / 2f;

        Vector2Int exitPoint = new Vector2Int(width - 2, height - 2);
        if (maze[exitPoint.x, exitPoint.y] == 0) exitPoint = new Vector2Int(width - 2, height - 3);

        deadEnds.Remove(new Vector2Int(1, 1));
        deadEnds.Remove(exitPoint);
        deadEnds.Remove(new Vector2Int(exitPoint.x - 1, exitPoint.y));
        deadEnds.Remove(new Vector2Int(exitPoint.x, exitPoint.y - 1));

        Vector3 portalPos = new Vector3(exitPoint.x * passageSize + centerOffset, 0.5f, exitPoint.y * passageSize + centerOffset);
        GameObject exitPortal = Instantiate(doorPrefab, portalPos, Quaternion.identity, transform);

        var selectedDeadEnds = deadEnds.OrderBy(x => Random.value).Take(buttonsToSpawn).ToList();
        List<GameObject> spawnedButtons = new List<GameObject>();

        foreach (var point in selectedDeadEnds)
        {
            Vector3 pos = new Vector3(point.x * passageSize + centerOffset, 0.1f, point.y * passageSize + centerOffset);
            GameObject btn = Instantiate(buttonPrefab, pos, Quaternion.identity, transform);
            spawnedButtons.Add(btn);
        }

        ExitScript exitScript = exitPortal.GetComponent<ExitScript>();
        if (exitScript != null)
        {
            exitScript.logicButtons = spawnedButtons.Select(b => b.GetComponent<ButtonScript>()).ToArray();
        }
    }

    private bool IsCorner(int x, int y)
    {
        bool up    = (y + 1 < height) && maze[x, y + 1] == 1;
        bool down  = (y - 1 >= 0)     && maze[x, y - 1] == 1;
        bool left  = (x - 1 >= 0)     && maze[x - 1, y] == 1;
        bool right = (x + 1 < width)  && maze[x + 1, y] == 1;

        int neighborsCount = 0;
        if (up) neighborsCount++;
        if (down) neighborsCount++;
        if (left) neighborsCount++;
        if (right) neighborsCount++;

        if (neighborsCount == 2)
        {
            if (!(up && down) && !(left && right))
            {
                return true;
            }
        }
        return false;
    }

    public void ClearMaze()
    {
        while (transform.childCount > 0) {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
    }
}