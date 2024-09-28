using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemButtonManager : MonoBehaviour
{
    private string itemName;
    private string itemDescription;
    private Sprite itemImage;
    private GameObject item3DModel;

    //private ARInteractionManager interactionManager; //Para integrar el Pointer al Modelo3D para asignar posicion.



    public string ItemName
    {
        set
        {
            itemName = value;
        }
    }

    public string ItemDescription { set => itemDescription = value; }
    public Sprite ItemImage { set => itemImage = value; }
    public GameObject Item3Dmdel { set => item3DModel = value; }



    // Start is called before the first frame update
    void Start()
    {
        transform.GetChild(0).GetComponent<Text>().text = itemName;
        transform.GetChild(1).GetComponent<RawImage>().texture = itemImage.texture;
        transform.GetChild(2).GetComponent<Text>().text = itemDescription;

        var button = GetComponent<Button>();
        button.onClick.AddListener(GameManager.Instance.ARPosition); //Que cuando eliga un item llame al evento ARPosition
        button.onClick.AddListener(Create3DModel);

        //Aqui defino el Pointer con el Modelo.
        //interactionManager = FindObjectOfType<ARInteractionManager>();
    }

    private void Create3DModel()
    {
        Instantiate(item3DModel);
        //como quiero que se asigne el modelo3D en el interactionManager. debo modificar esta parte.
        //interactionManager.Item3DModel = Instantiate(item3DModel);
    }
    // Update is called once per frame
    void Update()
    {

    }
}