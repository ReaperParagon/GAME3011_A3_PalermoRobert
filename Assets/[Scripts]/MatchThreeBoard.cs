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
        return (CheckIsMatched(thisTile) || CheckIsMatched(thatTile));
    }

    private bool CheckIsMatched(MatchThreeTile tile)
    {
        int matchCount = 1;

        // Horizontal
        CheckIsMatchedHorizontal(ref matchCount, tile);

        if (matchCount >= 3) return MatchTiles();
        matchCount = 1;

        ResetVisited();

        // Vertical
        CheckIsMatchedVertical(ref matchCount, tile);

        if (matchCount >= 3) return MatchTiles();
        matchCount = 1;

        ResetVisited();

        return false;
    }

    private void CheckIsMatchedHorizontal(ref int matchCount, MatchThreeTile tile)
    {
        tile.wasVisited = true;

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
        // Erase the matched tiles

        // Tell Tiles to move down

        // Calculate score

        ResetVisited();
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
}
