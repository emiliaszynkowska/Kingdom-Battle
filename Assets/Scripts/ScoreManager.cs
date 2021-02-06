using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ScoreManager : MonoBehaviour
{
    // Game Objects
    public static UIManager uiManager;
    // Variables
    public static int aggressive;
    public static int defensive;
    public static int exploration;
    public static int collection;
    public static int puzzlesolving;
    // Titles
    static List<string> titlesInitial = new List<string>() {"Beginner", "Fledgling", "Amateur", "Initiate", "Rookie", "Novice"};
    static List<string> titlesAggressive1 = new List<string>() {"Fighter", "Dueller", "Aggressor", "Swordsman"};
    static List<string> titlesAggressive2 = new List<string>() {"Mercenary", "Assassin", "Slayer", "Weaponmaster"};
    static List<string> titlesAggressive3 = new List<string>() {"Warrior", "Gladiator", "Destroyer", "Conqueror", "Barbarian", "Savage"};
    static List<string> titlesDefensive1 = new List<string>() {"Pacifist", "Defender", "Protector", "Mediator", "Shieldbearer"};
    static List<string> titlesDefensive2 = new List<string>() {"Purehearted", "Peacekeeper", "Shieldmaster", "Guardian", "Sentinel"};
    static List<string> titlesDefensive3 = new List<string>() {"Unbreakable", "Indestructible", "Eternal", "Immortal"};
    static List<string> titlesExploration1 = new List<string>() {"Wanderer", "Traveller", "Wayfarer"};
    static List<string> titlesExploration2 = new List<string>() {"Explorer", "Voyager", "Journeyman", "Discoverer"};
    static List<string> titlesExploration3 = new List<string>() {"Pioneer", "Harbinger", "Mapmaster", "Cartographer"};
    static List<string> titlesCollection1 = new List<string>() {"Finder", "Forager", "Gatherer"};
    static List<string> titlesCollection2 = new List<string>() {"Curator", "Acquirer", "Collector"};
    static List<string> titlesCollection3 = new List<string>() {"Conservator", "Connoisseur"};
    static List<string> titlesPuzzleSolving1 = new List<string>() {"Puzzle Solver", "Investigator", "Brainiac"};
    static List<string> titlesPuzzleSolving2 = new List<string>() {"Intellectual", "Academic", "Logician", "Analyst", "Detective"};
    static List<string> titlesPuzzleSolving3 = new List<string>() {"Sage", "Genius", "Savant", "Expert", "Mastermind", "Puzzlemaster"};
    static List<string> titlesExtreme = new List<string>() {"Hero", "Master", "Champion", "Legendary"};

    public static void AddAggressive(int score)
    {
        aggressive += score;
    }
    
    public static void AddDefensive(int score)
    {
        defensive += score;
    }
    
    public static void AddExploration(int score)
    {
        exploration += score;
    }

    public static void AddCollection(int score)
    {
        collection += score;
    }
    
    public static void AddPuzzleSolving(int score)
    {
        puzzlesolving += score;
    }
    
    public static int[] GetScores()
    {
        return new [] {aggressive, defensive, exploration, collection, puzzlesolving};
    }

    public static string DisciplinePrimary()
    {
        return aggressive > defensive ? "Aggressive" : "Defensive";
    }

    public static string DisciplineSecondary()
    {
        if (exploration >= collection && exploration >= puzzlesolving)
            return "Exploration";
        else if (collection >= exploration && collection >= puzzlesolving)
            return "Collection";
        else if (puzzlesolving >= collection && puzzlesolving >= exploration)
            return "Puzzle Solving";
        else
            return null;
    }

    public static string TitleInitial()
    {
        return titlesInitial[Random.Range(0, 6)];
    }

    public static string TitleAggressive(int level)
    {
        switch (level)
        {
            case 1:
                return titlesAggressive1[Random.Range(0, 4)];
            case 2:
                return titlesAggressive2[Random.Range(0, 4)];
            case 3:
                return titlesAggressive3[Random.Range(0, 6)];
        }
        return null;
    }

    public static string TitleDefensive(int level)
    {
        switch (level)
        {
            case 1:
                return titlesDefensive1[Random.Range(0, 5)];
            case 2:
                return titlesDefensive2[Random.Range(0, 5)];
            case 3:
                return titlesDefensive3[Random.Range(0, 4)];
        }
        return null;
    }
    
    public static string TitleExploration(int level)
    {
        switch (level)
        {
            case 1:
                return titlesExploration1[Random.Range(0, 3)];
            case 2:
                return titlesExploration2[Random.Range(0, 4)];
            case 3:
                return titlesExploration3[Random.Range(0, 4)];
        }
        return null;
    }
    
    public static string TitleCollection(int level)
    {
        switch (level)
        {
            case 1:
                return titlesCollection1[Random.Range(0, 3)];
            case 2:
                return titlesCollection2[Random.Range(0, 3)];
            case 3:
                return titlesCollection3[Random.Range(0, 2)];
        }
        return null; 
    }
    
    public static string TitlePuzzleSolving(int level)
    {
        switch (level)
        {
            case 1:
                return titlesPuzzleSolving1[Random.Range(0, 3)];
            case 2:
                return titlesPuzzleSolving2[Random.Range(0, 5)];
            case 3:
                return titlesPuzzleSolving3[Random.Range(0, 6)];
        }
        return null;
    }

    public static string TitleTertiary()
    {
        return titlesExtreme[Random.Range(0, 4)];
    }
    
}