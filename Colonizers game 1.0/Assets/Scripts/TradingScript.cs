using UnityEngine;
using UnityEngine.UI;
using GameClassLib;

public class TradingScript : MonoBehaviour {

    // Текстовые поля, отображающие имена игроков
    public Text player1Text;
    public Text player2Text;

    public Text player1Text_4players;
    public Text player2Text_4players;
    public Text player3Text_4players;

    // Текстовые поля, отображающие количество ресурсов у каждого игрока
    public Text[] player1resLabels;
    public Text[] player2resLabels;

    public Text[] player1resLabels_4players;
    public Text[] player2resLabels_4players;
    public Text[] player3resLabels_4players;

    // Экран торговли для 3-х и 4-х игроков
    public GameObject tradingScreen3players;
    public GameObject tradingScreen4players;

    // Экран обмена ресурсами
    public GameObject exchangeScreen;

    public Text player1Txt_ExchangeScreen;
    public Text player2Txt_ExchangeScreen;

    public Text[] player1ResLabels_ExchangeScreen;
    public Text[] player2ResLabels_ExchangeScreen;

    // Номер игрока, с которым производится обмен
    static int exchangingPlayerNum;

    // Количество игроков
    static int playersCount;

    // Данные о совершаемой сделке
    static Player currentPlayer;
    static Player exchangingPlayer;
    static int[] exchangingResValues;

    // Текстовые поля, отображающие количество ресурсов в основном интерфейсе
    public Text[] resLabels;

    /// <summary>
    /// Обрабатывает нажатие кнопки "Торговля" в игровом меню.
    /// Выводит меню торговли
    /// </summary>
    public void TradeBtn_OnClick()
    {
        exchangingResValues = new int[] { 0, 0, 0, 0, 0 };

        // Сбрасывает информацию о предыдущей сделке
        for (int i = 0; i < 5; i++)
        {
            player1ResLabels_ExchangeScreen[i].text = "0";
            player2ResLabels_ExchangeScreen[i].text = "0";
        }

        if (GameScript.Players.Length == 3)
        {
            playersCount = 3;
            int currentPlayer = GameScript.CurrentPlayerNum;

            // Загружает информацию о других игроках
            int player1Num = (currentPlayer + 1) % 3;
            player1Text.text = GameScript.Players[player1Num].Name;
            GameMethods.LoadResValues(player1resLabels, player1Num);

            int player2Num = (currentPlayer + 2) % 3;
            player2Text.text = GameScript.Players[player2Num].Name;
            GameMethods.LoadResValues(player2resLabels, player2Num);

            // Выводит меню торговли
            tradingScreen3players.SetActive(true);
        }

        else if (GameScript.Players.Length == 4)
        {
            playersCount = 4;
            int currentPlayer = GameScript.CurrentPlayerNum;

            // Загружает информацию о других игроках
            int player1Num = (currentPlayer + 1) % 4;
            player1Text_4players.text = GameScript.Players[player1Num].Name;
            GameMethods.LoadResValues(player1resLabels_4players, player1Num);

            int player2Num = (currentPlayer + 2) % 4;
            player2Text_4players.text = GameScript.Players[player2Num].Name;
            GameMethods.LoadResValues(player2resLabels_4players, player2Num);

            int player3Num = (currentPlayer + 3) % 4;
            player3Text_4players.text = GameScript.Players[player3Num].Name;
            GameMethods.LoadResValues(player3resLabels_4players, player3Num);

            // Выводит меню торговли
            tradingScreen4players.SetActive(true);
        }
    }

    /// <summary>
    /// Обрабатывает нажатие кнопки "Отмена" в меню торговли.
    /// Скрывает меню торговли.
    /// </summary>
    public void CancelBtn_OnClick()
    {
        if (playersCount == 3)
            tradingScreen3players.SetActive(false);
        else if (playersCount == 4)
            tradingScreen4players.SetActive(false);
    }

    /// <summary>
    /// Загружает экран обмена ресурсами 
    /// </summary>
    /// <param name="playerNum"></param>
    public void LoadExchangeScreen(int playerNum)
    {
        exchangingPlayerNum = playersCount == 3 ? (GameScript.CurrentPlayerNum + playerNum) % 3 : 
                                                    (GameScript.CurrentPlayerNum + playerNum) % 4;

        currentPlayer = GameScript.Players[GameScript.CurrentPlayerNum];
        exchangingPlayer = GameScript.Players[exchangingPlayerNum];

        // Скрывает меню торговли
        CancelBtn_OnClick();

        // Загружает экран обмена ресурсами
        player1Txt_ExchangeScreen.text = currentPlayer.Name;
        player2Txt_ExchangeScreen.text = GameScript.Players[exchangingPlayerNum].Name;

        exchangeScreen.SetActive(true);
    }

    /// <summary>
    /// Обрабатывает нажатие кнопки добавления ресурса на экране обмена.
    /// Добавляет ресурс текущему игроку.
    /// </summary>
    public void CurrentPlayerAddBtn_OnClick(int resIndex)
    {
        switch (resIndex)
        {
            case 0:
                if (exchangingPlayer.Wood > exchangingResValues[0])
                {
                    player1ResLabels_ExchangeScreen[0].text = (++exchangingResValues[0]).ToString();
                    player2ResLabels_ExchangeScreen[0].text = (-exchangingResValues[0]).ToString();
                }
                break;
            case 1:
                if (exchangingPlayer.Bricks > exchangingResValues[1])
                {
                    player1ResLabels_ExchangeScreen[1].text = (++exchangingResValues[1]).ToString();
                    player2ResLabels_ExchangeScreen[1].text = (-exchangingResValues[1]).ToString();
                }
                break;
            case 2:
                if (exchangingPlayer.Stone > exchangingResValues[2])
                {
                    player1ResLabels_ExchangeScreen[2].text = (++exchangingResValues[2]).ToString();
                    player2ResLabels_ExchangeScreen[2].text = (-exchangingResValues[2]).ToString();
                }
                break;
            case 3:
                if (exchangingPlayer.Wool > exchangingResValues[3])
                {
                    player1ResLabels_ExchangeScreen[3].text = (++exchangingResValues[3]).ToString();
                    player2ResLabels_ExchangeScreen[3].text = (-exchangingResValues[3]).ToString();
                }
                break;
            case 4:
                if (exchangingPlayer.Wheat > exchangingResValues[4])
                {
                    player1ResLabels_ExchangeScreen[4].text = (++exchangingResValues[4]).ToString();
                    player2ResLabels_ExchangeScreen[4].text = (-exchangingResValues[4]).ToString();
                }
                break;
        }
    }

    /// <summary>
    /// Обрабатывает нажатие кнопки добавления ресурса на экране обмена.
    /// Отнимает ресурс у текущего игрока.
    /// </summary>
    /// <param name="resIndex"></param>
    public void ExchangingPlayerAddBtn_OnClick(int resIndex)
    {
        switch (resIndex)
        {
            case 0:
                if (currentPlayer.Wood > -exchangingResValues[0])
                {
                    player1ResLabels_ExchangeScreen[0].text = (--exchangingResValues[0]).ToString();
                    player2ResLabels_ExchangeScreen[0].text = (-exchangingResValues[0]).ToString();
                }
                break;
            case 1:
                if (currentPlayer.Bricks > -exchangingResValues[1])
                {
                    player1ResLabels_ExchangeScreen[1].text = (--exchangingResValues[1]).ToString();
                    player2ResLabels_ExchangeScreen[1].text = (-exchangingResValues[1]).ToString();
                }
                break;
            case 2:
                if (currentPlayer.Stone > -exchangingResValues[2])
                {
                    player1ResLabels_ExchangeScreen[2].text = (--exchangingResValues[2]).ToString();
                    player2ResLabels_ExchangeScreen[2].text = (-exchangingResValues[2]).ToString();
                }
                break;
            case 3:
                if (currentPlayer.Wool > -exchangingResValues[3])
                {
                    player1ResLabels_ExchangeScreen[3].text = (--exchangingResValues[3]).ToString();
                    player2ResLabels_ExchangeScreen[3].text = (-exchangingResValues[3]).ToString();
                }
                break;
            case 4:
                if (currentPlayer.Wheat > -exchangingResValues[4])
                {
                    player1ResLabels_ExchangeScreen[4].text = (--exchangingResValues[4]).ToString();
                    player2ResLabels_ExchangeScreen[4].text = (-exchangingResValues[4]).ToString();
                }
                break;
        }
    }

    /// <summary>
    /// Обрабатывает нажатие кнопки "Подтвердить"
    /// </summary>
    public void ConfirmBtn_OnClick()
    {
        GameMethods.MakeDeal(exchangingResValues, exchangingPlayerNum);
        GameMethods.LoadResValues(resLabels, GameScript.CurrentPlayerNum);
        exchangeScreen.SetActive(false);
    }

    /// <summary>
    /// Обрабатывает нажатие кнопки "Назад" 
    /// </summary>
    public void ExchangeScreenCancelBtn_OnClick()
    {
        exchangeScreen.SetActive(false);

        // Загружает меню торговли
        TradeBtn_OnClick();
    }
}
