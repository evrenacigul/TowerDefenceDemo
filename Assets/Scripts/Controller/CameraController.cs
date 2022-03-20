using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Generator;

namespace Controller
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] Transform _cameraFocusObj;

        [SerializeField] string _rotateButtonName = "Fire3";
        [SerializeField] string _moveButtonName = "Fire2";
        
        [SerializeField] float _rotateSpeed = 10;
        [SerializeField] float _moveSpeed = 10;

        Vector3 _moveStartPoint;
        Vector3 _cameraStartPos;

        float rotateAngle = 0;

        void Start()
        {
            _cameraFocusObj.position = new Vector3(MapGenerator.instance.GetMapWidthX / 2,
                _cameraFocusObj.position.y,
                MapGenerator.instance.GetMapWidthZ / 2);
        }

        void Update()
        {
            if(Input.GetButton(_rotateButtonName))
            {
                rotateAngle = _cameraFocusObj.rotation.eulerAngles.y + (Input.GetAxis("Mouse X") * _rotateSpeed);
                Vector3 euler = _cameraFocusObj.transform.eulerAngles;
                euler.y = rotateAngle;
                _cameraFocusObj.transform.eulerAngles = euler;
            }
            if(Input.GetButtonDown(_moveButtonName))
            {
                _moveStartPoint = Input.mousePosition;
                _cameraStartPos = _cameraFocusObj.localPosition;
            }
            else if(Input.GetButton(_moveButtonName))
            {
                Vector3 moveTo = (new Vector3(-Input.GetAxis("Mouse X"), 0, -Input.GetAxis("Mouse Y")) * _moveSpeed * Time.deltaTime);

                _cameraFocusObj.Translate(moveTo);

                Vector3 pos = _cameraFocusObj.localPosition;
                pos.y = 0;
                pos.x = Mathf.Clamp(pos.x, 0, MapGenerator.instance.GetMapWidthX);
                pos.z = Mathf.Clamp(pos.z, 0, MapGenerator.instance.GetMapWidthZ);
                _cameraFocusObj.localPosition = pos;
            }
        }
    }
}