using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchThreeBoard : MonoBehaviour
{
    [SerializeField]
    private Vector2Int GridDimensions;
    private int GridCount { get { return GridDimensions.x * GridDimensions.y; } }
    private List<List<MatchThreeTile>> GridTiles = new List<List<MatchThreeTile>>();

    [Header("Grid Tile Information")]
    [SerializeField]
    private GameObject TilePrefab;
    public MatchThreeObjectList ItemList;

    private List<MatchThreeTile> MatchTileList = new List<MatchThreeTile>();
    private IEnumerator CheckTilesCoroutine_Ref = null;

    public float animationWaitTime = 1.0f;
    private Queue<IEnumerator> movingQueue = new Queue<IEnumerator>();

    void Awake()
    {
        // Setup Grid Layout
        GridLayoutGroup gridLayout = GetComponent<GridLayoutGroup>();
        gridLayout.constraintCount = GridDimensions.x;

        for (int i = 0; i < GridCount; i++)
        {
            // Add Tile to Grid
            GameObject tile = Instantiate(TilePrefab, transform);

            // Store info based on all grid positions
            int gridY = i / GridDimensions.x;
            int gridX = i % GridDimensions.x;

            MatchThreeTile matchTile = tile.GetComponent<MatchThreeTile>();

            matchTile.Init(tile, new Vector2Int(gridX, gridY), ItemList.GetRandomItem());

            // Check our Grid size
            if (gridX > GridTiles.Count - 1)
                GridTiles.Add(new List<MatchThreeTile>());

            if (gridY > GridTiles[gridX].Count - 1)
                GridTiles[gridX].Add(matchTile);

            GridTiles[gridX][gridY] = matchTile;
        }

        foreach (List<MatchThreeTile> row in GridTiles)
        {
            foreach (MatchThreeTile mTile in row)
            {
                // Set up neighbouring tiles
                mTile.CloseTiles.AddRange(GetCloseTiles(mTile.GridPosition));
            }
        }

        CheckAllTiles();
    }

    private void UpdateTileContents(List<MatchThreeTile> emptyTiles)
    {
        // Disable player interaction

        // Place Items into correct tiles
        foreach (MatchThreeTile tile in emptyTiles)
        {
            movingQueue.Enqueue(MoveColumnDown(tile));
        }

        FlushQueue();

        // Re-enable player interaction

        // CheckAllTiles();
    }

    private void FlushQueue()
    {
        if (movingQueue.Count > 0)
            StartCoroutine(movingQueue.Dequeue());
    }

    private IEnumerator MoveColumnDown(MatchThreeTile tile)
    {
        // This tile has an item in it
        if (tile.Item != null) yield break;

        MatchThreeTile top = tile.GetCloseTile(CloseTilePositions.Top);

        // This tile is at the top
        if (top == null)
        {
            tile.SetTileItem(ItemList.GetRandomItem());

            // Play Animation
            tile.PlayAnimation();
            yield return new WaitForSeconds(animationWaitTime);

            yield break;
        }

        // Move Down top Item
        while (top.Item == null)
            yield return StartCoroutine(MoveColumnDown(top));

        if (tile.Item == null)
        {
            tile.SetTileItem(top.Item);
            top.Remove();

            // Play Animation
            tile.PlayAnimation();
            yield return new WaitForSeconds(animationWaitTime);

            // This tile is at the bottom, start moving down tiles on top
            yield return StartCoroutine(MoveColumnDown(top));
        }

        FlushQueue();
    }

    private List<MatchThreeTile> GetCloseTiles(Vector2Int gridPosition)
    {
        List<MatchThreeTile> rTileList = new List<MatchThreeTile>();

        int x = gridPosition.x;
        int y = gridPosition.y;

        // Left
        rTileList.Add(x - 1 >= 0 ? GetTileAtPosition(x - 1, y) : null);

        // Right
        rTileList.Add(x + 1 <= GridTiles.Count - 1 ? GetTileAtPosition(x + 1, y) : null);

        // Top
        rTileList.Add(y > 0 ? GetTileAtPosition(x, y - 1) : null);

        // Bottom
        rTileList.Add(y < GridTiles[0].Count - 1 ? GetTileAtPosition(x, y + 1) : null);

        return rTileList;
    }

    private MatchThreeTile GetTileAtPosition(int x, int y)
    {
        return GridTiles[x][y];
    }

    public bool CheckIsMatched(MatchThreeTile thisTile, MatchThreeTile thatTile)
    {
        bool thisMatched = CheckIsMatched(thisTile);
        bool thatMatched = CheckIsMatched(thatTile);

        return (thisMatched || thatMatched);
    }

    private bool CheckIsMatched(MatchThreeTile tile)
    {
        int matchCount = 1;

        // Horizontal
        CheckIsMatchedHorizontal(ref matchCount, tile);

        if (matchCount >= 3) return MatchTiles();
        matchCount = 1;

        ResetVisited();
        MatchTileList.Clear();

        // Vertical
        CheckIsMatchedVertical(ref matchCount, tile);

        if (matchCount >= 3) return MatchTiles();
        matchCount = 1;

        ResetVisited();
        MatchTileList.Clear();

        return false;
    }

    private void CheckIsMatchedHorizontal(ref int matchCount, MatchThreeTile tile)
    {
        tile.wasVisited = true;
        MatchTileList.Add(tile);

        // Left
        MatchThreeTile leftTile = tile.GetCloseTile(CloseTilePositions.Left);
        if (leftTile != null && !leftTile.wasVisited && leftTile.Item == tile.Item)
        {
            matchCount++;
            CheckIsMatchedHorizontal(ref matchCount, leftTile);
        }

        // Right
        MatchThreeTile rightTile = tile.GetCloseTile(CloseTilePositions.Right);
        if (rightTile != null && !rightTile.wasVisited && rightTile.Item == tile.Item)
        {
            matchCount++;
            CheckIsMatchedHorizontal(ref matchCount, rightTile);
        }
    }

    private void CheckIsMatchedVertical(ref int matchCount, MatchThreeTile tile)
    {
        tile.wasVisited = true;
        MatchTileList.Add(tile);

        // Top
        MatchThreeTile topTile = tile.GetCloseTile(CloseTilePositions.Top);
        if (topTile != null && !topTile.wasVisited && topTile.Item == tile.Item)
        {
            matchCount++;
            CheckIsMatchedVertical(ref matchCount, topTile);
        }

        // Bottom
        MatchThreeTile botTile = tile.GetCloseTile(CloseTilePositions.Bottom);
        if (botTile != null && !botTile.wasVisited && botTile.Item == tile.Item)
        {
            matchCount++;
            CheckIsMatchedVertical(ref matchCount, botTile);
        }
    }

    private bool MatchTiles()
    {
        // Calculate score
        // TODO: Implement score
        int score = MatchTileList.Count * MatchTileList.Count;

        List<MatchThreeTile> removedTiles = MatchTileList;

        // Erase the matched tiles
        foreach (MatchThreeTile tile in MatchTileList)
        {
            tile.Remove();
        }

        // Tell Tiles to move down
        UpdateTileContents(removedTiles);

        ResetVisited();
        MatchTileList.Clear();
        return true;
    }

    private void ResetVisited()
    {
        foreach (List<MatchThreeTile> list in GridTiles)
        {
            foreach (MatchThreeTile tile in list)
            {
                tile.wasVisited = false;
            }
        }
    }

    private void CheckAllTiles()
    {
        if (CheckTilesCoroutine_Ref == null)
        {
            CheckTilesCoroutine_Ref = CheckAllTilesForMatch();
            StartCoroutine(CheckTilesCoroutine_Ref);
        }
    }

    private IEnumerator CheckAllTilesForMatch()
    {
        foreach (List<MatchThreeTile> list in GridTiles)
        {
            foreach (MatchThreeTile tile in list)
            {
                if (CheckIsMatched(tile))
                    yield return new WaitForSeconds(1.0f);
            }
        }

        CheckTilesCoroutine_Ref = null;
    }
}
