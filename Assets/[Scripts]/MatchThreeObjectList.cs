using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ColorType
{
    Red, Blue, Green
}

public enum ItemType
{
    Circle, Square, Triangle
}

[System.Serializable]
public class MatchThreeItem
{
    public Sprite itemSprite;
    public ColorType itemColor;
    public ItemType itemType;

    public MatchThreeItem(Sprite _itemSprite, ColorType _itemColor, ItemType _itemType)
    {
        itemSprite = _itemSprite;
        itemColor = _itemColor;
        itemType = _itemType;
    }
}

[CreateAssetMenu(fileName = "NewObjectList", menuName = "MatchThree/ObjectList")]
public class MatchThreeObjectList : ScriptableObject
{
    public List<MatchThreeItem> Items = new List<MatchThreeItem>();

    private MatchThreeItem GetRandomItemFromList(List<MatchThreeItem> list)
    {
        if (list.Count <= 0) return null;

        int index = Random.Range(0, list.Count);
        MatchThreeItem item = list[index];
        return item;
    }

    public MatchThreeItem GetRandomItem()
    {
        return GetRandomItemFromList(Items);
    }

    public MatchThreeItem GetRandomItem(ColorType colorType)
    {
        List<MatchThreeItem> list = new List<MatchThreeItem>();

        foreach (MatchThreeItem item in Items)
        {
            if (item.itemColor == colorType)
                list.Add(item);
        }

        return GetRandomItemFromList(list);
    }

    public MatchThreeItem GetRandomItem(ItemType itemType)
    {
        List<MatchThreeItem> list = new List<MatchThreeItem>();

        foreach (MatchThreeItem item in Items)
        {
            if (item.itemType == itemType)
                list.Add(item);
        }

        return GetRandomItemFromList(list);
    }
}
