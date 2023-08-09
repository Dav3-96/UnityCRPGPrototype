using System.Collections;
using System.Collections.Generic;
//using UnityEditor.Build.Content;
using UnityEngine;
using static UnityEngine.UI.Image;

namespace RPG.Core
{
    public class FollowCamera : MonoBehaviour
    {

        [SerializeField] Transform target;

        #region Camera Settings
        [Tooltip("Camera to use")]
        [SerializeField] Camera cam;

        [Tooltip("Max zoom allowed for the camera")]
        [SerializeField] float maxZoom = 10f;

        [Tooltip("Min zoom allowed for the camera")]
        [SerializeField] float minZoom = 3f;

        [Tooltip("Speed of the zoom")]
        [SerializeField] float sensitivity = 30f;
        #endregion

        #region Movement
        Vector3 origin;
        Vector3 difference;
        Vector3 cameraOffset;
        Vector3 cameraRotation;

        bool drag = false;
        bool followPlayer;

        [Tooltip("Speed the camera moves with keyboard input")]
        [SerializeField] float keyboardCameraSpeed = 10f;

        #endregion

        #region Edge Panning
        [Tooltip("Edge panning speed")]
        [Range(1f, 10f)]
        [SerializeField] float edgePanningSens = 10f;

        float edgePanAcceleration = 1f;
        //float edgePanDeceleration = 0.05f;
        #endregion

        // Start is called before the first frame update
        void Start()
        {
            cameraOffset = new Vector3(13f, 24f, 13f);
            cameraRotation = new Vector3(53.5f, 225f, 0f);
            ResetCameraLocation();
        }

        void Update()
        {
            CameraZoom();
            EdgePanning();
            KeyboardMovement();
        }

        private void CameraZoom()
        {
            if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, minZoom, sensitivity * Time.deltaTime);
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, maxZoom, sensitivity * Time.deltaTime);
            }
        }

        void KeyboardMovement()
        {
            

            if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            {
                followPlayer = false;
                cam.transform.Translate(Vector3.up * keyboardCameraSpeed * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            {
                followPlayer = false;
                cam.transform.Translate(Vector3.down * keyboardCameraSpeed * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                followPlayer = false;
                cam.transform.Translate(Vector3.left * keyboardCameraSpeed * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                followPlayer = false;
                cam.transform.Translate(Vector3.right * keyboardCameraSpeed * Time.deltaTime);
            }
        }


        private void EdgePanning()
        {
            // Size of the 'edge' 
            float edgeSize = 10f;
            // Handles the speed of the edge panning
            float panningSpeed = Mathf.Lerp(0, edgePanningSens, edgePanAcceleration * Time.deltaTime);


            if(Input.mousePosition.x > Screen.width - edgeSize)
            {
                followPlayer = false;
                cam.transform.Translate(Vector3.right * panningSpeed);
            }
            if (Input.mousePosition.x < edgeSize)
            {
                followPlayer = false;
                cam.transform.Translate(Vector3.left * panningSpeed);
            }
            if (Input.mousePosition.y > Screen.height - edgeSize)
            {
                followPlayer = false;
                cam.transform.Translate(Vector3.up * panningSpeed);
            }
            if (Input.mousePosition.y < edgeSize)
            {
                followPlayer = false;
                cam.transform.Translate(Vector3.down * panningSpeed);
            }
        }


        void LateUpdate()
        {
            //transform.position = target.position;
            MiddleMouseButtonCameraMovement();

            if (Input.GetKey(KeyCode.F))
            {
                ResetCameraLocation();
            }

            if (Input.GetKeyDown(KeyCode.T) && followPlayer == false)
            {
                ResetCameraLocation();
                followPlayer = true;
            }

            if (followPlayer)
            {
                if (IsCameraFocusedCorrectly())
                {
                    ResetCameraLocation();
                }
                transform.position = target.position;
            }
        }

        private void MiddleMouseButtonCameraMovement()
        {
            if (Input.GetMouseButton(2))
            {
                difference = (cam.ScreenToWorldPoint(Input.mousePosition)) - cam.transform.position;


                if (drag == false)
                {
                    drag = true;
                    origin = cam.ScreenToWorldPoint(Input.mousePosition);
                }
            }
            else
            {
                drag = false;
            }
            if (drag == true)
            {
                followPlayer = false;
                cam.transform.position = origin - difference;
                cam.transform.position = new Vector3(cam.transform.position.x, 24, cam.transform.position.z);
            }
        }

        private void ResetCameraLocation()
        {
            cam.transform.position = new Vector3(target.transform.position.x + cameraOffset.x, target.transform.position.y + cameraOffset.y, target.transform.position.z + cameraOffset.z);
        }
        private bool IsCameraFocusedCorrectly()
        {
            return cam.transform.position != new Vector3(target.transform.position.x + cameraOffset.x, target.transform.position.y + cameraOffset.y, target.transform.position.z + cameraOffset.z);
        }

    }
}
