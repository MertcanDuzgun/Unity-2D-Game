using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour {

    // Pozisyon alýnýyor.
    public Vector2 Pos => transform.position;

    // Node'un bir blokla dolu olup olmadýðýný belirtmek için.
    public Block OccupiedBlock;
}
