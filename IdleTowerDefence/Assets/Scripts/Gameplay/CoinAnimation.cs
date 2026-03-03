using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CoinAnimation : MonoBehaviour
{
    public static CoinAnimation Get;

    public List<GameObject> coin;
    public List<GameObject> gem;
    public List<GameObject> diamond;
    public GameObject dice;

    public TMP_Text coinText;
    public TMP_Text gemText;
    public TMP_Text diamondText;

    private List<Vector3> coinInitialPos = new List<Vector3>();
    private List<Vector3> gemInitialPos = new List<Vector3>();
    private List<Vector3> diamondInitialPos = new List<Vector3>();

    private void Awake()
    {
        Get = this;
    }

    private void Start()
    {
        for (int i = 0; i < coin.Count; i++)
        {
            coinInitialPos.Add(coin[i].transform.position);
        }

        for (int i = 0; i < gem.Count; i++)
        {
            gemInitialPos.Add(gem[i].transform.position);
        }

        for (int i = 0; i < diamond.Count; i++)
        {
            diamondInitialPos.Add(diamond[i].transform.position);
        }
    }

    internal void ShowCoin(Vector3 target, int gain, System.Action callback = default)
    {
        if (gain <= 0)
            return;
        for (int i = 0; i < coin.Count; i++)
        {
            coin[i].transform.position = coinInitialPos[i];
        }
        StartCoroutine(ShowAnim(coin, target, coinText, gain, callback));
    }

    internal void ShowDiamond(Vector3 target, int gain, System.Action callback = default)
    {
        return;

        if (gain <= 0)
            return;
        for (int i = 0; i < diamond.Count; i++)
        {
            diamond[i].transform.position = diamondInitialPos[i];
        }
        StartCoroutine(ShowAnim(diamond, target, diamondText, gain, callback));
    }

    internal void ShowGem(Vector3 target, int gain, System.Action callback = default)
    {
        if (gain <= 0)
            return;
        for (int i = 0; i < gem.Count; i++)
        {
            gem[i].transform.position = gemInitialPos[i];
        }
        StartCoroutine(ShowAnim(gem, target, gemText, gain, callback));
    }

    internal void ShowTicket(Vector3 initPos, Vector3 target, System.Action callback = default)
    {
        dice.transform.position = initPos;
        dice.gameObject.SetActive(true);
        LeanTween.move(dice.gameObject, target, 1.1f).setOnComplete(()=> {
            ActiveGameData.Instance.saveData.diceRollTicket += 1;
            EventManager.TriggerEvent(EventID.Update_Currency);
            dice.gameObject.SetActive(false);
        });
    }

    IEnumerator ShowAnim(List<GameObject> icons, Vector3 target, TMP_Text gainText, int gain, System.Action callback = default)
    {

        AudioManager.Instance?.PlaySFXSound(AudioClipsType.CoinCollectSound);
        for (int i = 0; i < icons.Count; i++)
        {
            icons[i].gameObject.transform.localScale = Vector3.zero;
            icons[i].gameObject.SetActive(true);

            LeanTween.scale(icons[i].gameObject, Vector3.one, 0.15f).setEase(LeanTweenType.easeOutBack).setIgnoreTimeScale(true);
            yield return new WaitForSeconds(Random.Range(0.1f, 0.15f));

            gainText.text = "+" + gain.ToString();
            gainText.gameObject.SetActive(true);
        }

        yield return new WaitForSeconds(Random.Range(0.65f, 0.8f));

        AudioManager.Instance?.PlaySFXSound(AudioClipsType.CoinCollectSound);
        foreach (var item in icons)
        {
            LeanTween.move(item.gameObject, target, 0.5f).setOnComplete(()=> { item.gameObject.SetActive(false); }).setEase(LeanTweenType.easeInBack).setIgnoreTimeScale(true);
            yield return new WaitForSeconds(Random.Range(0.08f, 0.1f));
        }

        callback?.Invoke();
        gainText.gameObject.SetActive(false);
    }
}
