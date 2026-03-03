using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card
{
    protected CardInfo cardInfo;

    protected GameObject SpawnedItem;
    public virtual void Init(CardInfo info)
    {
        cardInfo = info;
        var cardId = ActiveGameData.Instance.saveData.cardSlotIds.Find(x => x == info.cardId);
        if (cardId == info.cardId)
            Activate();
    }

    public virtual void Activate()
    {
        if (Player.Instance == null) 
            return;

        var obj = Resources.Load<GameObject>(cardInfo.prefabName);
        SpawnedItem = GameObject.Instantiate(obj.gameObject, Player.Instance.transform);
        SpawnedItem.transform.localPosition = Vector3.zero;
    }

    public virtual void Deactivate()
    {
        if (Player.Instance == null)
            return;

        GameObject.Destroy(SpawnedItem.gameObject);

    }
}

public class ShieldCard : Card
{
    public override void Init(CardInfo info)
    {
        base.Init(info);
    }

    public override void Activate()
    {
        base.Activate();

        if (Player.Instance == null)
            return;

        var shield = SpawnedItem.GetComponent<ShieldManager>();
        shield.ShieldHp = cardInfo.level1.value1;
        shield.ShieldSpawnTime = cardInfo.level1.value2;

        SpawnedItem.gameObject.SetActive(true);
    }

    public override void Deactivate()
    {
        base.Deactivate();
    }
}

public class MovingGroundsCard : Card
{
    public override void Init(CardInfo info)
    {
        base.Init(info);
    }

    public override void Activate()
    {
        base.Activate();

        if (Player.Instance == null)
            return;

        var slowaura = SpawnedItem.GetComponent<MovingGroundsManager>();
        slowaura.slowAmount = cardInfo.level1.value2;

        SpawnedItem.gameObject.SetActive(true);
    }

    public override void Deactivate()
    {
        base.Deactivate();
    }
}

public class LandMineCard : Card
{
    public override void Init(CardInfo info)
    {
        base.Init(info);
    }

    public override void Activate()
    {
        base.Activate();
        if (Player.Instance == null)
            return;

        var Mine = SpawnedItem.GetComponent<MinesManager>();
        Mine.numberOfMines = cardInfo.level1.value1;
        Mine.throwInterval = cardInfo.level1.value2;

        SpawnedItem.gameObject.SetActive(true);
    }

    public override void Deactivate()
    {
        base.Deactivate();
    }
}

public class InfernoTowerCard : Card
{
    public override void Init(CardInfo info)
    {
        base.Init(info);
    }

    public override void Activate()
    {
        base.Activate();
        if (Player.Instance == null)
            return;

        var inferno = SpawnedItem.GetComponent<InfernoManager>();
        inferno.numberOfAttack = cardInfo.level1.value1;
        inferno.attackDuration = cardInfo.level1.value2;

        SpawnedItem.gameObject.SetActive(true);
    }

    public override void Deactivate()
    {
        base.Deactivate();
    }
}

public class SateliteCard : Card
{
    public override void Init(CardInfo info)
    {
        base.Init(info);
    }

    public override void Activate()
    {
        base.Activate();

        if (Player.Instance == null)
            return;

        var sateliteManager = SpawnedItem.GetComponent<SateliteManager>();
        sateliteManager.numberOfSatelite = cardInfo.level1.value1;
        sateliteManager.spawnInterval = cardInfo.level1.value2;

        SpawnedItem.gameObject.SetActive(true);
    }

    public override void Deactivate()
    {
        base.Deactivate();
    }
}

public class DeadLaserCard : Card
{
    public override void Init(CardInfo info)
    {
        base.Init(info);
    }

    public override void Activate()
    {
        base.Activate();

        if (Player.Instance == null)
            return;

        var deadLaser = SpawnedItem.GetComponent<DeadLaserManager>();
        deadLaser.numberOfMissle = cardInfo.level1.value1;
        deadLaser.interval = cardInfo.level1.value2;

        SpawnedItem.gameObject.SetActive(true);
    }

    public override void Deactivate()
    {
        base.Deactivate();
    }
}


public class SpikeManagerCard : Card
{
    public override void Init(CardInfo info)
    {
        base.Init(info);
    }

    public override void Activate()
    {
        base.Activate();

        if (Player.Instance == null)
            return;

        var spikesManager = SpawnedItem.GetComponent<SpikesManager>();
        spikesManager.noOfHit = cardInfo.level1.value1;
        spikesManager.respawnInterval = cardInfo.level1.value2;

        SpawnedItem.gameObject.SetActive(true);
    }

    public override void Deactivate()
    {
        base.Deactivate();
    }
}