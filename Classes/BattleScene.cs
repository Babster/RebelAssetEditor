using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdmiralNamespace;

class BattleScene
{

    public BattleSceneType SceneType { get; set; }
    public SpaceshipRig Rig { get; set; }

    public BattleScene(BattleSceneType stype, AccountData player)
    {
        SceneType = stype;
        Rig = SpaceshipRig.RigForPlayer(player);
        Rig.SaveData(player.Id, "");

    }
}
