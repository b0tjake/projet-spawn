using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class PlayerInputHandler : MonoBehaviour
{
    [SerializeField] private playerMovement playerMovement;

    private void Awake()
    {
        if (playerMovement == null)
            playerMovement = GetComponent<playerMovement>();
    }

    private void Update()
    {
        ReadMovement();
        ReadActions();
    }

    private void ReadMovement()
    {
        float move = 0f;
        bool crouch = false;

        var keyboard = Keyboard.current;
        var gamepad = Gamepad.current;
        var joystick = Joystick.current;

        // Keyboard
        if (keyboard != null)
        {
            if (keyboard.aKey.isPressed) move = -1f;
            else if (keyboard.dKey.isPressed) move = 1f;

            crouch = keyboard.sKey.isPressed;
        }

        // Gamepad
        if (gamepad != null)
        {
            if (Mathf.Abs(gamepad.leftStick.x.ReadValue()) > 0.2f)
                move = gamepad.leftStick.x.ReadValue();
            else if (gamepad.dpad.left.isPressed)
                move = -1f;
            else if (gamepad.dpad.right.isPressed)
                move = 1f;

            if (gamepad.dpad.down.isPressed)
                crouch = true;
        }

        // Generic USB Joystick
        if (joystick != null)
        {
            float joystickX = joystick.stick.x.ReadValue();

            if (Mathf.Abs(joystickX) > 0.2f)
                move = joystickX;

            // Optional: crouch from joystick Y axis
            if (joystick.stick.y.ReadValue() < -0.5f)
                crouch = true;
        }

        playerMovement.SetMoveInput(move);
        playerMovement.SetCrouch(crouch);
    }

    // private void ReadActions()
    // {
    //     var keyboard = Keyboard.current;
    //     var gamepad = Gamepad.current;
    //     var joystick = Joystick.current;

    //     // Jump
    //     if ((keyboard != null && keyboard.spaceKey.wasPressedThisFrame) ||
    //         (gamepad != null && gamepad.buttonSouth.wasPressedThisFrame) ||
    //         (joystick != null && joystick.trigger.wasPressedThisFrame))
    //     {
    //         playerMovement.tryJump();
    //     }

    //     // Punch
    //     if ((keyboard != null && keyboard.jKey.wasPressedThisFrame) ||
    //         (gamepad != null && gamepad.buttonWest.wasPressedThisFrame) ||
    //         (joystick != null && joystick.button4.wasPressedThisFrame))
    //     {
    //         playerMovement.punch();
    //     }

    //     // Kick
    //     if ((keyboard != null && keyboard.kKey.wasPressedThisFrame) ||
    //         (gamepad != null && gamepad.buttonEast.wasPressedThisFrame) ||
    //         (joystick != null && joystick.button5.wasPressedThisFrame))
    //     {
    //         playerMovement.kick();
    //     }

    //     // Special 1
    //     if ((keyboard != null && keyboard.oKey.wasPressedThisFrame) ||
    //         (gamepad != null && gamepad.leftShoulder.wasPressedThisFrame) ||
    //         (joystick != null && joystick.button6.wasPressedThisFrame))
    //     {
    //         playerMovement.special1();
    //     }

    //     // Special 2
    //     if ((keyboard != null && keyboard.lKey.wasPressedThisFrame) ||
    //         (gamepad != null && gamepad.rightShoulder.wasPressedThisFrame) ||
    //         (joystick != null && joystick.button7.wasPressedThisFrame))
    //     {
    //         playerMovement.special2();
    //     }
    // }
private void ReadActions()
{
    var keyboard = Keyboard.current;
    var gamepad = Gamepad.current;
    var joystick = Joystick.current;

    // Jump
    if ((keyboard != null && keyboard.spaceKey.wasPressedThisFrame) ||
        (gamepad != null && gamepad.buttonSouth.wasPressedThisFrame) ||
        (joystick != null && joystick.trigger.wasPressedThisFrame))
    {
        playerMovement.tryJump();
    }

    // Punch
    if ((keyboard != null && keyboard.jKey.wasPressedThisFrame) ||
        (gamepad != null && gamepad.buttonWest.wasPressedThisFrame) ||
        (joystick != null && joystick["button4"] is ButtonControl button0 && button0.wasPressedThisFrame))
    {
        playerMovement.punch();
    }

    // Kick
    if ((keyboard != null && keyboard.kKey.wasPressedThisFrame) ||
        (gamepad != null && gamepad.buttonEast.wasPressedThisFrame) ||
        (joystick != null && joystick["button2"] is ButtonControl button1 && button1.wasPressedThisFrame))
    {
        playerMovement.kick();
    }

    // Special 1
    if ((keyboard != null && keyboard.oKey.wasPressedThisFrame) ||
        (gamepad != null && gamepad.leftShoulder.wasPressedThisFrame) ||
        (joystick != null && joystick["button5"] is ButtonControl button2 && button2.wasPressedThisFrame))
    {
        playerMovement.special1();
    }

    // Special 2
    if ((keyboard != null && keyboard.lKey.wasPressedThisFrame) ||
        (gamepad != null && gamepad.rightShoulder.wasPressedThisFrame) ||
        (joystick != null && joystick["button6"] is ButtonControl button3 && button3.wasPressedThisFrame))
    {
        playerMovement.special2();
    }
}

}