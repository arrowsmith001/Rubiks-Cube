using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RubiksFace{
    F,B,U,D,L,R
}
public enum RotationDirection{
    Clockwise,CounterClockwise
}

public class RubiksCubeScript : MonoBehaviour
{
    public GameObject cubesRoot;
    public GameObject facesRoot;
    public GameObject positionsRoot;

    // ONE TIME

    // Lookup for key positions (facing outward on Z)
    Dictionary<String, Vector3> positionMap = new Dictionary<String, Vector3>();

    // Name lookup for center pieces
    Dictionary<String, Transform> centerCubesMap = new Dictionary<String, Transform>();


    // NEED TO BE REPEATED

    // Maps perspective-relative face positions to the actual faces
    Dictionary<RubiksFace, Transform> faceCodeToFaceMap = new Dictionary<RubiksFace, Transform>();

    // Maps actual faces to center pieces
    Dictionary<Transform, Transform> faceCentersMap = new Dictionary<Transform, Transform>();

    // Maps center pieces to their turnable layers
    Dictionary<Transform, Layer> centersToLayersMap = new Dictionary<Transform, Layer>();



    bool isRotating = false;
    Quaternion targetRotation = Quaternion.identity;


    // Start is called before the first frame update
    void Start()
    {
        AddPositions();

        AddCenterPieces();
    }

    void AddPositions(){
        foreach(Transform position in positionsRoot.transform){
            positionMap.Add(position.gameObject.name, position.position);
        }
    }

    void AddCenterPieces(){
        foreach(Transform piece in cubesRoot.transform){
            if(piece.name.StartsWith("Center")){
                centerCubesMap.Add(piece.name, piece);
            }
        }
    }


    
    // Update is called once per frame
    void Update()
    {
        AssignFaces();

        AssignPieces();

        AddLayers();
    }

    void AssignFaces(){
        faceCodeToFaceMap.Clear();
        RaycastHit hit;    
    
        // Raycast from camera facing direction into world space, determines front face
        if(Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit))
        {
            faceCodeToFaceMap.Add(RubiksFace.F, hit.collider.transform);
        }

        // Raycast backwards from center, determines back
        if(Physics.Raycast(positionMap["Center"] + Camera.main.transform.forward * 5, -Camera.main.transform.forward, out hit))
        {
            faceCodeToFaceMap.Add(RubiksFace.B, hit.collider.transform);
        }

        // Raycast upwards from center, determines top
        if(Physics.Raycast(positionMap["Center"] + Camera.main.transform.up * 5, -Camera.main.transform.up, out hit))
        {
            faceCodeToFaceMap.Add(RubiksFace.U, hit.collider.transform);
        
        }

        // Raycast downwards from center, determines bottom
        if(Physics.Raycast(positionMap["Center"] - Camera.main.transform.up * 5, Camera.main.transform.up, out hit))
        {
            faceCodeToFaceMap.Add(RubiksFace.D, hit.collider.transform);
        
        }
        // Raycast rightwards from center, determines right
        if(Physics.Raycast(positionMap["Center"] + Camera.main.transform.right * 5, -Camera.main.transform.right, out hit))
        {
            faceCodeToFaceMap.Add(RubiksFace.R, hit.collider.transform);
        
        }
        // Raycast leftwards from center, determines left
        if(Physics.Raycast(positionMap["Center"] - Camera.main.transform.right * 5, Camera.main.transform.right, out hit))
        {
            faceCodeToFaceMap.Add(RubiksFace.L, hit.collider.transform);
        
        }

        foreach(Transform face in facesRoot.transform){
            face.gameObject.GetComponent<MeshRenderer>().enabled = (face == faceCodeToFaceMap[RubiksFace.F]);
        }
    }

    void AssignPieces(){

        faceCentersMap.Clear();

        foreach(Transform piece in centerCubesMap.Values){
            
                // Find face affinity
                RaycastHit hit;    

                Vector3 fromCenter = piece.position - positionMap["Center"];
    
                // Finds associated face with this center cube
                if(Physics.Raycast(positionMap["Center"] + 5*fromCenter, -fromCenter, out hit))
                {
                    faceCentersMap.Add(hit.collider.transform, piece);
                }

            
        }
    }

    bool CompareFloats(float a, float b, float tol){
        return Math.Abs(a-b) < tol;
    }

    void AddLayers(){

        centersToLayersMap.Clear();
        
        foreach(Transform piece in centerCubesMap.Values){
            centersToLayersMap.Add(piece, new Layer(piece));
        }

        float tol = 0.05f;

        foreach(Transform piece in cubesRoot.transform){
            if(piece.name.StartsWith("Temp")) continue;

            if(CompareFloats(piece.localPosition.x, 1, tol)){
                centersToLayersMap[centerCubesMap["CenterRight"]].AddOrbiter(piece);
            }

            if(CompareFloats(piece.localPosition.x, -1, tol)){
                centersToLayersMap[centerCubesMap["CenterLeft"]].AddOrbiter(piece);
            }

            if(CompareFloats(piece.localPosition.y, 1, tol)){
                
                centersToLayersMap[centerCubesMap["CenterUp"]].AddOrbiter(piece);
            }
            if(CompareFloats(piece.localPosition.y, -1, tol)){
                
                centersToLayersMap[centerCubesMap["CenterDown"]].AddOrbiter(piece);
            }
            if(CompareFloats(piece.localPosition.z, 1, tol)){
                
                centersToLayersMap[centerCubesMap["CenterFront"]].AddOrbiter(piece);
            }
            if(CompareFloats(piece.localPosition.z, -1, tol)){
                
                centersToLayersMap[centerCubesMap["CenterBack"]].AddOrbiter(piece);
            }


        }
    }

    Layer layer;
    Transform face;
    float layerTurningTarget = 90;
    Vector3 layerTurningPoint;
    Vector3 layerTurningAxis;
    float progress = 0;
    bool isTurning = false;

    private void LateUpdate() {

        RotateCamera();

        ArrowKeys();

        //if(Input.GetKeyDown(KeyCode.Space)) FaceTurnTest();

        // Test: Turn front layer CW/CCW on mouse click
        if(Input.GetMouseButtonDown(0) && !isTurning){

            face = faceCodeToFaceMap[RubiksFace.F];
            var center = faceCentersMap[face];
            layer = centersToLayersMap[center];
            layerTurningPoint = layer.GetCenter().position;
            layerTurningAxis = face.forward;

            StartCoroutine(Rotate(RotationDirection.Clockwise));
        }

        if(Input.GetMouseButtonDown(1) && !isTurning){

            face = faceCodeToFaceMap[RubiksFace.F];
            var center = faceCentersMap[face];
            layer = centersToLayersMap[center];
            layerTurningPoint = layer.GetCenter().position;
            layerTurningAxis = face.forward;

            StartCoroutine(Rotate(RotationDirection.CounterClockwise));
        }


        
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 10*Time.deltaTime);
    }

    Quaternion initialRotation;
    IEnumerator Rotate(RotationDirection direction)
    {   
        isTurning = true;
        progress = 0;

        // Find temporary game object
        Transform temp = cubesRoot.transform.Find("Temp");

        // Figure out target rotation
        temp.position = layerTurningPoint;
        temp.rotation = face.rotation;
        temp.RotateAround(layerTurningPoint, layerTurningAxis, 
            direction == RotationDirection.Clockwise ? 90 : -90);
            
        Quaternion targetRot = temp.rotation;

        // Now use temp
        temp.position = layerTurningPoint;
        temp.rotation = face.rotation;
        initialRotation = temp.rotation;

        foreach(var piece in layer.GetOrbiters())
        {
            piece.SetParent(temp);
        }


   

        while(progress < 1)
        {
            progress += Time.deltaTime;
            temp.rotation = Quaternion.Lerp(initialRotation, targetRot, progress);
            yield return null;
        }


        foreach(var piece in layer.GetOrbiters())
        {
            piece.SetParent(cubesRoot.transform);
        }

        //layer.SnapPiecesToGrid();

        progress = 0;
        isTurning = false;

    }

    void FaceTurnTest(){
        int i = 0;
        GameObject center = null;
        List<GameObject> cubes = new List<GameObject>();
        foreach(Transform cube in cubesRoot.transform){
            if(i < 9){
                if(i == 0) center = cube.gameObject;
                cubes.Add(cube.gameObject);
            }
            i++;
        }

        foreach(GameObject cube in cubes){
            cube.transform.RotateAround(
                center.transform.position, Vector3.up, 90
            );
        }

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
        float speed = 10;

            if(Input.GetKey(KeyCode.Space))
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
