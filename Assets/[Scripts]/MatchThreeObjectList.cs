using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ColorType
{
    Red = 1,
    Blue = 2,
    Green = 4
}

public enum ItemType
{
    Circle = 1,
    Square = 2,
    Triangle = 4
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
