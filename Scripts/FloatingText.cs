using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class FloatingText : MonoBehaviour {
    [SerializeField] private TextMeshPro _text;
    [SerializeField] private float _fadeTime = 1;
    public void Init(int value) {
        // Parametre olarak al�nan de�er stringe �evrilip text'e atan�yor.
        _text.text = value.ToString();

        // DOTween sequenc'i olu�turuluyor.
        var sequence = DOTween.Sequence();

        // Textin olu�turulup fadeTime kadar zamanda hareket etmesini ve kaybolmas�n� sa�l�yor.
        sequence.Insert(0, _text.DOFade(0, _fadeTime));
        sequence.Insert(0, _text.transform.DOMove(_text.transform.position + Vector3.up, _fadeTime));
        
        // Sequence tamamland���nda objenin oyunda g�z�kmemesini sa�l�yor.
        sequence.OnComplete(() => gameObject.SetActive(false));

        //Oyunun performans�n� daha k�t� etkiliyor. Fakat oyun �al��t�r�l�rken unity'de Hierarchy'deki karma�ay� �nl�yor.
        //sequence.OnComplete(() => Destroy(gameObject));  
    }
}
