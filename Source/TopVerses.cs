using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TopVerses : MonoBehaviour
{
  public TextMeshProUGUI verseText;
  public TextMeshProUGUI verse;
  public TextMeshProUGUI count;
  private int index = 0;
  private string[] topVerses = new string[]{
  "John 3:16",
  "John 1:1",
  "John 14:6",
  "Matthew 28:19",
  "Romans 3:23",
  "Ephesians 2:8",
  "Genesis 1:1",
  "Acts 1:8",
  "2 Timothy 3:16",
  "Romans 10:9",
  "Romans 6:23",
  "Acts 2:38",
  "John 1:12",
  "Romans 8:28",
  "John 1:9",
  "Genesis 1:26",
  "Romans 12:1",
  "Romans 5:8",
  "Matthew 28:18",
  "John 3:3",
  "Mark 16:15",
  "John 10:10",
  "John 1:14",
  "Acts 4:12",
  "Acts 2:42",
  "John 3:1",
  "Galatians 5:22",
  "Proverbs 3:5",
  "Jeremiah 29:11",
  "John 2:1",
  "Titus 3:5",
  "Romans 12:2",
  "John 14:1",
  "John 4:1",
  "Ephesians 4:11",
  "Romans 5:12",
  "Matthew 11:28",
  "Romans 5:1",
  "Genesis 1:27",
  "Romans 1:16",
  "1 John 1:9",
  "Acts 2:1",
  "2 Corinthians 5:17",
  "Hebrews 11:1",
  "2 Timothy 2:15",
  "Romans 8:1",
  "Romans 10:13",
  "John 8:32",
  "Isaiah 9:6",
  "John 14:15"
  };

  public void Refresh()
  {
    if (index < 0) index = 0;
    if (index > topVerses.Length - 1) index = topVerses.Length - 1;

    Verse v = Bible.GetVerseFromReference(topVerses[index]);

    if (v != null)
    {
      verseText.text = v.verse;
      verse.text = v.name;
    }

    count.text = (index + 1).ToString() + " / " + topVerses.Length;
  }

  public void Next()
  {
    index++;
    Refresh();
  }

  public void Previous()
  {
    index--;
    Refresh();
  }
}
