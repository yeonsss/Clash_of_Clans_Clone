using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemHandler : MonoBehaviour
{
    public int CardCode { get; set; }
    
    private void Start()
    { 
        var objImage = transform.Find("Image");
        var deleteButton = transform.Find("DeleteButton");
        var nameText = transform.Find("Name");
        
        if (DataManager.instance.cardDict.TryGetValue(CardCode, out var info) == true)
        {
            nameText.GetComponent<TMP_Text>().text = info.Name;
        }
        
        // 이미지가 있다면 이미지 업데이트
        

        deleteButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            GameManager.instance.deck.RemoveCard(CardCode);
            Destroy(gameObject);
        });
    }
}
