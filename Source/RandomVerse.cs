using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RandomVerse : MonoBehaviour
{
  public TextMeshProUGUI random;
  public TextMeshProUGUI reference;
  public int currentVerse;

  public void NewRandomVerse()
  {
    Verse v = Bible.RandomVerse();
    random.text = v.verse;
    reference.text = v.name;
    currentVerse = Bible.lastRandomVerseIndex;
  }

  public void NextVerse()
  {
    currentVerse++;
    Verse v = Bible.VerseAtIndex(currentVerse);
    random.text = v.verse;
    reference.text = v.name;
  }

  public void PreviousVerse()
  {
    currentVerse--;
    Verse v = Bible.VerseAtIndex(currentVerse);
    random.text = v.verse;
    reference.text = v.name;
  }
}
