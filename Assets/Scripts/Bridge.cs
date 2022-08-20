using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Bridge : MonoBehaviour
{
    public static Bridge instance;

    //Cached component references
    [SerializeField] Transform initialBridgeBodyPoint;

    [SerializeField] GameObject bridgePiecePrefab;
    [SerializeField] BridgePart[] bridgeParts;

    [SerializeField] CinemachineVirtualCamera virtualCamera;

    [SerializeField] SpriteRenderer spaceImage;

    //Config
    [SerializeField] float maxCameraSize;

    [SerializeField] float cameraIncreaseSpeed;
    [SerializeField] float cameraDecreaseSpeed;

    //States
    bool _isInfiniteModeActive = false;
    float _cameraOrthographicSizeInitialValue;
    Vector3 _initialSpaceLocalScale;
    bool _isInfiniteModRunning;

    void Start()
    {
        instance = this;
        UpdateBridge();

        _cameraOrthographicSizeInitialValue = virtualCamera.m_Lens.OrthographicSize;
        _initialSpaceLocalScale = spaceImage.gameObject.transform.localScale;
    }

    private void Update()
    {
        InfiniteModeHandler();
    }

    private void InfiniteModeHandler()
    {
        if (_isInfiniteModRunning)
        {
            AudioManager.instance.PlayBridgeMusic();
            _isInfiniteModRunning = false;
        }

        if (_isInfiniteModeActive)
        {
            spaceImage.gameObject.transform.localScale += new Vector3(0.0001f, 0.0001f, 0);
        }
        else
        {
            spaceImage.gameObject.transform.localScale = _initialSpaceLocalScale;
        }

        if (_isInfiniteModeActive && virtualCamera.m_Lens.OrthographicSize < maxCameraSize)
        {
            virtualCamera.m_Lens.OrthographicSize += cameraIncreaseSpeed * Time.deltaTime;
            spaceImage.color += new Color(spaceImage.color.r, spaceImage.color.g, spaceImage.color.b, cameraIncreaseSpeed * Time.deltaTime);
        }

        if (!_isInfiniteModeActive && virtualCamera.m_Lens.OrthographicSize > _cameraOrthographicSizeInitialValue)
        {
            virtualCamera.m_Lens.OrthographicSize -= cameraDecreaseSpeed * Time.deltaTime;

            spaceImage.color -= new Color(0, 0, 0, cameraDecreaseSpeed * Time.deltaTime);
        }
    }

    public void UpdateBridge()
    {
        bridgeParts = GetComponentsInChildren<BridgePart>();

        for (int i = 0; i < bridgeParts.Length; i++)
        {
            bridgeParts[i].GetComponent<BridgePart>().index = i;

            if (i < bridgeParts.Length - 1)
            {
                bridgeParts[i + 1].gameObject.transform.position = bridgeParts[i].positionOfNextPiece.position;
            }
        }
    }

    public void AddBridgePart()
    {
        Instantiate(bridgePiecePrefab, gameObject.transform);
        UpdateBridge();
    }

    public void RemoveFirstBridgePart()
    {
        Destroy(bridgeParts[0].gameObject);
        UpdateBridge();
    }

    public void CheckPlayerPositionOnBridge(int index)
    {
        if (index == bridgeParts.Length - 15)
        {
            AddBridgePart();
            RemoveFirstBridgePart();

            print("Infinite Mod Enabled");

            _isInfiniteModeActive = true;

            if (!_isInfiniteModRunning && !AudioManager.instance.IsBridgeMusicPlay())
            {
                _isInfiniteModRunning = true;
            }
        }
    }

    public void MoveBridgeBodyToInitial()
    {
        UpdateBridge();
        bridgeParts[0].gameObject.transform.position = initialBridgeBodyPoint.position;
        UpdateBridge();
        PlayerController.instance.gameObject.transform.position = new Vector2(bridgeParts[bridgeParts.Length - 19].gameObject.transform.position.x,
                                                                                PlayerController.instance.gameObject.transform.position.y);
    }
    public bool ReturnInfiniteModState()
    {
        return _isInfiniteModeActive;
    }

    public void DisableInfiniteMod()
    {
        AudioManager.instance.StopBridgeMusic();
        print("Infinite Mod Disabled");
        _isInfiniteModeActive = false;
    }
}
