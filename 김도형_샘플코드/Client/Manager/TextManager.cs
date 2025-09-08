
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TextManager : MonoBehaviour
{
    public static TextManager Instance;

    private Dictionary<string, string> _localizedTexts = new Dictionary<string, string>();

    public enum Language
    {
        Korean,
        English,
        Japanese
    }

    public Language CurrentLanguage = Language.Korean;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadLanguageFile(CurrentLanguage);
    }

    public void LoadLanguageFile(Language lang)
    {
        string langFileName = lang switch
        {
            Language.Korean => "textkey",
            _ => "textkey"
        };

        TextAsset csvFile = Resources.Load<TextAsset>($"Localization/{langFileName}");

        if (csvFile == null)
        {
            Debug.LogError($"Localization file not found: {langFileName}");
            return;
        }

        _localizedTexts.Clear();

        using (StringReader reader = new StringReader(csvFile.text))
        {
            string line;
            bool isFirstLine = true;

            while ((line = reader.ReadLine()) != null)
            {
                if (isFirstLine)
                {
                    isFirstLine = false; // skip header
                    continue;
                }

                // , 로 split
                string[] tokens = line.Split(',');

                if (tokens.Length >= 2)
                {
                    string key = tokens[0].Trim();
                    string value = tokens[1].Trim();

                    if (!_localizedTexts.ContainsKey(key))
                        _localizedTexts.Add(key, value);
                }
            }
        }
    }

    public string GetText(string key)
    {
        if (_localizedTexts.TryGetValue(key, out var value))
        {
            return value;
        }
        return $"<NoKey:{key}>";
    }
}
