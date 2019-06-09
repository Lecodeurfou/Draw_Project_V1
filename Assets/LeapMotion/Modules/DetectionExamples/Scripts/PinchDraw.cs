/******************************************************************************
 * Copyright (C) Leap Motion, Inc. 2011-2017.                                 *
 * Leap Motion proprietary and  confidential.                                 *
 *                                                                            *
 * Use subject to the terms of the Leap Motion SDK Agreement available at     *
 * https://developer.leapmotion.com/sdk_agreement, or another agreement       *
 * between Leap Motion and you, your company or other organization.           *
 ******************************************************************************/

using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
using Hover.Core.Renderers.Shapes.Arc;
using Hover.Core.Items.Types;
using SimpleJSON;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.Experimental.PlayerLoop;

//[RequireComponent(typeof(BoxSlider), typeof(RawImage)), ExecuteInEditMode()]

namespace Leap.Unity.DetectionExamples {

  public class PinchDraw : MonoBehaviour
  {

    [Tooltip("Each pinch detector can draw one line at a time.")] [SerializeField]
    private PinchDetector[] _pinchDetectors;

    private PinchDetector2[] _pinchDetectors2;
    [SerializeField] private Material _material;
    [SerializeField] private GameObject buttonprefab;

    [SerializeField] private Color _drawColor;

    [SerializeField] private float _smoothingDelay = 0.01f;

    [SerializeField] private float _drawRadius = 0.002f;

    [SerializeField] private int _drawResolution = 8;

    [SerializeField] private float _minSegmentLength = 0.005f;

    public string _selected;
    public int isDrag = 0;
    public float curd;
    public float curt;
    private int pHand = 0;
    bool isMoving = false;
    float myx = 0;
    float myy = 0;
    float my2 = 0;
    bool isTurning = false;
    private GameObject pinchL;
    private GameObject rightPinch;
    private GameObject cam;
    float asPinchPosition = 0f;
    float position = 0;
    private GameObject RIX;
    public List<Vector3> ringList;
    public List<int> loadRing;
    public List<List<Vector3>> loadList;
    public GameObject saveTextObj;
    public GameObject loadTextObj;
    public GameObject loadDel;
    public GameObject loadCont;
    private List<GameObject> ButtonsLoad;
    private List<GameObject> ButtonsDel;
    private GameObject DJBiom;

    
    private float dist;
    public static int _line;

    public int Line
    {
      get { return _line; }
      set { _line = value; }
    }

    private GameObject _slider; // get color box
    private GameObject _slider2;

    //Pile des objets pouvant être cancel
    public static List<int> zTab;

    public List<int> Ztab
    {
      get { return zTab; }
      set { zTab = value; }
    }

    //Pile des objets pouvant être décancel
    public static List<int> yTab;

    public List<int> Ytab
    {
      get { return yTab; }
      set { yTab = value; }
    }

    // Permet d'acceder à la valeur du slider
    public GameObject arcValue;

    // Permet d'acceder à la valeur du slider
    public static GameObject arcValueU;

    private int state;

    public int State
    {
      get { return state; }
      set { state = value; }
    }


    private DrawState[] _drawStates;
    private DrawState[] _drawStates2;

    public int prevState = 1;
    public Color DrawColor
    {
      get { return _drawColor; }
      set { _drawColor = value; }
    }

    public float DrawRadius
    {
      get { return _drawRadius; }
      set { _drawRadius = value; }
    }

    void OnValidate()
    {
      _drawRadius = Mathf.Max(0, _drawRadius);
      _drawResolution = Mathf.Clamp(_drawResolution, 3, 24);
      _minSegmentLength = Mathf.Max(0, _minSegmentLength);
    }

    void Awake()
    {
      if (_pinchDetectors.Length == 0)
      {
        Debug.LogWarning(
          "No pinch detectors were specified!  PinchDraw can not draw any lines without PinchDetectors.");
      }
      ringList = new List<Vector3>();
      loadRing = new List<int>();

/*      if (_pinchDetectors2.Length == 0)
      {
        Debug.LogWarning(
          "No pinch detectors were specified!  PinchDraw can not draw any lines without PinchDetectors.");
      }*/
    }

    void Start()
    {
      saveTextObj = GameObject.FindWithTag("saveText");
      saveTextObj.SetActive(false);
      loadTextObj = GameObject.FindWithTag("loadText");
      loadCont = GameObject.FindWithTag("loadCont");
      loadDel = GameObject.FindWithTag("loadDel");
      loadCont.SetActive(false);
      DJBiom = GameObject.FindWithTag("DungeonBiom");
      _drawStates = new DrawState[_pinchDetectors.Length];
      for (int i = 0; i < _pinchDetectors.Length; i++)
      {
        _drawStates[i] = new DrawState(this);

/*      _drawStates2 = new DrawState[_pinchDetectors2.Length];
      for (int j = 0; j < _pinchDetectors2.Length; j++) {
        _drawStates2[j] = new DrawState(this);*/
      }


      //nos initialisation
      _line = 0;
      state = 1;
      zTab = new List<int>();
      yTab = new List<int>();
      loadList = new List<List<Vector3>>();
      arcValueU = arcValue;
      arcValueU.GetComponent<HoverItemDataSlider>().Value = 0.1f;
      rightPinch = GameObject.FindWithTag("posIndR");
      pinchL = GameObject.FindWithTag("posIndL");
      cam = GameObject.FindWithTag("deplacement");
      RIX = GameObject.FindWithTag("riX");
    }

    public void removeZ()
    {
      if (zTab.Count >= 1)
      {
        string s = "line" + zTab[zTab.Count - 1];
        GameObject obj = GameObject.Find(s);
        MeshRenderer m = obj.GetComponent<MeshRenderer>();
        m.enabled = false;
        yTab.Add(zTab[zTab.Count - 1]);
        zTab.RemoveAt(zTab.Count - 1);

      }
    }

    public void removeY()
    {
      if (yTab.Count >= 1)
      {
        string s = "line" + yTab[yTab.Count - 1];
        GameObject obj = GameObject.Find(s);
        MeshRenderer m = obj.GetComponent<MeshRenderer>();
        m.enabled = true;
        zTab.Add(yTab[yTab.Count - 1]);
        yTab.RemoveAt(yTab.Count - 1);
      }
    }

    void Update()
    {

      if (Input.GetKeyDown(KeyCode.F1))
      {
        DJBiom.SetActive(!DJBiom.activeSelf);
      }

      
      if (state != 12 && state != 13)
      {
        if (Input.GetKeyDown(KeyCode.S))
        {
          if (state != 12 && state != 13)
            prevState = state;
          state = 12;
          saveTextObj.SetActive(true);
          saveTextObj.GetComponent<TMP_InputField>().ActivateInputField();
          saveTextObj.GetComponent<TMP_InputField>().text = "";

          SaveScene();
        }
      }

      if (!loadCont.activeSelf && state != 12)
        if (Input.GetKeyDown(KeyCode.L))
        {
          if (state != 12 && state != 13)
            prevState = state;
          state = 13;
          loadCont.SetActive(true);
          LoadScene();
        }
      
      if (loadCont.activeSelf)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
          foreach (GameObject bts in ButtonsLoad)
          {
            Destroy(bts);
          }
          foreach (GameObject bts in ButtonsDel)
          {
            Destroy(bts);
          }
          ButtonsLoad = new List<GameObject>();
          loadCont.SetActive(false);
          state = prevState;
        }
        
      
      _drawColor = GameObject.Find("Picker").GetComponent<ColorPicker>().CurrentColor;
      if (state == 1)
        drawTrail();
      if (state == 2 || state == 3)
        draw3DObject();
      if (state == 5)
        dragObject();
      if (state == 10)
      {
        GameObject.Find("CanvasPicker").GetComponent<Canvas>().enabled = true;
        pickColor();
      }
      else
        GameObject.Find("CanvasPicker").GetComponent<Canvas>().enabled = false;

      if (state == 11)
        deplacement();

      if (state == 12)
        SaveScene();

    }
    Vector3 tmp = new Vector3(0,0,0);
    Vector3 tmp2 = new Vector3(0,0,0);
    //Quaternion qtmp = new Quaternion();
    //Quaternion qtmp2 = new Quaternion();
    void LoadScene()
    {
      ButtonsLoad = new List<GameObject>();
      ButtonsDel = new List<GameObject>();

      string LEVEL_PATH =Application.persistentDataPath;
      Regex p = new Regex(@"\\[0-9A-z]*\.json");
      
      foreach (string worldDir in Directory.GetFiles(LEVEL_PATH)) {
        string s = p.Match(worldDir).Value;
        s = s.Substring(1,s.Length - 6);
        GameObject button = (GameObject) Instantiate(buttonprefab);
        button.GetComponentInChildren<Text>().text = s;
        button.GetComponent<Button>().onClick.AddListener(
          () =>
          {
            Load(Application.persistentDataPath + "/" + s + ".json");
            loadCont.SetActive(false);
            foreach (GameObject bts in ButtonsLoad)
            {
              Destroy(bts);
            }
            foreach (GameObject bts in ButtonsDel)
            {
              Destroy(bts);
            }
            ButtonsLoad = new List<GameObject>();
            state = prevState;
          }
          );
        button.transform.SetParent(loadTextObj.transform, false);
        ButtonsLoad.Add(button);
        
        button = (GameObject) Instantiate(buttonprefab);
        button.GetComponentInChildren<Text>().text = "D";
        button.GetComponent<Button>().onClick.AddListener(
          () =>
          {
            foreach (GameObject bts in ButtonsLoad)
            {
              Destroy(bts);
            }
            foreach (GameObject bts in ButtonsDel)
            {
              Destroy(bts);
            }
            File.Delete(Application.persistentDataPath + "/" + s + ".json");
            LoadScene();
          }
        );
        button.transform.SetParent(loadDel.transform, false);
        button.GetComponent<UnityEngine.UI.Image>().color = new Color(1,0,0,1); 

        ButtonsDel.Add(button);
      }
      
    }

    void SaveScene()
    {
      if (Input.GetKeyDown(KeyCode.Return))
      {
        string name = saveTextObj.GetComponent<TMP_InputField>().text;
        Regex r = new Regex("[0-9A-z]*");
        Match match = r.Match(name);
        if (match.Success && match.Value.Length == name.Length)
        {
          string path = Application.persistentDataPath + "/" + name + ".json";
          Save(path);
          saveTextObj.GetComponent<UnityEngine.UI.Image>().color = new Color(1,1,1,0.6F); 
          saveTextObj.SetActive(false);
          state = prevState;
        }
        else
        {
          saveTextObj.GetComponent<UnityEngine.UI.Image>().color = new Color(1,0.5F,0.5F,0.6F);
          saveTextObj.GetComponent<TMP_InputField>().ActivateInputField();
          saveTextObj.GetComponent<TMP_InputField>().text = "";


        }

      }

      if (Input.GetKeyDown(KeyCode.Escape))
      {
        state = prevState;
        saveTextObj.SetActive(false);
      }
    }

    public void HandleClick()
    {
      //scrollList.TryTransferItemToOtherShop (item);
    }
    
    private Rigidbody camShape;
    void deplacement()
    {

      for (int i = 0; i < _pinchDetectors.Length; i++)
      {
        var detector = _pinchDetectors[i];
        var drawState = _drawStates[i];
        
        if (detector == _pinchDetectors[1] && !isTurning) 
        {
          if (detector.DidStartHold)
          {
            asPinchPosition = rightPinch.transform.position.y;
            camShape = cam.GetComponent<Rigidbody>();
            isMoving = true;
          }

          if (detector.IsHolding)
          {
            position = (asPinchPosition - rightPinch.transform.position.y);
            camShape.velocity = camShape.transform.forward * position * -20;
          }

          if (detector.DidRelease)
          {
            isMoving = false;
            camShape.velocity = new Vector3(0,0,0);
          }
        }

        
        

        else if (detector == _pinchDetectors[0] && !isMoving)
        {

          if (detector.DidStartHold)
          {
            isTurning = true;
            asPinchPosition = pinchL.transform.position.y;
            camShape = cam.GetComponent<Rigidbody>();
          }

          if (detector.IsHolding)
          {
            position = asPinchPosition - pinchL.transform.position.y;
            camShape.MoveRotation(Quaternion.Euler(cam.transform.localRotation.x, cam.transform.eulerAngles.y + (position * 20), cam.transform.localRotation.z));
          }

          if (detector.DidRelease)
            isTurning = false;
        }
        
        
        
        
    }
  }
    
/*    int getHand(String detector)
    {
      if (detector == "PinchDetector_R")
        return 1;
      else
        return 0;
    }*/

    public static float RoundValue(float num, float precision)
    {
      return Mathf.Floor(num * precision + 0.5f) / precision;
    }
    void Save(string path)
    {
      JSONArray obj = new JSONArray();
      JSONArray listR = new JSONArray();
      JSONArray pos = new JSONArray();
      JSONArray typeO = new JSONArray();
      List<Vector3> rings = null;

      GameObject[] objs ;
      objs = GameObject.FindGameObjectsWithTag("line");
      JSONArray transINfos = new JSONArray();
      JSONArray infosJSON = new JSONArray();
      foreach (GameObject lineObj in objs)
      {
        listR = new JSONArray();
        rings = lineObj.GetComponent<SaveObjInfo>().coord;
        Vector4 colorInfo = lineObj.GetComponent<SaveObjInfo>().color;

        for (int i = 0; i < rings.Count; i++)
        {
          pos = new JSONArray();
          pos.Add(rings[i].x);
          pos.Add(rings[i].y);
          pos.Add(rings[i].z);
          pos.Add(colorInfo[0]);
          pos.Add(colorInfo[1]);
          pos.Add(colorInfo[2]);
          pos.Add(colorInfo[3]);
          pos.Add(lineObj.GetComponent<SaveObjInfo>().trailSize);
          
          listR.Add(pos);
        }

        //obj.Add(j);
        obj.Add(listR);
      }
      typeO.Add("Line",obj);


      objs = GameObject.FindGameObjectsWithTag("Cube");
      transINfos = new JSONArray();
      infosJSON = new JSONArray();
      obj = new JSONArray();
      foreach(GameObject cubeObj in objs) {
        infosJSON = new JSONArray();
        
        pos = new JSONArray();
        pos.Add(cubeObj.transform.position.x);
        pos.Add(cubeObj.transform.position.y);
        pos.Add(cubeObj.transform.position.z);
        infosJSON.Add(pos);
        
        pos = new JSONArray();
        pos.Add(cubeObj.transform.localRotation.x);
        pos.Add(cubeObj.transform.localRotation.y);
        pos.Add(cubeObj.transform.localRotation.z);
        pos.Add(cubeObj.transform.localRotation.w);
        infosJSON.Add(pos);
        
        pos = new JSONArray();
        pos.Add(cubeObj.transform.localScale.x);
        pos.Add(cubeObj.transform.localScale.y);
        pos.Add(cubeObj.transform.localScale.z);
        infosJSON.Add(pos);
        obj.Add(infosJSON);
        
        pos = new JSONArray();
        Vector4 ColorInfo = cubeObj.GetComponent<Renderer>().material.color;
        pos.Add(ColorInfo.x);
        pos.Add(ColorInfo.y);
        pos.Add(ColorInfo.z);
        pos.Add(ColorInfo.w);
        infosJSON.Add(pos);
        
        
        pos = new JSONArray();
        pos.Add(cubeObj.GetComponent<Renderer>().material.shader.name.ToString());
        infosJSON.Add(pos);
        

      }
      typeO.Add(obj);
      
      objs = GameObject.FindGameObjectsWithTag("Sphere");
      transINfos = new JSONArray();
      infosJSON = new JSONArray();
      obj = new JSONArray();
      foreach(GameObject cubeObj in objs) {
        infosJSON = new JSONArray();
        
        pos = new JSONArray();
        pos.Add(cubeObj.transform.position.x);
        pos.Add(cubeObj.transform.position.y);
        pos.Add(cubeObj.transform.position.z);
        infosJSON.Add(pos);
        
        pos = new JSONArray();
        pos.Add(cubeObj.transform.localRotation.x);
        pos.Add(cubeObj.transform.localRotation.y);
        pos.Add(cubeObj.transform.localRotation.z);
        pos.Add(cubeObj.transform.localRotation.w);
        infosJSON.Add(pos);
        
        pos = new JSONArray();
        pos.Add(cubeObj.transform.localScale.x);
        pos.Add(cubeObj.transform.localScale.y);
        pos.Add(cubeObj.transform.localScale.z);
        infosJSON.Add(pos);
        
        pos = new JSONArray();
        //Vector4 ColorInfo = cubeObj.GetComponent<Renderer>().material.color;
        Vector4 ColorInfo = cubeObj.GetComponent<Renderer>().material.color;
        pos.Add(ColorInfo.x);
        pos.Add(ColorInfo.y);
        pos.Add(ColorInfo.z);
        pos.Add(ColorInfo.w);
        infosJSON.Add(pos);
        obj.Add(infosJSON);
        
        pos = new JSONArray();
        pos.Add(cubeObj.GetComponent<Renderer>().material.shader.name.ToString());
        infosJSON.Add(pos);
      }
      typeO.Add(obj);
      
      
      
      pos = new JSONArray();
      pos.Add(cam.transform.position.x);
      pos.Add(cam.transform.position.z);
      pos.Add(cam.transform.localRotation.y);
      typeO.Add(pos);

      
      File.WriteAllText(path, typeO.ToString());
    }
    
    
    void DestroyAll(string tag)
    {
      GameObject[] enemies = GameObject.FindGameObjectsWithTag(tag);
      for(int i=0; i< enemies.Length; i++)
      {
        Destroy(enemies[i]);
      }
    }
    
    void Load(string path)
    {
      DestroyAll("line");
      DestroyAll("Cube");
      DestroyAll("Sphere");
      _line = 0;
      //string path = Application.persistentDataPath + "/objInfo.json";
      string jsonString = File.ReadAllText(path);
      JSONArray obj = (JSONArray) JSON.Parse(jsonString);
      var drawState = _drawStates[1];
      var detector = _pinchDetectors[1];
      
      foreach (JSONArray t1 in obj[0])
      {
        drawState.BeginNewLine();
        foreach (JSONArray t0 in t1)
        {
          DrawColor = new Color(t0[3],t0[4], t0[5], t0[6]);
          Vector3 val = new Vector3(t0[0],t0[1],t0[2]);
          float prevSize = arcValueU.GetComponent<HoverItemDataSlider>().Value;
          arcValueU.GetComponent<HoverItemDataSlider>().Value = t0[7];
          drawState.UpdateLine2(val);
          arcValueU.GetComponent<HoverItemDataSlider>().Value = prevSize;
        }
        drawState.FinishLine();
      }

      foreach (JSONArray t1 in obj[1])
      {

          GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
          cube.GetComponent<BoxCollider>().isTrigger = true;
          cube.tag = "Cube";
          cube.name = "line" + _line;
          cube.transform.position = new Vector3(t1[0][0],t1[0][1],t1[0][2]);
          cube.transform.localRotation = new Quaternion(t1[1][0],t1[1][1],t1[1][2],t1[1][3]);
          cube.transform.localScale = new Vector3(t1[2][0],t1[2][1],t1[2][2]);
          String text = t1[4][0];
          Material material = new Material(Shader.Find(text));
          cube.GetComponent<Renderer>().material = material;
          cube.GetComponent<Renderer>().material.SetColor("_Color", new Vector4(t1[3][0],t1[3][1],t1[3][2],t1[3][3]));

          zTab.Add(_line);
          _line++;
      }
      
      foreach (JSONArray t2 in obj[2])
      {

          GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
          //BoxCollider boxCollider = sphere.AddComponent<BoxCollider>();
          sphere.GetComponent<SphereCollider>().isTrigger = true;
          //boxCollider.isTrigger = true;
          sphere.tag = "Sphere";
          sphere.name = "line" + _line;
          sphere.transform.position = new Vector3(t2[0][0],t2[0][1],t2[0][2]);
          sphere.transform.localRotation = new Quaternion(t2[1][0],t2[1][1],t2[1][2],t2[1][3]);
          sphere.transform.localScale = new Vector3(t2[2][0],t2[2][1],t2[2][2]);
          String text = t2[4][0];

          Material material = new Material(Shader.Find(text));
          sphere.GetComponent<Renderer>().material = material;
          sphere.GetComponent<Renderer>().material.SetColor("_Color", new Vector4(t2[3][0],t2[3][1],t2[3][2],t2[3][3]));

          zTab.Add(_line);
          _line++;
      }

      cam.transform.position = new Vector3(obj[3][0],cam.transform.position.y,obj[3][1]);
      cam.transform.localRotation = new Quaternion(cam.transform.localRotation.x,obj[3][2],cam.transform.localRotation.z,cam.transform.localRotation.w);
    }
    
    
    void draw3DObject (){
      for (int i = 0; i < _pinchDetectors.Length; i++) {
        var detector = _pinchDetectors[i];
        var drawState = _drawStates[i];
        
        //if (detector == "PinchDetector_R")
        //  break;

        if (detector == _pinchDetectors[1])
        {
          if (detector.DidStartHold)
          {
            if (state == 2)
            {
              // Si state vaut 2 on dessine des cubes
              GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
              cube.GetComponent<BoxCollider>().isTrigger = true;
              cube.tag = "Cube";
              cube.name = "line" + _line;


              cube.transform.position = GameObject.Find("RightIndex").transform.position;
              cube.transform.localScale = new Vector3(0.05F, 0.05F, 0.05F);
              cube.GetComponent<Renderer>().material.SetColor("_Color", GameObject.Find("Picker").GetComponent<ColorPicker>().CurrentColor);
              zTab.Add(_line);
              _line++;

            }

            if (state == 3)
            {
              // Si state vaut 3 on dessine des Spheres
              GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
              //BoxCollider boxCollider = sphere.AddComponent<BoxCollider>();
              sphere.GetComponent<SphereCollider>().isTrigger = true;

              //boxCollider.isTrigger = true;
              sphere.tag = "Sphere";
              sphere.name = "line" + _line;


              sphere.transform.position = GameObject.Find("RightIndex").transform.position;
              sphere.transform.localScale = new Vector3(0.05F, 0.05F, 0.05F);
              sphere.GetComponent<Renderer>().material.SetColor("_Color", GameObject.Find("Picker").GetComponent<ColorPicker>().CurrentColor);
              zTab.Add(_line);
              _line++;
            }
          }
        }
      }
    }

    void drawTrail (){
      for (int i = 0; i < _pinchDetectors.Length; i++) {
        var detector = _pinchDetectors[i];
        var drawState = _drawStates[i];

        if (detector == _pinchDetectors[1])
        {
          if (detector.DidStartHold)
          {
            drawState.BeginNewLine();
          }  

          if (detector.DidRelease)
          {
            drawState.FinishLine();
          }

          if (detector.IsHolding)
          {
            drawState.UpdateLine(detector.Position);
          }
        }
      }    
    }

    void pickColor (){
      for (int i = 0; i < _pinchDetectors.Length; i++) {
        var detector = _pinchDetectors[i];
        var drawState = _drawStates[i];
        _slider = GameObject.Find("BoxSlider");

        
        if (detector.DidStartHold) {
          if (detector == _pinchDetectors[1]){
            myx = RIX.transform.position.x;
            myy = GameObject.Find("RightIndex").transform.position.y;
          }
          
          if (detector == _pinchDetectors[0]){
            myy = GameObject.Find("LeftIndex").transform.position.y;

          }
        }

        if (detector.DidRelease) {
        }

        if (detector.IsHolding)
        {
          if (detector == _pinchDetectors[1]){
            _slider.GetComponent<BoxSlider>().normalizedValueY = (-(myy-GameObject.Find("RightIndex").transform.position.y)*5) + 0.5F;
            _slider.GetComponent<BoxSlider>().normalizedValue =  ((myx-RIX.transform.position.x)*5) + 0.5F;
          }
          
          if (detector == _pinchDetectors[0]){
            GameObject.Find("Hue").GetComponent<Slider>().value = ((myy-GameObject.Find("LeftIndex").transform.position.y)*-5)+0.5F;
          }
        }
      }    
    }

    double sqr(double val)
    {
      return val * val;
    }
    
    float distance (GameObject p1, GameObject p2)
    {
      double p1x = (double)p1.transform.position.x;
      double p1y = (double)p1.transform.position.y;
      double p1z = (double)p1.transform.position.z;
      double p2x = (double)p2.transform.position.x;
      double p2y = (double)p2.transform.position.y;
      double p2z = (double)p2.transform.position.z;
      
      return (float)Math.Sqrt(sqr(p1x-p2x)+sqr(p1y-p2y)+sqr(p1z-p2z));
      
    }
    
    void dragObject (){
      for (int i = 0; i < _pinchDetectors.Length; i++) {
        var detector = _pinchDetectors[i];
        var drawState = _drawStates[i];

        if (detector.DidStartHold)
        {
          if (detector == _pinchDetectors[1])
            isDrag = 1;
          pHand++;
          if (pHand == 2 && isDrag == 1){
            GameObject a = GameObject.Find("PinchDetector_L");
            GameObject b = GameObject.Find("PinchDetector_R");
            curd = distance(a, b);
            curt = GameObject.Find(_selected).transform.localScale.x;
          }
        }

        if (detector.DidRelease){
          if (detector == _pinchDetectors[1]){
            isDrag = 0;
            _selected = null;
          }
          pHand--;
        }

        if (detector.IsHolding){
          if (GameObject.Find(_selected) != null){
            GameObject obj = GameObject.Find(_selected);
            //curt = obj.transform.localScale.x;
            obj.transform.position = GameObject.Find("RightIndex").transform.position;
            obj.transform.rotation = GameObject.Find("RightIndex").transform.rotation;
            if (pHand == 2){
              GameObject a = GameObject.Find("PinchDetector_L");
              GameObject b = GameObject.Find("PinchDetector_R");
              dist = distance(a, b);
              obj.transform.localScale = new Vector3(curt + ((curd - dist) / -2), curt + ((curd - dist) / -2), curt + ((curd - dist) / -2)); // curt taille initila du 3D / curd distance initial au clique
            }
          }
        }
      }
    }
    
    private class DrawState {
      
      private List<Vector3> _vertices = new List<Vector3>();
      private List<int> _tris = new List<int>();
      private List<Vector2> _uvs = new List<Vector2>();
      private List<Color> _colors = new List<Color>();

      private PinchDraw _parent;

      private int _rings = 0;
      GameObject currentObj;

      private Vector3 _prevRing0 = Vector3.zero;
      private Vector3 _prevRing1 = Vector3.zero;

      private Vector3 _prevNormal0 = Vector3.zero;
      private Mesh _mesh;
      private SmoothedVector3 _smoothedPosition;



      public DrawState(PinchDraw parent) {
        
        _parent = parent;
        _smoothedPosition = new SmoothedVector3();
        _smoothedPosition.delay = parent._smoothingDelay;
        _smoothedPosition.reset = true;
      }

      public GameObject BeginNewLine() {
        _rings = 0;
        _vertices.Clear();
        _tris.Clear();
        _uvs.Clear();
        _colors.Clear();
        _smoothedPosition.reset = true;

        string s = "line"+_line;
        zTab.Add(_line);
        _line ++;

        GameObject lineObj = new GameObject(s);
        _mesh = new Mesh();
        _mesh.name = "Line Mesh";
        _mesh.MarkDynamic();
        
        lineObj.transform.position = Vector3.zero;
        lineObj.transform.rotation = Quaternion.identity;
        lineObj.transform.localScale = Vector3.one;
        lineObj.tag = "line";
        lineObj.AddComponent<MeshFilter>().mesh = _mesh;
        lineObj.AddComponent<MeshRenderer>().sharedMaterial = _parent._material;
        lineObj.AddComponent<SaveObjInfo>();
        currentObj = lineObj;
        currentObj.GetComponent<SaveObjInfo>().coord = new List<Vector3>();
        return lineObj;
      }

      public void UpdateLine2(Vector3 position) {
        _smoothedPosition.Update(position, Time.deltaTime);

        bool shouldAdd = false;

        shouldAdd |= _vertices.Count == 0;
        shouldAdd |= Vector3.Distance(_prevRing0, _smoothedPosition.value) >= _parent._minSegmentLength;

        if (shouldAdd) {
          addRing(position);
          updateMesh();
        }
      }
      public void UpdateLine(Vector3 position) {
        _smoothedPosition.Update(position, Time.deltaTime);

        bool shouldAdd = false;

        shouldAdd |= _vertices.Count == 0;
        shouldAdd |= Vector3.Distance(_prevRing0, _smoothedPosition.value) >= _parent._minSegmentLength;

        if (shouldAdd) {
          addRing(_smoothedPosition.value);
          updateMesh();
        }
      }

      
      public void FinishLine() {
        _mesh.UploadMeshData(true);
      }

      private void updateMesh() {
        _mesh.SetVertices(_vertices);
        _mesh.SetColors(_colors);
        _mesh.SetUVs(0, _uvs);
        _mesh.SetIndices(_tris.ToArray(), MeshTopology.Triangles, 0);
        _mesh.RecalculateBounds();
        _mesh.RecalculateNormals();
      }
      
      public void addRing(Vector3 ringPosition){
        //_parent.ringList.Add(ringPosition);
        //_parent.loadRing.Add(_line-1);
        currentObj.GetComponent<SaveObjInfo>().coord.Add(ringPosition);
        currentObj.GetComponent<SaveObjInfo>().line = _line-1;
        currentObj.GetComponent<SaveObjInfo>().trailSize = arcValueU.GetComponent<HoverItemDataSlider>().Value;

        _rings++;
        //Debug.Log(arcValueU.GetComponent<HoverItemDataSlider>().Value*10); //Debug la valeur selectionné dans l'arc value

        if (_rings == 1) {
          addVertexRing();
          addVertexRing();
          addTriSegment();
        }

        addVertexRing();
        addTriSegment();

        Vector3 ringNormal = Vector3.zero;
        if (_rings == 2) {
          Vector3 direction = ringPosition - _prevRing0;
          float angleToUp = Vector3.Angle(direction, Vector3.up);

          if (angleToUp < 10 || angleToUp > 170) {
            ringNormal = Vector3.Cross(direction, Vector3.right);
          } else {
            ringNormal = Vector3.Cross(direction, Vector3.up);
          }

          ringNormal = ringNormal.normalized;

          _prevNormal0 = ringNormal;
        } else if (_rings > 2) {
          Vector3 prevPerp = Vector3.Cross(_prevRing0 - _prevRing1, _prevNormal0);
          ringNormal = Vector3.Cross(prevPerp, ringPosition - _prevRing0).normalized;
        }

        if (_rings == 2) {
          updateRingVerts(0,
                          _prevRing0,
                          ringPosition - _prevRing1,
                          _prevNormal0,
                          0);
        }

        if (_rings >= 2) {
          updateRingVerts(_vertices.Count - _parent._drawResolution,
                          ringPosition,
                          ringPosition - _prevRing0,
                          ringNormal,
                          0);

          updateRingVerts(_vertices.Count - _parent._drawResolution * 2,
                          ringPosition,
                          ringPosition - _prevRing0,
                          ringNormal,
                          arcValueU.GetComponent<HoverItemDataSlider>().Value*10); //On donne la valeur selectionné dans l'arc value

          updateRingVerts(_vertices.Count - _parent._drawResolution * 3,
                          _prevRing0,
                          ringPosition - _prevRing1,
                          _prevNormal0,
                          arcValueU.GetComponent<HoverItemDataSlider>().Value*10); //On donne la valeur selectionné dans l'arc value
        }

        _prevRing1 = _prevRing0;
        _prevRing0 = ringPosition;

        _prevNormal0 = ringNormal;

        //COLOR//
          _parent.DrawColor = GameObject.Find("Picker").GetComponent<ColorPicker>().CurrentColor;
          currentObj.GetComponent<SaveObjInfo>().color = _parent.DrawColor;
          
      }

      private void addVertexRing() {
        for (int i = 0; i < _parent._drawResolution; i++) {
          _vertices.Add(Vector3.zero);  //Dummy vertex, is updated later
          _uvs.Add(new Vector2(i / (_parent._drawResolution - 1.0f), 0));
          _colors.Add(_parent._drawColor);
        }
      }

      //Connects the most recently added vertex ring to the one before it
      private void addTriSegment() {
        for (int i = 0; i < _parent._drawResolution; i++) {
          int i0 = _vertices.Count - 1 - i;
          int i1 = _vertices.Count - 1 - ((i + 1) % _parent._drawResolution);

          _tris.Add(i0);
          _tris.Add(i1 - _parent._drawResolution);
          _tris.Add(i0 - _parent._drawResolution);

          _tris.Add(i0);
          _tris.Add(i1);
          _tris.Add(i1 - _parent._drawResolution);
        }
      }

      private void updateRingVerts(int offset, Vector3 ringPosition, Vector3 direction, Vector3 normal, float radiusScale) {
        direction = direction.normalized;
        normal = normal.normalized;

        for (int i = 0; i < _parent._drawResolution; i++) {
          float angle = 360.0f * (i / (float)(_parent._drawResolution));
          Quaternion rotator = Quaternion.AngleAxis(angle, direction);
          Vector3 ringSpoke = rotator * normal * _parent._drawRadius * radiusScale;
          _vertices[offset + i] = ringPosition + ringSpoke;
        }
      }
    }
  }
}