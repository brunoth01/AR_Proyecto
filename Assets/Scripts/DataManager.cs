using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    [SerializeField] private List<Items> items = new List<Items>();
    [SerializeField] private GameObject buttonContainer;
    [SerializeField] private ItemButtonManager itemButtonManager;

    void Start()
    {
        GameManager.Instance.OnItemsMenu += CreateButtons;
    }
    private void CreateButtons()
    {
        foreach (var item in items)
        {
            ItemButtonManager itemButton;
            itemButton = Instantiate(itemButtonManager, buttonContainer.transform);
            itemButton.ItemName = item.name;
            itemButton.ItemDescription = item.ItemDescription;
            itemButton.ItemImage = item.ItemImage;
            itemButton.Item3Dmdel = item.Item3DModel;
            itemButton.name = item.name;
        }
        GameManager.Instance.OnItemsMenu -= CreateButtons;
    }
    void Update()
    {

    }
}
