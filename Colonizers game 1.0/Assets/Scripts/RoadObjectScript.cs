using UnityEngine;

public class RoadObjectScript : MonoBehaviour {
    public Sprite sprite;

    // Событие, вызываемое при постройке нового поселения
    public delegate void RoadBuildedHandler(string objectID);
    public static event RoadBuildedHandler RoadBuilded;

    /// <summary>
    /// Назначает спрайт объекта
    /// </summary>
	public void OnMouseDown()
    {
        GetComponent<SpriteRenderer>().sprite = sprite;
        GetComponent<SpriteRenderer>().color = GameScript.Players[GameScript.CurrentPlayerNum].Color;

        if (RoadBuilded != null)
            RoadBuilded(gameObject.name);

        GetComponent<Collider2D>().enabled = false;
    }
}
