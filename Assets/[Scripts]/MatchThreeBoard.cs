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
    public List<MatchThreeObjectList> ItemLists = new List<MatchThreeObjectList>();
    public MatchThreeObjectList ItemList
    {
        get
        {
            return ItemLists[(int)currentDifficulty];
        }
    }

    private List<MatchThreeTile> MatchTileList = new List<MatchThreeTile>();
    private List<MatchThreeTile> TempMatchTileList = new List<MatchThreeTile>();
    private IEnumerator CheckTilesCoroutine_Ref = null;

    private Queue<MatchThreeTile> movingQueue = new Queue<MatchThreeTile>();

    private bool SettingUpBoard = true;
    public static bool allowInput { private set; get; } = false;
    private DifficultyLevel currentDifficulty = DifficultyLevel.Easy;

    private ColorType matchedColor = ColorType.None;
    private ItemType matchedType = ItemType.None;

    private void OnEnable()
    {
        MatchThreeEvents.MiniGameStart += Setup;
    }

    private void OnDisable()
    {
        MatchThreeEvents.MiniGameStart -= Setup;
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

        if (SettingUpBoard)
        {
            SettingUpBoard = false;
            allowInput = true;

            // Pause, say start, etc.

            MatchThreeEvents.InvokeOnBoardSetup();
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
        matchedColor = ColorType.None;
        matchedType = ItemType.None;

        // Horizontal
        CheckIsMatchedHorizontalColor(tile);

        if (TempMatchTileList.Count >= 3)
        {
            MatchTileList.AddRange(TempMatchTileList);
            matchedColor = tile.Item.itemColor;
        }

        ResetVisited();
        TempMatchTileList.Clear();

        CheckIsMatchedHorizontalType(tile);

        if (TempMatchTileList.Count >= 3)
        {
            MatchTileList.AddRange(TempMatchTileList);
            matchedType = tile.Item.itemType;
        }

        ResetVisited();
        TempMatchTileList.Clear();

        // Vertical
        CheckIsMatchedVerticalColor(tile);

        if (TempMatchTileList.Count >= 3)
        {
            MatchTileList.AddRange(TempMatchTileList);
            matchedColor = tile.Item.itemColor;
        }

        ResetVisited();
        TempMatchTileList.Clear();

        CheckIsMatchedVerticalType(tile);

        if (TempMatchTileList.Count >= 3)
        {
            MatchTileList.AddRange(TempMatchTileList);
            matchedType = tile.Item.itemType;
        }

        ResetVisited();
        TempMatchTileList.Clear();

        // Final match count check
        if (MatchTileList.Count >= 3)
            return MatchTiles();

        MatchTileList.Clear();

        return false;
    }

    private void CheckIsMatchedHorizontalColor(MatchThreeTile tile)
    {
        tile.wasVisited = true;
        TempMatchTileList.Add(tile);

        // Left
        MatchThreeTile leftTile = tile.GetCloseTile(CloseTilePositions.Left);
        if (leftTile != null && !leftTile.wasVisited && tile.Item != null && leftTile.Item != null && (leftTile.Item.itemColor & tile.Item.itemColor) != 0)
        {
            CheckIsMatchedHorizontalColor(leftTile);
        }

        // Right
        MatchThreeTile rightTile = tile.GetCloseTile(CloseTilePositions.Right);
        if (rightTile != null && !rightTile.wasVisited && tile.Item != null && rightTile.Item != null && (rightTile.Item.itemColor & tile.Item.itemColor) != 0)
        {
            CheckIsMatchedHorizontalColor(rightTile);
        }
    }

    private void CheckIsMatchedHorizontalType(MatchThreeTile tile)
    {
        tile.wasVisited = true;
        TempMatchTileList.Add(tile);

        // Left
        MatchThreeTile leftTile = tile.GetCloseTile(CloseTilePositions.Left);
        if (leftTile != null && !leftTile.wasVisited && tile.Item != null && leftTile.Item != null && (leftTile.Item.itemType & tile.Item.itemType) != 0)
        {
            CheckIsMatchedHorizontalType(leftTile);
        }

        // Right
        MatchThreeTile rightTile = tile.GetCloseTile(CloseTilePositions.Right);
        if (rightTile != null && !rightTile.wasVisited && tile.Item != null && rightTile.Item != null && (rightTile.Item.itemType & tile.Item.itemType) != 0)
        {
            CheckIsMatchedHorizontalType(rightTile);
        }
    }

    private void CheckIsMatchedVerticalColor(MatchThreeTile tile)
    {
        tile.wasVisited = true;
        TempMatchTileList.Add(tile);

        // Top
        MatchThreeTile topTile = tile.GetCloseTile(CloseTilePositions.Top);
        if (topTile != null && !topTile.wasVisited && tile.Item != null && topTile.Item != null && (topTile.Item.itemColor & tile.Item.itemColor) != 0)
        {
            CheckIsMatchedVerticalColor(topTile);
        }

        // Bottom
        MatchThreeTile botTile = tile.GetCloseTile(CloseTilePositions.Bottom);
        if (botTile != null && !botTile.wasVisited && tile.Item != null && botTile.Item != null && (botTile.Item.itemColor & tile.Item.itemColor) != 0)
        {
            CheckIsMatchedVerticalColor(botTile);
        }
    }

    private void CheckIsMatchedVerticalType(MatchThreeTile tile)
    {
        tile.wasVisited = true;
        TempMatchTileList.Add(tile);

        // Top
        MatchThreeTile topTile = tile.GetCloseTile(CloseTilePositions.Top);
        if (topTile != null && !topTile.wasVisited && tile.Item != null && topTile.Item != null && (topTile.Item.itemType & tile.Item.itemType) != 0)
        {
            CheckIsMatchedVerticalType(topTile);
        }

        // Bottom
        MatchThreeTile botTile = tile.GetCloseTile(CloseTilePositions.Bottom);
        if (botTile != null && !botTile.wasVisited && tile.Item != null && botTile.Item != null && (botTile.Item.itemType & tile.Item.itemType) != 0)
        {
            CheckIsMatchedVerticalType(botTile);
        }
    }

    private bool MatchTiles()
    {
        // Calculate score
        int score = Mathf.FloorToInt(MatchTileList.Count);
        MatchThreeEvents.InvokeOnAddScore(score);

        bool bombTriggered = false;

        // Erase the matched tiles
        foreach (MatchThreeTile tile in MatchTileList)
        {
            // Check if a bomb was matched
            if (tile.Item != null && tile.Item.itemType == ItemType.All && tile.Item.itemColor == ColorType.All)
                bombTriggered = true;

            movingQueue.Enqueue(tile);
            tile.Remove();
        }

        // Add all of the matched color and type
        if (bombTriggered)
        {
            int bombScore = 0;

            if (matchedColor != ColorType.None)
            {
                List<MatchThreeTile> cTiles = GetTilesOfColor(matchedColor);
                bombScore += cTiles.Count;

                foreach (MatchThreeTile tile in cTiles)
                {
                    movingQueue.Enqueue(tile);
                    tile.Remove();
                }
            }

            if (matchedType != ItemType.None)
            {
                List<MatchThreeTile> tTiles = GetTilesOfType(matchedType);
                bombScore += tTiles.Count;

                foreach (MatchThreeTile tile in tTiles)
                {
                    movingQueue.Enqueue(tile);
                    tile.Remove();
                }
            }

            MatchThreeEvents.InvokeOnAddScore(bombScore);
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

    private List<MatchThreeTile> GetTilesOfType(ItemType itemType)
    {
        List<MatchThreeTile> tilesOfType = new List<MatchThreeTile>();

        foreach (List<MatchThreeTile> list in GridTiles)
        {
            foreach (MatchThreeTile tile in list)
            {
                if (tile.Item != null && tile.Item.itemType == itemType)
                    tilesOfType.Add(tile);
            }
        }

        return tilesOfType;
    }

    private List<MatchThreeTile> GetTilesOfColor(ColorType itemColor)
    {
        List<MatchThreeTile> tilesOfColor = new List<MatchThreeTile>();

        foreach (List<MatchThreeTile> list in GridTiles)
        {
            foreach (MatchThreeTile tile in list)
            {
                if (tile.Item != null && tile.Item.itemColor == itemColor)
                    tilesOfColor.Add(tile);
            }
        }

        return tilesOfColor;
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

    public void Setup(DifficultyLevel difficulty)
    {
        currentDifficulty = difficulty;

        // Destroy grid
        for (int i = boardObject.transform.childCount - 1; i >= 0; i--)
        {
            boardObject.transform.GetChild(i).gameObject.GetComponent<MatchThreeTile>().StopGame();
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

            matchTile.Init(tile, new Vector2Int(gridX, gridY), null);

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

                movingQueue.Enqueue(mTile);
            }
        }

        SettingUpBoard = true;
        allowInput = false;

        // Check for matches on start
        CheckAllTiles();
    }
}
