using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RubiksCubeScript : MonoBehaviour
{
    public GameObject cubesRoot;
    public GameObject facesRoot;

    GameObject activeFace;

    bool isRotating = false;
    Quaternion targetRotation = Quaternion.identity;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        if(Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit))
        {
            activeFace = hit.collider.gameObject;
        }
    

    }

    private void LateUpdate() {


        foreach(Transform face in facesRoot.transform){
            face.gameObject.GetComponent<MeshRenderer>().enabled = (face.gameObject == activeFace);
        }

        RotateCamera();

        ArrowKeys();

        
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 10*Time.deltaTime);
    }

    void ArrowKeys(){

        if(Input.GetKeyDown(KeyCode.LeftArrow)){
            targetRotation = Quaternion.Euler(targetRotation.eulerAngles.x, targetRotation.eulerAngles.y - 90,  targetRotation.eulerAngles.z);


            
        }else if(Input.GetKeyDown(KeyCode.RightArrow)){
            targetRotation = Quaternion.Euler(targetRotation.eulerAngles.x, targetRotation.eulerAngles.y + 90,  targetRotation.eulerAngles.z);

            
        }else if(Input.GetKeyDown(KeyCode.UpArrow)){
            
            targetRotation = Quaternion.Euler(targetRotation.eulerAngles.x, targetRotation.eulerAngles.y,  targetRotation.eulerAngles.z + 90);

        }else if(Input.GetKeyDown(KeyCode.DownArrow)){
            
            targetRotation = Quaternion.Euler(targetRotation.eulerAngles.x, targetRotation.eulerAngles.y,  targetRotation.eulerAngles.z - 90);
        }

    }

    void RotateCamera()
    {
        float speed = 5;

            if(Input.GetMouseButton(1))
            {
            Camera.main.transform.RotateAround(transform.position, 
                                            Vector3.up,
                                            Input.GetAxis("Mouse X")*speed);

            Camera.main.transform.RotateAround(transform.position, 
                                            Camera.main.transform.right,
                                            -Input.GetAxis("Mouse Y")*speed);
            } 
            else{

            } 

    }
}
