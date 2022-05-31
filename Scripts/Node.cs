using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour {

    // Pozisyon al�n�yor.
    public Vector2 Pos => transform.position;

    // Node'un bir blokla dolu olup olmad���n� belirtmek i�in.
    public Block OccupiedBlock;
}
