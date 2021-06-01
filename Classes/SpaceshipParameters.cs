using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class SpaceShipParameters
{

    public SpaceshipRig rig { get; set; }

    public int StructurePoints { get; set; }
    public int ArmorPoints { get; set; }
    public int ShieldPoints { get; set; }
    public int ShieldRegen { get; set; }
    public int ReactorPoints { get; set; }
    public int ReactorConsumption { get; set; }

    public List<ShipModuleType> ModuleTypes { get; set; }
    public List<ShipModule> Modules { get; set; }
    public List<ParameterBonus> Bonuses { get; set; }

    public void CalculateParameters()
    {
        //Начинается всё с того, что старые параметры все стираются
        ClearParameters();

        //Расчет параметров отталкивается от корабля, если его нет, то обработка для модели корабля - внизу
        CalculateInner();
    }

    private void CalculateInner()
    {
        SetShipModelParameters();

        //Если расчет ведется для корабля игрока, то надо пробежаться по слотам и в них вести расчет
        //по реально установленным модулям
        if(rig.Ship != null)
        {
            foreach(var rigSlot in rig.Slots)
            {
                if(rigSlot.IsWeapon)
                {
                    if(!rigSlot.IsEmpty)
                    {
                        Weapons.Add(new WeaponParameters(rigSlot.Module));
                    }
                }
                else if(rigSlot.IsModule)
                {
                    AddModule(rigSlot.Module);
                }
            }
        }
        else
        {
            foreach (var rigSlot in rig.Slots)
            {
                if (rigSlot.IsWeapon)
                {
                    if (!rigSlot.IsEmpty)
                    {
                        WeaponParameters curWeapon = new WeaponParameters(rigSlot.ModuleType);
                        curWeapon.Slot = rigSlot.Slot;
                        curWeapon.CalculateParameters();
                        Weapons.Add(curWeapon);
                    }
                }
                else if (rigSlot.IsModule)
                {
                    AddShipModuleType(rigSlot.ModuleType);
                }
            }
        }
    }

    private void SetShipModelParameters()
    {
        StructurePoints = rig.sModel.BaseStructureHp;
    }

    private void AddModule(ShipModule module)
    {
        Modules.Add(module);
        AddShipModuleType(module.ModuleType);
    }

    private void AddShipModuleType(ShipModuleType moduleType)
    {
        ModuleTypes.Add(moduleType);
        ArmorPoints += moduleType.ArmorPoints;
        ShieldPoints += moduleType.ShieldPoints;
        ShieldRegen += moduleType.ShieldRegen;
        ReactorPoints += moduleType.ReactorPoints;
        ReactorConsumption += moduleType.EnergyNeed;
    }


    private void ClearParameters()
    {
        StructurePoints = 0;
        ArmorPoints = 0;
        ShieldPoints = 0;
        ShieldRegen = 0;
        ReactorPoints = 0;
        ReactorConsumption = 0;
        ModuleTypes = new List<ShipModuleType>();
        Modules = new List<ShipModule>();
        Weapons = new List<WeaponParameters>();
        
    }

    public List<WeaponParameters> Weapons;
    public class WeaponParameters
    {
        public ShipModuleType ModuleType { get; set; }
        public ShipModule Module { get; set; }
        public ShipModelSlot Slot { get; set; }

        public float FireRate { get; set; }
        public float DamageAmount { get; set; }
        public float ShieldEffectiveness { get; set; }
        public float ArmorEffectiveness { get; set; }
        public float StructureEffectiveness { get; set; }
        public bool IgnoreShield { get; set; }
        public float CriticalChance { get; set; }
        public float CriticalStrength { get; set; }

        public float DPS { get; set; }

        public WeaponParameters() { }
        public WeaponParameters(ShipModule module)
        {
            Module = module;
            ModuleType = module.ModuleType;
        }
        public WeaponParameters(ShipModuleType moduleType)
        {
            ModuleType = moduleType;
        }

        public void CalculateParameters()
        {
            SetModelParameters();

        }

        private void SetModelParameters()
        {

            int damageModifier = 1 + Slot.DoubleWeapon;

            FireRate = ModuleType.FireRate;
            DamageAmount = ModuleType.DamageAmount * damageModifier;
            ShieldEffectiveness = ModuleType.ShieldEffectiveness;
            ArmorEffectiveness = ModuleType.ArmorEffectiveness;
            StructureEffectiveness = ModuleType.StructureEffectiveness;
            IgnoreShield = ModuleType.IgnoreShield == 1;
            CriticalChance = ModuleType.CriticalChance;
            CriticalStrength = ModuleType.CriticalStrength;

            DPS = DamageAmount * (ShieldEffectiveness + ArmorEffectiveness + StructureEffectiveness) / 300;
            DPS = DPS * (1 + CriticalChance / 100 * CriticalStrength / 100);

        }

        public string BottomLineString()
        {
            StringBuilder t = new StringBuilder();
            t.Append($"DPS: {DPS}");
            t.AppendLine($"Fire rate: {FireRate}");
            t.AppendLine($"Damage amount: {DamageAmount}");
            t.AppendLine($"Shield effectiveness: {ShieldEffectiveness}");
            t.AppendLine($"Armor effectiveness: {ArmorEffectiveness}");
            t.AppendLine($"Structure effectiveness: {StructureEffectiveness}");
            if (IgnoreShield)
                t.AppendLine("Ignores shield");
            t.AppendLine($"Critical chance: {CriticalChance}");
            t.AppendLine($"Critical strength: {CriticalStrength}");
            return t.ToString();
        }

        public override string ToString()
        {
            return ModuleType.Name;
        }

    }

    public class ParameterBonus
    {
        public float StructureAdd { get; set; }
        public float StructurePercent { get; set; }
        public float ArmorAdd { get; set; }
        public float ArmorPercent { get; set; }
        public float ShieldAdd { get; set; }
        public float ShieldPercent { get; set; }
        public float ShieldRegenAdd { get; set; }
        public float ShieldRegenPercent { get; set; }
        public float DexterityAdd { get; set; }
        public float DexterityPercent { get; set; }
        public float FireRateAdd { get; set; }
        public float FireRatePercent { get; set; }
        public float DamageAmountAdd { get; set; }
        public float ShieldEffectivenessAdd { get; set; }
        public float ShieldEffectivenessPercent { get; set; }
        public float ArmorEffectivenessAdd { get; set; }
        public float ArmorEffectivenessPercent { get; set; }
        public float StructureEffectivenessAdd { get; set; }
        public float StructureEffectivenessPercent { get; set; }
        public float CriticalChanceAdd { get; set; }
        public float CriticalChancePercent { get; set; }
        public float CriticalStrengthAdd { get; set; }
        public float CriticalStrengthPercent { get; set; }

        public ParameterBonus() { }

        public void CreateFromOfficer(Crew.CrewOfficer officer)
        {

        }

    }

    public string BottomLineString()
    {
        StringBuilder t = new StringBuilder();

        t.AppendLine(rig.sModel.Name);

        if (StructurePoints > 0)
            t.AppendLine($"Structure points: {StructurePoints}");
        if(ArmorPoints > 0)
            t.AppendLine($"Armor points: {ArmorPoints}");
        if (ShieldPoints > 0)
            t.AppendLine($"Shield points: {ShieldPoints}");
        if (ShieldRegen > 0)
            t.AppendLine($"Shield regen: {ShieldRegen}");
        t.AppendLine($"Energy balance: {ReactorPoints - ReactorConsumption}");

        if(Weapons.Count > 0)
        {
            if(Weapons.Count > 1)
            {
                float DPS = 0;
                foreach (var weapon in Weapons)
                {
                    DPS += weapon.DPS;
                }
                t.AppendLine($"Total DPS: {DPS}");
            }


            foreach (var weapon in Weapons)
            {
                t.AppendLine("");
                t.AppendLine($"Weapon: {weapon.ToString()}");
                t.AppendLine(weapon.BottomLineString());
            }
        }
        

        return t.ToString();
    }

}
