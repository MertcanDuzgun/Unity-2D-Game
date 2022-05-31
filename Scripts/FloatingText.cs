using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class FloatingText : MonoBehaviour {
    [SerializeField] private TextMeshPro _text;
    [SerializeField] private float _fadeTime = 1;
    public void Init(int value) {
        // Parametre olarak alýnan deðer stringe çevrilip text'e atanýyor.
        _text.text = value.ToString();

        // DOTween sequenc'i oluþturuluyor.
        var sequence = DOTween.Sequence();

        // Textin oluþturulup fadeTime kadar zamanda hareket etmesini ve kaybolmasýný saðlýyor.
        sequence.Insert(0, _text.DOFade(0, _fadeTime));
        sequence.Insert(0, _text.transform.DOMove(_text.transform.position + Vector3.up, _fadeTime));
        
        // Sequence tamamlandýðýnda objenin oyunda gözükmemesini saðlýyor.
        sequence.OnComplete(() => gameObject.SetActive(false));

        //Oyunun performansýný daha kötü etkiliyor. Fakat oyun çalýþtýrýlýrken unity'de Hierarchy'deki karmaþayý önlüyor.
        //sequence.OnComplete(() => Destroy(gameObject));  
    }
}
