using System;
using UnityEngine;

namespace Manager
{
    public class GameManager : MonoBehaviour
    {
        private enum GameManagerState
        {
            FenceStart,
            FenceUpdate,
            FenceFinish,
            ScrewStart,
            ScrewUpdate,
            GameFinish,
            OnPause
        }
        private GameManagerState _currentGameManager;

        public void Stop()
        {
            _currentGameManager = GameManagerState.OnPause;
        }
        
        void Update()
        {
            switch (_currentGameManager)
            {
                case GameManagerState.FenceStart:
                    FenceManager.Manager.CameraCont();
                    FenceManager.Manager.SelectFence();
                    if (FenceManager.Manager.selectedFence)
                    {
                        _currentGameManager = GameManagerState.FenceUpdate;
                    }
                    break;
                case GameManagerState.FenceUpdate:
                    FenceManager.Manager.MoveHammer();
                    FenceManager.Manager.HitFence();
                    if (!FenceManager.Manager.selectedFence)
                    {
                        if (FenceManager.Manager.fenceList.Count<=0)
                        {
                            _currentGameManager = GameManagerState.FenceFinish;
                        }
                        else
                        {
                            _currentGameManager = GameManagerState.FenceStart;
                        }
                    
                    }
                
                    break;
                case GameManagerState.FenceFinish:
                    if (!FenceManager.Manager.levelComplete)
                    {
                        FenceManager.Manager.CheckGameIsEnd();
                    }
                
                    if (FenceManager.Manager.fenceGroupsList.Count<=0)
                    {
                        FenceManager.Manager.hammer.GetComponent<MeshRenderer>().enabled = false;
                        _currentGameManager = GameManagerState.ScrewStart;
                    }
                    else
                    {
                        _currentGameManager = GameManagerState.FenceStart;
                    }
                    break;
                case GameManagerState.ScrewStart:
                    if (DrillManager.Manager.first)
                    {
                        DrillManager.Manager.SetScrewGroup();
                        DrillManager.Manager.first = false;
                    }

                    if (!DrillManager.Manager.levelComplete)
                    {
                        DrillManager.Manager.CameraContPub();
                    }
                
                    DrillManager.Manager.MoveDril();
                    if (DrillManager.Manager.colliderCollision)
                    {
                        DrillManager.Manager.DrillTransformToScrew();
                        _currentGameManager = GameManagerState.ScrewUpdate;
                    }

                    break;
                case GameManagerState.ScrewUpdate:
                    DrillManager.Manager.Hit();
                    if (!DrillManager.Manager.selectedScrew)
                    {
                        if (DrillManager.Manager.screwsList.Count<=0)
                        {
                            _currentGameManager = GameManagerState.GameFinish;
                        }
                        else
                        {
                            _currentGameManager = GameManagerState.ScrewStart;
                        }
                    }
                    break;
                case GameManagerState.GameFinish:
                    if (DrillManager.Manager.screwGroupsList.Count<=0)
                    {
                        Debug.Log("Finish Game");
                        Time.timeScale = 0f;
                    }

                    if (DrillManager.Manager.screwGroupsList.Count>=1)
                    {
                        DrillManager.Manager.CheckGameIsEndPub();
                        _currentGameManager = GameManagerState.ScrewStart;
                    }
                
                    break;
                case GameManagerState.OnPause:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        

        }
    }
}
