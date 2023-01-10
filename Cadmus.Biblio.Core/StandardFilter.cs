using Fusi.Text.Unicode;
using System.Text;

namespace Cadmus.Biblio.Core;

/// <summary>
/// A simple, general purpose text filter. This preserves
/// only letters, apostrophes and whitespaces, also removing any diacritics
/// from the letters and lowercasing them. Whitespaces are flattened into
/// spaces and normalized. Digits are dropped (by default) or preserved
/// according to the options specified.
/// </summary>
public static class StandardFilter
{
    private static readonly UniData _ud = new();

    /// <summary>
    /// Apply this filter to the specified text, by keeping only
    /// letters/apostrophe and whitespaces. All the diacritics are removed,
    /// and uppercase letters are lowercased. Whitespaces are normalized
    /// and flattened into space, and get trimmed if initial or final.
    /// </summary>
    /// <param name="text">The text to apply this filter to.</param>
    /// <returns>Filtered text.</returns>
    /// <param name="preserveDigits">True to preserve digits;
    /// false to drop them.</param>
    public static string Apply(string text, bool preserveDigits)
    {
        if (string.IsNullOrEmpty(text)) return text;

        StringBuilder sb = new();
        bool prevWS = true;

        foreach (char c in text)
        {
            switch (c)
            {
                case '\'':
                    sb.Append('\'');
                    prevWS = false;
                    break;
                default:
                    if (char.IsWhiteSpace(c))
                    {
                        if (prevWS) break;
                        sb.Append(' ');
                        prevWS = true;
                        break;
                    }
                    if (char.IsLetter(c))
                    {
                        char seg = _ud.GetSegment(c, true);
                        if (seg != 0) sb.Append(char.ToLower(seg));
                        prevWS = false;
                        break;
                    }
                    if (preserveDigits && char.IsDigit(c))
                    {
                        sb.Append(c);
                        break;
                    }
                    prevWS = false;
                    break;
            }
        }

        // right trim
        if (sb.Length > 0 && sb[sb.Length - 1] == ' ')
            sb.Remove(sb.Length - 1, 1);

        return sb.ToString();
    }
}
