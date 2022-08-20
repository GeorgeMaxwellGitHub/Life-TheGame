using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraController : MonoBehaviour
{
    [SerializeField] Tilemap tilemapInWhichBoundsCameraWillClamp;

    Vector3 _bottomLeftLimit;
    Vector3 _topRightLimit;

    float _halfHeight;
    float _halfWidth;

    void Start()
    {
        SetBounds();
    }

    public void SetBounds()
    {
        _halfHeight = Camera.main.orthographicSize;
        _halfWidth = _halfHeight * Camera.main.aspect;

        //Actually I forgot what this doing and why this two lines needed, but I just leave them there
        _bottomLeftLimit = tilemapInWhichBoundsCameraWillClamp.localBounds.min + new Vector3(_halfWidth, _halfHeight, 0f);
        _topRightLimit = tilemapInWhichBoundsCameraWillClamp.localBounds.max + new Vector3(-_halfWidth, -_halfHeight, 0f);

        PlayerController.instance.SetBoundsForCamera(tilemapInWhichBoundsCameraWillClamp.localBounds.min, tilemapInWhichBoundsCameraWillClamp.localBounds.max);
    }
}
