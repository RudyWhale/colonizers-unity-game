using System.Collections.Generic;
using UnityEngine;

namespace GameClassLib
{
    /// <summary>
    /// Представляет объект гекса
    /// </summary>
    public class Hex
    {
        // Тип ресурса, который производит гекс
        string resType;
        public string ResType { get { return resType; } }

        // Число на кубиках, при котором гекс приносит урожай
        int gettingResValue;
        public int GettingResValue { get { return gettingResValue; } }

        // Событие, вызываемое при сборе урожая с клетки
        public delegate void GettingResHandler(string resType);
        public event GettingResHandler GettingRes;

        /// <summary>
        /// Конструктор класса
        /// </summary>
        /// <param name="newResType"></param>
        public Hex(string newResType, int newGettingResValue)
        {
            resType = newResType;
            gettingResValue = newGettingResValue;
        }

        /// <summary>
        /// Вызывает событие сбора урожая
        /// </summary>
        public void CheckGettingRes(int cubesValue)
        {
            if (cubesValue == gettingResValue || cubesValue == -1)
                if (GettingRes != null)
                    GettingRes(resType);
        }
    }

    /// <summary>
    /// Представляет объект игрока
    /// </summary>
    public class Player
    {
        // Имя игрока
        string name;
        public string Name { get { return name; } }

        // Цвет игровых объектов
        Color color;
        public Color Color { get { return color; } }

        // Количество ресурсов каждого вида
        public int Wood { get; set; }
        public int Bricks { get; set; }
        public int Stone { get; set; }
        public int Wool { get; set; }
        public int Wheat { get; set; }

        // Количество победных очков игрока
        public int Score { get; set; }

        /// <summary>
        /// Конструктор класса
        /// </summary>
        /// <param name="newName"></param>
        public Player(string newName, Color newColor)
        {
            name = newName;
            color = newColor;
        }

        /// <summary>
        /// Позволяет получать ресурсы с клеток
        /// </summary>
        /// <param name="resType"></param>
        public void GettingResHandler(string resType)
        {
            if (resType == "wood")
                Wood++;
            else if (resType == "bricks")
                Bricks++;
            else if (resType == "stone")
                Stone++;
            else if (resType == "wool")
                Wool++;
            else if (resType == "wheat")
                Wheat++;
        }

        /// <summary>
        /// Отнимает у игрока ресурсы после постройки объекта
        /// </summary>
        /// <param name=""></param>
        public void Build(string objType)
        {
            switch (objType)
            {
                case "road":
                    Wood--;
                    Bricks--;
                    break;
                case "village":
                    Wood--;
                    Bricks--;
                    Wheat--;
                    Wool--;
                    break;
                case "town":
                    Wheat = Wheat - 2;
                    Stone = Stone - 3;
                    break;
            }
        }
    }

    /// <summary>
    /// Представляет объект дороги
    /// </summary>
    public class Road
    {
        string name;
        public string Name { get { return name; } }

        GameObject gameObject;

        // Хранит информацию о том, кто из игроков может построить дорогу на этом месте
        public List<Player> EnableFor { get; set; }

        public bool Builded { get; set; }

        // Хранит номера граничащих гексов
        string[] hexes = new string[2];
        public string[] Hexes { get { return hexes; } }

        /// <summary>
        /// Конструктор класса
        /// </summary>
        /// <param name="newName"></param>
        public Road(string newName)
        {
            name = newName;
            gameObject = GameObject.Find(newName);
            EnableFor = new List<Player>();

            // Сохраняет номера граничащих гексов
            hexes[0] = newName.Split('_')[1];
            hexes[1] = newName.Split('_')[2];
        }

        public delegate void BorderingRoadBuildedHandler(Road road);
        public event BorderingRoadBuildedHandler BorderingRoadBuilded;

        /// <summary>
        /// Обрабатывает событие постройки нового объекта на карте
        /// </summary>
        public void RoadBuildedHandler(string buildedObjName)
        {
            if (buildedObjName == name)
            {
                EnableFor.Clear();

                Builded = true;
                BorderingRoadBuilded(this);
            }
        }

        /// <summary>
        /// Активирует/деактивирует данный объект для строительства
        /// </summary>
        public void SetActive(bool activate)
        {
            gameObject.GetComponent<Collider2D>().enabled = activate;
        }
    }

    /// <summary>
    /// Представляет объект поселения
    /// </summary>
    public class Town
    {
        // Название объекта
        string name;
        public string Name { get { return name; } }

        GameObject gameObject;

        // Хранит информацию о том, кто из игроков может построить дорогу на этом месте
        List<Player> enableFor = new List<Player>();
        public List<Player> EnableFor { get { return enableFor; } }

        public string Builded { get; set; }

        // Дороги, примыкающие к перекрестку
        List<Road> roads = new List<Road>();
        public List<Road> Roads { get { return roads; } }

        // Граничащие гексы
        List<string> hexes = new List<string>();
        public List<string> Hexes { get { return hexes; } }

        /// <summary>
        /// Конструктор класса
        /// </summary>
        /// <param name="name"></param>
        /// <param name="roadObjects"></param>
        public Town(string newName, List<Road> roadObjects)
        {
            name = newName;
            Builded = "none";

            string[] townData = name.Split('_');

            gameObject = GameObject.Find(newName);

            // Сохраняет номера граничащих гексов
            for (int i = 1; i < 4; i++)
            {
                hexes.Add(townData[i]);
            }

            // Сохраняет граничащие дороги
            foreach (Road road in roadObjects)
            {
                if (hexes.Contains(road.Hexes[0]) && hexes.Contains(road.Hexes[1]))
                {
                    roads.Add(road);
                    road.BorderingRoadBuilded += BorderingRoadBuildedHandler;
                }
            }
        }

        /// <summary>
        /// Обрабатывает событие постройки нового объекта поселения/города
        /// Делает перекресток недоступным для строительства
        /// </summary>
        /// <param name="buildedObjName"></param>
        public void TownBuildedHandler(string buildedObjName, bool townBuilded)
        {
            Player currentPlayer = GameScript.Players[GameScript.CurrentPlayerNum];

            if (buildedObjName == name)
            {
                enableFor.Clear();

                if (townBuilded)
                {
                    Builded = "town";
                }
                else
                {
                    Builded = "village";
                    enableFor.Add(currentPlayer);
                }
            }
        }

        /// <summary>
        /// Обрабатывает строительство дороги, примыкающей к перекрестку.
        /// Делает перекресток доступным для строительства поселения и дорог рядом для игрока,
        /// проложившего дорогу.
        /// </summary>
        /// <param name="buildedRoad"></param>
        public void BorderingRoadBuildedHandler(Road buildedRoad)
        {
            Player currentPlayer = GameScript.Players[GameScript.CurrentPlayerNum];

            if (Builded == "none" && !enableFor.Contains(currentPlayer))
            {
                enableFor.Add(currentPlayer);
            }

            foreach (Road road in roads)
            {
                if (!road.Builded && !road.EnableFor.Contains(currentPlayer))
                {
                    road.EnableFor.Add(currentPlayer);
                }
            }
        }

        /// <summary>
        /// Активирует/деактивирует данный объект для строительства
        /// </summary>
        public void SetActive(bool activate)
        {
            gameObject.GetComponent<Collider2D>().enabled = activate;
        }
    }
}
