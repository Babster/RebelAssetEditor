using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class SpaceShipParameters
{

    public SpaceshipRig rig { get; set; }

    public int StructurePoints;
    public int ArmorPoints;
    public int ShieldPoints;
    public int ShieldRegen;
    public int ReactorPoints;
    public int ReactorConsumption;
    public int Energy;

    public float Dexterity;
    public List<ShipModuleType> ModuleTypes;
    public List<ShipModule> Modules;
    public List<ParameterBonus> Bonuses;

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
                else if(rigSlot.IsOfficers)
                {
                    foreach(var officer in rigSlot.team.OfficerList)
                    {
                        Bonuses.Add(new ParameterBonus(officer));
                    }
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

        //Применение бонусов, которые действуют на корабле
        if(Bonuses.Count > 0)
        {
            //Бонусы применяются в два прохода. В первый раз добавляются абсолютные числа,
            //во второй раз - проценты
            foreach(ParameterBonus bonus in Bonuses)
            {
                ApplyBonusAdditive(bonus);
            }
            foreach (ParameterBonus bonus in Bonuses)
            {
                ApplyBonusMultiplicative(bonus);
            }
        }

    }

    private void ApplyBonusAdditive(ParameterBonus bonus)
    {
        StructurePoints += (int)bonus.StructureAdd;
        ArmorPoints += (int)bonus.ArmorAdd;
        ShieldPoints += (int)bonus.ShieldAdd;
        ShieldRegen += (int)bonus.ShieldRegenAdd;
        Dexterity += bonus.DexterityAdd;
        ReactorPoints += (int)bonus.ReactorAdd;

        foreach(var weapon in Weapons)
        {
            weapon.FireRate += bonus.FireRateAdd;
            weapon.DamageAmount += bonus.DamageAmountAdd;
            weapon.ShieldEffectiveness += bonus.ShieldEffectivenessAdd;
            weapon.ArmorEffectiveness += bonus.ArmorEffectivenessAdd;
            weapon.StructureEffectiveness += bonus.StructureEffectivenessAdd;
            weapon.CriticalChance += bonus.CriticalChanceAdd;
            weapon.CriticalStrength += bonus.CriticalStrengthAdd;
        }

    }

    public void ApplyBonusMultiplicative(ParameterBonus bonus)
    {
        AdjustParameterByPercent(ref StructurePoints, bonus.StructurePercent);
        AdjustParameterByPercent(ref ArmorPoints, bonus.ArmorPercent);
        AdjustParameterByPercent(ref ShieldPoints, bonus.ShieldPercent);
        AdjustParameterByPercent(ref ShieldRegen, bonus.ShieldRegenPercent);
        AdjustParameterByPercent(ref Dexterity, bonus.DexterityPercent);
        AdjustParameterByPercent(ref ReactorPoints, bonus.ReactorPercent);

        foreach (var weapon in Weapons)
        {
            AdjustParameterByPercent(ref weapon.FireRate, bonus.FireRatePercent);
            AdjustParameterByPercent(ref weapon.DamageAmount, bonus.DamageAmountPercent);
            if(weapon.Module.ModuleType.WeaponType == UnityShipModuleType.WeaponTypes.Laser)
            {
                AdjustParameterByPercent(ref weapon.DamageAmount, bonus.LaserDamagePercent);
            }
            else if(weapon.Module.ModuleType.WeaponType == UnityShipModuleType.WeaponTypes.Explosive)
            {
                AdjustParameterByPercent(ref weapon.DamageAmount, bonus.ExplosiveDamagePercent);
            }
            else if(weapon.Module.ModuleType.WeaponType == UnityShipModuleType.WeaponTypes.Explosive)
            {
                AdjustParameterByPercent(ref weapon.DamageAmount, bonus.KineticDamagePercent);
            }
            
            AdjustParameterByPercent(ref weapon.ShieldEffectiveness, bonus.ShieldEffectivenessPercent);
            AdjustParameterByPercent(ref weapon.ArmorEffectiveness, bonus.ArmorEffectivenessPercent);
            AdjustParameterByPercent(ref weapon.StructureEffectiveness, bonus.StructureEffectivenessPercent);
            AdjustParameterByPercent(ref weapon.CriticalChance, bonus.CriticalChancePercent);
            AdjustParameterByPercent(ref  weapon.CriticalStrength, bonus.CriticalStrengthPercent);
        }

        
    
    
    
    
    
    
    
    
    
    
}
    private void AdjustParameterByPercent(ref int parameter, float percent)
    {
        if (percent == 0)
        {
            return;
        }
        parameter = (int)( parameter * percent);
    }
    private void AdjustParameterByPercent(ref float parameter, float percent)
    {
        if (percent == 0)
        {
            return;
        }
        parameter = parameter * percent;
    }

    private void SetShipModelParameters()
    {
        StructurePoints = rig.sModel.BaseStructureHp;
        Energy = rig.sModel.BaseEnergy;
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
        Energy = 0;
        ModuleTypes = new List<ShipModuleType>();
        Modules = new List<ShipModule>();
        Weapons = new List<WeaponParameters>();
        Bonuses = new List<ParameterBonus>();
        
    }

    public List<WeaponParameters> Weapons;
    public class WeaponParameters
    {
        public ShipModuleType ModuleType { get; set; }
        public ShipModule Module { get; set; }
        public ShipModelSlot Slot { get; set; }

        public float FireRate;
        public float DamageAmount;
        public float ShieldEffectiveness;
        public float ArmorEffectiveness;
        public float StructureEffectiveness;
        public bool IgnoreShield;
        public float CriticalChance;
        public float CriticalStrength;

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

        public string BottomLineNames()
        {
            StringBuilder t = new StringBuilder();
            t.AppendLine($"DPS:");
            t.AppendLine($"Fire rate:");
            t.AppendLine($"Damage amount:");
            t.AppendLine($"Shield effectiveness:");
            t.AppendLine($"Armor effectiveness:");
            t.AppendLine($"Structure effectiveness:");
            if (IgnoreShield)
                t.AppendLine("Ignores shield");
            t.AppendLine($"Critical chance:");
            t.AppendLine($"Critical strength:");
            return t.ToString();
        }

        public string BottomLineValues()
        {
            StringBuilder t = new StringBuilder();
            t.AppendLine($"{DPS}");
            t.AppendLine($"{FireRate}");
            t.AppendLine($"{DamageAmount}");
            t.AppendLine($"{ShieldEffectiveness}");
            t.AppendLine($"{ArmorEffectiveness}");
            t.AppendLine($"{StructureEffectiveness}");
            if (IgnoreShield)
                t.AppendLine("");
            t.AppendLine($"{CriticalChance}");
            t.AppendLine($"{CriticalStrength}");
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
        public float DamageAmountPercent { get; set; }
        public float LaserDamagePercent { get; set; }
        public float ExplosiveDamagePercent { get; set; }
        public float KineticDamagePercent { get; set; }
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
        public float ReactorAdd { get; set; }
        public float ReactorPercent { get; set; }

        public ParameterBonus() { }

        public ParameterBonus(Crew.CrewOfficer officer) 
        {
            CreateFromOfficer(officer);
        }

        public void CreateFromOfficer(Crew.CrewOfficer officer)
        {
            
            //Добавление брони - +100% за каждые 10 очков статов
            int tStat = officer.StatValue(Crew.UnityOfficerTypeStat.StatType.ArmorBoost);
            ArmorPercent = GetStandartBonusForSkill(tStat);

            //Увеличение ловкости. Стат добавляет 0.1% ловкости за каждое очко
            tStat = officer.StatValue(Crew.UnityOfficerTypeStat.StatType.Dexterity);
            DexterityAdd = (float)tStat / 10;

            //Сумма всех оружейных статов - для расчета в нескольких статах
            int weaponTotal = officer.WeaponStatSum();
            FireRatePercent = (1 + weaponTotal / 10 / 100);
            DamageAmountPercent = (1 + weaponTotal / 10 / 100);
            CriticalChancePercent = (1 + weaponTotal / 100);
            CriticalStrengthPercent = (1 + weaponTotal / 10);

            //Увеличение мощности разных пушек - по 100% за каждые 10 очков умения
            tStat = officer.StatValue(Crew.UnityOfficerTypeStat.StatType.ExplosiveWeapons);
            ExplosiveDamagePercent = GetStandartBonusForSkill(tStat);

            tStat = officer.StatValue(Crew.UnityOfficerTypeStat.StatType.KineticWeapons);
            KineticDamagePercent = GetStandartBonusForSkill(tStat);

            tStat = officer.StatValue(Crew.UnityOfficerTypeStat.StatType.LaserWeapons);
            LaserDamagePercent = GetStandartBonusForSkill(tStat);

            //Что делать с реактором не решено. Пока не будет сета модулей кораблей говорить о
            //том, как будет сбалансирован этот аспект бессмысленно
            tStat = officer.StatValue(Crew.UnityOfficerTypeStat.StatType.ReactorBoost);
            ReactorPercent = (float)tStat;

            //Со щитом всё просто - офицер драматически увеличивает его
            tStat = officer.StatValue(Crew.UnityOfficerTypeStat.StatType.ShieldsBoost);
            ShieldPercent = GetStandartBonusForSkill(tStat);

            //Ускорители - пока не понятно как быть со скоростью, так что пропускаем ускорители
            tStat = officer.StatValue(Crew.UnityOfficerTypeStat.StatType.ThrustersBoost);

        }

        private float GetStandartBonusForSkill(int skill)
        {
            float bonus = 1;
            while (true)
            {
                if (skill == 0)
                    break;
                if (skill > 10)
                {
                    bonus = bonus * 2;
                    skill -= 10;
                }
                else
                {
                    bonus = (bonus * (1 + skill / 10));
                    skill = 0;
                }

            }
            return bonus;
        }


    }

    public void AddBonus(ParameterBonus bonus)
    {
        Bonuses.Add(bonus);
    }

    public WeaponParameters WeaponInSlot(int slotIdx)
    {
        foreach (var weapon in Weapons)
        {
            if (weapon.Slot.SlotNumber == slotIdx)
                return weapon;
        }
        throw new Exception("Error getting wepon for slot");
    }

    public string BottomLineString()
    {
        StringBuilder t = new StringBuilder();

        t.AppendLine(rig.sModel.Name);

        if (StructurePoints > 0)
            t.AppendLine($"Structure points: {StructurePoints}");
        if (ArmorPoints > 0)
            t.AppendLine($"Armor points: {ArmorPoints}");
        if (ShieldPoints > 0)
            t.AppendLine($"Shield points: {ShieldPoints}");
        if (ShieldRegen > 0)
            t.AppendLine($"Shield regen: {ShieldRegen}");
        t.AppendLine($"Energy: {Energy}");
        t.AppendLine($"Energy balance: {ReactorPoints - ReactorConsumption}");

        if (Weapons.Count > 0)
        {
            if (Weapons.Count > 1)
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
                t.AppendLine($"{weapon.ToString()}");
                t.AppendLine(weapon.BottomLineString());
            }
        }


        return t.ToString();
    }

    public string BottomLineNames()
    {
        StringBuilder t = new StringBuilder();

        t.AppendLine(rig.sModel.Name);

        if (StructurePoints > 0)
            t.AppendLine($"Structure points:");
        if (ArmorPoints > 0)
            t.AppendLine($"Armor points:");
        if (ShieldPoints > 0)
            t.AppendLine($"Shield points:");
        if (ShieldRegen > 0)
            t.AppendLine($"Shield regen:");
        t.AppendLine("Energy:");
        t.AppendLine($"Energy balance:");

        if (Weapons.Count > 0)
        {
            if (Weapons.Count > 1)
            {
                float DPS = 0;
                foreach (var weapon in Weapons)
                {
                    DPS += weapon.DPS;
                }
                t.AppendLine($"Total DPS:");
            }


            foreach (var weapon in Weapons)
            {
                t.AppendLine("");
                t.AppendLine($"{weapon.ToString()}");
                t.AppendLine(weapon.BottomLineNames());
            }
        }

        return t.ToString();
    }

    public string BottomLineValues()
    {
        StringBuilder t = new StringBuilder();

        t.AppendLine("");

        if (StructurePoints > 0)
            t.AppendLine($"{StructurePoints}");
        if (ArmorPoints > 0)
            t.AppendLine($"{ArmorPoints}");
        if (ShieldPoints > 0)
            t.AppendLine($"{ShieldPoints}");
        if (ShieldRegen > 0)
            t.AppendLine($"{ShieldRegen}");
        t.AppendLine($"{Energy}");
        t.AppendLine($"{ReactorPoints - ReactorConsumption}");
        
        if (Weapons.Count > 0)
        {
            if (Weapons.Count > 1)
            {
                float DPS = 0;
                foreach (var weapon in Weapons)
                {
                    DPS += weapon.DPS;
                }
                t.AppendLine($"{DPS}");
            }


            foreach (var weapon in Weapons)
            {
                t.AppendLine("");
                t.AppendLine("");
                t.AppendLine(weapon.BottomLineValues());
            }
        }


        return t.ToString();
    }


}
