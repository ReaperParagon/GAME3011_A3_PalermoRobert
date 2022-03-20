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
        if (x - 1 >= 0)
            rTileList.Add(GetTileAtPosition(x - 1, y));

        // Right
        if (x + 1 <= GridTiles.Count - 1)
            rTileList.Add(GetTileAtPosition(x + 1, y));

        // Top
        if (y > 0)
            rTileList.Add(GetTileAtPosition(x, y - 1));

        // Bottom
        if (y < GridTiles[0].Count - 1)
            rTileList.Add(GetTileAtPosition(x, y + 1));

        return rTileList;
    }

    private MatchThreeTile GetTileAtPosition(int x, int y)
    {
        return GridTiles[x][y];
    }

}
