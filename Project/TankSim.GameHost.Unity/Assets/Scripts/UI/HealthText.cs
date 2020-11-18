using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthText : MonoBehaviour
{
    private Tank _tank;
    private Text _text;
    // Start is called before the first frame update
    void Start()
    {
        _text = GetComponent<Text>();

        _tank = FindObjectOfType<Tank>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_tank != null)
        {

            _text.text = _tank.Health + "/" + Tank.MaxHealth;
        }
    }
}
