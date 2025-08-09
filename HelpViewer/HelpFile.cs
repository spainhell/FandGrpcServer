using System.Text;
using HelpViewer.Helpers;

namespace HelpViewer;

public class HelpFile
{
    private const int MPageSize = 512;
    private Dictionary<string, string> _helpKeys = new();

    public HelpFile(string fand0Name, string fandTname)
    {
        ProcessHelpFile(fand0Name, fandTname);
    }

    public Dictionary<string, string> GetRecords()
    {
        return _helpKeys;
    }

    private void ProcessHelpFile(string fand0Name, string fandTname)
    {
        using var fand0 = new FileStream(fand0Name, FileMode.Open, FileAccess.Read);
        using var reader0 = new BinaryReader(fand0);
        using var fandT = new FileStream(fandTname, FileMode.Open, FileAccess.Read);
        using var readerT = new BinaryReader(fandT);

        // Read header
        int recordCount = reader0.ReadInt32(); // 4 bytes - TODO: could be negative for index files
        int recordLength = reader0.ReadUInt16(); // 2 bytes

        // Parse records
        for (int i = 0; i < recordCount; i++)
        {
            // Read string (35 bytes)
            byte[] kamenickyBytes = reader0.ReadBytes(recordLength - 4);
            string key = KamToUtfConvertor.ConvertKamToUtf(kamenickyBytes).TrimEnd();

            // Read position of text in T file (4 bytes)
            uint pointerT = reader0.ReadUInt32();

            string text;
            // Read text from T file
            if (pointerT == 0)
            {
                // No text
                text = string.Empty;
            }
            else
            {
                text = ReadTextFromTFile(readerT, pointerT);
                
                // remove text between { }
                int start = text.IndexOf('{');
                int end = text.IndexOf('}');
                while (start != -1 && end != -1 && start < end)
                {
                    text = text.Remove(start, end - start + 1);
                    if (text.StartsWith("\r\n"))
                    {
                        text = text.Remove(0, 2);
                    }
                    else if (text.StartsWith("\r"))
                    {
                        text = text.Remove(0, 1);
                    }
                    else if (text.StartsWith("\n"))
                    {
                        text = text.Remove(0, 1);
                    }
                    start = text.IndexOf('{');
                    end = text.IndexOf('}');
                }
                
                // replace new lines with <br>
                text = text.Replace("\r\n", "<br>").Replace("\r", "<br>").Replace("\n", "<br>");
                
                text = ProcessLinks(text);
                
                //text = ReplaceNonPrintableChars(text);
            }

            // Add to dictionary
            _helpKeys[key.ToLower()] = text;
        }
    }

    private string ReadTextFromTFile(BinaryReader reader, uint pointerT)
    {
        string result = string.Empty;

        // Read text from T file
        if (pointerT == 0)
        {
            // no text
        }
        else
        {
            reader.BaseStream.Seek(pointerT, SeekOrigin.Begin);
            int textLength = reader.ReadUInt16(); // 2 bytes
            pointerT += 2;
            int rest = MPageSize - ((int)pointerT & (MPageSize - 1));

            while (textLength > rest)
            {
                int L = rest - 4;
                reader.BaseStream.Seek(pointerT, SeekOrigin.Begin);
                byte[] kamenickyBytes = reader.ReadBytes(L);
                result += KamToUtfConvertor.ConvertKamToUtf(kamenickyBytes);
                textLength -= L;
                pointerT = reader.ReadUInt32(); // next page
                rest = MPageSize;
            }

            // read last page
            reader.BaseStream.Seek(pointerT, SeekOrigin.Begin);
            byte[] lastPageBytes = reader.ReadBytes(textLength);
            result += KamToUtfConvertor.ConvertKamToUtf(lastPageBytes);
        }

        return result;
    }
    
    private string ReplaceNonPrintableChars(string text)
    {
        StringBuilder sb = new StringBuilder(text.Length);
        foreach (char c in text)
        {
            if (c >= 32)
            {
                sb.Append(c);
            }
            else
            {
                sb.Append('*');
            }
        }
        return sb.ToString();
    }

    private string ProcessLinks(string text)
    {
        // there are links between \x11 in the text -> replace with <a href="...">
        bool odd19 = true;
        bool odd2 = true;
        const string a_href_open = "<a href=\"?key=";
        const string a_href_mid = "\">";
        const string a_href_close = "</a>";
        string a_text = string.Empty;
        
        StringBuilder sb = new StringBuilder(text.Length);
        foreach (char c in text)
        {
            if (c == 2)
            {
                if (odd2)
                {
                    sb.Append("<span style=\"color:red;\">");
                }
                else
                {
                    sb.Append("</span>");
                }
                odd2 = !odd2;
            }
            else if (c == 19)
            {
                if (odd19)
                {
                    sb.Append(a_href_open);
                }
                else
                {
                    sb.Append(Uri.EscapeDataString(a_text));
                    sb.Append(a_href_mid);
                    sb.Append(a_text);
                    sb.Append(a_href_close);
                    a_text = string.Empty;
                }
                odd19 = !odd19;
            }
            else if (!odd19)
            {
                a_text += c;
            }
            else
            {
                sb.Append(c);
            }
        }
        
        return sb.ToString();
    }

    public string GetHelp(string keyValue)
    {
        string result = string.Empty;
        bool found = false;
        
        if (_helpKeys.ContainsKey(keyValue.ToLower()))
        {
            foreach (KeyValuePair<string, string> pair in _helpKeys)
            {
                if (found && (pair.Value.Length != 0))
                {
                    result = pair.Value;
                    break;
                }
                if (pair.Key == keyValue.ToLower()) 
                {
                    found = true;
                    if (pair.Value.Length != 0)
                    {
                        result = pair.Value;
                        break;
                    }
                }
            }
        }

        return result;
    }
}