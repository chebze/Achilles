using Achilles.Habbo.Utilities;

namespace Achilles.Habbo.Messaging.Abstractions.Incoming;

public class IncomingMessageContent
{
    public string Content { get; }

    public int Index { get; private set; }
    public int Length { get => Content.Length; }
    public int Remaining { get => Length - Index; }

    public IncomingMessageContent(string content)
    {
        Content = content;
        Index = 0;
    }

    public int ReadWiredInt()
    {
        int value = HabboEncoding.VL64.Decode(Content.Substring(Index));
        Index += HabboEncoding.VL64.Encode(value).Length;
        return value;
    }
    public int ReadBase64Int()
    {
        int value = HabboEncoding.Base64.Decode(Content.Substring(Index, 2));
        Index += 2;
        return value;
    }
    public int ReadBase64Int(int length)
    {
        int value = HabboEncoding.Base64.Decode(Content.Substring(Index, length));
        Index += length;
        return value;
    }
    public bool ReadWiredBoolean()
    {
        int value = ReadWiredInt();
        return value == 1 ? true : false;
    }
    public bool ReadBase64Boolean()
    {
        int value = HabboEncoding.Base64.Decode(Content.Substring(Index, 1));
        Index += 1;
        return value == 1 ? true : false;
    }
    public string ReadString()
    {
        int length = HabboEncoding.Base64.Decode(Content.Substring(Index, 2));
        string value = Content.Substring(Index + 2, length);
        Index += 2 + length;
        return value;
    }
    public string ReadRemainingString()
    {
        string value = Content.Substring(Index);
        Index = Content.Length;
        return value;
    }

    public void SkipString(string value)
    {
        if(Content.Substring(Index, value.Length) == value)
            Index += value.Length;
    }
    public string ReadUntil(string value)
    {
        int index = Content.IndexOf(value, Index);
        if(index == -1)
            return ReadRemainingString();
        string result = Content.Substring(Index, index - Index);
        Index = index + value.Length;
        return result;
    }
    public string ReadUntil(char value)
    {
        int index = Content.IndexOf(value, Index);
        if(index == -1)
            return ReadRemainingString();
        string result = Content.Substring(Index, index - Index);
        Index = index + 1;
        return result;
    }


    public string ReadLength(int length)
    {
        string result = Content.Substring(Index, length);
        Index += length;
        return result;
    }
    public bool ReadIntegerBoolean()
    {
        string value = Content.Substring(Index, 1);
        Index += 1;
        return value == "1" ? true : (value == "0" ? false : throw new Exception("Invalid integer boolean: " + value));
    }
    public char ReadChar()
    {
        char value = Content[Index];
        Index += 1;
        return value;
    }
    public char Peek() => Content[Index];
    public void Skip(int count = 1)
    {
        Index += count;
    }
}