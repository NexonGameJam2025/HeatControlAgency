using System.Runtime.CompilerServices;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public enum InputKey
{
    Up, Down, Left, Right
}

public class InputController : MonoBehaviour
{
    [SerializeField] private TileNodeSystem _tileNodeSystem;
    [SerializeField] private GameObject[] objPanels;
    
    private bool _isClickedFrozenBeamLauncher = false;
    private FrozenBeamLauncher _clickedFrozenBeanLauncher;
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0) || Input.GetKeyDown(KeyCode.Keypad0))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            _tileNodeSystem.Action(InputKey.Down);
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            _tileNodeSystem.Action(InputKey.Up);
        }
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            _tileNodeSystem.Action(InputKey.Left);
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            _tileNodeSystem.Action(InputKey.Right);
        }
        
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            var mousePos = Mouse.current.position.ReadValue();
            mousePos = Camera.main.ScreenToWorldPoint(mousePos);
            LayerMask layerMask = LayerMask.GetMask("Building");
            var hit = Physics2D.Raycast(mousePos, Vector2.zero, Mathf.Infinity, layerMask);


            if (hit && hit.transform.gameObject.TryGetComponent<FrozenBeamLauncher>(out var building))
            {
                if (!building.IsInit || !building.IsOnMana)
                    return;

                if (_isClickedFrozenBeamLauncher)
                {
                    if (_clickedFrozenBeanLauncher == building)
                    {
                        _isClickedFrozenBeamLauncher = false;
                        _clickedFrozenBeanLauncher = null;
                        TogglePanels(false);
                    }
                    else
                    {
                        return;
                    }
                }

                _isClickedFrozenBeamLauncher = true;
                GameManager.Instance.IsCannonReady = true;
                TogglePanels(true);
                DOVirtual.DelayedCall(0.2f, () =>
                {
                    building.OnClickHandler(() =>
                    {
                        TogglePanels(false);
                        _isClickedFrozenBeamLauncher = false;
                    });
                });
            }
        }
    }

    private void TogglePanels(bool isOn)
    {
        foreach (var panel in objPanels)
        {
            panel.SetActive(isOn);
        }
    }
}