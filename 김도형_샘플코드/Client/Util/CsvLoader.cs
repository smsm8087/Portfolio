using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using UnityEngine;

public static class CsvLoader
{
    public static Dictionary<int, T> Load<T>(string resourcePath) where T : new()
    {
        var dict = new Dictionary<int, T>();

        TextAsset textAsset = Resources.Load<TextAsset>(resourcePath);
        if (textAsset == null)
        {
            Debug.LogError($"[CsvLoader] CSV 파일 없음: {resourcePath}");
            return dict;
        }

        var lines = textAsset.text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
        if (lines.Length < 2)
        {
            Debug.LogError($"[CsvLoader] CSV 파일 빈 내용: {resourcePath}");
            return dict;
        }

        var headers = lines[0].Split(',');
        var type = typeof(T);
        var fields = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        for (int i = 1; i < lines.Length; i++)
        {
            var cols = lines[i].Split(',');
            if (cols.Length < headers.Length)
                continue;

            T obj = new T();

            for (int j = 0; j < headers.Length; j++)
            {
                var header = headers[j].Trim();
                var field = Array.Find(fields, f => f.Name.Equals(header, StringComparison.OrdinalIgnoreCase));
                if (field == null) continue;

                try
                {
                    var rawValue = cols[j].Trim();

                    // List<T> 지원
                    if (field.PropertyType.IsGenericType &&
                        field.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
                    {
                        var elementType = field.PropertyType.GetGenericArguments()[0];

                        rawValue = rawValue.Trim('[', ']'); // [1,2,3] → 1,2,3
                        var stringValues = rawValue.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);

                        var list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(elementType));
                        foreach (var val in stringValues)
                        {
                            var trimmed = val.Trim().Trim('"');
                            object converted;

                            if (elementType == typeof(int))
                                converted = int.Parse(trimmed);
                            else if (elementType == typeof(float))
                                converted = float.Parse(trimmed, CultureInfo.InvariantCulture);
                            else if (elementType == typeof(double))
                                converted = double.Parse(trimmed, CultureInfo.InvariantCulture);
                            else if (elementType == typeof(string))
                                converted = trimmed;
                            else
                                converted = Convert.ChangeType(trimmed, elementType);

                            list.Add(converted);
                        }

                        field.SetValue(obj, list);
                    }
                    else
                    {
                        // 단일 값 처리
                        object value;
                        if (field.PropertyType == typeof(float))
                            value = float.Parse(rawValue, CultureInfo.InvariantCulture);
                        else if (field.PropertyType == typeof(double))
                            value = double.Parse(rawValue, CultureInfo.InvariantCulture);
                        else if (field.PropertyType == typeof(int))
                            value = int.Parse(rawValue);
                        else if (field.PropertyType == typeof(string))
                            value = rawValue.Trim('"');
                        else
                            value = Convert.ChangeType(rawValue, field.PropertyType);

                        field.SetValue(obj, value);
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"[CsvLoader] 변환 오류: {header} / 값: {cols[j]} / {ex.Message}");
                }
            }

            var keyField = Array.Find(fields, f => f.Name.Equals("id", StringComparison.OrdinalIgnoreCase));
            if (keyField == null)
            {
                Debug.LogError($"[CsvLoader] 기본키(Id) 없음 - {typeof(T).Name}");
                continue;
            }

            int key = (int)Convert.ChangeType(keyField.GetValue(obj), typeof(int));
            dict[key] = obj;
        }

        Debug.Log($"[CsvLoader] {typeof(T).Name} → {dict.Count}개 로드 완료!");
        return dict;
    }
}