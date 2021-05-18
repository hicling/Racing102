using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Version : MonoBehaviour
{
    [SerializeField] private Text versionText;

    private void Start()
    {
        versionText.text = "Version: " + Application.version.ToString();
    }
}
