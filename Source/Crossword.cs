using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using TMPro;

public class Crossword : MonoBehaviour
{
  public TMP_InputField searchInput;
  private bool hasLoaded = false;
  public GameObject originalText;
  public TextMeshProUGUI extraWords;
  public TextMeshProUGUI infoText;
  private TextMeshProUGUI[][] texts;
  public bool useWords;
  public int size;

  public void CreateCrossword()
  {
    string text = searchInput.text;
    Verse v = Bible.GetVerseFromReference(text);
    if (useWords)
    {
      Verse ver = new Verse();
      ver.verse = text;
      v = ver;
    }

    if (v != null)
    {
      //remove punctuation
      string verse = v.verse.ToLower();
      string newVerse = "";
      string allowable = " qwertyuiopasdfghjklzxcvbnm";

      for (int i = 0; i < verse.Length; i++)
      {
        if (allowable.IndexOf(verse[i].ToString()) == -1)
        {
          //punctuaction
        } else {
          newVerse += verse[i].ToString();
        }
      }

      if (!hasLoaded)
      {
        hasLoaded = true;
        LoadInBoard();
      }

      string[] words = newVerse.Split(' ');

      GenerateCrossword(words);
    }
  }

  public void LoadInBoard()
  {
    texts = new TextMeshProUGUI[size][];

    float offset = (size * 1.35f);
    for (int i = 0; i < size; i++)
    {
      texts[i] = new TextMeshProUGUI[size];
      for (int k = 0; k < size; k++)
      {
        GameObject g = Instantiate(originalText, originalText.GetComponent<RectTransform>().anchoredPosition, Quaternion.identity);
        g.transform.SetParent(originalText.transform.parent, false);
        g.transform.localScale = Vector3.one;
        texts[i][k] = g.GetComponent<TextMeshProUGUI>();
        Vector3 tempPos = g.GetComponent<RectTransform>().anchoredPosition;
        tempPos.x += i * offset;
        tempPos.y -= k * offset;
        g.GetComponent<RectTransform>().anchoredPosition = tempPos;
      }
    }

    Destroy(originalText);
  }

  public void ToggleUseWords()
  {
    useWords = !useWords;
    Refresh();
  }

  public void Refresh()
  {
    if (useWords)
    {
      infoText.text = "Generate a crossword by typing words separated by spaces. ";
    } else {
      infoText.text = "Generate a crossword by a verse. ex: Esther 8:9";
    }
  }

  public void Setup()
  {
    Refresh();
    if (searchInput.text == "")
    {
      if (!useWords)
      {
        searchInput.text = "Esther 8:9";
        CreateCrossword();
      }
    }
  }

  public void GenerateCrossword(string[] words)
  {
    //takes in an array of words
    //and outputs a crossword in this format
    //string[]
    //"-h--"
    //"-a--"
    //"plan"
    //"-f--"

    //sort words longest to shortest
    Array.Sort(words, (y, x) => x.Length.CompareTo(y.Length));
    string[] wordsBackup = (string[]) words.Clone();

    //score tracking
    CrosswordClass best = new CrosswordClass();
    int bestScore = -10000;
    string[] bestWords = null;

    //k decides how many crosswords to generate and compare scores. The crossword with the highest score is displayed
    for (int k = 0; k < 10; k++)
    {
      //we have words, now generate crossword
      CrosswordClass cross = new CrosswordClass();
      cross.Init(size);
      words = (string[]) wordsBackup.Clone();

      //add first word
      if (CoinFlip())
      {
        cross.AddWord((int)(size / 2), (int)(size / 2 - words[0].Length / 2), words[0], true);
      } else {
        cross.AddWord((int)(size / 2 - words[0].Length / 2), (int)(size / 2), words[0], false);
      }

      words = words.Except(new string[]{words[0]}).ToArray();

      //now go through each word and add it to an open spot, 3 times
      for (int count = 0; count < 5; count++)
      {
        for (int i = 0; i < words.Length; i++)
        {
          Location loc = cross.FindSpotForWord(words[i]);
          if (loc != null)
          {
            if (!cross.AddWord(loc.x, loc.y, words[i], loc.isVertical))
            {
              print(words[i] + " could not fit into the crossword");
            } else {
              //sucess
              words = words.Except(new string[]{words[i]}).ToArray();
              i--;
            }
          }
        }
      }

      //score this crossword
      int score = 0;

      score -= words.Length * 250; //extra words decrease score
      score += ScoreCrossword(cross);

      if (score > bestScore)
      {
        bestScore = score;
        best = cross;
        bestWords = words;
      }
    }


    //finalize by displaying results
    for (int i = 0; i < size; i++)
    {
      for (int k = 0; k < size; k++)
      {
        texts[i][k].text = " ";
        if (best.data[i][k] != '-') texts[i][k].text = best.data[i][k].ToString().ToUpper();
      }
    }

    //and display extra words
    string extra = "";
    for (int i = 0; i < bestWords.Length; i++)
    {
      extra += bestWords[i] + "\n";
    }
    extraWords.text = extra;
    if (extra == "") extraWords.text = "no extra words";

    //PrintCrossword(cross);
  }

  public void PrintCrossword(CrosswordClass c)
  {
    string s = "";
    for (int i = 0; i < c.data.Length; i++)
    {
      for (int k = 0; k < c.data.Length; k++)
      {
        s += c.data[i][k];
      }
      s += "\n";
    }
    print(s);
  }

  public bool CoinFlip()
  {
    bool b = UnityEngine.Random.Range(0, 2) == 1;
    //print(b);
    return b;
  }

  public int ScoreCrossword(CrosswordClass c)
  {
    int score = 0;
    int minX = size + 1;
    int minY = size + 1;
    int maxX = -1;
    int maxY = -1;
    for (int i = 1; i < c.data.Length - 1; i++)
    {
      for (int k = 1; k < c.data.Length - 1; k++)
      {
        //check for these patterns
        //   O    OOO
        //  OOO   O O
        //   O    OOO
        //the more there are, the better the score
        if (c.data[i - 1][k] != '-' && c.data[i + 1][k] != '-')
        {
          if (c.data[i][k - 1] != '-' && c.data[i][k + 1] != '-')
          {
            //one found, first one detailed above
            score += 50;

            if (c.data[i - 1][k - 1] != '-' && c.data[i + 1][k + 1] != '-')
            {
              if (c.data[i + 1][k - 1] != '-' && c.data[i - 1][k + 1] != '-')
              {
                //one found, second one
                score += 150;
              }
            }
          }
        }
      }
    }
    //calculate max x and y and min x and y
    for (int i = 0; i < c.data.Length; i++)
    {
      for (int k = 0; k < c.data.Length; k++)
      {
        if (c.data[i][k] != '-')
        {
          if (i > maxX) maxX = i;
          if (i < minX) minX = i;
          if (k > maxY) maxY = k;
          if (k < minY) minY = k;
        }
      }
    }
    int sizeX = maxX - minX;
    int sizeY = maxY - minY;
    //the ideal size ratio is 1:1
    score += (int)(Mathf.Min(((float)sizeX / sizeY), ((float)sizeY / sizeX)) * 1000);

    return score;
  }
}





public class CrosswordClass
{
  public string[] data;

  public void Init(int size)
  {
    //a size 2 would make data equal ["--", "--"]
    data = new string[size];
    string s = "";
    int i = 0;

    for (i = 0; i < size; i++)
    {
      s += "-";
    }

    for (i = 0; i < size; i++)
    {
      data[i] = s;
    }
  }

  public bool AddWord(int x, int y, string word, bool isVertical)
  {
    //adds a word but stops and returns false if it does not fit

    //first make a buffer of the data
    string[] buffer = new string[data.Length];

    for (int i = 0; i < data.Length; i++)
    {
      string s = "";
      for (int k = 0; k < data.Length; k++)
      {
        s += data[i][k];
      }
      buffer[i] = s;
    }

    for (int i = 0; i < word.Length; i++)
    {
      if (isVertical)
      {
        if (OutOfBounds(y + i)) return false;
        ReplaceLetter(buffer, x, y + i, word[i]);
      } else {
        if (OutOfBounds(x + i)) return false;
        ReplaceLetter(buffer, x + i, y, word[i]);
      }
    }

    data = buffer;
    return true;
  }

  public void ReplaceLetter(string[] localData, int x, int y, char letter)
  {
    //given data, a position, and a letter, it replaces that letter
    localData[x] = localData[x].Substring(0, y) + letter.ToString() + localData[x].Substring(y + 1);
  }

  public bool OutOfBounds(int i)
  {
    //input a number, this returns true if it is out of the size of the crossword.
    return i < 0 || i >= data.Length;
  }

  public Location FindSpotForWord(string word)
  {
    //given a word, returns a location object that represents a spot that the word can go into, or null if there are none

    //algorithm:
    //find letters on the board that match letters in the word
    //find valid intersections of these letters
    //predict where it would go
    //check each letter to make sure it is empty/already the right letter
    //select a random one from the list of ones that work and return it

    //step 1: find letters on the board that match letters in the word
    List<Location>[] common = new List<Location>[word.Length];
    //this is an array of location lists
    //a list for each letter
    //each location in a list is an occurance of that letter.
    for (int i = 0; i < word.Length; i++)
    {
      common[i] = new List<Location>();
      //each letter
      for (int k = 0; k < data.Length; k++)
      {
        for (int j = 0; j < data.Length; j++)
        {
          //each position: check if it matches letter and add it to common
          if (word[i] == data[k][j])
          {
            Location loc = new Location();
            loc.x = k;
            loc.y = j;
            common[i].Add(loc);
          }
        }
      }
    }

    //step 2: for each possible intersection, determine if it works
    List<Location> possible = new List<Location>();

    for (int i = 0; i < common.Length; i++)
    {
      //each letter of the original word
      for (int k = 0; k < common[i].Count; k++)
      {
        //each intersection
        bool worksVertical = true; //innocent until proven guilty
        bool worksHorizontal = true;
        char c;

        for (int j = 0; j < word.Length; j++)
        {
          //each letter of the word as we attempt to fill the word in
          int offset = j - i; //how far from the intersection each letter should be
          //each letter of the word
          if (worksVertical)
          {
            if (!OutOfBounds(common[i][k].y + offset))
            {
              c = data[common[i][k].x][common[i][k].y + offset];
              //check if this works (vertically)
              if (c != '-' && c != word[j])
              {
                worksVertical = false;
              }
              //now check if the spots right and left to each spot are empty
              if (offset != 0 && c != word[j]) //make sure not to check for this at the intersection
              {
                if (!OutOfBounds(common[i][k].x - 1))
                {
                  c = data[common[i][k].x - 1][common[i][k].y + offset];
                  //check if this works (left of vertical spot)
                  if (c != '-')
                  {
                    worksVertical = false;
                  }
                }
                if (!OutOfBounds(common[i][k].x + 1))
                {
                  c = data[common[i][k].x + 1][common[i][k].y + offset];
                  //check if this works (right of vertical spot)
                  if (c != '-')
                  {
                    worksVertical = false;
                  }
                }
              }
            } else {
              worksVertical = false;
            }

          }
          if (worksHorizontal)
          {
            if (!OutOfBounds(common[i][k].x + offset))
            {
              c = data[common[i][k].x + offset][common[i][k].y];
              //check if this works (left)
              if (c != '-' && c != word[j])
              {
                worksHorizontal = false;
              }

              //now check if the spots above and below to each spot are empty
              if (offset != 0 && c != word[j])
              {
                if (!OutOfBounds(common[i][k].y - 1))
                {
                  c = data[common[i][k].x + offset][common[i][k].y - 1];
                  //check if this works (left of vertical spot)
                  if (c != '-')
                  {
                    worksHorizontal = false;
                  }
                }
                if (!OutOfBounds(common[i][k].y + 1))
                {
                  c = data[common[i][k].x + offset][common[i][k].y + 1];
                  //check if this works (right of vertical spot)
                  if (c != '-')
                  {
                    worksHorizontal = false;
                  }
                }
              }
            } else {
              worksHorizontal = false;
            }
          }
        }

        //now check the spots to the left and right of a horizontal word
        //above and below a vertical word
        if (worksHorizontal)
        {
          if (!OutOfBounds(common[i][k].x - i - 1))
          {
            c = data[common[i][k].x - i - 1][common[i][k].y];
            if (c != '-')
            {
              worksHorizontal = false;
            }
          }
          if (!OutOfBounds(common[i][k].x - i + word.Length))
          {
            c = data[common[i][k].x - i + word.Length][common[i][k].y];
            if (c != '-')
            {
              worksHorizontal = false;
            }
          }
        }

        if (worksVertical)
        {
          if (!OutOfBounds(common[i][k].y - i - 1))
          {
            c = data[common[i][k].x][common[i][k].y - i - 1];
            if (c != '-')
            {
              worksVertical = false;
            }
          }
          if (!OutOfBounds(common[i][k].y - i + word.Length))
          {
            c = data[common[i][k].x][common[i][k].y - i + word.Length];
            if (c != '-')
            {
              worksVertical = false;
            }
          }
        }

        //we have finished calculations for this intersection
        //if the intersection worked either horizontally or vertically, then note it
        if (worksHorizontal)
        {
          Location l = new Location();
          l.x = common[i][k].x - i;
          l.y = common[i][k].y;
          l.isVertical = false;
          possible.Add(l);
        }
        if (worksVertical)
        {
          Location l = new Location();
          l.x = common[i][k].x;
          l.y = common[i][k].y - i; //minus i so the word ends up with matching letters at the intersection
          l.isVertical = true;
          possible.Add(l);
        }
      }
    }

    //step 3: select a random intersection that works and return it
    int r = UnityEngine.Random.Range(0, possible.Count);
    if (possible.Count == 0) return null;
    return possible[r];
  }
}

public class Location
{
  public int x;
  public int y;
  public bool isVertical;
}
