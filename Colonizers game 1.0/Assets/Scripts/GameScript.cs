using UnityEngine;
using UnityEngine.UI;
using GameClassLib;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameScript : MonoBehaviour
{
    // Текстовые поля, отображающие введенные имена игроков
    public Text[] playerNames;
    public Text[] playerNames_4Players;

    // Текстовые поля, отображающие количество ресурсов у игрока
    public Text[] resLabels;

    // Объекты игроков
    static Player[] players;
    public static Player[] Players { get { return players; } }

    static int currentPlayerNum;
    public static int CurrentPlayerNum { get { return currentPlayerNum; } }

    // Объекты поселений и дорог
    static List<Road> roads = new List<Road>();
    public static List<Road> Roads { get { return roads; } }

    static List<Town> towns = new List<Town>();
    public static List<Town> Towns { get { return towns; } }

    // Цвета для игроков
    Color[] playerColors = { Color.red, new Color(0.1f, 0.7f, 1f), new Color(1f, 0.7f, 0.01f), Color.white };

    // Спрайты местностей
    public Sprite mountains;
    public Sprite lake;
    public Sprite forest;
    public Sprite grassland;
    public Sprite fields;

    // Массив гексов
    public SpriteRenderer[] hexObjects;
    Hex[] hexes = new Hex[18];

    // Массивы игровых объектов мест для строительства поселений и дорог
    public GameObject[] townObjArray;
    public GameObject[] roadObjArray;

    // Экран уведомлений
    public GameObject infoScreen;
    public Text infoScreenText;
    public Text infoScreenBtnText;
    public Button infoScreenButton;

    // Экран смены хода в основной фазе
    public GameObject changeTurnScreen;
    public Text changeTurnPlayerTxt;
    public Text changeTurnCubesTxt;

    // Экран строительства
    public GameObject buildingScreen;
    public Button[] buildingScreenBtns;

    // Экран подсчета победных очков
    public GameObject finalScreen;
    public GameObject finalScreen_player4Obj;
    public Button quitBtn;
    public Button continueGameBtn;
    public Text[] endGamePlayerNames;
    public Text[] endGamePlayerScores;

    // Интерфейсы для разных стадий хода
    public GameObject standartInterface;
    public GameObject standartInterface_3Players;
    public GameObject standartInterface_4Players;
    public GameObject buildingInterface;
    public Button buildingInterfaceCancelBtn;

    // Кнопки игрового меню
    public Button endGameBtn;
    public Button tradeBtn;
    public Button buildBtn;
    public Button endTurnBtn;

    // Текущая фаза игры
    static int phase = 1;

    // Количество игроков в данный момент
    public static int playersCount;

    // Генератор случайных значений
    System.Random rand = new System.Random();

    // Последнее выпавшее число на кубиках
    static int cubesValue;

    /// <summary>
    /// Обрабатывает постройку нового поселения в фазе основания
    /// </summary>
    /// <param name="townID"></param>
    public void TownBuildedHandler_SettlePhase(string townID, bool townBuilded)
    {
        GameMethods.DeactivateTownObjects(towns);

        // Настраивает интерфейс
        buildingInterface.GetComponent<Text>().text = "Проложите дорогу от поселения";

        // Находит объект построенного поселения
        Town buildedVillage;

        foreach(Town town in towns)
        {
            if (town.Name == townID)
            {
                buildedVillage = town;

                // Делает недоступными для строительства соседние перекрестки
                GameMethods.DeleteBorderingTownObjects(buildedVillage, towns);

                // Активирует соседние объекты дорог
                foreach (Road road in buildedVillage.Roads)
                {
                    road.EnableFor.Add(players[currentPlayerNum]);
                    road.SetActive(true);
                }

                // Подписывает игрока на события собра ресурсов с гексов
                GameMethods.AddHexListeners(townID, hexes, players[currentPlayerNum]);

                return;
            }
        }
    }

    /// <summary>
    /// Обрабатывает постройку дороги в фазе основания
    /// </summary>
    /// <param name="roadID"></param>
    public void RoadBuildedHandler_SettlePhase(string roadID)
    {
        GameMethods.DeactivateRoadObjects(roads);

        // Настраивает игровой интерфейс
        buildingInterface.GetComponent<Text>().text = "Выберите место для поселения";
        buildingInterface.SetActive(false);
        standartInterface.SetActive(true);

        // Сменяет игрока
        if (phase == 1)
        {
            if (currentPlayerNum == playersCount - 1)
                phase++;

            else
            {
                ++currentPlayerNum;
                infoScreenText.text = "Ход игрока " + players[currentPlayerNum].Name;
            }
        }

        else if (phase == 2)
        {
            if (currentPlayerNum == 0)
            {
                phase++;

                // Настраивает экран уведомления о смене хода
                infoScreenText.text = "Этап игры";

                // Назначает новый обработчик для кнопки продолжения игры
                infoScreenButton.onClick.RemoveAllListeners();
                infoScreenButton.onClick.AddListener(StartMainPhase);
            }

            else
            {
                --currentPlayerNum;
                infoScreenText.text = "Ход игрока " + players[currentPlayerNum].Name;
            }
        }
    }

    /// <summary>
    /// Обрабатывает постройку нового поселения в основной фазе
    /// </summary>
    /// <param name="townID"></param>
    /// <param name="townBuilded"></param>
    public void TownBuildedHandler_MainPhase(string townID, bool townBuilded)
    {
        Player currentPlayer = players[currentPlayerNum];
        currentPlayer.Score++;

        GameMethods.DeactivateTownObjects(towns);

        // Отнимает у игрока ресурсы, необходимые для постройки объекта
        if (townBuilded)
            currentPlayer.Build("town");
        else
            currentPlayer.Build("village");

        GameMethods.LoadResValues(resLabels, currentPlayerNum);

        // Выводит стандартный интерфейс
        buildingInterface.SetActive(false);
        standartInterface.SetActive(true);

        // Находит объект построенного поселения
        Town buildedTown;

        foreach (Town town in towns)
        {
            if (town.Name == townID)
            {
                buildedTown = town;

                // Делает недоступными для строительства соседние перекрестки
                if (buildedTown.Builded == "village")
                    GameMethods.DeleteBorderingTownObjects(buildedTown, towns);

                // Подписывает игрока на события собра ресурсов с гексов
                GameMethods.AddHexListeners(townID, hexes, players[currentPlayerNum]);

                return;
            }
        }
    }

    /// <summary>
    /// Обрабатывает постройку новой дороги в основной фазе
    /// </summary>
    /// <param name="roadID"></param>
    public void RoadBuildedHandler_MainPhase(string roadID)
    {
        GameMethods.DeactivateRoadObjects(roads);

        // Отнимает у игрока ресурсы, необходимые для постройки объекта
        players[currentPlayerNum].Build("road");
        GameMethods.LoadResValues(resLabels, currentPlayerNum);

        // Выводит стандартный интерфейс
        buildingInterface.SetActive(false);
        standartInterface.SetActive(true);
    }

    /// <summary>
    /// Загружает сцену
    /// </summary>
    public void Start()
    {
        // Создает новый остров
        GameMethods.CreateIsland(hexObjects, ref hexes, mountains, lake, forest, grassland, fields);

        // Инициализирует объекты игроков
        playersCount = CreatingGameMenu.playerNames[3] == null ? 3 : 4;
        players = new Player[playersCount];
        currentPlayerNum = 0;

        for (int i = 0; i < playersCount; i++)
        {
            players[i] = new Player(CreatingGameMenu.playerNames[i] ?? "Player " + (i + 1), playerColors[i]);
        }

        // Инициализирует объекты дорог и поселений
        for (int i = 0; i < 72; i++)
        {
            string roadName = roadObjArray[i].name;
            Road newRoad = new Road(roadName);
            roads.Add(newRoad);
            RoadObjectScript.RoadBuilded += roads[i].RoadBuildedHandler;
        }

        for (int i = 0; i < 54; i++)
        {
            towns.Add(new Town(townObjArray[i].name, roads));
            TownObjectScript.TownBuilded += towns[i].TownBuildedHandler;
        }

        // Определяет корректный вариант игрового интерфейса
        if (playersCount == 4)
        {
            playerNames = playerNames_4Players;
            standartInterface_3Players.SetActive(false);
            standartInterface_4Players.SetActive(true);
        }

        // Загружает интерфейс
        GameMethods.LoadPlayerInterface(playerNames, resLabels);

        // Добавляет методы-обработчики события появления нового объекта
        RoadObjectScript.RoadBuilded += RoadBuildedHandler_SettlePhase;
        TownObjectScript.TownBuilded += TownBuildedHandler_SettlePhase;

        // Добавляет обработчики нажатий для кнопок интерфейса
        infoScreenButton.onClick.AddListener(InfoScreenBtn_OnClick_SettlePhase);
        endTurnBtn.onClick.AddListener(EndTurnBtn_OnClick_SettlePhase);
    }

    /// <summary>
    /// Начинает фазу основания
    /// </summary>
    public void InfoScreenBtn_OnClick_SettlePhase()
    {
        // Настраивает интерфейс
        infoScreen.SetActive(false);
        infoScreenBtnText.text = "Продолжить";

        if (phase != 3)
        {
            standartInterface.SetActive(false);
            buildingInterface.SetActive(true);
        }

        // Активирует режим строительства поселения
        foreach(Town town in towns)
        {
            if (town.Builded == "none")
            {
                town.SetActive(true);
            }
        }
    }

    /// <summary>
    /// Начинает основную фазу игры
    /// </summary>
    public void StartMainPhase()
    {
        // Назначает новый обработчик кнопки окончания хода
        endTurnBtn.onClick.RemoveAllListeners();
        endTurnBtn.onClick.AddListener(EndTurnBtn_OnClick_MainPhase);

        // Назначает новые методы-обработчики события появления нового объекта
        RoadObjectScript.RoadBuilded -= RoadBuildedHandler_SettlePhase;
        RoadObjectScript.RoadBuilded += RoadBuildedHandler_MainPhase;

        TownObjectScript.TownBuilded -= TownBuildedHandler_SettlePhase;
        TownObjectScript.TownBuilded += TownBuildedHandler_MainPhase;

        // Добавляет каждому игроку по два ПО за основанные поселения
        foreach (Player player in players)
            player.Score = 2;

        // Загружает интерфейс игрока
        GameMethods.GetResources(hexes, -1);
        GameMethods.LoadResValues(resLabels, currentPlayerNum);

        // Делает доступными кнопки игрового меню
        tradeBtn.interactable = true;
        buildBtn.interactable = true;

        // Добавляет кнопку "Отмена" в интерфейсе режима строительства
        buildingInterfaceCancelBtn.interactable = true;

        // Скрывает экран уведомлений
        infoScreen.SetActive(false);
    }

    /// <summary>
    /// Обрабатывает нажатие кнопки "продолжить" во время основной фазы
    /// </summary>
    public void ChangeTurnScreenBtn_OnClick()
    {
        GameMethods.GetResources(hexes, cubesValue);
        GameMethods.LoadResValues(resLabels, currentPlayerNum);

        changeTurnScreen.SetActive(false);
    }

    /// <summary>
    /// Заканчивает ход во время фазы основания
    /// </summary>
    public void EndTurnBtn_OnClick_SettlePhase()
    {
        infoScreen.SetActive(true);
        GameMethods.LoadPlayerInterface(playerNames, resLabels);
    }

    /// <summary>
    /// Заканчивает ход во время основной фазы
    /// </summary>
    public void EndTurnBtn_OnClick_MainPhase()
    {
        currentPlayerNum = (currentPlayerNum + 1) % playersCount;

        // Гененрирует новый случайный результат броска кубиков
        int cubeValue1 = rand.Next(1, 7);
        int cubeValue2 = rand.Next(1, 7);
        cubesValue = cubeValue1 + cubeValue2;

        changeTurnPlayerTxt.text = "Ход игрока " + players[currentPlayerNum].Name;
        changeTurnCubesTxt.text = "Число на кубиках: " + cubesValue;
        changeTurnScreen.SetActive(true);

        GameMethods.LoadPlayerInterface(playerNames, resLabels);
    }

    /// <summary>
    /// Загружает экран строительства
    /// </summary>
    public void BuildBtn_OnClick()
    {
        Player currentPlayer = players[currentPlayerNum];

        // Делает неактивными кнопки меню строительства
        foreach (Button button in buildingScreenBtns)
        {
            button.GetComponent<Image>().color = Color.white;
            button.interactable = false;
        }

        if (currentPlayer.Wood >= 1 && currentPlayer.Bricks >= 1)
        {
            // Активирует кнопку строительства дороги
            buildingScreenBtns[0].interactable = true;
            buildingScreenBtns[0].GetComponent<Image>().color = currentPlayer.Color;

            if (currentPlayer.Wheat >= 1 && currentPlayer.Wool >= 1)
            {
                // Активирует кнопку строительства поселения
                buildingScreenBtns[1].interactable = true;
                buildingScreenBtns[1].GetComponent<Image>().color = currentPlayer.Color;
            }
        }

        if (currentPlayer.Wheat >= 2 && currentPlayer.Stone >= 3)
        {
            // Активирует кнопку строительства города
            buildingScreenBtns[2].interactable = true;
            buildingScreenBtns[2].GetComponent<Image>().color = currentPlayer.Color;
        }

        buildingScreen.SetActive(true);
    }

    /// <summary>
    /// Обрабатывает нажатие кнопки строительства дороги
    /// </summary>
    public void BuildRoadBtn_OnClick()
    {
        buildingInterface.GetComponent<Text>().text = "Выберите место для дороги";
        standartInterface.SetActive(false);
        buildingInterface.SetActive(true);

        buildingScreen.SetActive(false);

        GameMethods.ActivateRoadBuildingMode(roads);
    }

    /// <summary>
    /// Обрабатывает нажатие кнопки строительства поселения
    /// </summary>
    public void BuildVillageBtn_OnClick()
    {
        buildingInterface.GetComponent<Text>().text = "Выберите место для поселения";
        standartInterface.SetActive(false);
        buildingInterface.SetActive(true);

        buildingScreen.SetActive(false);

        GameMethods.ActivateVillageBuildingMode(towns);
    }

    /// <summary>
    /// Обрабатывает нажатие кнопки строительства города
    /// </summary>
    public void BuildTownBtn_OnClick()
    {
        buildingInterface.GetComponent<Text>().text = "Выберите место для города";
        standartInterface.SetActive(false);
        buildingInterface.SetActive(true);

        buildingScreen.SetActive(false);

        GameMethods.ActivateTownBuildingMode(towns);
    }

    /// <summary>
    /// Обрабатывает нажатие кнопки "Отмена" в интерфейсе строительства
    /// </summary>
    public void BuildingInterfaceCancelBtn_OnClick()
    {
        GameMethods.DeactivateRoadObjects(roads);
        GameMethods.DeactivateTownObjects(towns);

        buildingInterface.SetActive(false);
        standartInterface.SetActive(true);
    }

    /// <summary>
    /// Обрабатывает нажатие кнопки "Отмена" на экране выбора объекта для строительства
    /// </summary>
    public void BuildingScreenCancelBtn_OnClick()
    {
        // Делает кнопки меню выбора объекта для строительства недоступными
        foreach(Button button in buildingScreenBtns)
        {
            button.interactable = false;
        }

        buildingScreen.SetActive(false);
    }

    /// <summary>
    /// Обрабатывает нажатие кнопки "Закончить игру"
    /// Выводит экран подсчета победных очков
    /// </summary>
    public void EndGameBtn_OnClick()
    {
        if (playersCount == 4)
            finalScreen_player4Obj.SetActive(true);

        for (int i = 0; i < playersCount; i++)
        {
            endGamePlayerNames[i].text = players[i].Name;
            endGamePlayerScores[i].text = players[i].Score.ToString();
        }

        finalScreen.SetActive(true);
    }

    /// <summary>
    /// Обрабатывает нажатие кнопки "продолжить" на экране подсчета очков
    /// Скрывает экран уведомлений
    /// </summary>
    public void FinalScreen_ContinueBtn_OnClick()
    {
        finalScreen.SetActive(false);
    }

    /// <summary>
    /// Обрабатывает нажатие кнопки "Выйти из игры"
    /// </summary>
    public void QuitBtn_OnClick()
    {
        Application.Quit();
    }
}
