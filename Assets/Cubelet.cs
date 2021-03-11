using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CubeletColor { red, blue, green, orange, yellow, white };

[ExecuteInEditMode]
public class Cubelet : MonoBehaviour
{
    public GameObject colorSurface;

    public CubeletColor color;

    Dictionary<CubeletColor, Material> materialMap = new Dictionary<CubeletColor, Material>();

    private void Awake() {
        materialMap.Add(CubeletColor.red, Resources.Load<Material>("Materials/Red"));
        materialMap.Add(CubeletColor.blue, Resources.Load<Material>("Materials/Blue"));
    }

    private void OnInspectorGUI() {
        
        switch(color){
            case CubeletColor.red:
            colorSurface.GetComponent<Renderer>().material = materialMap[CubeletColor.red];
            break;
            case CubeletColor.blue:
            colorSurface.GetComponent<Renderer>().material = materialMap[CubeletColor.blue];
            break;
            case CubeletColor.green:

            break;
            case CubeletColor.orange:

            break;
            case CubeletColor.yellow:

            break;
            case CubeletColor.white:

            break;
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
}
