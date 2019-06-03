using UnityEngine;
using System.Collections;
using Leap.Unity.DetectionExamples;

public class Collision : MonoBehaviour
{
	public GameObject obj;

	void OnTriggerEnter(Collider col)
	{
		if ((col.gameObject.CompareTag("Cube")  || col.gameObject.CompareTag("Sphere")) && obj.GetComponent<PinchDraw>().State == 4){
			Destroy(col.gameObject);
		}

		if ((col.gameObject.CompareTag("Cube")  || col.gameObject.CompareTag("Sphere")) && obj.GetComponent<PinchDraw>().State == 6){
			Material material = new Material(Shader.Find("Hand_Make/GeowireShader"));
			//material.color = Color.green;
			col.GetComponent<Renderer>().material = material;
			col.GetComponent<Renderer>().material.SetColor("_Color", GameObject.Find("Picker").GetComponent<ColorPicker>().CurrentColor);
		}
		
		if ((col.gameObject.CompareTag("Cube")  || col.gameObject.CompareTag("Sphere")) && obj.GetComponent<PinchDraw>().State == 7){
			Material material = new Material(Shader.Find("Hand_Make/HoloSurfaceShader"));
			//material.color = Color.green;
			col.GetComponent<Renderer>().material = material;
			col.GetComponent<Renderer>().material.SetColor("_Color", GameObject.Find("Picker").GetComponent<ColorPicker>().CurrentColor);
		}
		
		if ((col.gameObject.CompareTag("Cube")  || col.gameObject.CompareTag("Sphere")) && obj.GetComponent<PinchDraw>().State == 8){
			Material material = new Material(Shader.Find("Hand_Make/Contour"));
			//material.color = Color.white;
			col.GetComponent<Renderer>().material = material;
			col.GetComponent<Renderer>().material.SetColor("_Color", GameObject.Find("Picker").GetComponent<ColorPicker>().CurrentColor);
		}
		
		if ((col.gameObject.CompareTag("Cube")  || col.gameObject.CompareTag("Sphere")) && obj.GetComponent<PinchDraw>().State == 9){
			Material material = new Material(Shader.Find("Hand_Make/BlurShader"));
			//material.color = Color.grey;
			col.GetComponent<Renderer>().material = material;
			col.GetComponent<Renderer>().material.SetColor("_Color", GameObject.Find("Picker").GetComponent<ColorPicker>().CurrentColor);
		}
		
		if ((col.gameObject.CompareTag("Cube")  || col.gameObject.CompareTag("Sphere"))&& obj.GetComponent<PinchDraw>().State == 10){
			col.GetComponent<Renderer>().material.SetColor("_Color", GameObject.Find("Picker").GetComponent<ColorPicker>().CurrentColor);
		}
		
		
/*        if (col.gameObject.tag == "colorPicker"){
	        col.transform.position = this.transform.position;
        }*/

	}
}