using System;
using System.IO;
using System.Xml.Serialization;

public static class Serializer {
    static public void SaveXml(Game game, string datapath) {
        Type[] extraTypes = { typeof(Player) };
        XmlSerializer serializer = new XmlSerializer(typeof(Game), extraTypes);

        FileStream fs = new FileStream(datapath, FileMode.Create);
        serializer.Serialize(fs, game);
        fs.Close();
    }

    static public Game DeXml(string datapath) {
        Type[] extraTypes = { typeof(Player) };
        XmlSerializer serializer = new XmlSerializer(typeof(Game), extraTypes);

        FileStream fs = new FileStream(datapath, FileMode.Open);
        Game game = (Game)serializer.Deserialize(fs);
        fs.Close();

        return game;
    }
}