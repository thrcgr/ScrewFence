using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

namespace Manager
{
    public class FenceManager : MonoBehaviour
    {
        public static FenceManager Manager;
    
        [Header("UI")]
        public Button startButton;
    
        [Header("Objects")]
        public GameObject hammer;
    
        [HideInInspector]public Camera mainCamera;
        [HideInInspector]public Fence selectedFence;
        [HideInInspector]public List<Fence> fenceList;
        public List<FenceGroup> fenceGroupsList;

        private bool _nextLevel;
        private List<FenceGroup> _fenceGroupsList = new List<FenceGroup>();
        private FenceGroup _selectedFenceGroup;


        private void Awake()
        {
            
            Manager = this;
            Time.timeScale = 0;
        }
        void Start()
        {
            foreach (var fenceGroup in fenceGroupsList)
            {
                _fenceGroupsList.Add(fenceGroup);
            }
            mainCamera = Camera.main;
            SetFenceGroup();

        }
        
        public void SetFenceGroup()
        {
            if (fenceGroupsList.Count<=0)
            {
                return;
            }
            _selectedFenceGroup = fenceGroupsList[0];
            var fenceGroupInFence = fenceGroupsList[0].transform.GetComponentsInChildren<Fence>();
            foreach (var fence in fenceGroupInFence)
            {
                fence.EnableFence();
                fenceList.Add(fence);
            }
            _isCameraMoving = true;
        }
        public void SelectFence()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit raycastHit;
                if (Physics.Raycast(ray, out raycastHit))
                {
                    var fenceCompenent = raycastHit.transform.GetComponent<Fence>();
                    if (selectedFence == fenceCompenent)
                    {
                        return;
                    }
                
                    if (fenceCompenent)
                    {
                        selectedFence = fenceCompenent;
                        _mouseIsHitOnce = false;
                    }
                    

                }
            }
        }

        private bool _hammerReach;
        private float _moveHammerTimer;
        public void MoveHammer()
        {
            var hammerLastDir = hammer.transform.position;
                var hammerNextDir = selectedFence.hammerTransform.position;
                _moveHammerTimer += Time.deltaTime * 15f;
                hammer.transform.SetParent(selectedFence.hammerTransform);
                hammer.transform.position = Vector3.Lerp(hammerLastDir, hammerNextDir, _moveHammerTimer);
                hammer.transform.rotation = Quaternion.Lerp(hammer.transform.rotation,selectedFence.hammerTransform.rotation,_moveHammerTimer);
                if (_moveHammerTimer>=1f)
                {
                    _moveHammerTimer = 0f;
                    _hammerReach = true;
                }
        }
        

        private bool _mouseIsHitOnce;
        public void HitFence()
        {
            if (_hammerReach)
            {
                if (selectedFence ) 
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        _mouseIsHitOnce = false;
                    }
                    if (!_mouseIsHitOnce)
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
            }
        }

        private void CheckHit()
        {
            if (selectedFence.fenceModel.transform.localPosition.y >= 0.1f)
            {
                HammerDown();
                
                FenceDown();
                    
                HammerUp();
                    
            }
            else
            {
                selectedFence.DisableFence();
                fenceList.Remove(selectedFence);
                selectedFence = null;
                if (!selectedFence)
                {
                    _hammerReach = false;
                }
            }
        }

        private bool _gameIsComplete;
        public bool levelComplete;
        public void CheckGameIsEnd()
        {
            if (!levelComplete)
            {
                if (fenceList.Count <= 0)
                {
                    if (fenceGroupsList.Count<=0)
                    {
                        Debug.Log("Fence Level Complete");
                        levelComplete = true;
                    }
                    else
                    {
                        fenceGroupsList.Remove(fenceGroupsList[0]);
                        SetFenceGroup();
                    }
                    
                }

                
            }
            
        }
    
        

        #region UIScripts

        public void StartButton()
        {
            startButton.enabled = false;
            Time.timeScale = 1;
        }

        #endregion

        private bool _hammerIsUp ;
        private bool _hammerIsDown = true;
        private float _hammerDownTimer;
        private float _hammerUpTimer;
        
        public void HammerDown()
        {
            if (_hammerIsDown)
            {
                _hammerDownTimer += Time.deltaTime * 10f;
                hammer.transform.localRotation =
                    Quaternion.Lerp(hammer.transform.localRotation, Quaternion.Euler(55, 0, 0), _hammerDownTimer);
                if (_hammerDownTimer>=1f)
                {
                    _hammerDownTimer = 0;
                    _hammerIsDown = false;
                    _fenceDown = true;
                    _hammerIsUp = true;
                }
            }
        }

        private bool _fenceDown;
        private void FenceDown()
        {
            if (_fenceDown)
            {
                Vector3 dir = selectedFence.fenceModel.transform.localPosition;
                dir.y -= 0.1f;
                selectedFence.fenceModel.transform.localPosition = dir;
                _fenceDown = false;
            }
            
        }

        public void HammerUp()
        {
            if (_hammerIsUp)
            {
                _hammerUpTimer += Time.deltaTime * 10f;
            }
            hammer.transform.localRotation =
                Quaternion.Lerp(hammer.transform.localRotation, Quaternion.Euler(0, 0, 0), _hammerUpTimer);
            if (_hammerUpTimer>=1f)
            {
                _hammerUpTimer = 0;
                _hammerIsUp = false;
                _hammerIsDown = true;
                _mouseIsHitOnce = true;
            }
       
        }

        private bool _isCameraMoving;
        public void CameraCont()
        {
            if (_isCameraMoving)
            {
                var timer = 1f * Time.deltaTime;
                mainCamera.transform.SetParent(fenceGroupsList[0].camTransform);
                mainCamera.transform.localPosition = Vector3.Lerp(mainCamera.transform.localPosition, Vector3.zero, timer);
                mainCamera.transform.localRotation = Quaternion.Lerp(mainCamera.transform.localRotation, Quaternion.Euler(Vector3.zero), timer);
                if (timer>=1f)
                {
                    _isCameraMoving = false;
                }
            }
        }
       
    }
    }
