using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ScoreManager : MonoBehaviour
{
    // Game Objects
    public UIManager uiManager;
    // Variables
    public int aggressive;
    public int defensive;
    public int exploration;
    public int collection;
    public int puzzlesolving;
    // Titles
    List<string> titlesInitial = new List<string>() {"Beginner", "Fledgling", "Amateur", "Initiate", "Rookie", "Novice"};
    List<string> titlesAggressive1 = new List<string>() {"Fighter", "Dueller", "Aggressor", "Swordsman"};
    List<string> titlesAggressive2 = new List<string>() {"Mercenary", "Assassin", "Slayer", "Weaponmaster"};
    List<string> titlesAggressive3 = new List<string>() {"Warrior", "Gladiator", "Destroyer", "Conqueror", "Barbarian", "Savage"};
    List<string> titlesDefensive1 = new List<string>() {"Pacifist", "Defender", "Protector", "Mediator", "Shieldbearer"};
    List<string> titlesDefensive2 = new List<string>() {"Purehearted", "Peacekeeper", "Shieldmaster", "Guardian", "Sentinel"};
    List<string> titlesDefensive3 = new List<string>() {"Unbreakable", "Indestructible", "Eternal", "Immortal"};
    List<string> titlesExploration1 = new List<string>() {"Wanderer", "Traveller", "Wayfarer"};
    List<string> titlesExploration2 = new List<string>() {"Explorer", "Voyager", "Journeyman", "Discoverer"};
    List<string> titlesExploration3 = new List<string>() {"Pioneer", "Harbinger", "Mapmaster", "Cartographer"};
    List<string> titlesCollection1 = new List<string>() {"Finder", "Forager", "Gatherer"};
    List<string> titlesCollection2 = new List<string>() {"Curator", "Acquirer", "Collector"};
    List<string> titlesCollection3 = new List<string>() {"Conservator", "Connoisseur"};
    List<string> titlesPuzzleSolving1 = new List<string>() {"Puzzle Solver", "Investigator", "Brainiac"};
    List<string> titlesPuzzleSolving2 = new List<string>() {"Intellectual", "Academic", "Logician", "Analyst", "Detective"};
    List<string> titlesPuzzleSolving3 = new List<string>() {"Sage", "Genius", "Savant", "Expert", "Mastermind", "Puzzlemaster"};
    List<string> titlesExtreme = new List<string>() {"Hero", "Master", "Champion", "Legendary"};

    public void AddAggressive(int score)
    {
        aggressive += score;
    }
    
    public void AddDefensive(int score)
    {
        defensive += score;
    }
    
    public void AddExploration(int score)
    {
        exploration += score;
    }
    
    public void AddPuzzleSolving(int score)
    {
        puzzlesolving += score;
    }
    
    public void AddCollection(int score)
    {
        collection += score;
    }

    public string DisciplinePrimary()
    {
        return aggressive > defensive ? "Aggressive" : "Defensive";
    }

    public int ScorePrimary()
    {
        return Math.Max(aggressive, defensive);
    }

    public string DisciplineSecondary()
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

    public int ScoreSecondary()
    {
        if (exploration >= collection && exploration >= puzzlesolving)
            return exploration;
        else if (collection >= exploration && collection >= puzzlesolving)
            return collection;
        else if (puzzlesolving >= collection && puzzlesolving >= exploration)
            return puzzlesolving;
        else
            return 0;
    }

    public string TitleInitial()
    {
        return titlesInitial[Random.Range(0, 6)];
    }

    public string TitleAggressive(int level)
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

    public string TitleDefensive(int level)
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
    
    public string TitleExploration(int level)
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
    
    public string TitleCollection(int level)
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
    
    public string TitlePuzzleSolving(int level)
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

    public string TitleTertiary()
    {
        return titlesExtreme[Random.Range(0, 4)];
    }
    
}