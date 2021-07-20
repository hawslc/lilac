using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class VerseLookup : MonoBehaviour
{
  public TMP_InputField searchInput;
  public TextMeshProUGUI verseText;
  public TextMeshProUGUI verse;
  private int index = 0;

  public void Search()
  {
    string text = searchInput.text;

    Verse v = Bible.GetVerseFromReference(text); //try entering in the reference directly

    if (v != null)
    {
      verseText.text = v.verse;
      verse.text = v.name;
      searchInput.text = v.name;
      index = Bible.IndexFromReference(v.name);
    }
  }

  public void Setup()
  {
    //called when page changes to this
    if (searchInput.text == "")
    {
      searchInput.text = "Genesis 1:1";
      Search();
    }
  }

  public void NextVerse()
  {
    index++;
    Verse v = Bible.VerseAtIndex(index);
    verseText.text = v.verse;
    verse.text = v.name;
    searchInput.text = v.name;
  }

  public void PreviousVerse()
  {
    index--;
    Verse v = Bible.VerseAtIndex(index);
    verseText.text = v.verse;
    verse.text = v.name;
    searchInput.text = v.name;
  }


}
