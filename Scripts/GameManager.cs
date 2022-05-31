using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using TMPro;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour {
    [SerializeField] private int _width = 4;
    [SerializeField] private int _height = 4;
    [SerializeField] private Node _nodePrefab;
    [SerializeField] private Block _blockPrefab;
    [SerializeField] private SpriteRenderer _boardPrefab;
    [SerializeField] private List<BlockType> _types;
    [SerializeField] private float _travelTime = 0.2f;
    [SerializeField] private int _winCondition = 2048;
    [SerializeField] private int achieveCount = 0;
    [SerializeField] private GameObject _winScreen, _loseScreen, _winScreenText;

    /*Objeler birle�ince bir efekt g�z�kmesi istenirse.
    [SerializeField] private GameObject _mergeEffectPrefab;*/

    /*Objeler birle�ince birle�en yerde o obje tipine g�re s�z�l�p kaybolan textler olu�turmay� sa�lar.
    [SerializeField] private FloatingText _floatingTextPrefab;*/

    /*Audio ile alakal� i�lem yap�lmas� istenirse.
    [SerializeField] private AudioClip[] _moveClips;
    [SerializeField] private AudioClip[] _matchClips;
    [SerializeField] private AudioSource _source;*/

    // Canvas ile ilgili i�lemler i�in.
    // Oyun s�ras�nda de�i�im g�stermeyecek TextMeshPro elementleri.
    public TextMeshPro cpTextSt, coTextSt, ipTextSt, ioTextSt, spTextSt, soTextSt, gpTextSt, goTextSt, diaTextSt;

    // Oyundan �nce Hierarchy'deki GameManager elementi �zerinde oyun s�ras�nda de�i�im g�stermeyecek TextMeshPro elementlerinin �nceden belirlenmesi i�in de�i�kenler.
    public int cpSetStatic, coSetStatic, ipSetStatic, ioSetStatic, spSetStatic, soSetStatic, gpSetStatic, goSetStatic, diaSetStatic;

    // Oyun s�ras�nda de�i�im g�sterecek TextMeshPro elementleri.
    public TextMeshPro cpTextDy, coTextDy, ipTextDy, ioTextDy, spTextDy, soTextDy, gpTextDy, goTextDy, diaTextDy;

    // Oyun de�i�im g�sterecek TextMeshPro elementlerinin de�erlerini belirlemek i�in gerekli integer de�i�keneler.
    private int cpDynamic, coDynamic, ipDynamic, ioDynamic, spDynamic, soDynamic, gpDynamic, goDynamic, diaDynamic;

    // Seviyenin ge�ilmesi i�in kontrol� sa�layacak boolean tipinde de�i�kenler.
    private bool cpAT, coAT, ipAT, ioAT, spAT, soAT, gpAT, goAT, diaAT;

    private List<Node> _nodes;
    private List<Block> _blocks;
    private GameState _state;
    private int _round;


    Vector2 firstPressPos;
    Vector2 secondPressPos;
    Vector2 currentSwipe;

    // BlockType'� �ekmek i�in metot
    private BlockType GetBlockTypeByValue(int value) => _types.First(t => t.Value == value);


    // AT(AchivementTrue)'nun kontrol edilmesi i�in metot
    private bool CheckAT(int value1, int value2)
    {
        if (value1 >= value2) return true;
        else return false;

    }

    // Text'in aktif olup olmad���n� kontrol etmek i�in static de�erinin direk k�yaslanabilece�i metot
    private bool CheckTextActive(int val)
    {
        if (val != 0) return true;
        else return false;
    }

    // Her seviye ba�lang�c�nda s�f�rlanmas� gereken de�i�kenlerin s�f�rlanmas�, oyunun win durumu i�in kullan�lacak AT de�i�kenlerinin belirli text aktif olmad��� s�rece true atanmas� ve son olarak state'in GenerateLevel'a �ekilmesi.
    void Start() {

        
        cpDynamic = coDynamic = ipDynamic = ioDynamic = spDynamic = soDynamic = gpDynamic = goDynamic = diaDynamic = achieveCount = 0;

        if (CheckTextActive(cpSetStatic)) cpTextSt.text = "/ " + cpSetStatic.ToString();
        else cpAT = true;

        if (CheckTextActive(coSetStatic)) coTextSt.text = "/ " + coSetStatic.ToString();
        else coAT = true;

        if (CheckTextActive(ipSetStatic)) ipTextSt.text = "/ " + ipSetStatic.ToString();
        else ipAT = true;

        if (CheckTextActive(ioSetStatic)) ioTextSt.text = "/ " + ioSetStatic.ToString();
        else ioAT = true;

        if (CheckTextActive(spSetStatic)) spTextSt.text = "/ " + spSetStatic.ToString();
        else spAT = true;

        if (CheckTextActive(soSetStatic)) soTextSt.text = "/ " + soSetStatic.ToString();
        else soAT = true;

        if (CheckTextActive(gpSetStatic)) gpTextSt.text = "/ " + gpSetStatic.ToString();
        else gpAT = true;

        if (CheckTextActive(goSetStatic)) goTextSt.text = "/ " + goSetStatic.ToString();
        else goAT = true;

        if (CheckTextActive(diaSetStatic)) diaTextSt.text = "/ " + diaSetStatic.ToString();
        else diaAT = true;


        ChangeState(GameState.GenerateLevel);
    }

    // Oyun state'lerinin ayarlanmas�.
    private void ChangeState(GameState newState) {
        _state = newState;

        switch (newState) {
            case GameState.GenerateLevel:
                GenerateGrid();
                break;
            case GameState.SpawningBlocks:
                SpawnBlocks(_round++ == 0 ? 2 : 1);
                break;
            case GameState.WaitingInput:
                break;
            case GameState.Moving:
                break;
            case GameState.Win:
                _winScreen.SetActive(true);
                Invoke(nameof(DelayedWinScreenText),1.5f);
                break;
            case GameState.Lose:
                _loseScreen.SetActive(true);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }
    }

    // Seviyeyi ge�me halindeki yaz�n�n ekranda g�r�lebilir hale getirilmesi.
    void DelayedWinScreenText() {
        _winScreenText.SetActive(true);
    }

    // B�t�n AT de�i�kenleri do�ruysa oyun durumunun Win durumuna �ekilmesi de�ilse ve oyun state'i WaitinInput'taysa girdi al�nmas�.
    void Update() {
        if (cpAT && coAT && ipAT && ioAT && spAT && soAT && gpAT && goAT && diaAT)
        {
            ChangeState(GameState.Win);
        }
        else
        {
            if (_state != GameState.WaitingInput) return;


            if (Input.GetMouseButtonDown(0))
            {
                firstPressPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            }

            if (Input.GetMouseButtonUp(0))
            {
                secondPressPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                currentSwipe = new Vector2(secondPressPos.x - firstPressPos.x, secondPressPos.y - firstPressPos.y);
                currentSwipe.Normalize();

                if (currentSwipe.y > 0 && currentSwipe.x > -0.5f && currentSwipe.x < 0.5f)
                {
                    Shift(Vector2.up);
                    Debug.Log("up swipe");
                }
                if (currentSwipe.y < 0 && currentSwipe.x > -0.5f && currentSwipe.x < 0.5f)
                {
                    Shift(Vector2.down);
                    Debug.Log("down swipe");
                }
                if (currentSwipe.x < 0 && currentSwipe.y > -0.5f && currentSwipe.y < 0.5f)
                {
                    Shift(Vector2.left);
                }
                if (currentSwipe.x > 0 && currentSwipe.y > -0.5f && currentSwipe.y < 0.5f)
                {
                    Shift(Vector2.right);
                }
            }
        } 
    }

    // Maden bloklar�n�n �zerinde duraca�� Node h�crelerinin olu�turulmas�.
    void GenerateGrid() {
        _round = 0;
        _nodes = new List<Node>();
        _blocks = new List<Block>();
        for (int x = 0; x < _width; x++) {
            for (int y = 0; y < _height; y++) {
                var node = Instantiate(_nodePrefab, new Vector2(x, y), Quaternion.identity);
                _nodes.Add(node);
            }
        }

        var center = new Vector2((float) _width /2 - 0.5f,(float) _height / 2 -0.5f);

        var board = Instantiate(_boardPrefab, center, Quaternion.identity);
        board.size = new Vector2(_width*1.03f,_height * 1.03f);

        ChangeState(GameState.SpawningBlocks);
    }

    // Bloklar�n olu�turulmas�.
    void SpawnBlocks(int amount) {

        var freeNodes = _nodes.Where(n => n.OccupiedBlock == null).OrderBy(b => Random.value).ToList();

        foreach (var node in freeNodes.Take(amount)) {
           SpawnBlock(node, 2);
        }

        if (freeNodes.Count() == 1) {
            ChangeState(GameState.Lose);
            return;
        }

        ChangeState(_blocks.Any(b => b.Value == _winCondition) ? GameState.Win : GameState.WaitingInput);
    }

    // Bloklar�n olu�turulmas� ve olu�turulurken seviyeyi ge�me ko�uluna bak�l�p, ekrandaki text'lerin g�ncellenmesi.
    void SpawnBlock(Node node, int value) {
        var block = Instantiate(_blockPrefab, node.Pos, Quaternion.identity);
        block.Init(GetBlockTypeByValue(value));
        block.SetBlock(node);
        _blocks.Add(block);

        
        switch (GetBlockTypeByValue(value).Value)
        {
            case 2:
                if (CheckTextActive(cpSetStatic))
                {
                    cpDynamic++;
                    cpTextDy.text = cpDynamic.ToString();
                    if (CheckAT(cpDynamic, cpSetStatic)) cpAT = true;
                    else cpAT = false;
                }
                break;
            case  4:
                if (CheckTextActive(coSetStatic))
                {
                    coDynamic++;
                    coTextDy.text = coDynamic.ToString();
                    if (CheckAT(coDynamic, coSetStatic)) coAT = true;
                    else coAT = false;
                }
                break;
            case 8:
                if (CheckTextActive(ipSetStatic))
                {
                    ipDynamic++;
                    ipTextDy.text = ipDynamic.ToString();
                    if (CheckAT(ipDynamic, ipSetStatic)) ipAT = true;
                    else ipAT = false;
                }
                break;
            case 16:
                if (ioSetStatic != 0)
                {
                    ioDynamic++;
                    ioTextDy.text = ioDynamic.ToString();
                    if (CheckAT(ioDynamic, ioSetStatic)) ioAT = true;
                    else ioAT = false;
                }
                break;
            case 32:
                if (CheckTextActive(spSetStatic))
                {
                    spDynamic++;
                    spTextDy.text = spDynamic.ToString();
                    if (CheckAT(spDynamic, spSetStatic)) spAT = true;
                    else spAT = false;
                }
                break;
            case 64:
                if (CheckTextActive(soSetStatic))
                {
                    soDynamic++;
                    soTextDy.text = soDynamic.ToString();
                    if (CheckAT(soDynamic, soSetStatic)) soAT = true;
                    else soAT = false;
                }
                break;
            case 128:
                if (CheckTextActive(gpSetStatic))
                {
                    gpDynamic++;
                    gpTextDy.text = gpDynamic.ToString();
                    if (CheckAT(gpDynamic, gpSetStatic)) gpAT = true;
                    else gpAT = false;
                }
                break;
            case 256:
                if (CheckTextActive(goSetStatic))
                {
                    goDynamic++;
                    goTextDy.text = goDynamic.ToString();
                    if (CheckAT(goDynamic, goSetStatic)) goAT = true;
                    else goAT = false;
                }
                break;
            case 512:
                if (CheckTextActive(diaSetStatic))
                {
                    diaDynamic++;
                    diaTextDy.text = diaDynamic.ToString();
                    if (CheckAT(diaDynamic, diaSetStatic)) diaAT = true;
                    else diaAT = false;
                }
                break;
            case 1024:
                if (CheckTextActive(diaSetStatic))
                {
                    diaDynamic+=2;
                    diaTextDy.text = diaDynamic.ToString();
                    if (CheckAT(diaDynamic, diaSetStatic)) diaAT = true;
                    else diaAT = false;
                }
                break;
            default:
                break;
        }
    }

    // Bloklar�n swpie hareketi hangi do�rultudaysa o do�rultuya y�nelik kayd�r�lmas�
    void Shift(Vector2 dir) {
        ChangeState(GameState.Moving);

        var orderedBlocks = _blocks.OrderBy(b => b.Pos.x).ThenBy(b => b.Pos.y).ToList();
        if (dir == Vector2.right || dir == Vector2.up) orderedBlocks.Reverse();

        foreach (var block in orderedBlocks) {
            var next = block.Node;
            do {
                block.SetBlock(next);

                var possibleNode = GetNodeAtPosition(next.Pos + dir);
                if (possibleNode != null) {
                    if (possibleNode.OccupiedBlock != null && possibleNode.OccupiedBlock.CanMerge(block.Value)) {
                        block.MergeBlock(possibleNode.OccupiedBlock);
                    }
                    else if (possibleNode.OccupiedBlock == null) next = possibleNode;
                }
            } while (next != block.Node);
        }

        var sequence = DOTween.Sequence();

        foreach (var block in orderedBlocks) {
            var movePoint = block.MergingBlock != null ? block.MergingBlock.Node.Pos : block.Node.Pos;

            sequence.Insert(0, block.transform.DOMove(movePoint, _travelTime).SetEase(Ease.InQuad));
        }

        sequence.OnComplete(() => {
            var mergeBlocks = orderedBlocks.Where(b => b.MergingBlock != null).ToList();
            foreach (var block in mergeBlocks) {
                MergeBlocks(block.MergingBlock,block);
            }
            //if(mergeBlocks.Any()) _source.PlayOneShot(_matchClips[Random.Range(0, _matchClips.Length)], 0.2f);
            ChangeState(GameState.SpawningBlocks);
        });

        //_source.PlayOneShot(_moveClips[Random.Range(0,_moveClips.Length)],0.2f);
    }


    // Bloklar�n birle�tirilmesi
    void MergeBlocks(Block baseBlock, Block mergingBlock) {
        var newValue = baseBlock.Value * 2;

        //Instantiate(_mergeEffectPrefab, baseBlock.Pos, Quaternion.identity);
        //Instantiate(_floatingTextPrefab, baseBlock.Pos, Quaternion.identity).Init(newValue);

        SpawnBlock(baseBlock.Node, newValue);

        RemoveBlock(baseBlock);
        RemoveBlock(mergingBlock);
    }

    // Birle�tirilen bloklar�n yok edilmesi
    void RemoveBlock(Block block) {
        _blocks.Remove(block);
        Destroy(block.gameObject);
    }

    // Node Pozisyonu alma 
    Node GetNodeAtPosition(Vector2 pos) {
        return _nodes.FirstOrDefault(n => n.Pos == pos);
    }
   
}

// Script i�erisinde s�k bir �ekilde kullan�lan BlockType'�n belirlenmesi
[Serializable]
public struct BlockType {
    public int Value;
    public Color Color;
    public Sprite newSprite;
}

//Oyun durumlar�n�n belirlenmesi
public enum GameState {
    GenerateLevel,
    SpawningBlocks,
    WaitingInput,
    Moving,
    Win,
    Lose
}