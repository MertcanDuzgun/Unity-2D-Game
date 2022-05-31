using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Block : MonoBehaviour {
    public int Value;
    public Node Node;
    public Block MergingBlock;
    public bool Merging;
    public Vector2 Pos => transform.position;
    [SerializeField] private SpriteRenderer _renderer;

    
    public void Init(BlockType type) {
        Value = type.Value;
        _renderer.color = type.Color;
        _renderer.sprite = type.newSprite;
    }

    // Node'a bir Node'da o anda bir Block olduðunu belirtmek için kullanýlýr.
    public void SetBlock(Node node) {
        if (Node != null) Node.OccupiedBlock = null;
        Node = node;
        Node.OccupiedBlock = this;
    }

    // Bloklarýn birleþtirilmesini saðlar.
    public void MergeBlock(Block blockToMergeWith) {

        MergingBlock = blockToMergeWith;

        Node.OccupiedBlock = null;

        blockToMergeWith.Merging = true;
    }

    // Merge iþleminin yapýlýp yapýlamayacaðýnýn kontrolünü saðlar.
    public bool CanMerge(int value) => value == Value && !Merging && MergingBlock == null;
}
