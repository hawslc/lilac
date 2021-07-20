using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameMaster : MonoBehaviour
{
  public List<GameObject> pages;
  public TMP_Dropdown pageChanger;
  public int startPage;

  void Start()
  {
    Bible.Init();
    pageChanger.value = startPage;
    DropdownChanged();
    this.gameObject.GetComponent<RandomVerse>().NewRandomVerse();
  }

  public void DropdownChanged()
  {
    for (int i = 0; i < pages.Count; i++)
    {
      pages[i].SetActive(false);
    }
    pages[pageChanger.value].SetActive(true);

    if (pageChanger.value == 1)
    {
      this.gameObject.GetComponent<VerseLookup>().Setup();
    }
    if (pageChanger.value == 2)
    {
      this.gameObject.GetComponent<KeywordSearch>().Setup();
    }
    if (pageChanger.value == 3)
    {
      this.gameObject.GetComponent<Crossword>().Setup();
    }
    if (pageChanger.value == 4)
    {
      this.gameObject.GetComponent<TopVerses>().Refresh();
    }
  }
}
