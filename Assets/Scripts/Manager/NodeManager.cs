using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeManager : MonoBehaviour
{
    public static NodeManager instance;
    
    public List<TowerBase> towerBaseList;
    private List<Node> nodeList;
    

    public List<Node> NodeList
    {
        get { return nodeList; }
    }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("More than one NodeManager in scene!");
        }
        
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        nodeList = new List<Node>();

        Node[] nodes= FindObjectsOfType<Node>();
        foreach (var node in nodes)
        {
            nodeList.Add(node);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BuildTower(TowerType type,Node node)
    {
        //Debug.Log(type);
        if (ResourceManager.Instance().Coin < towerBaseList[(int)type - 1].CoinCost[0] ||
            ResourceManager.Instance().Wood < towerBaseList[(int)type - 1].WoodCost[0] ||
            ResourceManager.Instance().Rock < towerBaseList[(int)type - 1].RockCost[0])
        {
            Debug.Log("need more coin");
            return;
        }

        //Debug.Log("Build tower");
        //Debug.Log($"type = {(int)type}");
        node.Tower = Instantiate(towerBaseList[(int)type - 1].TowerPrefab[0],
            node.transform.position + towerBaseList[(int)type - 1].TowerBuildOffset, Quaternion.identity);
        node.TowerType = type;
        ChangeResource(-towerBaseList[(int)type - 1].CoinCost[0], -towerBaseList[(int)type - 1].WoodCost[0],
            -towerBaseList[0].RockCost[(int)type - 1]);
        SetTowerData(node);
        Debug.Log($"Build {type}");

        HUDManager.instance.UpdateResourceText(ResourceType.ALL);
    }

    public void UpgradeTower(Node node)
    {
        int level = node.Tower.GetComponent<BaseTower>().Level;
        if (ResourceManager.Instance().Coin < towerBaseList[(int)node.TowerType - 1].CoinCost[level] ||
            ResourceManager.Instance().Wood < towerBaseList[(int)node.TowerType - 1].WoodCost[level] ||
            ResourceManager.Instance().Rock < towerBaseList[(int)node.TowerType - 1].RockCost[level])
        {
            Debug.Log("need more resource");
            return;
        }
        
        Debug.Log("Upgrade tower");
        Destroy(node.Tower);
        node.Tower.GetComponent<BaseTower>().Level = level + 1;
        node.Tower = Instantiate(towerBaseList[(int)TowerType.ARCHER].TowerPrefab[level - 1],
            node.transform.position + towerBaseList[1].TowerBuildOffset, Quaternion.identity);
        ChangeResource(-towerBaseList[(int)TowerType.ARCHER].CoinCost[level - 1],
            -towerBaseList[(int)TowerType.ARCHER].WoodCost[level - 1],
            -towerBaseList[(int)TowerType.ARCHER].RockCost[level - 1]);
        SetTowerData(node);

        // switch (node.TowerType)
        // {
        //     case TowerType.BASIC:
        //     {
        //         Destroy(node.Tower);
        //         node.Tower = Instantiate(towerBaseList[(int)TowerType.BASIC].TowerPrefab[level - 1],
        //             node.transform.position + towerBaseList[1].TowerBuildOffset, Quaternion.identity);
        //         node.Tower.GetComponent<BaseTower>().Level = level + 1;
        //         ChangeResource(-towerBaseList[(int)TowerType.BASIC].CoinCost[level],
        //             -towerBaseList[(int)TowerType.BASIC].WoodCost[level],
        //             -towerBaseList[(int)TowerType.BASIC].RockCost[level]);
        //         break;
        //     }
        // }
    }

    public void SellTower(Node node, int level)
    {
        Debug.Log("Sell " + node.TowerType);
        
        /*
        switch (node.TowerType)
        {
            case TowerType.ARCHER:
            {
                ChangeResource(towerBaseList[0].CoinCost[0], towerBaseList[0].WoodCost[0],
                    towerBaseList[0].RockCost[0]);
                break;
            }
            case TowerType.WOODEN:
            {
                ChangeResource(towerBaseList[1].CoinCost[0], towerBaseList[1].WoodCost[0],
                    towerBaseList[1].RockCost[0]);
                break;
            }
            default: break;
        }
        */

        ChangeResource(-towerBaseList[(int)node.TowerType - 1].CoinCost[level - 1],
            -towerBaseList[(int)node.TowerType - 1].WoodCost[level - 1],
            -towerBaseList[(int)node.TowerType - 1].RockCost[level - 1]);
        
        HUDManager.instance.UpdateResourceText(ResourceType.ALL);
        
        Destroy(node.Tower);
        node.TowerType = TowerType.NULL;
        
    }

    public void LoadTower(Node node, TowerType type, int level)
    {
        if (node.TowerType == type)
        {
            if (node.Tower != null && node.Tower.GetComponent<BaseTower>().Level != level)
            {
                //do nothing
            }
            else
            {
                return;
            }
        }
        else if (node.Tower != null)
        {
            Debug.Log("Destroy tower");
            Destroy(node.Tower);
            node.TowerType = TowerType.NULL;
        }
        /*
        switch (type)
        {
            case TowerType.ARCHER:
            {
                node.Tower = Instantiate(towerBaseList[0].TowerPrefab[level - 1],
                    node.transform.position + towerBaseList[0].TowerBuildOffset, Quaternion.identity);
                node.TowerType = TowerType.ARCHER;
                node.Tower.GetComponent<ArcherTower>().Level = level;
                Debug.Log("Load archer");
                break;
            }
            case TowerType.WOODEN:
            {
                node.Tower = Instantiate(towerBaseList[1].TowerPrefab[level - 1],
                    node.transform.position + towerBaseList[1].TowerBuildOffset, Quaternion.identity);
                node.TowerType = TowerType.WOODEN;
                node.Tower.GetComponent<ResourceTower>().Level = level;
                Debug.Log("Load wooden tower");
                break;
            }
            default: break;
        }
        */
        
        node.Tower = Instantiate(towerBaseList[(int)type - 1].TowerPrefab[0],
            node.transform.position + towerBaseList[(int)type - 1].TowerBuildOffset, Quaternion.identity);
        node.TowerType = type;
        node.Tower.GetComponent<BaseTower>().Level = level;
        SetTowerData(node);
    }

    public void ChangeResource(int coin = 0, int wood = 0, int rock = 0)
    {
        ResourceManager.Instance().ChangeCoin(coin);
        ResourceManager.Instance().ChangeWood(wood);
        ResourceManager.Instance().ChangeRock(rock);
    }

    private void SetTowerData(Node node)
    {
        int level = node.Tower.GetComponent<BaseTower>().Level;
        
        switch (node.TowerType)
        {
            case TowerType.ARCHER:
            case TowerType.FIRE:
            case TowerType.ICE:
            case TowerType.WATER:
            case TowerType.THUNDER:
            {
                node.Tower.GetComponent<AttackTower>().SetAttackTowerData(towerBaseList[(int)node.TowerType], level);
                break;
            }
            case TowerType.WOODEN:
            case TowerType.CRYSTAL:
            {
                break;
            }
            default: break;
        }
    }
}
