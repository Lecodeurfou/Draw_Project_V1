using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap.Unity.DetectionExamples;

public class ActiveDrawingOn : MonoBehaviour
{

    public GameObject obj;

/*    public void setLeftDetector(bool active)
    {
        if (active == true)
            GameObject.Find("PinchDetector_L").SetActive(true);
        else
            GameObject.Find("PinchDetector_L").SetActive(false);
    }*/
    public void activeTrail()
    {
        obj.GetComponent<PinchDraw>().State = 1;
    }

    public void activeCube(){
        obj.GetComponent<PinchDraw>().State = 2;
    }

    public void activeSphere(){
        obj.GetComponent<PinchDraw>().State = 3;
    }

    public void delete (){
        obj.GetComponent<PinchDraw>().State = 4;
    }

    public void drag (){
        obj.GetComponent<PinchDraw>().State = 5;
    }
    
    public void geoShaders (){
        obj.GetComponent<PinchDraw>().State = 6;
    }
    
    public void holoShaders (){
        obj.GetComponent<PinchDraw>().State = 7;
    }
    
    public void contourShaders (){
        obj.GetComponent<PinchDraw>().State = 8;
    }
    
    public void blurShaders (){
        obj.GetComponent<PinchDraw>().State = 9;
    }

    public void pickColor (){
        obj.GetComponent<PinchDraw>().State = 10;
    }

    public void movement (){
        obj.GetComponent<PinchDraw>().State = 11;
    }

}
