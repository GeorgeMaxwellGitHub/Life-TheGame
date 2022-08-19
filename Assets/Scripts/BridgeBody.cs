using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class BridgeBody : MonoBehaviour
{
    public static BridgeBody instance;

    [SerializeField] Transform initialPoint;

    [SerializeField] GameObject bridgePiecePrefab;
    [SerializeField] BridgePiece[] bridgePieces;

    private bool isInfiniteModeActive = false;

    [SerializeField] CinemachineVirtualCamera virtualCamera;
    [SerializeField] float maxCameraSize;

    [SerializeField] float cameraIncreaseSpeed;
    [SerializeField] float cameraDecreaseSpeed;

    [SerializeField] SpriteRenderer spaceImage;

    private float cameraOrthographicSizeInitialValue;
    private Vector3 initialSpaceLocalScale;

    bool isInfiniteModRunning;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        UpdateBridge();

        cameraOrthographicSizeInitialValue = virtualCamera.m_Lens.OrthographicSize;
        initialSpaceLocalScale = spaceImage.gameObject.transform.localScale;
    }

    private void Update()
    {
        if (isInfiniteModRunning)
        {
            AudioManager.instance.PlayBridgeMusic();
            isInfiniteModRunning = false;
        }

        if (isInfiniteModeActive)
        {
            spaceImage.gameObject.transform.localScale += new Vector3(0.0001f, 0.0001f, 0);
        } else
        {
            spaceImage.gameObject.transform.localScale = initialSpaceLocalScale;
        }

        if (isInfiniteModeActive && virtualCamera.m_Lens.OrthographicSize < maxCameraSize)
        {
            virtualCamera.m_Lens.OrthographicSize += cameraIncreaseSpeed * Time.deltaTime;
            spaceImage.color += new Color(spaceImage.color.r, spaceImage.color.g, spaceImage.color.b, cameraIncreaseSpeed * Time.deltaTime);
        }

        if (!isInfiniteModeActive && virtualCamera.m_Lens.OrthographicSize > cameraOrthographicSizeInitialValue)
        {
            virtualCamera.m_Lens.OrthographicSize -= cameraDecreaseSpeed * Time.deltaTime;

            spaceImage.color -= new Color(0, 0, 0, cameraDecreaseSpeed * Time.deltaTime);
        }
    }

    public void UpdateBridge()
    {
        bridgePieces = GetComponentsInChildren<BridgePiece>();

        for (int i = 0; i < bridgePieces.Length; i++)
        {
            bridgePieces[i].GetComponent<BridgePiece>().index = i;

            if (i < bridgePieces.Length - 1)
            {
                bridgePieces[i + 1].gameObject.transform.position = bridgePieces[i].positionOfNextPiece.position;
            }
        }
    }

    public void AddPiece()
    {
        Instantiate(bridgePiecePrefab, gameObject.transform);
        UpdateBridge();
    }

    public void RemovePiece()
    {
        Destroy(bridgePieces[0].gameObject);
        UpdateBridge();
    }

    public void CheckPlayerStep(int index)
    {
        if (index == bridgePieces.Length - 15)
        {
            AddPiece();
            RemovePiece();

            print("Infinite Mod Enabled");

            isInfiniteModeActive = true;

            if (!isInfiniteModRunning && !AudioManager.instance.IsBridgeMusicPlay())
            {
                isInfiniteModRunning = true;
            }
        }
    }

    public void MoveBridgeBodyToInitial()
    {
        UpdateBridge();
        bridgePieces[0].gameObject.transform.position = initialPoint.position;
        UpdateBridge();
        PlayerController.instance.gameObject.transform.position = new Vector2(bridgePieces[bridgePieces.Length - 19].gameObject.transform.position.x,
                                                                                PlayerController.instance.gameObject.transform.position.y);
    }
    public bool ReturnInfiniteModState()
    {
        return isInfiniteModeActive;
    }

    public void DisableInfiniteMod()
    {
        AudioManager.instance.StopBridgeMusic();
        print("Infinite Mod Disabled");
        isInfiniteModeActive = false;
    }
}
