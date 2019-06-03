using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap.Unity.DetectionExamples;

public class drag : MonoBehaviour
{
    // Start is called before the first frame update
	public GameObject obj;
    public void OnTriggerEnter (Collider col)
    {
    	if ((col.gameObject.CompareTag("Cube") || col.gameObject.CompareTag("Sphere")) && obj.GetComponent<PinchDraw>().isDrag == 1 && obj.GetComponent<PinchDraw>().State == 5){
    		obj.GetComponent<PinchDraw>()._selected = col.name;
    	}
    }
}
