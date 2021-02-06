using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

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
    // Tilemaps
    public Tilemap groundMap;
    public Tilemap innerWallMap;
    public Tilemap outerWallMap;
    public Tilemap edgeMap;
    // Variables
    public int width;
    public int height;
    public int avgRoomSize;
    public int difficulty;
    public int level;
    private List<Dungeon> dungeons = new List<Dungeon>();
    public DungeonManager dungeonManager;
    public GameObject dungeonColliders;
    public GameObject dungeonCollider;

    public void AddDungeon(Dungeon dungeon)
    {
        dungeons.Add(dungeon);
    }

    void Start()
    {
        // Generate 
        if (difficulty == 4)
            GenerateBossRoom();
        else
        {
            width = difficulty * 75;
            height = difficulty * 75;
            Dungeon root = new Dungeon(new Rect(0, 0, width, height));
            Generate(root);
            root.CreateRoom();
            FillRooms(root);
            FillCorridors(root);
            FillTiles();
            PlaceColliders();
            // Place Player
            dungeonManager.PlacePlayer(dungeons[Random.Range(0, dungeons.Count)].room.center, true);
            // Place Doors
            FillDoors();
            // Place Entities
            PlaceEntities();
            // Start Level
            dungeonManager.uiManager.Level(level, difficulty);
        }
    }

    void Generate(Dungeon dungeon)
    {
        if (dungeon.IsLeaf())
        {
            if (dungeon.rect.width > avgRoomSize || dungeon.rect.height > avgRoomSize || Random.Range(0, 1) > 0.25)
            {
                if (dungeon.Split(avgRoomSize))
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
                if (Random.Range(0, 4) < difficulty)
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
                        dungeonManager.PlaceDoor(posUpperRight);
                    }
                }
            }
        }
    }

    private void PlaceColliders()
    {
        foreach (Dungeon dungeon in dungeons)
        {
            GameObject col = Instantiate(dungeonCollider, dungeon.room.center, Quaternion.Euler(0,0,0), dungeonColliders.transform);
            col.GetComponent<BoxCollider2D>().size = new Vector2(dungeon.room.width, dungeon.room.height);
            col.name = "Collider";
        }
    }

    private void PlaceEntities()
    {
        foreach (Dungeon dungeon in dungeons)
        {
            // Place NPCs
            bool wizard = false;
            if (difficulty >= 1 && Random.Range(0, 4) == 0)
            {
                dungeonManager.PlaceWizard(dungeon, difficulty);
                wizard = true;
            }
            if (difficulty >= 2 && Random.Range(0, 5) == 0 && !wizard)
            {
                dungeonManager.PlaceWizard(dungeon, difficulty);
                wizard = true; 
            }
            if (difficulty >= 3 && Random.Range(0, 6) == 0 && !wizard)
            {
                dungeonManager.PlaceWizard(dungeon, difficulty);
                wizard = true; 
            }
            bool merchant = false;
            if (difficulty >= 3 && Random.Range(0, 10) == 0 && !wizard)
            {
                dungeonManager.PlaceMerchant(dungeon);
                merchant = true;
            }
            if (difficulty >= 2 && Random.Range(0, 9) == 0 && !wizard && !merchant)
            {
                dungeonManager.PlaceMerchant(dungeon);
                merchant = true;
            }
            if (difficulty >= 1 && Random.Range(0, 8) == 0 && !wizard && !merchant)
            {
                dungeonManager.PlaceMerchant(dungeon);
                merchant = true;
            }
            // Place Enemies
            if (difficulty >= 1 && !wizard && !merchant)
                dungeonManager.PlaceEnemy(dungeon, difficulty);
            if (difficulty >= 2 && Random.Range(0, 2) == 0 && !wizard && !merchant)
                dungeonManager.PlaceEnemy(dungeon, difficulty);
            if (difficulty >= 3 && Random.Range(0, 2) == 0 && !wizard && !merchant)
                dungeonManager.PlaceEnemy(dungeon, difficulty);
            // Place Chests
            var chest = false;
            if (difficulty >= 3 && Random.Range(0, 2) == 0)
            {
                if (Random.Range(0, 3) == 0 && !wizard && !merchant)
                {
                    // Place Den
                    dungeonManager.PlaceDen(dungeon, difficulty);
                    chest = true;
                }
                else
                {
                    dungeonManager.PlaceChest(dungeon);
                    chest = true;
                }
            }
            if (difficulty >= 2 && Random.Range(0, 3) == 0 && !chest)
            {
                if (Random.Range(0, 3) == 0 && !wizard && !merchant)
                {
                    // Place Den
                    dungeonManager.PlaceDen(dungeon, difficulty);
                    chest = true;
                }
                else
                {
                    dungeonManager.PlaceChest(dungeon);
                    chest = true;
                }
            }
            if (difficulty >= 1 && Random.Range(0, 4) == 0 && !chest)
            {
                dungeonManager.PlaceChest(dungeon);
            }
            // Place Spikes
            if (difficulty >= 1)
            {
                dungeonManager.PlaceSpikes(dungeon);
                dungeonManager.PlaceSpikes(dungeon);
            }
            if (difficulty >= 2)
            {
                dungeonManager.PlaceSpikes(dungeon);
                dungeonManager.PlaceSpikes(dungeon);
                dungeonManager.PlaceSpikes(dungeon);
            }
            if (difficulty >= 3)
            {
                dungeonManager.PlaceSpikes(dungeon);
                dungeonManager.PlaceSpikes(dungeon);
                dungeonManager.PlaceSpikes(dungeon);
                dungeonManager.PlaceSpikes(dungeon);
            }
            // Place Items
            var key = false;
            if (difficulty <= 1)
            {
                dungeonManager.PlaceItem("Coin", dungeonManager.RandomPosition(dungeon.room));
                dungeonManager.PlaceItem("Coin", dungeonManager.RandomPosition(dungeon.room));
                if (Random.Range(0, 3) == 0)
                    dungeonManager.PlaceItem("Coin", dungeonManager.RandomPosition(dungeon.room));
                if (Random.Range(0, 3) == 0)
                    dungeonManager.PlaceItem("Coin", dungeonManager.RandomPosition(dungeon.room));
                if (Random.Range(0, 3) == 0)
                {
                    dungeonManager.PlaceItem("Key", dungeonManager.RandomPosition(dungeon.room));
                    key = true;
                }
            }
            if (difficulty <= 2)
            {
                dungeonManager.PlaceItem("Coin", dungeonManager.RandomPosition(dungeon.room));
                if (Random.Range(0, 3) == 0)
                    dungeonManager.PlaceItem("Coin", dungeonManager.RandomPosition(dungeon.room));
                if (Random.Range(0, 3) == 0)
                    dungeonManager.PlaceItem("Coin", dungeonManager.RandomPosition(dungeon.room));
                if (Random.Range(0, 3) == 0 && !key)
                {
                    dungeonManager.PlaceItem("Key", dungeonManager.RandomPosition(dungeon.room));
                    key = true;
                }
            }
            if (difficulty <= 3)
            {
                if (Random.Range(0, 3) == 0)
                    dungeonManager.PlaceItem("Coin", dungeonManager.RandomPosition(dungeon.room));
                if (Random.Range(0, 3) == 0)
                    dungeonManager.PlaceItem("Coin", dungeonManager.RandomPosition(dungeon.room));
                if (Random.Range(0, 3) == 0 && !key)
                {
                    dungeonManager.PlaceItem("Key", dungeonManager.RandomPosition(dungeon.room));
                }
            }
        }
    }

    private void GenerateBossRoom()
    {
        // Generate room
        for (int x = 0; x < 16; x++) {
            for (int y = 0; y < 16; y++) {
                groundMap.SetTile(new Vector3Int(x,y,0), groundTile);
            }
        }
        FillTiles();
        // Place Player
        dungeonManager.PlacePlayer(new Vector3(8, 3, 0), false);
        // Place Boss
        dungeonManager.PlaceBoss(new Vector3(8, 8, 0), 1);
        // Place Door
        dungeonManager.PlaceBossDoor(new Vector3(8, 17, 0));
    }

}
