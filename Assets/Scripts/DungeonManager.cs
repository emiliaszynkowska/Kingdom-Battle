using UnityEngine;
using Random = UnityEngine.Random;

public class DungeonManager : MonoBehaviour
{
    // Game Objects
    public UIManager uiManager;
    public GameObject player;
    public GameObject NPCs;
    public GameObject enemies;
    public GameObject items;
    public GameObject objects;
    public GameObject colliders;
    public Rect playerRoom;
    // Enemies
    public GameObject impPrefab;
    public GameObject goblinPrefab;
    public GameObject chortPrefab;
    public GameObject orcPrefab;
    public GameObject knightPrefab;
    public GameObject necromancerPrefab;
    public GameObject demonPrefab;
    public GameObject ogrePrefab;
    // Bosses
    public GameObject eliteKnightPrefab;
    public GameObject royalGuardianPrefab;
    public GameObject golemPrefab;
    public GameObject trollPrefab;
    // NPCs
    public GameObject merchantPrefab;
    public GameObject wizardFetchPrefab;
    public GameObject wizardDefeatPrefab;
    public GameObject wizardRescuePrefab;
    // Items
    public GameObject coinPrefab;
    public GameObject keyPrefab;
    public GameObject bossKeyPrefab;
    public GameObject scrollPrefab;
    public GameObject wiggsBrewPrefab;
    public GameObject liquidLuckPrefab;
    public GameObject ogresStrengthPrefab;
    public GameObject elixirofSpeedPrefab;
    // Objects
    public GameObject denPrefab;
    public GameObject doorPrefab;
    public GameObject chestPrefab;
    public GameObject spikesPrefab;
    public GameObject shinePrefab;
    public GameObject campfirePrefab;

    void Start()
    {
        StartCoroutine(uiManager.FadeOut());
    }

    public Vector2 RandomPosition(Rect room)
    {
        Vector2 position = new Vector2((int) Random.Range(room.xMin + 1, room.xMax - 1),
            (int) Random.Range(room.yMin + 1, room.yMax - 1));
        if (Mathf.Abs(player.transform.position.magnitude - position.magnitude) > 1)
            return position;
        else
            return RandomPosition(room);
    }

    public void PlacePlayer(Rect room)
    {
        playerRoom = room;
        player.transform.position = room.center;
        GameObject campfire = Instantiate(campfirePrefab, new Vector3(room.center.x - 1, room.center.y - 1, -3), Quaternion.Euler(0, 0, 0), objects.transform);
        campfire.name = "Campfire";
    }
    
    public void PlacePlayer(Vector3 position)
    {
        player.transform.position = position;
    }

    public void PlaceDoor(Vector3 position)
    {
        if (position.magnitude - player.transform.position.magnitude > 3)
        {
            GameObject door = Instantiate(doorPrefab, position, Quaternion.Euler(0, 0, 0), objects.transform);
            door.name = "Door";
        }
    }
    
    public void PlaceBossDoor(Vector3 position)
    {
        GameObject door = Instantiate(doorPrefab, position, Quaternion.Euler(0, 0, 0), objects.transform);
        door.name = "Boss Door";
        door.GetComponent<DoorController>().boss = true;
        GameObject shine = Instantiate(shinePrefab, position, Quaternion.Euler(0, 0, 0), objects.transform);
        shine.name = "Shine";
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
        if (position != Vector3.zero)
        {
            position.x += 0.5f;
            position.y += 0.5f;
            GameObject spikes = Instantiate(spikesPrefab, position, Quaternion.Euler(0, 0, 0), objects.transform);
            spikes.name = "Spikes";
        }
    }

    public void PlaceMerchant(Dungeon dungeon)
    {
        if (dungeon.rect != playerRoom)
        {
            if (dungeon.room.width >= 10 && dungeon.room.height >= 4 &&
                dungeon.room.center != (Vector2) player.transform.position)
            {
                GameObject merchant = Instantiate(merchantPrefab, dungeon.room.center, Quaternion.Euler(0, 0, 0),
                    NPCs.transform);
                merchant.name = "Merchant";
            }
        }
    }

    public void PlaceWizard(Vector3 position, int difficulty)
    {
        Dungeon dungeon = new Dungeon(new Rect(position.x - 1, position.y - 3, 2, 6));
        dungeon.room = new Rect(position.x - 1, position.y - 3, 2, 6);
        var choice = Random.Range(0, 3);
        switch (choice)
        {
            case (0):
                GameObject questFetch = Instantiate(wizardFetchPrefab, position, Quaternion.Euler(0, 0, 0), NPCs.transform);
                questFetch.name = "WizardFetch";
                break;
            case (1):
                GameObject questDefeat = Instantiate(wizardDefeatPrefab, position, Quaternion.Euler(0, 0, 0), NPCs.transform);
                GameObject obj = PlaceEnemy(dungeon,difficulty);
                questDefeat.name = "WizardDefeat";
                questDefeat.GetComponent<QuestDefeat>().obj = obj;
                break;
            case (2):
                GameObject questRescue = Instantiate(wizardRescuePrefab, position, Quaternion.Euler(0, 0, 0), NPCs.transform);
                GameObject obj1 = PlaceEnemy(dungeon,difficulty);
                GameObject obj2 = PlaceEnemy(dungeon,difficulty);
                GameObject obj3 = PlaceEnemy(dungeon,difficulty);
                questRescue.name = "WizardRescue";
                questRescue.GetComponent<QuestRescue>().obj1 = obj1;
                questRescue.GetComponent<QuestRescue>().obj2 = obj2;
                questRescue.GetComponent<QuestRescue>().obj3 = obj3;
                break;
        }
    }

    public GameObject PlaceEnemy(Dungeon dungeon, int difficulty)
    {
        if (uiManager.bossFight || (!uiManager.bossFight && dungeon.rect != playerRoom))
        {
            if (difficulty >= 75)
                return PlaceEnemyHard(dungeon);
            else if (difficulty >= 50)
                return PlaceEnemyMed(dungeon);
            else if (difficulty >= 25)
                return PlaceEnemyEasy(dungeon);
            else if (difficulty >= 10)
                return PlaceEnemyVeryEasy(dungeon);
        }
        return null;
    }
    
    public GameObject PlaceEnemyVeryEasy(Dungeon dungeon)
    {
        GameObject enemy = Instantiate(impPrefab, RandomPosition(dungeon.room), Quaternion.Euler(0, 0, 0), enemies.transform);
        enemy.GetComponent<EnemyController>().SetDungeon(dungeon);
        enemy.name = enemy.name.Replace("(Clone)", "");
        return enemy;
    }
    
    public GameObject PlaceEnemyEasy(Dungeon dungeon)
    {
        var choice1 = Random.Range(0, 2);
        GameObject prefab = null;
        switch (choice1)
        {
            case(0):
                prefab = impPrefab;
                break;
            case(1):
                prefab = goblinPrefab;
                break;
        }
        GameObject enemy = Instantiate(prefab, RandomPosition(dungeon.room), Quaternion.Euler(0, 0, 0), enemies.transform);
        enemy.GetComponent<EnemyController>().SetDungeon(dungeon);
        enemy.name = enemy.name.Replace("(Clone)", "");
        return enemy;
    }
    
    public GameObject PlaceEnemyMed(Dungeon dungeon)
    {
        var choice = Random.Range(0, 6);
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
        }
        GameObject enemy = Instantiate(prefab, RandomPosition(dungeon.room), Quaternion.Euler(0, 0, 0), enemies.transform);
        enemy.GetComponent<EnemyController>().SetDungeon(dungeon);
        enemy.name = enemy.name.Replace("(Clone)", "");
        return enemy;
    }
    
    public GameObject PlaceEnemyHard(Dungeon dungeon)
    {
        var choice = Random.Range(0, 8);
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
    
    public void PlaceDen(Dungeon dungeon, int difficulty)
    {
        if (dungeon.rect != playerRoom)
        {
            Vector2 position = dungeon.room.center;
            if (position != (Vector2) player.transform.position)
            {
                position = new Vector2((int) position.x, (int) position.y);
                GameObject den = Instantiate(denPrefab, position, Quaternion.Euler(0, 0, 0), objects.transform);
                den.name = "Den";
                if (difficulty >= 75)
                {
                    den.GetComponent<DenController>().enemy1 = PlaceEnemyHard(dungeon);
                    den.GetComponent<DenController>().enemy2 = PlaceEnemyHard(dungeon);
                    den.GetComponent<DenController>().enemy3 = PlaceEnemyHard(dungeon); 
                }
                else if (difficulty >= 50)
                {
                    den.GetComponent<DenController>().enemy1 = PlaceEnemyMed(dungeon);
                    den.GetComponent<DenController>().enemy2 = PlaceEnemyMed(dungeon);
                    den.GetComponent<DenController>().enemy3 = PlaceEnemyMed(dungeon);
                }
                else if (difficulty >= 10)
                {
                    den.GetComponent<DenController>().enemy1 = PlaceEnemyEasy(dungeon);
                    den.GetComponent<DenController>().enemy2 = PlaceEnemyEasy(dungeon);
                    den.GetComponent<DenController>().enemy3 = PlaceEnemyEasy(dungeon);
                }
                den.GetComponent<DenController>().enemy1.GetComponent<EnemyController>().den = true;
                den.GetComponent<DenController>().enemy2.GetComponent<EnemyController>().den = true;
                den.GetComponent<DenController>().enemy3.GetComponent<EnemyController>().den = true;
                den.GetComponent<DenController>().active = true;
            }
        }
    }
    
    public void PlaceBoss(Vector3 position, int level, int difficulty)
    {
        var choice = Random.Range(0, 4);
        GameObject prefab = null;
        switch (choice)
        {
            case(0):
                prefab = eliteKnightPrefab;
                break;
            case(1):
                prefab = royalGuardianPrefab;
                break;
            case(2):
                prefab = golemPrefab;
                break;
            case(3):
                prefab = trollPrefab;
                break;
        }
        GameObject boss = Instantiate(prefab, position, Quaternion.Euler(0, 0, 0), enemies.transform);
        boss.name = boss.name.Replace("(Clone)", "");
        boss.GetComponent<EnemyController>().SetBoss();
        boss.GetComponent<EnemyController>().SetDifficulty(difficulty);
        boss.GetComponent<EnemyController>().health = (difficulty <= 10 ? 10 : difficulty <= 25 ? 25 : difficulty <= 50 ? 50 : difficulty <= 75 ? 75 : 100);
        boss.GetComponent<EnemyController>().attack = (difficulty <= 10 ? 1 : difficulty <= 25 ? 2 : difficulty <= 50 ? 3 : difficulty <= 75 ? 4 : 5);
        switch (level)
        {
            // Boss 1 - Jagged Blade
            case(1):
                boss.GetComponent<EnemyController>().SetUpgrade("Jagged Blade");
                break;
            // Boss 4 - Warped Edge & Blue Robe
            case(4):
                boss.GetComponent<EnemyController>().SetUpgrade("Warped Edge");
                boss.GetComponent<EnemyController>().SetUpgrade("Blue Robe");
                break;
            // Boss 7 - Knight's Sword & Red Robe
            case(7):
                boss.GetComponent<EnemyController>().SetUpgrade("Knight's Sword");
                boss.GetComponent<EnemyController>().SetUpgrade("Red Robe");
                break;
            // Boss 10 - Kingsbane
            case(10):
                boss.GetComponent<EnemyController>().SetUpgrade("Kingsbane");
                break;
        }
        uiManager.background.color = Color.red;
        uiManager.Level(boss.name);
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
            case("Boss Key"):
                item = Instantiate(bossKeyPrefab, position, Quaternion.Euler(0, 0, 0), items.transform);
                item.name = "Boss Key";
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
    
}