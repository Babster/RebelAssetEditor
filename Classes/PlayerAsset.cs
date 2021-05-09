﻿using AdmiralNamespace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


class PlayerAsset
{
    public List<SpaceshipRig> Rigs { get; set; }
    public List<Crew.CrewOfficer> Officers { get; set; }
    public List<Ship> Ships { get; set; }
    public List<ShipModule> Modules { get; set; }

    public PlayerAsset() { }

    public PlayerAsset(int playerId)
    {
        Rigs = SpaceshipRig.PlayerRigs(playerId);
        Officers = Crew.CrewOfficer.OfficersForPlayer(playerId, false);
        Ships = Ship.PlayerShips(playerId);
        Modules = ShipModule.PlayerModules(playerId);

    }

}
