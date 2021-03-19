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

    public GameObject labels;

    // ONE TIME

    // Lookup for key positions (facing outward on Z)
    Dictionary<String, Vector3> positionMap = new Dictionary<String, Vector3>();

    // Name lookup for center pieces
    Dictionary<String, Transform> centerCubesMap = new Dictionary<String, Transform>();
    Dictionary<Transform, LabelScript> labelsMap = new Dictionary<Transform, LabelScript>();


    // NEED TO BE REPEATED

    // Maps perspective-relative face positions to the actual faces
    Dictionary<RubiksFace, Transform> faceCodeToFaceMap = new Dictionary<RubiksFace, Transform>();

    // Maps actual faces to center pieces
    Dictionary<Transform, Transform> faceCentersMap = new Dictionary<Transform, Transform>();

    // Maps center pieces to their turnable layers
    Dictionary<Transform, Layer> centersToLayersMap = new Dictionary<Transform, Layer>();


    // FIELDS
    public float cameraRotateSpeed = 10;
    public float cubeRotationSpeed = 10;
    public float layerTurnSpeed = 10;


    // RESOURCES
    Material faceHighlightBlue;
    Material faceHighlightOrange;


    bool isCubeRotating = false;
    Quaternion targetRotation = Quaternion.identity;


    // Start is called before the first frame update
    void Start()
    {
        AddMaterials();

        AddPositions();

        AddCenterPieces();

        AssignLabels();
    }

    private void AssignLabels()
    {
        foreach(Transform t in facesRoot.transform){
            LabelScript ls = labels.transform.Find(t.name).GetComponent<LabelScript>();
            ls.SetTransform(t);
            labelsMap.Add(t, ls);
            Debug.Log("Added " + t.name + " to " + ls.gameObject.transform.name);
        }
    }

    void AddMaterials(){
        faceHighlightBlue = Resources.Load<Material>("Materials/FaceHighlightBlue");
        faceHighlightOrange = Resources.Load<Material>("Materials/FaceHighlightOrange");
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


    Transform frontFace;
    Transform selectedFace;
    bool mustUpdate = false;
    
    // Update is called once per frame
    void Update()
    {
        // Check that front face is still the same, otherwise update all existing information
        RaycastHit hit;    
        if(Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit))
        {
            if(hit.collider.gameObject != frontFace){

                frontFace = hit.collider.gameObject.transform;
                mustUpdate = true;
            }
        }

        if(mustUpdate && !isTurning){
            
            AssignFaces();

            AssignPieces();

            AddLayers(); 
            
            int i = 0;
            foreach(var layer in centersToLayersMap.Values){
                //Debug.Log(i + " : " + layer.GetPieceCount());
            }

            mustUpdate = false;
        }

        FaceHighlight();


    }

    
    private void LateUpdate() {

        RotateCamera();

        ArrowKeys();

        //if(Input.GetKeyDown(KeyCode.Space)) FaceTurnTest();

        Mouse();

        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, cubeRotationSpeed*Time.deltaTime);
    }

    void AssignFaces(){
        faceCodeToFaceMap.Clear();
        RaycastHit hit;    
    
        faceCodeToFaceMap.Add(RubiksFace.F, frontFace.transform);
        labelsMap[frontFace].SetText(RubiksFace.F.ToString());

        // Raycast backwards from center, determines back
        if(Physics.Raycast(positionMap["Center"] + Camera.main.transform.forward * 5, -Camera.main.transform.forward, out hit))
        {
            RubiksFace thisFaceCode = RubiksFace.B;
            Transform thisFace = hit.collider.transform;

            faceCodeToFaceMap.Add(thisFaceCode, thisFace);
            labelsMap[thisFace].SetText(thisFaceCode.ToString());
        }

        // Raycast upwards from center, determines top
        if(Physics.Raycast(positionMap["Center"] + Camera.main.transform.up * 5, -Camera.main.transform.up, out hit))
        {
            RubiksFace thisFaceCode = RubiksFace.U;
            Transform thisFace = hit.collider.transform;

            faceCodeToFaceMap.Add(thisFaceCode, thisFace);
            labelsMap[thisFace].SetText(thisFaceCode.ToString());
        }

        // Raycast downwards from center, determines bottom
        if(Physics.Raycast(positionMap["Center"] - Camera.main.transform.up * 5, Camera.main.transform.up, out hit))
        {
            RubiksFace thisFaceCode = RubiksFace.D;
            Transform thisFace = hit.collider.transform;

            faceCodeToFaceMap.Add(thisFaceCode, thisFace);
            labelsMap[thisFace].SetText(thisFaceCode.ToString());
        
        }
        // Raycast rightwards from center, determines right
        if(Physics.Raycast(positionMap["Center"] + Camera.main.transform.right * 5, -Camera.main.transform.right, out hit))
        {
            RubiksFace thisFaceCode = RubiksFace.R;
            Transform thisFace = hit.collider.transform;

            faceCodeToFaceMap.Add(thisFaceCode, thisFace);
            labelsMap[thisFace].SetText(thisFaceCode.ToString());
        
        }
        // Raycast leftwards from center, determines left
        if(Physics.Raycast(positionMap["Center"] - Camera.main.transform.right * 5, Camera.main.transform.right, out hit))
        {
            RubiksFace thisFaceCode = RubiksFace.L;
            Transform thisFace = hit.collider.transform;

            faceCodeToFaceMap.Add(thisFaceCode, thisFace);
            labelsMap[thisFace].SetText(thisFaceCode.ToString());
        
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

    bool isTurning = false;


    List<KeyCode> keyCodes = new List<KeyCode>(new KeyCode[] {KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D});
    List<KeyCode> keyPressLog = new List<KeyCode>();

    void FaceHighlight()
    {
        foreach(Transform face in facesRoot.transform){
            face.gameObject.GetComponent<MeshRenderer>().enabled = false;
        }

        foreach(KeyCode k in keyCodes){
            if(Input.GetKeyDown(k)){
                if(!keyPressLog.Contains(k)){
                    keyPressLog.Add(k);
                }
            }else if(Input.GetKeyUp(k)){
                if(keyPressLog.Contains(k)){
                    keyPressLog.Remove(k);
                }   
            }
        }

        if(keyPressLog.Count == 0){
            selectedFace = frontFace;
        }else{
            switch(keyPressLog[keyPressLog.Count-1]){

                case KeyCode.W:
                selectedFace = faceCodeToFaceMap[RubiksFace.U];
                break;

                case KeyCode.A:
                selectedFace = faceCodeToFaceMap[RubiksFace.L];
                break;

                case KeyCode.S:
                selectedFace = faceCodeToFaceMap[RubiksFace.D];
                break;

                case KeyCode.D:
                selectedFace = faceCodeToFaceMap[RubiksFace.R];
                break;
            }
        }

        frontFace.gameObject.GetComponent<MeshRenderer>().enabled = true;
        frontFace.gameObject.GetComponent<MeshRenderer>().sharedMaterial = faceHighlightBlue;

        if(selectedFace != frontFace){
            selectedFace.gameObject.GetComponent<MeshRenderer>().enabled = true;
            selectedFace.gameObject.GetComponent<MeshRenderer>().sharedMaterial = faceHighlightOrange;
        }


    }

    void Mouse(){
        
        if(mustUpdate || isTurning) return;

        // Test: Turn front layer CW/CCW on mouse click
        if(Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)){


            var face = selectedFace;
            var center = faceCentersMap[face];
            var layer = centersToLayersMap[center];
            var layerTurningPoint = layer.GetCenter().position;
            var layerTurningAxis = face.forward;

            var direction =  Input.GetMouseButtonDown(0) ? RotationDirection.Clockwise : RotationDirection.CounterClockwise;

            LayerTurnData turnData = new LayerTurnData(direction, layer, layerTurningPoint, layerTurningAxis, face);

            StartCoroutine(Rotate(turnData));
        }
    }

    Interpolator layerTurnInterpolator = new AccelerateDecelerateInterpolator();
    IEnumerator Rotate(LayerTurnData data)
    {   
        isTurning = true;
        float progress = 0;

        // Find temporary game object
        Transform temp = cubesRoot.transform.Find("Temp");

        // Figure out target rotation
        temp.position = data.layerTurningPoint;
        temp.rotation = data.face.rotation;
        temp.RotateAround(data.layerTurningPoint, data.layerTurningAxis, 
            data.direction == RotationDirection.Clockwise ? 90 : -90);
            
        Quaternion targetRot = temp.rotation;

        // Now use temp
        temp.position = data.layerTurningPoint;
        temp.rotation = data.face.rotation;

        Quaternion initialRotation = temp.rotation;

        foreach(var piece in data.layer.GetOrbiters())
        {
            piece.SetParent(temp);
        }

        while(progress < 1)
        {
            temp.rotation = Quaternion.Lerp(initialRotation, targetRot, layerTurnInterpolator.getValue(progress));
            progress += layerTurnSpeed*Time.deltaTime;
            yield return null;
        }

        temp.rotation = targetRot;


        foreach(var piece in data.layer.GetOrbiters())
        {
            piece.SetParent(cubesRoot.transform);
        }

        //layer.SnapPiecesToGrid();

        progress = 0;
        isTurning = false;

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

            if(Input.GetKey(KeyCode.Space))
            {
                Camera.main.transform.RotateAround(transform.position, 
                                                Vector3.up,
                                                Input.GetAxis("Mouse X")*cameraRotateSpeed);

                Camera.main.transform.RotateAround(transform.position, 
                                                Camera.main.transform.right,
                                                -Input.GetAxis("Mouse Y")*cameraRotateSpeed);
            } 
            else{

            } 

    }
}
