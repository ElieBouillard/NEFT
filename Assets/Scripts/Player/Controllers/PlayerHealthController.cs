using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthController : MonoBehaviour
{
    [SerializeField] private Image _heatlhBar;

    public void TakeDamage()
    {
        _heatlhBar.fillAmount -= 0.1f;
    }
}
