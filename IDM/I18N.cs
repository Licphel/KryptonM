using KryptonM.IO;

namespace KryptonM.IDM;

public class I18N
{

    public static Dictionary<string, Dictionary<string, IBinaryCompound>> Langs =
        new Dictionary<string, Dictionary<string, IBinaryCompound>>();

    public static string LangKey = "english";

    public static void Load(string namespc, string langkey, IBinaryCompound compound)
    {
        if(!Langs.ContainsKey(langkey)) Langs[langkey] = new Dictionary<string, IBinaryCompound>();
        Dictionary<string, IBinaryCompound> dict = Langs[langkey];
        if(dict.ContainsKey(namespc))
            dict[namespc].Merge(compound);
        else
            dict[namespc] = compound;
    }

    public static string GetText(string key, params string[] repmt)
    {
        return GetText(new ID(key), repmt);
    }

    public static string GetText(ID idt, params string[] repmt)
    {
        if(!Langs.ContainsKey(LangKey)) return idt.Full;

        IBinaryCompound compound = Langs[LangKey][idt.Space];
        var urep = compound.Search<string>(idt.Key) ?? idt.Full;

        var i = 0;
        foreach(var r in repmt) urep = urep.Replace("${" + i + "}", r);

        return urep;
    }

}