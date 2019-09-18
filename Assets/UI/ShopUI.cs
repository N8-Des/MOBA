using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopUI : MonoBehaviour {

    public List<GameObject> trees = new List<GameObject>();
    public GameObject shop;
    [SerializeField]
    private GameObject xButton;
    [SerializeField]
    private GameObject backButton;
    public void XButton()
    {
        foreach (GameObject tree in trees)
        {
            tree.SetActive(false);
        }
        shop.gameObject.SetActive(false);
        backButton.SetActive(false);
        xButton.SetActive(false);
    }
    public void BackButton()
    {
        foreach (GameObject tree in trees)
        {
            tree.SetActive(false);
        }
        shop.gameObject.SetActive(true);
        backButton.SetActive(false);
        xButton.SetActive(false);
    }
    public void openShop()
    {
        shop.SetActive(true);
        xButton.SetActive(true);
    }
    public void openTree()
    {
        shop.SetActive(false);
        backButton.SetActive(true);
        xButton.SetActive(true);
    }
}
