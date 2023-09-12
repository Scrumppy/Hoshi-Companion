using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu] // This script holds the data from the Striker

public class StrikersCardSO : ScriptableObject
{
    [SerializeField] public int ID => GetInstanceID();
    [field: SerializeField] public Sprite icon{ get; set; }
    [field: SerializeField] public int token_id { get; set; }
    [field: SerializeField] public string name { get; set; }
    [field: SerializeField] public string description { get; set; }
    [field: SerializeField] public string collection { get; set; }
    [field: SerializeField] public int rarity { get; set; }
    [field: SerializeField] public string image_url { get; set; }
    [field: SerializeField] public string animation_url { get; set; }
    [field: SerializeField] public List<Attribute> attributes { get; set; }

    /// <summary>
    /// Add variables as needed manually above.
    /// Then just use GPT AI to update this function... 
    /// or do it manually yourself if you are feeling a bit masochist :D
    /// </summary>
    public void SetData(Sprite _icon, int _token_id, string _name, string _description, string _collection,
        int _rarity, string _imageUrl, string _animationUrl, List<Attribute> _attributes)
    {
        this.icon = _icon;
        this.token_id = _token_id;
        this.name = _name;
        this.description = _description;
        this.collection = _collection;
        this.rarity = _rarity;
        this.image_url = _imageUrl;
        this.animation_url = _animationUrl;
        this.attributes = _attributes;
    }
}

[System.Serializable]
public class Attribute
{
    [SerializeField] public string trait_type;
    [SerializeField] public object value;

}

