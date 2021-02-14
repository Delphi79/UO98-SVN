using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sharpkick.WorldBuilding
{
    class Teleporters
    {
        private static int created = 0;
        private static int existed = 0;
        private static int failed = 0;

        public enum TeleporterGraphic
        {
            NoDraw=1,
            Sparkles=4435,
            PentagramCenter=4074,
            MGtoLycauem = 6117,
            MGtoNorth = 6126,
            MGtoTelescope = 6108,
            MGtoSouth = 6126,
            MGreturn = 6153,
            jhelomNorth=6126,
            jhelomSouth=6144,
            jhelomReturn = 6153,
        }

        public static void GenerateBaseTeleporters()
        {
            DungeonDespise();
            DungeonWrong();
            DungeonDestard();
            DungeonDeceit();
            DungeonShame();

            Wind();
            SpiritualityShrine();
            Moonglow();
            Jhelom();
            BucsDen();
        }

        public static void GenerateSkaraFerryTeleporters()
        {
            SkaraFerry();
        }

        private static void DungeonDespise()
        {
            TeleporterWithFixedDest DespiseTeleporters = new TeleporterWithFixedDest("despise_stairs");
            DespiseTeleporters.CreateAt(new Location(5572, 629, 45));
            DespiseTeleporters.CreateAt(new Location(5571, 632, 10));
            DespiseTeleporters.CreateAt(new Location(5505, 570, 39));
            DespiseTeleporters.CreateAt(new Location(5523, 673, 35));
            DespiseTeleporters.CreateAt(new Location(5385, 756, -13));
            DespiseTeleporters.CreateAt(new Location(5411, 859, 62));
        }

        private static void DungeonWrong()
        {
            TeleporterWithFixedDest WrongTele1 = new TeleporterWithFixedDest("despise_teleporter_one");
            WrongTele1.CreateAt(new Location(5792, 525, 10));   // despise_teleporter_one -> wrong 3 center 5703, 639, 0

            TeleporterWithFixedDest WrongTele2 = new TeleporterWithFixedDest("despise_teleporter_two");
            WrongTele2.CreateAt(new Location(5698, 662, 0));    // despise_teleporter_two -> wrong 1 north west 5792, 525, 10

            TeleporterWithFixedDest WrongTele3 = new TeleporterWithFixedDest("despise_teleporter_three");
            WrongTele3.CreateAt(new Location(5827, 593, 0));  // despise_teleporter_three -> wrong 2 south 5690, 569, 25

            TeleporterWithFixedDest WrongTele4 = new TeleporterWithFixedDest("despise_teleporter_three");
            WrongTele4.CreateAt(new Location(5827, 594, 0));  // despise_teleporter_three -> wrong 2 south 5690, 569, 25

            TeleporterWithFixedDest WrongTele5 = new TeleporterWithFixedDest("despise_teleporter_four");
            WrongTele5.CreateAt(new Location(5690, 569, 25));  // despise_teleporter_four -> wrong 1 south 5827, 593, 0
        }

        private static void DungeonDestard()
        {
            // Destard: see also dest_tele_one
            TeleporterWithFixedDest DestardTeleporters = new TeleporterWithFixedDest("des_stairs");
            DestardTeleporters.CreateAt(new Location(5130, 908, -22));
            DestardTeleporters.CreateAt(new Location(5144, 797, 22));
            DestardTeleporters.CreateAt(new Location(5152, 810, -19));
            DestardTeleporters.CreateAt(new Location(5133, 985, 22));
            // Unknown: dest_tele_one
        }
        
        private static void DungeonDeceit()
        {
            // Deceit: there are two similar scripts for Deceit teleporters, dec_stairs and dec_teleport.
            TeleporterWithFixedDest DeceitTeleporters = new TeleporterWithFixedDest("dec_stairs");
            DeceitTeleporters.CreateAt(new Location(5217, 587, -20));
            DeceitTeleporters.CreateAt(new Location(5305, 531, 10));
            DeceitTeleporters.CreateAt(new Location(5347, 578, -20));
            DeceitTeleporters.CreateAt(new Location(5137, 650, 15));
            DeceitTeleporters.CreateAt(new Location(5218, 762, -35));
            DeceitTeleporters.CreateAt(new Location(5306, 649, 0));
        }

        private static void DungeonShame()
        {            
            // Shame: see also sha_tele_new. I think the below is the original configuration.
            //  These are all designed to be on pedestals (5 Z above floor)
            TeleporterWithFixedDest ShameTeleporters = new TeleporterWithFixedDest("sha_stairs");
            ShameTeleporters.CreateAt(new Location(5491, 18, -52));
            ShameTeleporters.CreateAt(new Location(5512, 8, 0));
            ShameTeleporters.CreateAt(new Location(5512, 148, 20));
            ShameTeleporters.CreateAt(new Location(5602, 101, -23));
            ShameTeleporters.CreateAt(new Location(5873, 17, 10));
            ShameTeleporters.CreateAt(new Location(5516, 174, -23));

            TeleporterWithFixedDest ShameTeleporter1 = new TeleporterWithFixedDest("sha_teleporter", TeleporterGraphic.Sparkles);
            ShameTeleporter1.CreateAt(new Location(5548, 115, 0));
            ShameTeleporter1.CreateAt(new Location(5579, 52, 5));

            TeleporterWithFixedDest ShameTeleporter2 = new TeleporterWithFixedDest("sha_teleporter2", TeleporterGraphic.Sparkles);
            ShameTeleporter2.CreateAt(new Location(5584, 187, 5));
            ShameTeleporter2.CreateAt(new Location(5448, 179, 5));

            TeleporterWithFixedDest ShameTeleporter3 = new TeleporterWithFixedDest("sha_teleporter3", TeleporterGraphic.Sparkles);
            ShameTeleporter3.CreateAt(new Location(5507, 187, 5));
            ShameTeleporter3.CreateAt(new Location(5538, 170, 5));

            TeleporterWithFixedDest ShameTeleporter4 = new TeleporterWithFixedDest("sha_teleporter4", TeleporterGraphic.Sparkles);
            ShameTeleporter4.CreateAt(new Location(5507, 162, 5));
            ShameTeleporter4.CreateAt(new Location(5498, 178, 5));

            TeleporterWithFixedDest ShameTeleporter5 = new TeleporterWithFixedDest("sha_teleporter5", TeleporterGraphic.Sparkles);
            ShameTeleporter5.CreateAt(new Location(5820, 51, 27));
            ShameTeleporter5.CreateAt(new Location(5817, 79, 5));

            TeleporterWithFixedDest ShameTeleporter6 = new TeleporterWithFixedDest("sha_teleporter6", TeleporterGraphic.Sparkles);
            ShameTeleporter6.CreateAt(new Location(5661, 116, 15));
            ShameTeleporter6.CreateAt(new Location(5674, 12, -5));

            TeleporterWithFixedDest ShameTeleporter7 = new TeleporterWithFixedDest("sha_teleporter7", TeleporterGraphic.Sparkles);
            ShameTeleporter7.CreateAt(new Location(5802, 17, 5));
            ShameTeleporter7.CreateAt(new Location(5700, 21, 15));
        }

        private static void SpiritualityShrine()
        {
            Location SpiritualityEntranceLoc1 = new Location(1600, 2489, 12);
            Location SpiritualityEntranceLoc2 = new Location(1600, 2490, 12);
            Location SpiritualityEntranceDest = new Location(1595, 2490, 20);
            TeleporterGeneric SpiritualityEntrance1 = new TeleporterGeneric(SpiritualityEntranceLoc1, "spiritual", SpiritualityEntranceDest);
            TeleporterGeneric SpiritualityEntrance2 = new TeleporterGeneric(SpiritualityEntranceLoc2, "spiritual", SpiritualityEntranceDest);
            SpiritualityEntrance1.Create();
            SpiritualityEntrance2.Create();

            Location SpiritualityExitLoc1 = new Location(1593, 2488, 17);
            Location SpiritualityExitLoc2 = new Location(1594, 2488, 17);
            Location SpiritualityExitLoc3 = new Location(1595, 2488, 17);
            Location SpiritualityExitDest = SpiritualityEntranceLoc1;
            TeleporterGeneric SpiritualityExit1 = new TeleporterGeneric(SpiritualityExitLoc1, "teleporter", SpiritualityExitDest);
            TeleporterGeneric SpiritualityExit2 = new TeleporterGeneric(SpiritualityExitLoc2, "teleporter", SpiritualityExitDest);
            TeleporterGeneric SpiritualityExit3 = new TeleporterGeneric(SpiritualityExitLoc3, "teleporter", SpiritualityExitDest);
            SpiritualityExit1.Create();
            SpiritualityExit2.Create();
            SpiritualityExit3.Create();
        }

        private static void Moonglow()
        {
            Location mgReturnExit = new Location(4442, 1122, 5);

            Location LocMGLycaeumEntrance = new Location(4436, 1107, 5);
            Location LocMGLycaeumExit = new Location(4300, 992, 5);
            Location LocMGLycaeumReturnEntrance = new Location(4300, 968, 5);
            TeleporterGeneric TeleLycaeumEntrance = new TeleporterGeneric(LocMGLycaeumEntrance, "teleporter", LocMGLycaeumExit, TeleporterGraphic.MGtoLycauem);
            TeleporterGeneric TeleLycaeumReturn = new TeleporterGeneric(LocMGLycaeumReturnEntrance, "teleporter", mgReturnExit, TeleporterGraphic.MGreturn);
            TeleLycaeumEntrance.Create();
            TeleLycaeumReturn.Create();

            Location LocMGNorthEntrance = new Location(4449, 1107, 5);
            Location LocMGNorthExit = new Location(4539, 890, 28);
            Location LocMGNorthReturnEntrance = new Location(4540, 898, 32);
            TeleporterGeneric TeleMGNorthEntrance = new TeleporterGeneric(LocMGNorthEntrance, "teleporter", LocMGNorthExit, TeleporterGraphic.MGtoNorth);
            TeleporterGeneric TeleMGNorthReturn = new TeleporterGeneric(LocMGNorthReturnEntrance, "teleporter", mgReturnExit, TeleporterGraphic.MGreturn);
            TeleMGNorthEntrance.Create();
            TeleMGNorthReturn.Create();

            Location LocMGTelescopeEntrance = new Location(4449, 1115, 5);
            Location LocMGTelescopeExit = new Location(4671, 1135, 10);
            Location LocMGTelescopeReturnEntrance = new Location(4663, 1134, 13);
            TeleporterGeneric TeleTelescopeEntrance = new TeleporterGeneric(LocMGTelescopeEntrance, "teleporter", LocMGTelescopeExit, TeleporterGraphic.MGtoTelescope);
            TeleporterGeneric TeleTelescopeReturn = new TeleporterGeneric(LocMGTelescopeReturnEntrance, "teleporter", mgReturnExit, TeleporterGraphic.MGreturn);
            TeleTelescopeEntrance.Create();
            TeleTelescopeReturn.Create();

            Location LocMGSouthEntrance = new Location(4443, 1137, 5);
            Location LocMGSouthExit = new Location(4487, 1475, 5);
            Location LocMGSouthReturnEntrance = new Location(4496, 1475, 15);
            TeleporterGeneric TeleMGSouthEntrance = new TeleporterGeneric(LocMGSouthEntrance, "teleporter", LocMGSouthExit, TeleporterGraphic.MGtoSouth);
            TeleporterGeneric TeleMGSouthReturn = new TeleporterGeneric(LocMGSouthReturnEntrance, "teleporter", mgReturnExit, TeleporterGraphic.MGreturn);
            TeleMGSouthEntrance.Create();
            TeleMGSouthReturn.Create();
        }

        private static void Jhelom()
        {
            Location jhelomReturnExit = new Location(1414, 3828, 5);

            Location jhelomNorthEntrance = new Location(1409, 3824, 5);
            Location jhelomNorthExit = new Location(1124, 3623, 5);
            Location jhelomNorthReturnEntrance = new Location(1142, 3621, 5);
            TeleporterGeneric TeleJhelomNorthEntrance = new TeleporterGeneric(jhelomNorthEntrance, "teleporter", jhelomNorthExit, TeleporterGraphic.jhelomNorth);
            TeleporterGeneric TeleJhelomNorthReturn = new TeleporterGeneric(jhelomNorthReturnEntrance, "teleporter", jhelomReturnExit, TeleporterGraphic.jhelomReturn);
            TeleJhelomNorthEntrance.Create();
            TeleJhelomNorthReturn.Create();

            Location jhelomSouthEntrance = new Location(1419, 3832, 5);
            Location jhelomSouthExit = new Location(1466, 4015, 5);
            Location jhelomSouthReturnEntrance = new Location(1406, 3996, 5);
            TeleporterGeneric TeleJhelomSouthEntrance = new TeleporterGeneric(jhelomSouthEntrance, "teleporter", jhelomSouthExit, TeleporterGraphic.jhelomSouth);
            TeleporterGeneric TeleJhelomSouthReturn = new TeleporterGeneric(jhelomSouthReturnEntrance, "teleporter", jhelomReturnExit, TeleporterGraphic.jhelomReturn);
            TeleJhelomSouthEntrance.Create();
            TeleJhelomSouthReturn.Create();
        }

        private static void BucsDen()
        {
            Location toBucsDen = new Location(2618, 977, 5);
            Location fromBuscDen = new Location(2727, 2133, 5);
            TeleporterGeneric TeleToBucs = new TeleporterGeneric(toBucsDen, "teleporter", fromBuscDen);
            TeleporterGeneric TeleFromBucs = new TeleporterGeneric(fromBuscDen, "teleporter", toBucsDen);
            TeleToBucs.Create();
            TeleFromBucs.Create();
        }

        private static void SkaraFerry()
        {
            Location Boat1 = new Location(709, 2238, -2);
            Location Boat2 = new Location(683, 2242, -2);
            Location Boat3 = new Location(683, 2234, -2);

            TeleporterGeneric teleBoat1 = new TeleporterGeneric(Boat1, "skaraferry", Boat2, TeleporterGraphic.NoDraw);
            TeleporterGeneric teleBoat2 = new TeleporterGeneric(Boat2, "skaraferry", Boat1, TeleporterGraphic.NoDraw);
            TeleporterGeneric teleBoat3 = new TeleporterGeneric(Boat3, "skaraferry", Boat1, TeleporterGraphic.NoDraw);

            teleBoat1.Create();
            teleBoat2.Create();
            teleBoat3.Create();
        }

        private static void Wind()
        {
            Location windEntranceLocation = new Location(1361, 883, 0);
            Location windEntranceDestination = new Location(5166, 245, 15);
            TeleporterGeneric windEntrance = new TeleporterGeneric(windEntranceLocation, "windentr", windEntranceDestination, TeleporterGraphic.PentagramCenter);
            windEntrance.Create();

            Location windExitLocation = new Location(5191, 152, 0);
            Location windExitDestination = new Location(1367, 891, 0);
            TeleporterGeneric windExit = new TeleporterGeneric(windExitLocation, "teleporter", windExitDestination, TeleporterGraphic.PentagramCenter);
            windExit.Create();
            
            Location toWindParkEnter = new Location(5200, 71, 17);
            Location toWindParkExit = new Location(5211, 22, 15);
            Location fromWindParkEnter = new Location(5217, 18, 15);
            Location fromWindParkExit = new Location(5204, 74, 17);
            TeleporterGeneric TeleToWindPark = new TeleporterGeneric(toWindParkEnter, "teleporter", toWindParkExit, TeleporterGraphic.PentagramCenter);
            TeleporterGeneric TeleFromWindPark = new TeleporterGeneric(fromWindParkEnter, "teleporter", fromWindParkExit, TeleporterGraphic.PentagramCenter);
            TeleToWindPark.Create();
            TeleFromWindPark.Create();
        }

        protected static void CreateTeleporterItem(ref ItemAndLocation ItemAndLocation)
        {
            if (!Builder.ItemExistsAtExactLocation(ItemAndLocation))
            {
                if (Builder.TryCreateItem(ItemAndLocation))
                    created++;
                else
                {
                    Console.WriteLine("Teleporters Error: Failed to create teleporter @ {0} {1} {2}", ItemAndLocation.Location.X, ItemAndLocation.Location.Y, ItemAndLocation.Location.Z);
                    failed++;
                }
            }
            else
                existed++;

        }

        protected static void AddTeleporterScript(ref ItemAndLocation ItemAndLocation, string script)
        {
            string result = Builder.AddScriptToFirstItemAtLocation(ItemAndLocation, script);
           
            if (result != null && !result.StartsWith("Script already attached"))
                Console.WriteLine("Teleporters Error: Failed to attach script {0} to teleporter @ {1} {2} {3} Message: {4}", script, ItemAndLocation.Location.X, ItemAndLocation.Location.Y, ItemAndLocation.Location.Z, result);
        }

        protected static void SetTeleporterDest(ref ItemAndLocation ItemAndLocation, Location destination)
        {
            if (destination.Equals(Location.Zero))
            {
                Console.WriteLine("Teleporters Error: Tried to set location on teleporter @ {0} {1} {2} to ZERO", ItemAndLocation.Location.X, ItemAndLocation.Location.Y, ItemAndLocation.Location.Z);
                return;
            }

            int serial = Builder.SerialOfFirstExistingItemAtLocation(ItemAndLocation);

            if (serial > 0)
                Server.setObjVar(serial, "dest", destination);
            else
                Console.WriteLine("Teleporters Error: Failed to set location on teleporter @ {0} {1} {2}", ItemAndLocation.Location.X, ItemAndLocation.Location.Y, ItemAndLocation.Location.Z);
        }

        /// <summary>
        /// A teleporter whose script has the destinations defined internally
        /// </summary>
        struct TeleporterWithFixedDest
        {
            TeleporterGraphic Graphic;
            string Script;

            public TeleporterWithFixedDest(string script, TeleporterGraphic graphic = TeleporterGraphic.NoDraw)
            {
                Script = script;
                Graphic = graphic;
            }

            public void CreateAt(Location loc)
            {
                ItemAndLocation ItemAndLocation = new ItemAndLocation()
                {
                    Location = loc,
                    ItemID = (ushort)Graphic
                };

                CreateTeleporterItem(ref ItemAndLocation);
                AddTeleporterScript(ref ItemAndLocation, Script);
            }
        }

        /// <summary>
        /// A teleporter whose script requires the ObjVar "dest"
        /// </summary>
        public struct TeleporterGeneric
        {
            ItemAndLocation ItemAndLocation;
            Location Destination;
            string Script;

            public TeleporterGeneric(Location worldLocation, string script, Location destination, TeleporterGraphic graphic = TeleporterGraphic.NoDraw)
            {
                ItemAndLocation = new ItemAndLocation()
                {
                    Location = worldLocation,
                    ItemID = (ushort)graphic
                };
                Script = script;
                Destination = destination;
            }

            public void Create()
            {
                CreateTeleporterItem(ref ItemAndLocation);
                AddTeleporterScript(ref ItemAndLocation, Script);
                SetTeleporterDest(ref ItemAndLocation, Destination);
            }
        }
    }
}
