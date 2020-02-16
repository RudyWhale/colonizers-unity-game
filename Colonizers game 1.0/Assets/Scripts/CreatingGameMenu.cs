using UnityEngine;
using UnityEngine.UI;

public class CreatingGameMenu : MonoBehaviour {

    public GameObject _3Players;
    public GameObject _4Players;

    public GameObject player1Input;
    public GameObject player2Input;
    public GameObject player3Input;
    public GameObject player4Input;

    // имена игроков
    public static string[] playerNames = new string[4];

    /// <summary>
    /// Добавляет четвертого игрока
    /// </summary>
    public void AddPlayer()
    {
        _3Players.SetActive(false);
        _4Players.SetActive(true);
    }

    /// <summary>
    /// Убирает четвертого игрока
    /// </summary>
    public void RemovePlayer()
    {
        _3Players.SetActive(true);
        _4Players.SetActive(false);
    }

    /// <summary>
    /// Сохраняет введенные имена игроков
    /// </summary>
    public void SavePlayerNames()
    {
        playerNames[0] = player1Input.GetComponent<InputField>().text == "" ? "Player 1" : player1Input.GetComponent<InputField>().text;
        playerNames[1] = player2Input.GetComponent<InputField>().text == "" ? "Player 2" : player2Input.GetComponent<InputField>().text;
        playerNames[2] = player3Input.GetComponent<InputField>().text == "" ? "Player 3" : player3Input.GetComponent<InputField>().text;

        if (_4Players.activeSelf)
        {
            playerNames[3] = player4Input.GetComponent<InputField>().text == "" ? "Player 4" : player4Input.GetComponent<InputField>().text;
        }
    }
}
