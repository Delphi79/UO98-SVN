using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Sharpkick.WorldBuilding
{
    static class Decoration
    {
        static int created = 0;
        static int existed = 0;
        static int failed = 0;

        static string BaseDecoFilePath = Persistance.GetDataPathname("deco.txt");

        static string SkaraFerryDecoFilePath = Persistance.GetDataPathname("decoSkaraFerry.txt");

        public static void DecorateBase()
        {
            Decorate(Decoration.BaseDecoFilePath);
        }

        public static void DecorateSkaraFerry()
        {
            Decorate(Decoration.SkaraFerryDecoFilePath);
        }

        public static void Decorate(string decoFilePath)
        {
            created = existed = failed = 0;

            if (!File.Exists(decoFilePath))
                Console.WriteLine("Decorate: File not found: {0}", decoFilePath);
            else
                using (FileStream fs = new FileStream(decoFilePath, FileMode.Open, FileAccess.Read))
                using (StreamReader reader = new StreamReader(fs))
                    ProcessDecorationFile(reader);
        }

        static void ProcessDecorationFile(StreamReader reader)
        {
            string line;

            Console.WriteLine("Decorating");

            while ((line = reader.ReadLine()) != null)
            {
                if (line.StartsWith("#")) continue;

                DecorationDefinition decoration = DecorationDefinition.Instanciate(line);
                if (decoration != null)
                    TryCreateDecoration(decoration);
            }
            Console.WriteLine("Decorate: Total: {3} Existed: {0} Created {1} Failed {2}", existed, created, failed, existed + created + failed);
        }

        static void TryCreateDecoration(DecorationDefinition definition)
        {
            int serial = Builder.FindExistingItemSerial(definition.ItemAndLocation);

            if (serial == 0)
            {
                serial = Builder.TryCreateItemReturnSerial(definition.ItemAndLocation);
                if (serial != 0)
                {
                    ApplyPropertiesToNewItem(serial, definition);
                    created++;
                }
                else
                {
                    Console.WriteLine("Decorate Error: Failed to create {0}", definition);
                    failed++;
                }

            }
            else
                existed++;

            if (serial != 0 && Builder.isAtCreationLocation(serial))
                Builder.setHomeAndHeavy(serial);

        }

        static void ApplyPropertiesToNewItem(int serial, DecorationDefinition definition)
        {
            if (!string.IsNullOrEmpty(definition.name))
                Server.setObjVar(serial, "lookAtText", definition.name);

            if (definition.hue > 0)
                Server.setHue(serial, definition.hue);

            AddScriptsToItemByDefinitionClass(serial, definition);
        }

        static void AddScriptsToItemByDefinitionClass(int ItemSerial, DecorationDefinition definition)
        {
            string result = null;
            switch (definition.ItemClass)
            {
                case "PublicMoongate":
                    result = Server.addScript(ItemSerial, "moongate");
                    if (result == null)
                    {
                        int gateid = GetGateID(definition.ItemAndLocation.Location);
                        Server.setObjVar(ItemSerial, "gateID", gateid);
                    }
                    break;
                case "RejuvinationAddonComponent":
                    if (definition.ItemAndLocation.ItemID == 2 || definition.ItemAndLocation.ItemID == 4)
                        result = Server.addScript(ItemSerial, "des1_ankh_2");
                    else
                        result = Server.addScript(ItemSerial, "des1_ankh");
                    break;
            }
            if (result != null)
                Console.WriteLine("Decorate Error: Failed to attach script to {0} Message: {1}", definition, result);
        }

        static int GetGateID(Location gateLocation)
        {
            short x = gateLocation.X;
            short y = gateLocation.Y;
            if (x == 4467 && y == 1283) return 0;
            else if (x == 1336 && y == 1997) return 1;
            else if (x == 1499 && y == 3771) return 2;
            else if (x == 771 && y == 752) return 3;
            else if (x == 2701 && y == 692) return 4;
            else if (x == 1828 && y == 2948) return 5;
            else if (x == 643 && y == 2067) return 6;
            else if (x == 3563 && y == 2139) return 7;
            return 0;
        }

       class DecorationDefinition
       {
           public ItemAndLocation ItemAndLocation;
           public short hue = 0;
           public string name = null;
           public string ItemClass = null;

           static char[] stringSplitSeperators = new char[] { ' ' };

           public bool isDoor
           {
               get
               {
                   return Builder.isDoor(ItemAndLocation.ItemID);
               }
           }

           public static DecorationDefinition Instanciate(string definition)
           {

               string[] DefinitionAndClass = definition.Split(';');
               
               if (DefinitionAndClass.Length < 1)
                   return null;

               string[] itemAndLocationArray = DefinitionAndClass[0].Split(stringSplitSeperators, StringSplitOptions.RemoveEmptyEntries);

               ItemAndLocation DecorationItemAndLocation;
               if (!TryParseItemAndLocation(itemAndLocationArray, out DecorationItemAndLocation))
                   return null;

               DecorationDefinition decoration = new DecorationDefinition()
               {
                   ItemAndLocation = DecorationItemAndLocation
               };

               decoration.hue = (short)TryExtractIntFromArray(itemAndLocationArray, 4);
               decoration.name = TryExtractNameFromArray(itemAndLocationArray, 5, itemAndLocationArray.Length - 5);

               decoration.ItemClass = DefinitionAndClass.Length > 1 ? DefinitionAndClass[1].Trim() : null;

               return decoration;
           }

           static bool TryParseItemAndLocation(string[] itemAndLocationArray, out ItemAndLocation itemAndLocation)
           {
               ushort id;
               Location loc;

               if (itemAndLocationArray.Length >= 4 &&
                   ushort.TryParse(itemAndLocationArray[0], out id) &&
                   short.TryParse(itemAndLocationArray[1], out loc.X) &&
                   short.TryParse(itemAndLocationArray[2], out loc.Y) &&
                   short.TryParse(itemAndLocationArray[3], out loc.Z))
               {
                   ItemAndLocation toReturn = new ItemAndLocation()
                   {
                       ItemID = id,
                       Location = loc,
                   };
                   itemAndLocation = toReturn;
                   return true;
               }
               itemAndLocation = new ItemAndLocation();
               return false;

           }

           static int TryExtractIntFromArray(string[] array, int position)
           {
               int hue = 0;
               if (array.Length > position && int.TryParse(array[position], out hue))
                   return hue;
               else
                   return 0;
           }

           static string TryExtractNameFromArray(string[] array, int startIndex, int ElementCount)
           {
               if (array.Length >= startIndex + 1)
                   return string.Join(" ", array, startIndex, ElementCount);
               else
                   return null;
           }

           public override string ToString()
           {
               return string.Format("{0} @ {1}", ItemClass ?? ItemAndLocation.ItemID.ToString(), ItemAndLocation.Location);
           }
       }
    }

}
