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
    public float animationWaitTime = 0.1666f;
    private bool playingAnimation = false;

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
        // If we are allowed input
        if (!MatchThreeBoard.allowInput) return;

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

    public bool GetIsEmptySpace(CloseTilePositions dir)
    {
        // If we are at the edge, return false
        if (GetCloseTile(dir) == null) return false;

        // If there is an item in that space, keep going
        if (GetCloseTile(dir).Item != null) return GetCloseTile(dir).GetIsEmptySpace(dir);

        // There is empty space
        return true;
    }

    public void Remove()
    {
        Item = null;
        SetupItem();
    }

    public void PlayAnimation()
    {
        if (!GetIsEmptySpace(CloseTilePositions.Bottom)) playingAnimation = false;

        if (playingAnimation) return;

        playingAnimation = true;

        if (animator != null)
            animator.SetTrigger("AnimTrigger");
    }

    public IEnumerator MoveColumnDown()
    {
        // This tile has an item in it
        if (Item != null) yield break;

        MatchThreeTile top = GetCloseTile(CloseTilePositions.Top);

        // This tile is at the top
        if (top == null)
        {
            if (board == null) yield break;

            SetTileItem(board.ItemList.GetRandomItem());

            // Play Animation
            PlayAnimation();
            yield return new WaitForSeconds(animationWaitTime);

            yield break;
        }

        // Move Down top Item
        while (top.Item == null)
            yield return StartCoroutine(top.MoveColumnDown());

        if (Item == null)
        {
            SetTileItem(top.Item);
            top.Remove();

            // Play Animation
            PlayAnimation();
            yield return new WaitForSeconds(animationWaitTime);

            // This tile is at the bottom, start moving down tiles on top
            yield return StartCoroutine(top.MoveColumnDown());
        }
    }

    public void StopGame()
    {
        StopAllCoroutines();
    }

}
