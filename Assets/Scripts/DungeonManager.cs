using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class DungeonManager : MonoBehaviour
{
    // Game Objects
    public UIManager uiManager;
    public GameObject player;
    public GameObject enemies;
    public GameObject bosses;
    public GameObject items;
    public GameObject objects;
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
    public GameObject coinPrefab;
    public GameObject keyPrefab;
    public GameObject scrollPrefab;
    public GameObject wiggsBrewPrefab;
    public GameObject liquidLuckPrefab;
    public GameObject ogresStrengthPrefab;
    public GameObject elixirofSpeedPrefab;
    public GameObject doorPrefab;
    public GameObject chestPrefab;
    public GameObject spikesPrefab;
    public GameObject shinePrefab;
    public GameObject denPrefab;
    public GameObject campfirePrefab;

    void Start()
    {
        StartCoroutine(uiManager.FadeOut());
        uiManager.Level(1);
    }

    public Vector2 RandomPosition(Rect room)
    {
        Vector2 position = new Vector2((int) Random.Range(room.xMin + 1, room.xMax - 1), (int) Random.Range(room.yMin + 1, room.yMax - 1));
        if (position != (Vector2) player.transform.position && position != new Vector2(player.transform.position.x - 1, player.transform.position.y - 1))
            return position;
        else
            return RandomPosition(room);
    }

    public void PlacePlayer(Vector3 position)
    {
        player.transform.position = position;
        GameObject campfire = Instantiate(campfirePrefab, new Vector3(position.x - 1, position.y - 1, -3), Quaternion.Euler(0, 0, 0), objects.transform);
        campfire.name = "Campfire";
    }

    public void PlaceShine(Vector3 position)
    {
        position.x += 0.5f;
        position.y += 0.6f;
        GameObject shine = Instantiate(shinePrefab, position, Quaternion.Euler(0, 0, 0), objects.transform);
        shine.name = "Shine";
    }

    public void PlaceDoor(Vector3 position)
    {
        GameObject door = Instantiate(doorPrefab, position, Quaternion.Euler(0, 0, 0), objects.transform);
        door.name = "Door";
    }
    
    public void PlaceChest(Vector3 position)
    {
        position.x += 0.5f;
        position.y += 0.6f;
        GameObject chest = Instantiate(chestPrefab, position, Quaternion.Euler(0, 0, 0), objects.transform);
        chest.name = "Chest";
    }
    
    public void PlaceChest(Dungeon dungeon)
    {
        Vector3 position = RandomPosition(dungeon.room);
        position.x += 0.5f;
        position.y += 0.6f;
        GameObject chest = Instantiate(chestPrefab, position, Quaternion.Euler(0, 0, 0), objects.transform);
        chest.name = "Chest";
    }
    
    public void PlaceSpikes(Vector3 position)
    {
        position.x += 0.5f;
        position.y += 0.5f;
        GameObject spikes = Instantiate(spikesPrefab, position, Quaternion.Euler(0, 0, 0), objects.transform);
        spikes.name = "Spikes";
    }

    public void PlaceSpikes(Dungeon dungeon)
    {
        Vector3 position = RandomPosition(dungeon.room);
        position.x += 0.5f;
        position.y += 0.5f;
        GameObject spikes = Instantiate(spikesPrefab, position, Quaternion.Euler(0, 0, 0), objects.transform);
        spikes.name = "Spikes";
    }
    
    public void PlaceItem(string itemName, Vector3 position)
    {
        switch (itemName)
        {
            case("Coin"):
                GameObject item = Instantiate(coinPrefab, position, Quaternion.Euler(0, 0, 0), items.transform);
                item.name = "Coin";
                break;
            case("Coins"):
                item = Instantiate(coinPrefab, position, Quaternion.Euler(0, 0, 0), items.transform);
                item.name = "Coin";
                item = Instantiate(coinPrefab, new Vector3(position.x + Random.Range(-1,1), position.y + Random.Range(-1,1), 0), Quaternion.Euler(0, 0, 0), items.transform);
                item.name = "Coin";
                var chance = Random.Range(0, 3);
                if (chance > 0)
                {
                    item = Instantiate(coinPrefab, new Vector3(position.x + Random.Range(-1, 1), position.y + Random.Range(-1, 1), 0), Quaternion.Euler(0, 0, 0), items.transform);
                    item.name = "Coin";
                }
                if (chance > 1)
                {
                    item = Instantiate(coinPrefab, new Vector3(position.x + Random.Range(-1,1), position.y + Random.Range(-1,1), 0), Quaternion.Euler(0, 0, 0), items.transform);
                    item.name = "Coin";
                }
                break;
            case("Key"):
                item = Instantiate(keyPrefab, position, Quaternion.Euler(0, 0, 0), items.transform);
                item.name = "Key";
                break;
            case("Scroll"):
                item = Instantiate(scrollPrefab, position, Quaternion.Euler(0, 0, 0), items.transform);
                item.name = "Scroll";
                break;
            case("Wigg's Brew"):
                item = Instantiate(wiggsBrewPrefab, position, Quaternion.Euler(0, 0, 0), items.transform);
                item.name = "Wigg's Brew";
                break;
            case("Liquid Luck"):
                item = Instantiate(liquidLuckPrefab, position, Quaternion.Euler(0, 0, 0), items.transform);
                item.name = "Liquid Luck";
                break;
            case("Ogre's Strength"):
                item = Instantiate(ogresStrengthPrefab, position, Quaternion.Euler(0, 0, 0), items.transform);
                item.name = "Ogre's Strength";
                break;
            case("Elixir of Speed"):
                item = Instantiate(elixirofSpeedPrefab, position, Quaternion.Euler(0, 0, 0), items.transform);
                item.name = "Elixir of Speed";
                break;
        }
    }

    public GameObject PlaceEnemy(Dungeon dungeon)
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
        GameObject enemy = Instantiate(prefab, RandomPosition(dungeon.room), Quaternion.Euler(0, 0, 0), enemies.transform);
        enemy.GetComponent<EnemyController>().SetDungeon(dungeon);
        enemy.name = enemy.name.Replace("(Clone)", "");
        return enemy;
    }

    public void PlaceDen(Dungeon dungeon)
    {
        Vector2 position = dungeon.room.center;
        if (position != (Vector2) player.transform.position)
        {
            position = new Vector2((int) position.x, (int) position.y);
            GameObject den = Instantiate(denPrefab, position, Quaternion.Euler(0, 0, 0), objects.transform);
            den.name = "Den";
            den.GetComponent<DenController>().enemy1 = PlaceEnemy(dungeon);
            den.GetComponent<DenController>().enemy2 = PlaceEnemy(dungeon);
            den.GetComponent<DenController>().enemy3 = PlaceEnemy(dungeon);
            den.GetComponent<DenController>().active = true;
        }
    }
    
}