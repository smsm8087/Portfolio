using System;
using System.Collections.Generic;
using UnityEngine;

public static class InputKeyParser
{
    // 흔히 쓰는 별칭/단축어 매핑
    private static readonly Dictionary<string, KeyCode> Alias = new(StringComparer.OrdinalIgnoreCase)
    {
        { "LEFT", KeyCode.LeftArrow }, { "RIGHT", KeyCode.RightArrow },
        { "UP", KeyCode.UpArrow },     { "DOWN", KeyCode.DownArrow },
        { "SPACE", KeyCode.Space },    { "ENTER", KeyCode.Return },
        { "RETURN", KeyCode.Return },  { "ESC", KeyCode.Escape }, { "ESCAPE", KeyCode.Escape },
        { "LCTRL", KeyCode.LeftControl },  { "RCTRL", KeyCode.RightControl },
        { "LSHIFT", KeyCode.LeftShift },   { "RSHIFT", KeyCode.RightShift },
        { "LALT", KeyCode.LeftAlt },       { "RALT", KeyCode.RightAlt },
        { "MOUSE0", KeyCode.Mouse0 }, { "MOUSE1", KeyCode.Mouse1 }, { "MOUSE2", KeyCode.Mouse2 },
    };

    /// <summary>
    /// CSV의 default_key 문자열을 Unity KeyCode로 변환.
    /// 예) "Q" -> KeyCode.Q, "Left" -> LeftArrow, "F5" -> F5
    /// </summary>
    public static bool TryParse(string keyText, out KeyCode key)
    {
        key = KeyCode.None;
        if (string.IsNullOrWhiteSpace(keyText)) return false;

        var s = keyText.Trim();

        // 별칭 우선
        if (Alias.TryGetValue(s, out key))
            return true;

        // F키 (F1~F15)
        if (s.Length >= 2 && (s[0] == 'F' || s[0] == 'f'))
        {
            if (int.TryParse(s.Substring(1), out int f) && f >= 1 && f <= 15)
            {
                key = (KeyCode)Enum.Parse(typeof(KeyCode), $"F{f}", true);
                return true;
            }
        }

        // 한 글자 알파벳/숫자 (A~Z, 0~9)
        if (s.Length == 1)
        {
            char c = char.ToUpperInvariant(s[0]);
            if (c >= 'A' && c <= 'Z')
            {
                key = (KeyCode)Enum.Parse(typeof(KeyCode), c.ToString(), true);
                return true;
            }
            if (c >= '0' && c <= '9')
            {
                key = (KeyCode)Enum.Parse(typeof(KeyCode), $"Alpha{c}", true);
                return true;
            }
        }

        // KeyCode 이름과 동일하게 온 경우 (예: "LeftArrow", "BackQuote"...)
        if (Enum.TryParse<KeyCode>(s, true, out var parsed))
        {
            key = parsed;
            return true;
        }

        Debug.LogWarning($"[InputKeyParser] 지원하지 않는 키 문자열: '{keyText}'");
        return false;
    }
}
