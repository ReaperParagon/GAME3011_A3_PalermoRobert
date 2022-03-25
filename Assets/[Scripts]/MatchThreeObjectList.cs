using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ColorType
{
    Red = 1,
    Blue = 2,
    Green = 4,
    Cyan = 8,
    Magenta = 16,
    Yellow = 32,
    White = 64,
    None = 0,
    All = 127
}

public enum ItemType
{
    Default = 1,
    Heart = 2,
    Octagon = 4,
    Tear = 8,
    Immovable = 16,
    None = 0,
    All = 31
}

[System.Serializable]
public class MatchThreeItem
{
    public Sprite itemSprite;
    public Color itemTint;
    public ColorType itemColor;
    public ItemType itemType;

    public MatchThreeItem(Sprite _itemSprite, ColorType _itemColor, ItemType _itemType)
    {
        itemSprite = _itemSprite;
        itemTint = Color.white;
        itemColor = _itemColor;
        itemType = _itemType;
    }
}

[CreateAssetMenu(fileName = "NewObjectList", menuName = "MatchThree/ObjectList")]
public class MatchThreeObjectList : ScriptableObject
{
    public List<MatchThreeItem> Items = new List<MatchThreeItem>();
    public MatchThreeItem Bomb;

    public string HintMessage = "";

    private bool spawnBomb = false;

    private void OnEnable()
    {
        MatchThreeEvents.AddScore += SpawnBomb;
        MatchThreeEvents.MiniGameStart += Setup;
    }

    private void OnDisable()
    {
        MatchThreeEvents.AddScore -= SpawnBomb;
        MatchThreeEvents.MiniGameStart -= Setup;
    }

    private void Setup(DifficultyLevel _)
    {
        spawnBomb = false;
    }

    private void SpawnBomb(int score)
    {
        if (Bomb.itemType == ItemType.None) return;

        if (score >= 5)
            spawnBomb = true;
    }

    private MatchThreeItem GetRandomItemFromList(List<MatchThreeItem> list)
    {
        if (list.Count <= 0) return null;

        int index = Random.Range(0, list.Count);
        MatchThreeItem item = list[index];

        if (item.itemType == ItemType.Immovable)
        {
            index = Random.Range(0, list.Count);
            item = list[index];
        }

        return item;
    }

    public MatchThreeItem GetRandomItem()
    {
        if (spawnBomb)
        {
            spawnBomb = false;
            return Bomb;
        }

        return GetRandomItemFromList(Items);
    }

    public MatchThreeItem GetRandomItemFromColor(int colorType)
    {
        List<MatchThreeItem> list = new List<MatchThreeItem>();

        foreach (MatchThreeItem item in Items)
        {
            if (((int)item.itemColor & colorType) != 0)
                list.Add(item);
        }

        if (list.Count > 0)
            return GetRandomItemFromList(list);

        return GetRandomItem();
    }

    public MatchThreeItem GetRandomItemFromType(int itemType)
    {
        List<MatchThreeItem> list = new List<MatchThreeItem>();

        foreach (MatchThreeItem item in Items)
        {
            if (((int)item.itemType & itemType) != 0)
                list.Add(item);
        }

        if (list.Count > 0)
            return GetRandomItemFromList(list);

        return GetRandomItem();
    }
}
