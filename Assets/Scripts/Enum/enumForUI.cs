using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enumForUI : MonoBehaviour
{
    //[SerializeField] ButtonToTower BuildThisType;
    [SerializeField] ButtonToTower BuildThisType;


    public void BuildThisTower()
    {
        switch (BuildThisType)
        {
            case ButtonToTower.Archer:
            {
                NodeManager.instance.BuildTower(TowerType.ARCHER, CreateTowerUI.instance.selectNode);
                break;
            }
            case ButtonToTower.Fire:
            {
                NodeManager.instance.BuildTower(TowerType.FIRE, CreateTowerUI.instance.selectNode);
                break;
            }
            case ButtonToTower.Ice:
            {
                NodeManager.instance.BuildTower(TowerType.ICE, CreateTowerUI.instance.selectNode);
                break;
            }

        }
    }
}

enum ButtonToTower
{
    Archer, Fire, Water, Ice
}