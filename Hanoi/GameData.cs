using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.IO.IsolatedStorage;
using System.Xml.Serialization;

namespace Hanoi
{
    public class GameData
    {
        private const string gameDataFileName = "gameData.xml";

        public SaveGame SaveGame = new SaveGame();
        public GameSettings GameSettings = new GameSettings();

        public static GameData LoadGameData()
        {
            using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
            {
                //isf.DeleteFile(gameDataFileName);
                if (isf.FileExists(gameDataFileName))
                {
                    using (var stream = isf.OpenFile(gameDataFileName, System.IO.FileMode.Open))
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(GameData));
                        return (GameData)serializer.Deserialize(stream);
                    }
                }

                return new GameData();
            }
        }

        public static void SaveGameData(GameData gameData)
        {
            using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (isf.FileExists(gameDataFileName))
                    isf.DeleteFile(gameDataFileName);

                using (var stream = isf.OpenFile(gameDataFileName, System.IO.FileMode.CreateNew))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(GameData));
                    serializer.Serialize(stream, gameData);
                }
            }
        }
    }
}
