using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : Singleton<PlayerHealthUI>
{
    Text levelText;
    Image healthSlider;
    Image expSlider;

    protected override void Awake()
    {
        levelText = transform.GetChild(2).GetComponent<Text>();
        healthSlider = transform.GetChild(0).GetChild(0).GetComponent<Image>();
        expSlider = transform.GetChild(1).GetChild(0).GetComponent<Image>();

    }
    private void Update()
    {
        if(PlayerController.Instance!=null)
        {
            levelText.text = "Level" + GameManager.Instance.playerStats.characterData.currentlevel.ToString("00");
            updateHealth();
            UpdateExp();
        }

    }

    //血条更新
    void updateHealth()
    {
        float sliderPercent = (float)GameManager.Instance.playerStats.CurrentHealth / GameManager.Instance.playerStats.MaxHealth;
        healthSlider.fillAmount = sliderPercent;
    }
    //经验条更新
    void UpdateExp()
    {
        float sliderPercent = (float)GameManager.Instance.playerStats.characterData.currentExp / GameManager.Instance.playerStats.characterData.baseExp;
        expSlider.fillAmount = sliderPercent;
    }
}

