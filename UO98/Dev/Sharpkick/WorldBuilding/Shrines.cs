using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sharpkick.WorldBuilding
{
    class Shrines
    {
        public static void Generate()
        {
            ShrineInfo chaosShrine = new ShrineInfo()
            {
                LeftStartPoint = new Location(1456, 844, 5),
                Type = AnkhType.EastWest,
                Chaos = true,
            };
            chaosShrine.Create();

            ShrineInfo compassionShrine = new ShrineInfo()
            {
                LeftStartPoint = new Location(1857, 877, -1),
                Type = AnkhType.NorthSouth,
                Chaos = false,
            };
            compassionShrine.Create();

            ShrineInfo honestyShrine1 = new ShrineInfo()
            {
                LeftStartPoint = new Location(4208, 566, 42),
                Type = AnkhType.EastWest,
                Chaos = false,
            };
            honestyShrine1.Create();

            ShrineInfo honestyShrine2 = new ShrineInfo()
            {
                LeftStartPoint = new Location(4208, 562, 42),
                Type = AnkhType.EastWest,
                Chaos = false,
            };
            honestyShrine2.Create();

            ShrineInfo honorShrine = new ShrineInfo()
            {
                LeftStartPoint = new Location(1723, 3528, 5),
                Type = AnkhType.EastWest,
                Chaos = false,
            };
            honorShrine.Create();

            ShrineInfo humilityShrine = new ShrineInfo()
            {
                LeftStartPoint = new Location(4272, 3697, 0),
                Type = AnkhType.EastWest,
                Chaos = false,
            };
            humilityShrine.Create();

            ShrineInfo justiceShrine = new ShrineInfo()
            {
                LeftStartPoint = new Location(1300, 629, 21),
                Type = AnkhType.NorthSouth,
                Chaos = false,
            };
            justiceShrine.Create();

            ShrineInfo sacrificeShrine = new ShrineInfo()
            {
                LeftStartPoint = new Location(3354, 287, 4),
                Type = AnkhType.NorthSouthBloody,
                Chaos = false,
            };
            sacrificeShrine.Create();

            ShrineInfo spiritualityShrine = new ShrineInfo()
            {
                LeftStartPoint = new Location(1592, 2490, 20),
                Type = AnkhType.EastWest,
                Chaos = false,
            };
            spiritualityShrine.Create();

            ShrineInfo valorShrine = new ShrineInfo()
            {
                LeftStartPoint = new Location(1489, 3931, 2),
                Type = AnkhType.EastWest,
                Chaos = false,
            };
            valorShrine.Create();

            ShrineInfo destardShrine = new ShrineInfo()
            {
                LeftStartPoint = new Location(5204, 774, 0),
                Type = AnkhType.NorthSouth,
                Chaos = false,
            };
            destardShrine.Create();


        }

        enum AnkhType
        {
            EastWest,
            NorthSouth,
            NorthSouthBloody
        }

        private struct ShrineInfo
        {
            public Location LeftStartPoint;
            public AnkhType Type;
            public bool Chaos;

            public void Create()
            {
                ItemAndLocation[] allComponents = GetItems();
                foreach (ItemAndLocation piece in allComponents)
                    RemoveInvalidAnhkObjectsAtLocation(piece);

                IEnumerable<ItemAndLocation> piecesToAdd = allComponents.Where(p => !Builder.ItemExistsAtExactLocation(p));

                foreach (ItemAndLocation piece in piecesToAdd)
                    if (!Builder.ItemExistsAtExactLocation(piece) && !Builder.TryCreateItem(piece))
                        Console.WriteLine("Failed to create shrine piece {0}", piece);

                foreach (ItemAndLocation piece in allComponents)
                {
                    string result=Builder.AddScriptToFirstItemAtLocation(piece, Chaos ? "btshrine" : "lbshrine");
                    if (result != null && !result.StartsWith("Script already attached"))
                        Console.WriteLine("Shrine Error: Failed to attach script to piece @ {0} Message: {1}", piece, result);
                }
            }

            private ItemAndLocation[] GetItems()
            {
                ItemAndLocation[] toReturn = new ItemAndLocation[2];
                switch (Type)
                {
                    case AnkhType.NorthSouthBloody:
                        toReturn[0].ItemID = 7773;
                        toReturn[1].ItemID = 7772;
                        toReturn[0].Location = toReturn[1].Location = LeftStartPoint;
                        toReturn[1].Location.X++;
                        //toReturn[1].location.Y++;
                        break;
                    case AnkhType.NorthSouth:
                        toReturn[0].ItemID = 4;
                        toReturn[1].ItemID = 5;
                        toReturn[0].Location = toReturn[1].Location = LeftStartPoint;
                        toReturn[1].Location.X++;
                        //toReturn[1].location.Y++;
                        break;
                    default:
                        toReturn[0].ItemID = 2;
                        toReturn[1].ItemID = 3;
                        toReturn[0].Location = toReturn[1].Location = LeftStartPoint;
                        //toReturn[1].location.X++;
                        toReturn[1].Location.Y--;
                        break;
                }
                return toReturn;
            }

            private static int[] AllAnhkItemIDs = new int[] { 2, 3, 4, 5, 7772, 7773 };
            private void RemoveInvalidAnhkObjectsAtLocation(ItemAndLocation ankhPiece)
            {
                Location location=ankhPiece.Location;
                int ProperAnhkItemID = ankhPiece.ItemID;
                
                foreach (int id in AllAnhkItemIDs)
                {
                    if (id == ProperAnhkItemID) continue;
                    int serial = Builder.SerialOfFirstExistingItemAtLocation(ankhPiece);
                    if (serial > 0) Builder.DeleteItem(serial);
                }
            }
        }
    }
}
