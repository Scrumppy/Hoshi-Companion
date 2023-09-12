using Gameplay.AI;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.EventSystems;
using static Gameplay.AI.StrikerAI;
using static UnityEngine.GraphicsBuffer;

namespace UI.Gameplay
{
    public class PlayerCanvasUI : MonoBehaviour
    {
        [Header("Camera")]
        [SerializeField] private Camera cameraTarget;
        private Transform cameraTargetTransform;

        [Header("UI")]
        [SerializeField] private GameObject floatingMenuUI;

        [Header("Striker")]
        [SerializeField] private GameObject strikerObject;

        private bool isFloatingMenuActive = false;

        public Vector3 rotationOfCanvas;

        private void Start()
        {
            floatingMenuUI.SetActive(false);

            if (Camera.main)
            {
                cameraTargetTransform = Camera.main.transform;
            }
        }

        private void Update()
        {
            //Check if striker was clicked
            RaycastStriker();
        }
        
        private void LateUpdate()
        {
            //Rotate the UI to always face the camera
            RotateUI();
        }

        private void RotateUI()
        {
            transform.LookAt(cameraTargetTransform);
            transform.eulerAngles = new Vector3(transform.eulerAngles.x + rotationOfCanvas.x, rotationOfCanvas.y, rotationOfCanvas.z);
        }

        private void RaycastStriker()
        {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit[] hits = Physics.RaycastAll(Camera.main.ScreenPointToRay(Input.mousePosition));
                foreach (RaycastHit hit in hits)
                {
                    if (hit.collider.gameObject == strikerObject && (hit.collider.isTrigger || !hit.collider.isTrigger))
                    {
                        //Player clicked, toggle the UI
                        isFloatingMenuActive = !isFloatingMenuActive;
                        floatingMenuUI.gameObject.SetActive(isFloatingMenuActive);
                        break;
                    }
                    else if (!EventSystem.current.IsPointerOverGameObject())
                    {
                        //Clicked somewhere else, deactivate the UI
                        isFloatingMenuActive = false;
                        floatingMenuUI.gameObject.SetActive(isFloatingMenuActive);
                    }
                }
            }
        }
    }
 }


