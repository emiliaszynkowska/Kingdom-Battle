using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Schema;
using Unity.Collections;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class DungeonGenerator : MonoBehaviour
{
    // Tiles
    public Tile groundTile;
    public Tile wallTile;
    public Tile edgeTile;
    public Tile leftEdgeTile;
    public Tile rightEdgeTile;
    public Tile leftCornerTile;
    public Tile rightCornerTile;
    public Tile leftPointTile;
    public Tile rightPointTile;
    public Tile leftWallTile;
    public Tile rightWallTile;
    public Tile doorClosedTile;
    public Tile doorOpenTile;
    public Tile doorEdgeTile;
    public Tile doorLeftTile;
    public Tile doorRightTile;
    // Tilemaps
    public Tilemap groundMap;
    public Tilemap innerWallMap;
    public Tilemap outerWallMap;
    public Tilemap edgeMap;
    public Tilemap doorMap;
    // Variables
    public int width;
    public int height;
    public int minRoomSize;
    public int maxRoomSize;
    public int doorChance;
    public int enemyChance;
    private List<Dungeon> dungeons = new List<Dungeon>();
    // Game Objects
    public Image fade;
    public GameObject player;
    public GameObject enemies;
    public GameObject bosses;
    // Prefabs
    public GameObject impPrefab;
    public GameObject goblinPrefab;
    public GameObject chortPrefab;
    public GameObject orcPrefab;
    public GameObject knightPrefab;
    public GameObject necromancerPrefab;
    public GameObject demonPrefab;
    public GameObject ogrePrefab;
    public GameObject eliteKnightPrefab;
    public GameObject royalGuardianPrefab;
    public GameObject golemPrefab;
    public GameObject trollPrefab;

    public void AddDungeon(Dungeon dungeon)
    {
        dungeons.Add(dungeon);
    }
    
    void Start()
    {
        // Hide
        fade.GetComponent<CanvasRenderer>().SetAlpha(1);
        // Generate 
        Dungeon root = new Dungeon(new Rect(0,0,width,height));
        Generate(root); 
        root.CreateRoom();
        FillRooms(root);
        FillCorridors(root);
        FillTiles();
        // Show 
        StartCoroutine("FadeIn");
        // Place Player
        player.transform.position = dungeons[Random.Range(0, dungeons.Count)].room.center;
        // Place Doors
        FillDoors();
        // Place Enemies
        foreach (Dungeon dungeon in dungeons)
        {
            var chance = Random.Range(0, 100);
            if (chance < enemyChance)
                GenerateEnemy(dungeon);
            if (chance < 20) 
                GenerateEnemy(dungeon);
            if (chance < 10)
                GenerateEnemy(dungeon);
        }
        // Place Items
    }

    void Generate(Dungeon dungeon)
    {
        if (dungeon.IsLeaf())
        {
            if (dungeon.rect.width > maxRoomSize || dungeon.rect.height > maxRoomSize || Random.Range(0, 1) > 0.25)
            {
                if (dungeon.Split(minRoomSize, maxRoomSize))
                {
                    Generate(dungeon.left);
                    Generate(dungeon.right);
                }
            }
        }
    }

    void FillRooms(Dungeon dungeon)
    {
        if (dungeon == null)
            return;
        if (dungeon.IsLeaf())
        {
            for (int x = (int) dungeon.room.x; x < dungeon.room.xMax; x++) {
                for (int y = (int) dungeon.room.y; y < dungeon.room.yMax; y++) {
                    groundMap.SetTile(new Vector3Int(x,y,0), groundTile);
                }
            }
        } else {
            FillRooms(dungeon.left);
            FillRooms(dungeon.right);
        }
    }

    void FillCorridors(Dungeon dungeon)
    {
        if (dungeon == null)
            return;
        
        FillCorridors(dungeon.left);
        FillCorridors(dungeon.right);

        foreach (Rect corridor in dungeon.corridors)
        {
            for (int i = (int) corridor.x; i < corridor.xMax; i++)
            {
                for (int j = (int) corridor.y; j < corridor.yMax; j++)
                {
                    groundMap.SetTile(new Vector3Int(i, j, 0), groundTile);
                }
            }
        }
    }
    
    private void FillTiles()
    {
        BoundsInt bounds = groundMap.cellBounds;
        for (int xMap = bounds.xMin - 10; xMap <= bounds.xMax + 10; xMap++)
        {
            for (int yMap = bounds.yMin - 10; yMap <= bounds.yMax + 10; yMap++)
            {
                Vector3Int pos = new Vector3Int(xMap, yMap, 0);
                Vector3Int posLower = new Vector3Int(xMap, yMap - 1, 0);
                Vector3Int posUpper = new Vector3Int(xMap, yMap + 1, 0);

                if (groundMap.GetTile(pos) == null)
                {
                    //If it is a top wall
                    if (groundMap.GetTile(posLower) != null)
                    {
                        //Fill in a wall and edge
                        outerWallMap.SetTile(pos,wallTile);
                        edgeMap.SetTile(posUpper,edgeTile);
                        
                    }
                    //If it is a bottom wall
                    else if (groundMap.GetTile(posUpper) != null)
                    {
                        //Fill in a wall and edge
                        outerWallMap.SetTile(pos,wallTile);
                        edgeMap.SetTile(posUpper,edgeTile);
                    }
                }
            }
        }
        
        for (int xMap = bounds.xMin - 10; xMap <= bounds.xMax + 10; xMap++)
        {
            for (int yMap = bounds.yMin - 10; yMap <= bounds.yMax + 10; yMap++)
            {
                Vector3Int pos = new Vector3Int(xMap, yMap, 0);
                Vector3Int posLower = new Vector3Int(xMap, yMap - 1, 0);
                Vector3Int posUpper = new Vector3Int(xMap, yMap + 1, 0);
                Vector3Int posLeft = new Vector3Int(xMap - 1, yMap, 0);
                Vector3Int posRight = new Vector3Int(xMap + 1, yMap, 0);
                Vector3Int posUpperLeft = new Vector3Int(xMap - 1, yMap + 1, 0);
                Vector3Int posUpperRight = new Vector3Int(xMap + 1, yMap + 1, 0);
                Vector3Int posLowerLeft = new Vector3Int(xMap - 1, yMap - 1, 0);
                Vector3Int posLowerRight = new Vector3Int(xMap + 1, yMap - 1, 0);

                if (groundMap.GetTile(pos) != null)
                {
                    //It it is a left edge
                    if (groundMap.GetTile(posLeft) == null && outerWallMap.GetTile(posLeft) == null)
                    {
                        //If it is a top left corner
                        if (outerWallMap.GetTile(posUpper) != null)
                        {
                            //Fill in a top left corner
                            edgeMap.SetTile(pos,leftEdgeTile);
                            edgeMap.SetTile(posUpper,leftEdgeTile);
                        }
                        //If it is a bottom left corner
                        else if (outerWallMap.GetTile(posLower) != null)
                        {
                            //Fill in a bottom left corner
                            edgeMap.SetTile(pos,leftCornerTile);
                        }
                        else
                        {
                            //Fill in a left edge
                            edgeMap.SetTile(pos,leftEdgeTile);
                        }
                    }
                    
                    //If it is a right edge
                    if (groundMap.GetTile(posRight) == null && outerWallMap.GetTile(posRight) == null)
                    {
                        //If it is a top right corner
                        if (outerWallMap.GetTile(posUpper) != null)
                        {
                            //Fill in a top right corner
                            edgeMap.SetTile(pos,rightEdgeTile);
                            edgeMap.SetTile(posUpper,rightEdgeTile);
                        }
                        //If it is a bottom right corner
                        else if (outerWallMap.GetTile(posLower) != null)
                        {
                            //Fill in a bottom right corner
                            edgeMap.SetTile(pos,rightCornerTile);
                        }
                        else
                        {
                            //Fill in a right edge
                            edgeMap.SetTile(pos,rightEdgeTile);
                        }
                    }
                    
                    //If a left edge appears by a wall
                    if (outerWallMap.GetTile(posLeft) != null && groundMap.GetTile(posRight) != null && groundMap.GetTile(posUpper) != null && groundMap.GetTile(posLowerLeft) == null)
                        edgeMap.SetTile(pos,leftEdgeTile);
                    //If a right edge appears by a wall
                    if (outerWallMap.GetTile(posRight) != null && groundMap.GetTile(posLeft) != null && groundMap.GetTile(posUpper) != null && groundMap.GetTile(posLowerRight) == null)
                        edgeMap.SetTile(pos,rightEdgeTile);
                    
                    //If two walls appear diagonally
                    if (outerWallMap.GetTile(posLeft) != null && outerWallMap.GetTile(posUpper) != null)
                    {
                        edgeMap.SetTile(posUpperLeft, rightCornerTile);
                        edgeMap.SetTile(new Vector3Int(xMap - 1, yMap + 2, 0),leftPointTile);
                    }

                    if (outerWallMap.GetTile(posRight) != null && outerWallMap.GetTile(posUpper) != null)
                    {
                        edgeMap.SetTile(posUpperRight, leftCornerTile);
                        edgeMap.SetTile(new Vector3Int(xMap + 1, yMap + 2, 0),rightPointTile);
                    }

                    if (outerWallMap.GetTile(posLeft) != null && outerWallMap.GetTile(posLower) != null)
                    {
                        edgeMap.SetTile(pos,leftCornerTile);
                        edgeMap.SetTile(posUpper,rightPointTile);
                    }
                    if (outerWallMap.GetTile(posRight) != null && outerWallMap.GetTile(posLower) != null)
                    {
                        edgeMap.SetTile(pos,rightCornerTile);
                        edgeMap.SetTile(posUpper,leftPointTile);
                    }
                }

                if (outerWallMap.GetTile(pos) != null)
                {
                    //If a wall comes to a point
                    if (groundMap.GetTile(posLeft) != null && groundMap.GetTile(posUpper) != null)
                        edgeMap.SetTile(posUpperLeft,leftPointTile);
                    if (groundMap.GetTile(posRight) != null && groundMap.GetTile(posUpper) != null)
                        edgeMap.SetTile(posUpperRight,rightPointTile);
                    
                    //If a wall extends with the edge
                    if (groundMap.GetTile(posLeft) != null && edgeMap.GetTile(posLeft) == null && edgeMap.GetTile(posUpper) != null && outerWallMap.GetTile(posUpperLeft) == null)
                    {
                        innerWallMap.SetTile(posLeft,leftWallTile);
                    }
                    if (groundMap.GetTile(posRight) != null && edgeMap.GetTile(posRight) == null && edgeMap.GetTile(posUpper) != null && outerWallMap.GetTile(posUpperRight) == null)
                    {
                        innerWallMap.SetTile(posRight,rightWallTile);
                    }
                }
            }
        }
    }

    private void FillDoors()
    {
        BoundsInt bounds = groundMap.cellBounds;
        for (int xMap = bounds.xMin - 10; xMap <= bounds.xMax + 10; xMap++)
        {
            for (int yMap = bounds.yMin - 10; yMap <= bounds.yMax + 10; yMap++)
            {
                // Calculate probability
                if (Random.Range(0, 100) < doorChance)
                {
                    Vector3Int pos1 = new Vector3Int(xMap, yMap, 0);
                    Vector3Int pos2 = new Vector3Int(xMap + 1, yMap, 0);
                    Vector3Int posUpperLeft = new Vector3Int(xMap, yMap + 1, 0);
                    Vector3Int posUpperRight = new Vector3Int(xMap + 1, yMap + 1, 0);
                    Vector3Int posLeft = new Vector3Int(xMap - 1, yMap, 0);
                    Vector3Int posRight = new Vector3Int(xMap + 2, yMap, 0);
                    Vector3Int posLeftLeft = new Vector3Int(xMap - 2, yMap, 0);
                    Vector3Int posRightRight = new Vector3Int(xMap + 3, yMap, 0);

                    // If a doorway exists
                    if (groundMap.GetTile(pos1) != null && groundMap.GetTile(pos2) != null &&
                        groundMap.GetTile(posUpperLeft) != null && groundMap.GetTile(posUpperRight) != null &&
                        outerWallMap.GetTile(posLeft) != null && outerWallMap.GetTile(posRight) != null
                        && outerWallMap.GetTile(posLeftLeft) != null & outerWallMap.GetTile(posRightRight) != null)
                    {
                        // Place a door
                        doorMap.SetTile(pos1, doorClosedTile);
                        doorMap.SetTile(posUpperLeft, doorEdgeTile);
                        doorMap.SetTile(posLeft, doorLeftTile);
                        doorMap.SetTile(pos2, doorRightTile);
                    }
                }
            }
        }
    }

    private Vector2 RandomPosition(Rect room)
    {
        return new Vector2(Random.Range(room.xMin + 1, room.xMax - 1), Random.Range(room.yMin + 1, room.yMax - 1));
    }

    private void GenerateEnemy(Dungeon dungeon)
    {
        var choice = Random.Range(0, 7);
        GameObject prefab = null;
        switch (choice)
        {
            case(0):
                prefab = impPrefab;
                break;
            case(1):
                prefab = goblinPrefab;
                break;
            case(2):
                prefab = chortPrefab;
                break;
            case(3):
                prefab = orcPrefab;
                break;
            case(4):
                prefab = knightPrefab;
                break;
            case(5):
                prefab = necromancerPrefab;
                break;
            case(6):
                prefab = demonPrefab;
                break;
            case(7):
                prefab = ogrePrefab;
                break;
        }
        GameObject enemy = Instantiate(prefab, RandomPosition(dungeon.room), Quaternion.Euler(0, 0, 0));
        enemy.transform.SetParent(enemies.transform);
    }
    
    IEnumerator FadeIn()
    {
        fade.CrossFadeAlpha(0.0f, 1, false);
        yield return new WaitForSeconds(2);
    }
    
    IEnumerator FadeOut()
    {
        fade.CrossFadeAlpha(1.0f, 1, false);
        yield return new WaitForSeconds(2);
    }

}
