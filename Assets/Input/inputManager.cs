using UnityEngine;
using UnityEngine.InputSystem;

public class inputManager : MonoBehaviour
{
    public static inputManager instance; // This is the 'brain' the other script looks for
    public bool menuOpensClose { get; private set; }
    
    private PlayerInput playerInput;
    private InputAction menuAction;

    private void Awake()
    {
        // 1. Set the instance so menuManager can find it
        if (instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
            return;
        }

        // 2. Link to your 'playerControls' asset
        playerInput = GetComponent<PlayerInput>();
        menuAction = playerInput.actions["MenuOpensClose"];
    }

    private void Update()
    {
        // 3. This MUST be inside Update to catch the frame you press the button
        menuOpensClose = menuAction.WasPressedThisFrame();
    }
}