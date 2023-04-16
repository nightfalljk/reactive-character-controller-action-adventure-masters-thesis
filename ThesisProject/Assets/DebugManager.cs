using System.Collections;
using System.Collections.Generic;
using Features.Character.Stats;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DebugManager : MonoBehaviour
{
    [SerializeField] private Health playerHealth;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Restart"))
        {
            SceneManager.LoadScene(0);
        }

        if (Input.GetButtonDown("TakeDamage"))
        {
            playerHealth.TakeDamage(10);
        }
    }
}
