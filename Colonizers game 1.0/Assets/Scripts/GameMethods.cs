using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameClassLib;

public class GameMethods : MonoBehaviour {

    /// <summary>
    /// Создает случайный остров
    /// </summary>
    public static void CreateIsland(SpriteRenderer[] hexObjects, ref Hex[] hexes, 
        Sprite mountains, Sprite lake, Sprite forest, Sprite grassland, Sprite fields)
    {
        // Значения, при которых гексы приносят урожай
        int[] gettingResValues = { 6, 3, 11, 9, 4, 5, 10, 9, 12, 11, 4, 8, 10, 5, 2, 6, 3, 8 };

        // Список возможных местностей
        List<string> landscapes = new List<string> {
            "Mountains", "Mountains", "Mountains",
            "Lake", "Lake", "Lake",
            "Forest", "Forest", "Forest", "Forest",
            "Grassland", "Grassland", "Grassland", "Grassland",
            "Fields", "Fields", "Fields", "Fields"};

        string[] island = new string[18];
        System.Random rand = new System.Random();

        // Заполняет список island местностями в случайном порядке
        for (int i = 0; i < 18; i++)
        {
            int landScapeID = rand.Next(18 - i);
            island[i] = landscapes[landScapeID];
            landscapes.RemoveAt(landScapeID);
        }

        // Присваивает каждому элементу из списка гексов соответствующий спрайт
        // Инициализирует объекты гексов
        for (int i = 0; i < 18; i++)
        {
            int gettingResValue = gettingResValues[i];

            switch (island[i])
            {
                case "Mountains":
                    hexObjects[i].sprite = mountains;
                    hexes[i] = new Hex("stone", gettingResValue);
                    break;
                case "Lake":
                    hexObjects[i].sprite = lake;
                    hexes[i] = new Hex("bricks", gettingResValue);
                    break;
                case "Forest":
                    hexObjects[i].sprite = forest;
                    hexes[i] = new Hex("wood", gettingResValue);
                    break;
                case "Grassland":
                    hexObjects[i].sprite = grassland;
                    hexes[i] = new Hex("wool", gettingResValue);
                    break;
                case "Fields":
                    hexObjects[i].sprite = fields;
                    hexes[i] = new Hex("wheat", gettingResValue);
                    break;
            }
        }
    }

    /// <summary>
    /// Активирует объекты дорог, доступные для стоительства текущим игроком
    /// </summary>
    /// <param name="roads"></param>
    /// <param name="activate"></param>
    public static void ActivateRoadBuildingMode(List<Road> roads)
    {
        Player currentPlayer = GameScript.Players[GameScript.CurrentPlayerNum];

        foreach (Road road in roads)
        {
            if (!road.Builded && road.EnableFor.Contains(currentPlayer))
                road.SetActive(true);
        }
    }

    /// <summary>
    /// Активирует объекты перекрестков, доступные для стоительства поселения текущим игроком
    /// </summary>
    /// <param name="towns"></param>
    /// <param name="activate"></param>
    public static void ActivateVillageBuildingMode(List<Town> towns)
    {
        Player currentPlayer = GameScript.Players[GameScript.CurrentPlayerNum];

        foreach (Town town in towns)
        {
            if (town.Builded == "none" && town.EnableFor.Contains(currentPlayer))
            {
                town.SetActive(true);
                Debug.Log("Town " + town.Name + " activated");
            }
        }
    }

    /// <summary>
    /// Активирует объекты перекрестков, доступные для строительства города текущим игроком
    /// </summary>
    /// <param name="towns"></param>
    /// <param name="activate"></param>
    public static void ActivateTownBuildingMode(List<Town> towns)
    {
        Player currentPlayer = GameScript.Players[GameScript.CurrentPlayerNum];

        foreach (Town town in towns)
        {
            if (town.Builded == "village" && town.EnableFor.Contains(currentPlayer))
                town.SetActive(true);
        }
    }

    /// <summary>
    /// Удаляет объекты городов, граничащие с построенным поселением
    /// </summary>
    /// <param name="buildedTown"></param>
    public static void DeleteBorderingTownObjects(Town buildedTown, List<Town> towns)
    {
        foreach (Town town in towns)
        {
            if ((buildedTown.Hexes.Contains(town.Hexes[0]) &&
                buildedTown.Hexes.Contains(town.Hexes[1]))
                ||
                (buildedTown.Hexes.Contains(town.Hexes[1]) &&
                buildedTown.Hexes.Contains(town.Hexes[2]))
                ||
                (buildedTown.Hexes.Contains(town.Hexes[0]) &&
                buildedTown.Hexes.Contains(town.Hexes[2])))
            {
                if (town != buildedTown)
                {
                    town.Builded = "blocked";
                }
            }
        }
    }

    /// <summary>
    /// Собирает ресурсы с нужных гексов. При значении cubesValue = -1 
    /// собирает ресурсы с каждого гекса
    /// </summary>
    /// <param name="hexes"></param>
    /// <param name="cubesValue"></param>
    public static void GetResources(Hex[] hexes, int cubesValue)
    {
        foreach(Hex hex in hexes)
        {
            hex.CheckGettingRes(cubesValue);
        }
    }

    /// <summary>
    /// Подписывает метод для сбора ресурсов игроком на событие сбора ресурсов с гексов
    /// </summary>
    /// <param name="borderingHexes"></param>
    /// <param name="hexes"></param>
    /// <param name="player"></param>
    public static void AddHexListeners(string townID, Hex[] hexes, Player player)
    {
        string[] townData = townID.Split('_');

        for (int i = 1; i < 4; i++)
        {
            int hexNumber = int.Parse(townData[i]);

            if (hexNumber > 0 && hexNumber < 19)
                hexes[hexNumber - 1].GettingRes += player.GettingResHandler;
        }
    }

    /// <summary>
    /// Загружает данные о количестве ресурсов у игрока
    /// </summary>
    /// <param name="player"></param>
    /// <param name="resLabels"></param>
    public static void LoadResValues(Text[] resLabels, int playerNum)
    {
        Player player = GameScript.Players[playerNum];

        resLabels[0].text = player.Wood.ToString();
        resLabels[1].text = player.Bricks.ToString();
        resLabels[2].text = player.Stone.ToString();
        resLabels[3].text = player.Wool.ToString();
        resLabels[4].text = player.Wheat.ToString();
    }

    /// <summary>
    /// Загружает интерфейс пользователя
    /// </summary>
    /// <param name="playerNames"></param>
    /// <param name="resLabels"></param>
    public static void LoadPlayerInterface(Text[] playerNames, Text[] resLabels)
    {
        int playersCount = GameScript.playersCount;

        for (int i = 0; i < playersCount; i++)
        {
            playerNames[i].text = GameScript.Players[(GameScript.CurrentPlayerNum + i) % playersCount].Name;
        }
    
        LoadResValues(resLabels, GameScript.CurrentPlayerNum);
    }

    /// <summary>
    /// Деактивирует объекты дорог
    /// </summary>
    public static void DeactivateRoadObjects(List<Road> roads)
    {
        foreach(Road road in roads)
        {
            road.SetActive(false);
        }
    }

    /// <summary>
    /// Деактивирует объекты городов
    /// </summary>
    public static void DeactivateTownObjects(List<Town> towns)
    {
        foreach (Town town in towns)
        {
            town.SetActive(false);
        }
    }

    /// <summary>
    /// Обменивает ресурсы игроков
    /// </summary>
    /// <param name="resValues"></param>
    /// <param name="exchangingPlayerNum"></param>
    public static void MakeDeal(int[] resValues, int exchangingPlayerNum)
    {
        Player currentPlayer = GameScript.Players[GameScript.CurrentPlayerNum];
        Player exchangingPlayer = GameScript.Players[exchangingPlayerNum];

        // Изменяет количество ресурсов у игроков
        currentPlayer.Wood += resValues[0];
        exchangingPlayer.Wood -= resValues[0];

        currentPlayer.Bricks += resValues[1];
        exchangingPlayer.Bricks -= resValues[1];

        currentPlayer.Stone += resValues[2];
        exchangingPlayer.Stone -= resValues[2];

        currentPlayer.Wool += resValues[3];
        exchangingPlayer.Wool -= resValues[3];

        currentPlayer.Wheat += resValues[4];
        exchangingPlayer.Wheat -= resValues[4];
    }
}
