using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARInteractionManager : MonoBehaviour
{
    [SerializeField] private Camera aRCamera;  //Para la Camara
    private ARRaycastManager aRRaycastManager; //Para el ArrayCast

    private List<ARRaycastHit> hits = new List<ARRaycastHit>(); //Lista de Hits captados

    private GameObject aRPointer; // Para el Pointer
    private GameObject item3DModel;  //Para el Modelo 3D

    private GameObject itemSelected;//Variable para asignar un modelo3D temporal

    private bool isInitalPosition;
    private bool isOverUI;//Boleano que permitira validar la condicion de que no se toco la interfaz

    private bool isOver3DModel;

    private Vector2 initialTouchPos;
    public GameObject Item3DModel
    {
        set
        {
            item3DModel = value;
            item3DModel.transform.position = aRPointer.transform.position; //Aqui defino que la posicion de mi objeto 3D sera la del Pointer.  
            item3DModel.transform.parent = aRPointer.transform; //Aqui defino que el modelo3D sera hijo de Pointer.
            isInitalPosition = true;
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        aRPointer = transform.GetChild(0).gameObject;
        aRRaycastManager = FindObjectOfType<ARRaycastManager>();
        GameManager.Instance.OnMainMenu += SetItemPosition; // Funcion para fijar el modelo 3D
    }

    // Update is called once per frame
    void Update()
    {
        if (isInitalPosition)  //Aqui definimos yu damos la posicion inicial al modelo 3D creado 
        {
            Debug.Log("isInitalPosition" + isInitalPosition);
            Vector2 middlePointScreen = new Vector2(Screen.width / 2, Screen.height / 2); //Para posicionarnos en la mitad de la pantalla.

            aRRaycastManager.Raycast(middlePointScreen, hits, TrackableType.Planes); // Aqui pasamos la posicion al Arraycastmanager.
            if (hits.Count > 0)
            {
                transform.position = hits[0].pose.position;
                transform.rotation = hits[0].pose.rotation;
                aRPointer.SetActive(true);
                isInitalPosition = false;
            }
        }


        if (Input.touchCount > 0) //Para validar si se toco la pantalla
        {
            Debug.Log("touch " + Input.touchCount);
            Touch touchOne = Input.GetTouch(0); //En esta variable asigno el touch que se realizo
            if (touchOne.phase == TouchPhase.Began)
            {   //Validar que el Touch no sea un boton de la interfaz, con el fin de evitar errores
                var touchPosition = touchOne.position;
                isOverUI = isTapOverUI(touchPosition); //Funcion para validar si no se esta tocando la interfaz.
                isOver3DModel = isTapOver3DModel(touchPosition); //Para validar si el touch ha sido sobre el modelo 3D
            }

            //Codigo para mover el pointer y definir donde colocar el Objeto 
            if (touchOne.phase == TouchPhase.Moved) //Validar el movimiento de la pantalla sea dentro de los planos
            {
                Debug.Log("touch move " + Input.touchCount);
                if (aRRaycastManager.Raycast(touchOne.position, hits, TrackableType.Planes))//Verificar que esos movimientos sea dentro de los planos que se implementaron dentro de realidad aumentada.
                {
                    Pose hitPose = hits[0].pose;
                    if (!isOverUI && isOver3DModel)
                    {
                        transform.position = hitPose.position; //Aqui es donde permitimos mover el modelo.
                    }
                }
            }

            //Codigo para Rotar los Objetos en 3D //para ello el sujeto debe tocar con dos dedos ya sea a la derecha o a la izquierda hacer rotar los objetos.
            if (Input.touchCount == 2) //Aca valido los dos inputs
            {
                Touch touchTwo = Input.GetTouch(1); //Registro el touch
                if (touchOne.phase == TouchPhase.Began || touchTwo.phase == TouchPhase.Began) // Aca valido que el touch ha iniciado antes de realizar la accion. 
                {
                    initialTouchPos = touchTwo.position - touchOne.position;
                }

                //Ahora necesito validar si uno de esos dedos se esta moviendo.
                if (touchOne.phase == TouchPhase.Moved || touchTwo.phase == TouchPhase.Moved)
                {
                    //si es asi entonces debo registrar su nueva rotacion.
                    Vector2 currentTouchPos = touchTwo.position - touchOne.position;
                    float angle = Vector2.SignedAngle(initialTouchPos, currentTouchPos); //aca tengo establecido la rotaci�n.
                    //ahora debo asignar la rotacion al modelo 3D
                    item3DModel.transform.rotation = Quaternion.Euler(0, item3DModel.transform.eulerAngles.y - angle, 0);
                    initialTouchPos = currentTouchPos;
                }
            }

            if (isOver3DModel && item3DModel == null && !isOverUI) // validamos que el touch sea sobre el modelo 3D, que item3Dmodel no tenga ningun modelo selccionado
                                                                   // y por ultimo el touch no sea en la interfaz
            {
                GameManager.Instance.ARPosition();
                item3DModel = itemSelected;
                itemSelected = null;
                aRPointer.SetActive(true);
                transform.position = item3DModel.transform.position;
                item3DModel.transform.parent = aRPointer.transform;
            }
        }

    }

    private bool isTapOver3DModel(Vector2 touchPosition)
    {
        Ray ray = aRCamera.ScreenPointToRay(touchPosition); //
        if (Physics.Raycast(ray, out RaycastHit hit3DModel)) //Aca utilizamos para validar si el phisic a tocado un collider
        {
            if (hit3DModel.collider.CompareTag("Items"))
            {
                itemSelected = hit3DModel.transform.gameObject; //Para asignar a la variable el Objeto seleccionado.
                return true;
            }
        }
        return false;
    }

    private bool isTapOverUI(Vector2 touchPosition)
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = new Vector2(touchPosition.x, touchPosition.y);
        List<RaycastResult> result = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, result);
        return result.Count > 0;
    }

    private void SetItemPosition()
    {
        if (item3DModel != null)
        {
            item3DModel.transform.parent = null;
            aRPointer.SetActive(false); //desactivo el pointer una vez haya sido fijado el modelo.
            item3DModel = null; //limpio el Objeto
        }
    }
    public void DeleteItem()
    {
        Destroy(item3DModel);
        aRPointer.SetActive(false);
        GameManager.Instance.MainMenu(); //indico que llame a la interfaz luego de eliminar.
    }

}