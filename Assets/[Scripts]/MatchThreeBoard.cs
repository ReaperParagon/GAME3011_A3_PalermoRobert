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
    public GameObject boardObject;
    [SerializeField]
    private GameObject TilePrefab;
    public MatchThreeObjectList ItemList;

    private List<MatchThreeTile> MatchTileList = new List<MatchThreeTile>();
    private List<MatchThreeTile> TempMatchTileList = new List<MatchThreeTile>();
    private IEnumerator CheckTilesCoroutine_Ref = null;

    public float animationWaitTime = 1.0f;
    private Queue<MatchThreeTile> movingQueue = new Queue<MatchThreeTile>();

    private void OnEnable()
    {
        MatchThreeEvents.MiniGameStart += Setup;
        MatchThreeEvents.MiniGameComplete += StopGame;
        MatchThreeEvents.TimerFinished += StopGame;
    }

    private void OnDisable()
    {
        MatchThreeEvents.MiniGameStart -= Setup;
        MatchThreeEvents.MiniGameComplete -= StopGame;
        MatchThreeEvents.TimerFinished -= StopGame;
    }

    private void UpdateTileContents()
    {
        // Disable player interaction

        StartCoroutine(FlushQueue());

        // Re-enable player interaction

    }

    private IEnumerator FlushQueue()
    {
        while (movingQueue.Count > 0)
        {
            yield return StartCoroutine(movingQueue.Dequeue().MoveColumnDown());
        }

        CheckAllTiles();
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

        if (thisMatched || thatMatched)
        {
            // Tell Tiles to move down
            UpdateTileContents();
        }
        
        return (thisMatched || thatMatched);
    }

    private bool CheckIsMatched(MatchThreeTile tile)
    {
        // Horizontal
        CheckIsMatchedHorizontal(tile);

        if (TempMatchTileList.Count >= 3)
            MatchTileList.AddRange(TempMatchTileList);

        ResetVisited();
        TempMatchTileList.Clear();

        // Vertical
        CheckIsMatchedVertical(tile);

        if (TempMatchTileList.Count >= 3)
            MatchTileList.AddRange(TempMatchTileList);

        ResetVisited();
        TempMatchTileList.Clear();

        // Final match count check
        if (MatchTileList.Count >= 3)
            return MatchTiles();

        MatchTileList.Clear();

        return false;
    }

    private void CheckIsMatchedHorizontal(MatchThreeTile tile)
    {
        tile.wasVisited = true;
        TempMatchTileList.Add(tile);

        // Left
        MatchThreeTile leftTile = tile.GetCloseTile(CloseTilePositions.Left);
        if (leftTile != null && !leftTile.wasVisited && tile.Item != null && leftTile.Item == tile.Item)
        {
            CheckIsMatchedHorizontal(leftTile);
        }

        // Right
        MatchThreeTile rightTile = tile.GetCloseTile(CloseTilePositions.Right);
        if (rightTile != null && !rightTile.wasVisited && tile.Item != null && rightTile.Item == tile.Item)
        {
            CheckIsMatchedHorizontal(rightTile);
        }
    }

    private void CheckIsMatchedVertical(MatchThreeTile tile)
    {
        tile.wasVisited = true;
        TempMatchTileList.Add(tile);

        // Top
        MatchThreeTile topTile = tile.GetCloseTile(CloseTilePositions.Top);
        if (topTile != null && !topTile.wasVisited && tile.Item != null && topTile.Item == tile.Item)
        {
            CheckIsMatchedVertical(topTile);
        }

        // Bottom
        MatchThreeTile botTile = tile.GetCloseTile(CloseTilePositions.Bottom);
        if (botTile != null && !botTile.wasVisited && tile.Item != null && botTile.Item == tile.Item)
        {
            CheckIsMatchedVertical(botTile);
        }
    }

    private bool MatchTiles()
    {
        // Calculate score
        int score = MatchTileList.Count * MatchTileList.Count;
        MatchThreeEvents.InvokeOnAddScore(score);

        // Erase the matched tiles
        foreach (MatchThreeTile tile in MatchTileList)
        {
            movingQueue.Enqueue(tile);
            tile.Remove();
        }

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
                    yield return new WaitForSeconds(0.0f);
            }
        }

        UpdateTileContents();

        CheckTilesCoroutine_Ref = null;
    }

    public void Setup(DifficultyLevel _)
    {
        // Destroy grid
        for (int i = boardObject.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(boardObject.transform.GetChild(i).gameObject);
        }

        // Setup Grid Layout
        GridLayoutGroup gridLayout = boardObject.GetComponent<GridLayoutGroup>();
        gridLayout.constraintCount = GridDimensions.x;

        for (int i = 0; i < GridCount; i++)
        {
            // Add Tile to Grid
            GameObject tile = Instantiate(TilePrefab, boardObject.transform);

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

        // Check for matches on start
        CheckAllTiles();
    }

    public void StopGame()
    {
        StopAllCoroutines();
        CheckTilesCoroutine_Ref = null;
    }
}
