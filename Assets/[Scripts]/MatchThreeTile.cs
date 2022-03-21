using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum CloseTilePositions
{
    Left, Right, Top, Bottom
}

public class MatchThreeTile : MonoBehaviour
{
    public static MatchThreeTile currentTile;
    public static MatchThreeBoard board;

    public Image icon;
    public MatchThreeItem Item;

    public GameObject Tile { get; set; }
    public Vector2Int GridPosition { get; set; }

    public List<MatchThreeTile> CloseTiles = new List<MatchThreeTile>();

    public bool wasVisited = false;

    private Animator animator;

    private void Awake()
    {
        if (icon == null)
            icon = GetComponent<Image>();

        if (board == null)
            board = FindObjectOfType<MatchThreeBoard>();

        animator = GetComponent<Animator>();
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
        // If we have a current tile
        if (currentTile == this || currentTile == null) return;

        // If this tile has something in it
        if (Item == null || currentTile.Item == null) return;

        // Check if it is a close tile
        if (!CheckIsClose(currentTile)) return;

        SwapItem(this, currentTile);

        // Check if it completes a set, if not, swap back
        if (!board.CheckIsMatched(this, currentTile))
            SwapItem(this, currentTile);
    }

    private static void SwapItem(MatchThreeTile thisTile, MatchThreeTile thatTile)
    {
        MatchThreeItem _item = thatTile.Item;

        thatTile.SetTileItem(thisTile.Item);
        thisTile.SetTileItem(_item);
    }

    public void SetTileItem(MatchThreeItem _item)
    {
        Item = _item;
        SetupItem();
    }

    private void SetupItem()
    {
        if (Item == null)
        {
            icon.sprite = null;
            icon.enabled = false;
            return;
        }

        icon.sprite = Item.itemSprite;
        icon.enabled = true;
    }

    private bool CheckIsClose(MatchThreeTile tile)
    {
        foreach (MatchThreeTile t in CloseTiles)
        {
            if (tile == t)
                return true;
        }

        return false;
    }

    public MatchThreeTile GetCloseTile(CloseTilePositions pos)
    {
        return CloseTiles[(int)pos];
    }

    public void Remove()
    {
        Item = null;
        SetupItem();
    }

    public void PlayAnimation()
    {
        animator.SetTrigger("AnimTrigger");
    }

}
