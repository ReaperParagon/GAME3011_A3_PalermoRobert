using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchThreeBoard : MonoBehaviour
{
    public List<Vector2Int> GridSizes = new List<Vector2Int>();

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

    private ColorType tempMatchedColor = ColorType.None;
    private ItemType tempMatchedType = ItemType.None;

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

            yield return FindObjectOfType<MatchThreeMinigameManager>().DisplayHintMessage("Start!");

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
        matchedColor = tempMatchedColor = ColorType.None;
        matchedType = tempMatchedType = ItemType.None;

        // Horizontal
        CheckIsMatchedHorizontalColor(tile);
        AddTempToMatchList();

        CheckIsMatchedHorizontalType(tile);
        AddTempToMatchList();

        // Vertical
        CheckIsMatchedVerticalColor(tile);
        AddTempToMatchList();

        CheckIsMatchedVerticalType(tile);
        AddTempToMatchList();

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
        if (CheckItemsAreMatched(tile, leftTile, false))
        {
            CheckIsMatchedHorizontalColor(leftTile);
        }

        // Right
        MatchThreeTile rightTile = tile.GetCloseTile(CloseTilePositions.Right);
        if (CheckItemsAreMatched(tile, rightTile, false))
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
        if (CheckItemsAreMatched(tile, leftTile, true))
        {
            CheckIsMatchedHorizontalType(leftTile);
        }

        // Right
        MatchThreeTile rightTile = tile.GetCloseTile(CloseTilePositions.Right);
        if (CheckItemsAreMatched(tile, rightTile, true))
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
        if (CheckItemsAreMatched(tile, topTile, false))
        {
            CheckIsMatchedVerticalColor(topTile);
        }

        // Bottom
        MatchThreeTile botTile = tile.GetCloseTile(CloseTilePositions.Bottom);
        if (CheckItemsAreMatched(tile, botTile, false))
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
        if (CheckItemsAreMatched(tile, topTile, true))
        {
            CheckIsMatchedVerticalType(topTile);
        }

        // Bottom
        MatchThreeTile botTile = tile.GetCloseTile(CloseTilePositions.Bottom);
        if (CheckItemsAreMatched(tile, botTile, true))
        {
            CheckIsMatchedVerticalType(botTile);
        }
    }

    private bool CheckItemsAreMatched(MatchThreeTile thisTile, MatchThreeTile thatTile, bool checkType)
    {
        if (thatTile == null || thatTile.wasVisited) return false;
        if (thisTile.Item == null || thatTile.Item == null) return false;

        if (checkType)
        {
            if (thisTile.Item.itemType == ItemType.None) return false;

            if ((thatTile.Item.itemType & thisTile.Item.itemType) == 0)
                return false;
        }
        else
        {
            if (thisTile.Item.itemColor == ColorType.None) return false;

            if ((thatTile.Item.itemColor & thisTile.Item.itemColor) == 0)
                return false;
        }

        return true;
    }

    private void AddTempToMatchList()
    {
        if (TempMatchTileList.Count >= 3)
        {
            tempMatchedType = TempMatchTileList[0].Item.itemType;
            tempMatchedColor = TempMatchTileList[0].Item.itemColor;

            foreach (MatchThreeTile tile in TempMatchTileList)
            {
                tempMatchedType &= tile.Item.itemType;
                tempMatchedColor &= tile.Item.itemColor;
            }

            matchedType |= tempMatchedType;
            matchedColor |= tempMatchedColor;

            if (matchedColor != ColorType.None || matchedType != ItemType.None)
                MatchTileList.AddRange(TempMatchTileList);
        }

        ResetVisited();
        TempMatchTileList.Clear();

        tempMatchedColor = ColorType.None;
        tempMatchedType = ItemType.None;
    }

    private bool MatchTiles()
    {
        // Calculate score
        MatchThreeEvents.InvokeOnAddScore(MatchTileList.Count);

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
            MatchThreeEvents.InvokeOnBomb();

            if (matchedColor != ColorType.None)
            {
                List<MatchThreeTile> cTiles = GetTilesOfColor(matchedColor);

                foreach (MatchThreeTile tile in cTiles)
                {
                    movingQueue.Enqueue(tile);
                    tile.Remove();
                }
            }

            if (matchedType != ItemType.None)
            {
                List<MatchThreeTile> tTiles = GetTilesOfType(matchedType);

                foreach (MatchThreeTile tile in tTiles)
                {
                    movingQueue.Enqueue(tile);
                    tile.Remove();
                }
            }
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
                if (tile.Item != null && (tile.Item.itemType & itemType) != 0)
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
                if (tile.Item != null && (tile.Item.itemColor & itemColor) != 0)
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

        GridDimensions = GridSizes[(int)difficulty];

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

        FindObjectOfType<MatchThreeMinigameManager>().Hint(ItemList.HintMessage);

        // Check for matches on start
        CheckAllTiles();
    }
}
