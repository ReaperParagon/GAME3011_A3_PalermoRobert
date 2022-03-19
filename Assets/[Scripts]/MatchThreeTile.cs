using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchThreeTile : MonoBehaviour
{
    public static MatchThreeTile currentTile;

    // Replace with a class / something
    public Image item;

    public GameObject Tile { get; set; }
    public Vector2Int GridPosition { get; set; }

    public List<MatchThreeTile> CloseTiles = new List<MatchThreeTile>();

    private void Awake()
    {
        if (item == null)
            item = GetComponent<Image>();

        float r = Random.Range(0.0f, 1.0f);
        float g = Random.Range(0.0f, 1.0f);
        float b = Random.Range(0.0f, 1.0f);

        item.color = new Color(r, g, b, 1.0f);
    }

    public void OnDrag()
    {
        currentTile = this;
    }

    public void OnRelease()
    {
        // Swap this with Current Tile
        if (currentTile == this || currentTile == null) return;

        Color c = currentTile.item.color;

        currentTile.SetTileSprite(item.color);
        SetTileSprite(c);
    }

    private void SetTileSprite(Color color)
    {
        item.color = color;
    }

}
