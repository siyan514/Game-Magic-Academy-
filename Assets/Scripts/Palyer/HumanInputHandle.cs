using UnityEngine;

public class HumanInputHandler : MonoBehaviour
{
    public int PlayerIndex { get; set; }

    public Vector2 GetMovementInput()
    {
        if (PlayerIndex == 1)
            return new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        if (PlayerIndex == 2)
            return new Vector2(Input.GetAxis("Horizontal2"), Input.GetAxis("Vertical2"));

        return Vector2.zero;
    }

    public bool GetBombInput()
    {
        if (PlayerIndex == 1)
            return Input.GetKeyDown(KeyCode.Space);

        if (PlayerIndex == 2)
            return Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter);

        return false;
    }
}