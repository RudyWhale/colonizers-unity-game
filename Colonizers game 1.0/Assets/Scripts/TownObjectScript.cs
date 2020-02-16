using UnityEngine;

public class TownObjectScript : MonoBehaviour {
    public Sprite villageSprite;
    public Sprite townSprite;

    // Событие, вызываемое при постройке нового поселения
    public delegate void VillageBuildedHandler(string objectID, bool townBuilded);
    public static event VillageBuildedHandler TownBuilded;

    /// <summary>
    /// Назначает спрайт объекта
    /// </summary>
	public void OnMouseDown () {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        spriteRenderer.sprite = spriteRenderer.sprite == null ? villageSprite : townSprite;
        spriteRenderer.color = GameScript.Players[GameScript.CurrentPlayerNum].Color;

        if (TownBuilded != null)
            TownBuilded(name, spriteRenderer.sprite == townSprite ? true : false);

        GetComponent<Collider2D>().enabled = false;
    }
}
