using UnityEngine;

[CreateAssetMenu(menuName = "Shop/ItemData")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public int price;
}