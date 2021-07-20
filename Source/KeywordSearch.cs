using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class KeywordSearch : MonoBehaviour
{
  public TMP_InputField searchInput;
  public TMP_InputField countInput;
  public TextMeshProUGUI verseText;
  public TextMeshProUGUI verse;
  public TextMeshProUGUI countText;
  public List<Verse> results = new List<Verse>();
  public int index = 0;

  public void Search()
  {
    string text = searchInput.text.ToLower();

    Verse[] data = Bible.GetBibleData().data; //list of all the Verses
    results = new List<Verse>();

    for (int i = 0; i < data.Length; i++)
    {
      if (data[i].verse.ToLower().IndexOf(text) != -1)
      {
        results.Add(data[i]);
      }
    }

    index = 0;

    if (results.Count > 0)
    {
      Refresh();
    }
  }

  public void Setup()
  {
    //called when page changes to this
    if (searchInput.text == "")
    {
      searchInput.text = "Hope";
      Search();
    }
  }

  public void NextVerse()
  {
    index++;
    Refresh();
  }

  public void PreviousVerse()
  {
    index--;
    Refresh();
  }

  public void Refresh()
  {
    //refreshes UI
    if (index < 0) index = 0;
    if (index > results.Count - 1) index = results.Count - 1;
    verse.text = results[index].name;
    verseText.text = results[index].verse;
    countText.text = "/ " + results.Count;
    countInput.text = (index + 1).ToString();

    //capitalize search phrase
    string s = results[index].verse;
    int start = s.ToLower().IndexOf(searchInput.text.ToLower());
    if (start != -1)
    {
      string t = s.Substring(0, start) + searchInput.text.ToUpper() + s.Substring(start + searchInput.text.Length);
      verseText.text = t;
    }
  }

  public void CountInputChange()
  {
    //count input changed
    int newIndex = -1;
    Int32.TryParse(countInput.text, out newIndex);

    if (newIndex != -1)
    {
      //its a number
      index = newIndex;
      Refresh();
    }
  }
}
