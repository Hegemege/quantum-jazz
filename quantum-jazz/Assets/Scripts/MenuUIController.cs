using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuUIController : MonoBehaviour
{
    private bool _flag;

    void Update()
    {
        if (_flag) return;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameState = GameState.Menu;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            _flag = true;
            SceneManager.LoadScene("main");
        }
    }
}
