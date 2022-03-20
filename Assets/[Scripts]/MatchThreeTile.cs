using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchThreeTile : MonoBehaviour
{
    public static MatchThreeTile currentTile;

    public Image icon;
    public MatchThreeItem Item;

    public GameObject Tile { get; set; }
    public Vector2Int GridPosition { get; set; }

    public List<MatchThreeTile> CloseTiles = new List<MatchThreeTile>();

    private void Awake()
    {
        if (icon == null)
            icon = GetComponent<Image>();
    }

    public void Init(GameObject _tile, Vector2Int _pos, MatchThreeItem _item)
    {
        Tile = _tile;
        GridPosition = _pos;
        Item = _item;

        if (icon == null) icon = GetComponent<Image>();

        SetupItem();
    }

    public void OnDrag()
    {
        currentTile = this;
    }

    public void OnRelease()
    {
        // Swap this with Current Tile
        if (currentTile == this || currentTile == null) return;

        MatchThreeItem _item = currentTile.Item;

        currentTile.SetTileItem(Item);
        SetTileItem(_item);
    }

    private void SetTileItem(MatchThreeItem _item)
    {
        Item = _item;

        SetupItem();
    }

    private void SetupItem()
    {
        icon.sprite = Item.itemSprite;
    }

}
