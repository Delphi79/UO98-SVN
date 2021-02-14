using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sharpkick.WorldBuilding
{
    class DungeonEntranceTeleporters
    {
        public static void Generate()
        {
            // Dungeon Entrances and Exits

            // Deceit (4110 430 5) <-> (5186 639 0)
            DungeonEntranceDefinition Deceit = new DungeonEntranceDefinition()
            {
                EntranceFirstPoint = new Location(4110, 430, 5),
                ExitFirstPoint = new Location(5186, 639, 0),
                Width = 4,
                Facing = Facing.NorthSouth
            };
            CreateDungeonEntrance(Deceit);

            // Destard
            DungeonEntranceDefinition Destard = new DungeonEntranceDefinition()
            {
                EntranceFirstPoint = new Location(1175, 2635, 0),
                ExitFirstPoint = new Location(5242, 1007, 0),
                Width = 3,
                Facing = Facing.NorthSouth
            };
            CreateDungeonEntrance(Destard);

            // Covetous
            DungeonEntranceDefinition CovetousLvl1a = new DungeonEntranceDefinition()
            {
                EntranceFirstPoint = new Location(2498, 916, 0),
                ExitFirstPoint = new Location(5455, 1864, 0),
                Width = 3,
                Facing = Facing.NorthSouth
            };
            CreateDungeonEntrance(CovetousLvl1a);

            DungeonEntranceDefinition CovetousLvl1b = new DungeonEntranceDefinition()
            {
                EntranceFirstPoint = new Location(2420, 883, 0),
                ExitFirstPoint = new Location(5392, 1959, 0),
                Width = 3,
                Facing = Facing.NorthSouth
            };
            CreateDungeonEntrance(CovetousLvl1b);

            DungeonEntranceDefinition CovetousLvl2a = new DungeonEntranceDefinition()
            {
                EntranceFirstPoint = new Location(2384, 836, 0),
                ExitFirstPoint = new Location(5615, 1996, 0),
                Width = 3,
                Facing = Facing.EastWest
            };
            CreateDungeonEntrance(CovetousLvl2a);

            DungeonEntranceDefinition CovetousLvl2b = new DungeonEntranceDefinition()
            {
                EntranceFirstPoint = new Location(2455, 858, 0),
                ExitFirstPoint = new Location(5388, 2027, 0),
                Width = 3,
                Facing = Facing.NorthSouth
            };
            CreateDungeonEntrance(CovetousLvl2b);

            DungeonEntranceDefinition CovetousLvl3 = new DungeonEntranceDefinition()
            {
                EntranceFirstPoint = new Location(2544, 851, 0),
                ExitFirstPoint = new Location(5578, 1927, 0),
                Width = 3,
                Facing = Facing.NorthSouth
            };
            CreateDungeonEntrance(CovetousLvl3);
            // Shame
            DungeonEntranceDefinition Shame = new DungeonEntranceDefinition()
            {
                EntranceFirstPoint = new Location(512, 1559, 0),
                ExitFirstPoint = new Location(5394, 127, 0),
                Width = 3,
                Facing = Facing.NorthSouth
            };
            CreateDungeonEntrance(Shame);

            // Wrong
            DungeonEntranceDefinition Wrong = new DungeonEntranceDefinition()
            {
                EntranceFirstPoint = new Location(2041, 215, 14),
                ExitFirstPoint = new Location(5824, 631, 0),
                Width = 3,
                Facing = Facing.NorthSouth
            };
            CreateDungeonEntrance(Wrong);

            // Despise
            DungeonEntranceDefinition Despise = new DungeonEntranceDefinition()
            {
                EntranceFirstPoint = new Location(1296, 1080, 0),
                ExitFirstPoint = new Location(5588, 630, 30),
                Width = 3,
                Facing = Facing.EastWest
            };
            CreateDungeonEntrance(Despise);

            // Hythloth
            DungeonEntranceDefinition Hythloth = new DungeonEntranceDefinition()
            {
                EntranceFirstPoint = new Location(4721, 3813, 0),
                ExitFirstPoint = new Location(5904, 16, 64),
                Width = 3,
                Facing = Facing.NorthSouth
            };
            CreateDungeonEntrance(Hythloth);

        }

        struct DungeonEntranceDefinition
        {
            public Location EntranceFirstPoint;
            public Location ExitFirstPoint;
            public int Width;
            public Facing Facing;
        }

        static void CreateDungeonEntrance(DungeonEntranceDefinition definition)
        {
            LocationDelta offsetStep = definition.Facing == Facing.EastWest ? new LocationDelta(0, 1, 0) : new LocationDelta(1, 0, 0);

            Location EntrancePoint = definition.EntranceFirstPoint;
            Location ExitPoint = definition.ExitFirstPoint;

            for (int i = 0; i < definition.Width; i++)
            {
                Teleporters.TeleporterGeneric entrance = new Teleporters.TeleporterGeneric(EntrancePoint, "teleporter", ExitPoint);
                Teleporters.TeleporterGeneric exit = new Teleporters.TeleporterGeneric(ExitPoint, "teleporter", EntrancePoint);
                entrance.Create();
                exit.Create();
                EntrancePoint += offsetStep;
                ExitPoint += offsetStep;
            }

        }

    }
}
