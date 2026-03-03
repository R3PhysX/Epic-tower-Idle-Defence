using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShieldManager : MonoBehaviour
{
    public CardData cardData;
    [SerializeField] private List<Sprite> shieldSprites;
    [SerializeField] private SpriteRenderer shield;

    private Image shieldhealthBar;
    private TMP_Text shieldhealthText;

    [SerializeField] internal float currentShieldHealth;
    internal Coroutine shieldRoutine;

    [SerializeField] internal float ShieldHp = 25;
    [SerializeField] internal float ShieldSpawnTime = 3f;

    private void OnEnable()
    {
        if (Player.Instance != null) Player.Instance.shieldManager = this;
        EventManager.AddListener(EventID.Player_ShieldUpdate, OnShieldHealthUpdate);
    }

    private void OnDisable()
    {
        Player.Instance.shieldManager = null;
        StopAllCoroutines();
        EventManager.RemoveListener(EventID.Player_ShieldUpdate, OnShieldHealthUpdate);
        shield.gameObject.SetActive(false);
        shieldhealthBar.transform.parent.gameObject.SetActive(false);
    }

    void Start()
    {
        if (Player.Instance != null) Player.Instance.shieldManager = this;

        var data = cardData.cards[0];
        switch (data.savedData.level)
        {
            case 2:
                ShieldHp = data.level2.value1;
                ShieldSpawnTime = data.level2.value2;
                break;
            case 3:
                ShieldHp = data.level3.value1;
                ShieldSpawnTime = data.level3.value2;
                break;
            default:
                ShieldHp = data.level1.value1;
                ShieldSpawnTime = data.level1.value2;
                break;
        }

        shieldhealthBar = Player.Instance.shieldhealthBar;
        shieldhealthText = Player.Instance.shieldhealthText;
        shield.gameObject.SetActive(false);
        shieldhealthBar.transform.parent.gameObject.SetActive(false);
        shieldRoutine = StartCoroutine(WaitForShield());
    }

    internal void DoDamageToShield(float damage)
    {
        currentShieldHealth -= damage;
        EventManager.TriggerEvent(EventID.Player_ShieldUpdate, null);
        if (currentShieldHealth <= 0)
        {
            shield.gameObject.SetActive(false);
            shieldhealthBar.transform.parent.gameObject.SetActive(false);
            if (shieldRoutine != null)
            {
                StopCoroutine(shieldRoutine);
                shieldRoutine = StartCoroutine(WaitForShield());
            }
        }
    }

    private IEnumerator WaitForShield()
    {
        float waitTime = 0;
        while (waitTime <= ShieldSpawnTime)
        {
            waitTime += Time.deltaTime;
            yield return null;
        }

        if (Player.Instance.isDead == false)
        {
            shield.gameObject.SetActive(true);
            shieldhealthBar.transform.parent.gameObject.SetActive(true);
            currentShieldHealth = ShieldHp;
            EventManager.TriggerEvent(EventID.Player_ShieldUpdate, null);

        }
    }

    private void OnShieldHealthUpdate(object arg)
    {
        shieldhealthBar.fillAmount = currentShieldHealth / ShieldHp;
        shieldhealthText.text = currentShieldHealth.ToString("0") + "/" + ShieldHp.ToString("0");
        SetShieldSprite();
    }

    private void SetShieldSprite()
    {
        if (currentShieldHealth >= 0 && currentShieldHealth <= ShieldHp / 4)
        {
            //0-25%
            shield.sprite = shieldSprites[3];
        }
        else if (currentShieldHealth > ShieldHp / 4 && currentShieldHealth <= ShieldHp / 2f)
        {
            //25-50%
            shield.sprite = shieldSprites[2];
        }
        else if (currentShieldHealth > ShieldHp / 2 && currentShieldHealth <= ShieldHp / 1.3333f)
        {
            //50-75
            shield.sprite = shieldSprites[1];
        }
        else
        {
            // 100
            shield.sprite = shieldSprites[0];
        }
    }
}
