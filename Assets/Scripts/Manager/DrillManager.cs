using System.Collections.Generic;
using UnityEngine;

namespace Manager
{
    public class DrillManager : MonoBehaviour
    {
        public static DrillManager Manager;
        
        private Camera _mainCamera;
        public GameObject drill;
        [HideInInspector]public bool colliderCollision = false;
        public List<ScrewGroup> screwGroupsList;
        [HideInInspector]public List<Screw> screwsList = new List<Screw>();
        [HideInInspector]public Screw selectedScrew;
        private ScrewGroup _selectedScrewGroups;
        [HideInInspector]public bool entryScrew=true;
        private List<MeshRenderer> _meshRenderersDrill = new List<MeshRenderer>();
        private void Awake()
        {
            _mainCamera = Camera.main;
            Manager = this;
        }
        private void Start()
        {
            var meshRenderers =  drill.transform.GetComponentsInChildren<MeshRenderer>();
            foreach (var meshRenderer in meshRenderers)
            {
                _meshRenderersDrill.Add(meshRenderer);
            }
            MeshRendDisableScrewDrill();
        }
        public float zDat=8.5f;
        private float _drillTransformTime;
        public void MoveDril()
        {
            MeshRendEnableScrewDrill();
            if (entryScrew)
            {
                if (Input.GetMouseButton(0))
                {
                    Vector3 mousePos = Input.mousePosition;
                    var target = _mainCamera.ScreenToWorldPoint(new Vector3(mousePos.x,mousePos.y,zDat));
                    _drillTransformTime = 5 * Time.deltaTime;
                    var lastDrillDir = drill.transform.position;
                    var nextDrillDir = target;
                    drill.transform.position = Vector3.Lerp(lastDrillDir,nextDrillDir,_drillTransformTime);
                    if (_drillTransformTime>=1) 
                    { 
                        _drillTransformTime = 0;
                    }
                
                }
            }
        
        }
    
        public bool first = true;
        private bool _camContScrew;
        public void SetScrewGroup()
        {
            _selectedScrewGroups = screwGroupsList[0];
            var screwGroupInScrew = screwGroupsList[0].transform.GetComponentsInChildren<Screw>();
            foreach (var screw in screwGroupInScrew)
            {
                screw.EnableScrewMesh();
                screw.EnableScrew();
                screwsList.Add(screw);
            }

            _isNewGroup = true;
        }

        private float _freeTimer;
        public void DrillTransformToScrew()
        {
            if (selectedScrew)
            {
                Vector3 dir = selectedScrew.drillLookRotate.localPosition - selectedScrew.drillTransform.localPosition;
                _freeTimer += 5 * Time.deltaTime;
                var drilLastDir = drill.transform;
                var drillNextDir=selectedScrew.drillTransform;
                drill.transform.SetParent(selectedScrew.drillTransform);
                drill.transform.position = Vector3.Lerp(drilLastDir.position, drillNextDir.position, _freeTimer);
                drill.transform.rotation=Quaternion.Lerp(drilLastDir.rotation,drillNextDir.rotation, _freeTimer);
                drill.transform.localRotation=Quaternion.LookRotation(dir);
                if (_freeTimer>=1)
                {
                    _freeTimer = 0;
                }
            }
        }
        
        private bool _drillDown = true;
        private bool _screwDown;
        private void ScrewDown()
        {
            if (selectedScrew)
            {
                if (_screwDown)
                {
                    _freeTimer += 5 * Time.deltaTime;
                    Vector3 dir = selectedScrew.screwModel.localPosition;
                    dir.x += 0.5f*Time.deltaTime;
                    selectedScrew.screwModel.localPosition=Vector3.Lerp(selectedScrew.screwModel.localPosition,dir,_freeTimer);
                    if (_freeTimer>=1)
                    {
                        _freeTimer = 0;
                    }
                }
            }
        }

        private bool _mouseHit;
        public void Hit()
        {
            if (Input.GetMouseButtonDown(0))
            {
                _mouseHit = false;
            }

            if (!_mouseHit)
            {
                CheckHit();
            }
            else
            {
                if (Input.GetMouseButton(0))
                {
                    CheckHit();
                }
            }
        }
        
        private float _freeTimer1;
        private void DrillDown()
        {
            if (selectedScrew)
            {
                if (_drillDown)
                {
                    _freeTimer1 += 5 * Time.deltaTime;
                    drill.transform.localPosition = Vector3.Lerp(drill.transform.localPosition,selectedScrew.drillTransform.localPosition,_freeTimer1);
                    if (_freeTimer1>=1)
                    {
                        _freeTimer1 = 0;
                        _mouseHit = true;
                        _drillDown = false;
                        _screwDown = true;
                    }
                }
            }
        }

        private void CheckHit()
        {
            if (selectedScrew)
            {
                if (selectedScrew.screwModel.localPosition.x<=0.15f)
                {
                    DrillDown();
                    ScrewDown();
                }
                else
                {
                    entryScrew = true;
                    selectedScrew.DisableScrew();
                    screwsList.Remove(selectedScrew);
                    selectedScrew = null;
                    if (!selectedScrew)
                    {
                        colliderCollision = false;
                    }
                }
            }
        }
        
        public bool levelComplete;
        public void CheckGameIsEndPub()
        {
            if (screwsList.Count<=0 )
            {
                screwGroupsList.Remove(screwGroupsList[0]);
                if (screwGroupsList.Count>=1)
                {
                    _isCameraMoving = true;
                    drill.transform.LookAt(screwGroupsList[0].transform);
                    SetScrewGroup();

                }
                else
                { 
                    Debug.Log("Game Complete");
                    levelComplete = true;
                }
            }
        }

        private bool _isNewGroup = true;
        private bool _isCameraMoving=true;
        public void CameraContPub()
        {
        
            if (_isCameraMoving)
            {
                if (_isNewGroup)
                {
                    //Vector3 dir = screwGroupsList[0].drillTransformGroup.localEulerAngles
                    Debug.Log("1");
                    drill.transform.SetParent(screwGroupsList[0].drillTransformGroup);
                    drill.transform.localRotation = Quaternion.LookRotation(screwGroupsList[0].drillTransformGroup.localPosition);
                    drill.transform.position = screwGroupsList[0].drillTransformGroup.position;
                    _isNewGroup = false;
                }
                
              
                var timer3 = 5f * Time.deltaTime;
                _mainCamera.transform.SetParent(screwGroupsList[0].camTransform);
                _mainCamera.transform.localPosition = Vector3.Lerp(_mainCamera.transform.localPosition, Vector3.zero, timer3);
                _mainCamera.transform.localRotation = Quaternion.Lerp(_mainCamera.transform.localRotation, Quaternion.Euler(Vector3.zero), timer3);
                if (timer3>=1)
                {
                    _isCameraMoving = false;
                }
            }
        }
        public void MeshRendEnableScrewDrill()
        {
            foreach (var meshRenderer in _meshRenderersDrill)
            {
                meshRenderer.enabled = true;
            }
        }

        public void MeshRendDisableScrewDrill()
        {
            foreach (var meshRenderer in _meshRenderersDrill)
            {
                meshRenderer.enabled = false;
            }
        }
    }
}

