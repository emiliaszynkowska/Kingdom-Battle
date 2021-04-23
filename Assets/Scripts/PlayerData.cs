using System.Collections.Generic;

public static class PlayerData
{
    // Player
    public static int LivesActive;
    public static int Health;
    public static string Name;
    public static int Coins;
    public static List<string> Inventory;
    public static int Level;
    public static bool Boss;
    public static List<string> Disciplines;
    public static List<string> Titles;
    public static List<bool> Levels;
    // Difficulty
    public static int Wins;
    public static int Losses;
    public static int Kills;
    public static int Hits;
    public static float PreviousScore = 0.5f;
    public static int Difficulty;
    public static bool Toggle = true;
}