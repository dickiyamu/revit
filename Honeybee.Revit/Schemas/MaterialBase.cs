using System;
using System.Collections.Generic;
using DF = DragonflySchema;
// ReSharper disable UnusedMember.Global

namespace Honeybee.Revit.Schemas
{
    public abstract class MaterialBase : ISchema<object>
    {
        public abstract string Type { get; }
        public abstract string Name { get; set; }
        public abstract object ToDragonfly();
    }

    public class EnergyMaterial : MaterialBase
    {
        public override string Type
        {
            get { return GetType().Name; }
        }

        public override string Name { get; set; } = $"EnergyMaterial_{Guid.NewGuid()}";
        public double Thickness { get; set; }
        public double Conductivity { get; set; }
        public double Density { get; set; }
        public double SpecificHeat { get; set; }
        public string Roughness { get; set; }
        public double ThermalAbsorptance { get; set; }
        public double SolarAbsorptance { get; set; }
        public double VisibleAbsorptance { get; set; }

        public override object ToDragonfly()
        {
            return new DF.EnergyMaterial(Name, Thickness, Conductivity, Density, SpecificHeat, Type);
        }
    }

    public class EnergyMaterialNoMass : MaterialBase
    {
        public override string Type
        {
            get { return GetType().Name; }
        }

        public override string Name { get; set; } = $"EnergyMaterialNoMass_{Guid.NewGuid()}";
        public double RValue { get; set; }
        public string Roughness { get; set; }
        public double ThermalAbsorptance { get; set; } = 0.9d;
        public double SolarAbsorptance { get; set; } = 0.7d;
        public double VisibleAbsorptance { get; set; } = 0.7d;

        public override object ToDragonfly()
        {
            return new DF.EnergyMaterialNoMass(Name, RValue, Type);
        }
    }

    public class EnergyWindowMaterialBlind : MaterialBase
    {
        public override string Type
        {
            get { return GetType().Name; }
        }

        public override string Name { get; set; } = $"EnergyWindowMaterialBlind_{Guid.NewGuid()}";
        public string SlatOrientation { get; set; }
        public double SlatWidth { get; set; }
        public double SlatSeparation { get; set; }
        public double SlatThickness { get; set; }
        public double SlatAngle { get; set; }
        public double SlatConductivity { get; set; }
        public double BeamSolarTransmittance { get; set; }
        public double BeamSolarReflectance { get; set; }
        public double BeamSolarReflectanceBack { get; set; }
        public double DiffuseSolarTransmittance { get; set; }
        public double DiffuseSolarReflectance { get; set; }
        public double DiffuseSolarReflectanceBack { get; set; }
        public double BeamVisibleTransmittance { get; set; }
        public double BeamVisibleReflectance { get; set; }
        public double BeamVisibleReflectanceBack { get; set; }
        public double DiffuseVisibleTransmittance { get; set; }
        public double DiffuseVisibleReflectance { get; set; }
        public double DiffuseVisibleReflectanceBack { get; set; }
        public double InfraredTransmittance { get; set; }
        public double Emissivity { get; set; }
        public double EmissivityBack { get; set; }
        public double DistanceToGlass { get; set; }
        public double TopOpeningMultiplier { get; set; }
        public double BottomOpeningMultiplier { get; set; }
        public double LeftOpeningMultiplier { get; set; }
        public double RightOpeningMultiplier { get; set; }

        public override object ToDragonfly()
        {
            return new DF.EnergyWindowMaterialBlind(Name, Type);
        }
    }

    public class EnergyWindowMaterialGas : MaterialBase
    {
        public override string Type
        {
            get { return GetType().Name; }
        }

        public override string Name { get; set; } = $"EnergyWindowMaterialGas_{Guid.NewGuid()}";
        public double Thickness { get; set; } = 0.0125;
        public string GasType { get; set; }

        public override object ToDragonfly()
        {
            return new DF.EnergyWindowMaterialGas(Name, Type);
        }
    }

    public class EnergyWindowMaterialGasCustom : MaterialBase
    {
        public override string Type
        {
            get { return GetType().Name; }
        }

        public override string Name { get; set; } = $"EnergyWindowMaterialGasCustom_{Guid.NewGuid()}";
        public double ConductivityCoeffA { get; set; }
        public double ViscosityCoeffA { get; set; }
        public double SpecificHeatCoeffA { get; set; }
        public double SpecificHeatRatio { get; set; }
        public double MolecularWeight { get; set; }
        public double Thickness { get; set; }
        public double ConductivityCoeffB { get; set; }
        public double ConductivityCoeffC { get; set; }
        public double ViscosityCoeffB { get; set; }
        public double ViscosityCoeffC { get; set; }
        public double SpecificHeatCoeffB { get; set; }
        public double SpecificHeatCoeffC { get; set; }

        public override object ToDragonfly()
        {
            return new DF.EnergyWindowMaterialGasCustom(
                Name,
                ConductivityCoeffA,
                ViscosityCoeffA,
                SpecificHeatCoeffA,
                SpecificHeatRatio,
                MolecularWeight,
                Type
            );
        }
    }

    public class EnergyWindowMaterialGasMixture : MaterialBase
    {
        public override string Type
        {
            get { return GetType().Name; }
        }

        public override string Name { get; set; } = $"EnergyWindowMaterialGasMixture_{Guid.NewGuid()}";
        public List<DF.EnergyWindowMaterialGasMixture.GasTypesEnum> GasTypes { get; set; }
        public List<double> GasFractions { get; set; }
        public double Thickness { get; set; }

        public override object ToDragonfly()
        {
            return new DF.EnergyWindowMaterialGasMixture(Name, GasTypes, GasFractions, Type);
        }
    }

    public class EnergyWindowMaterialGlazing : MaterialBase
    {
        public override string Type
        {
            get { return GetType().Name; }
        }

        public override string Name { get; set; } = $"EnergyWindowMaterialGlazing_{Guid.NewGuid()}";
        public double Thickness { get; set; }
        public double SolarTransmittance { get; set; }
        public double SolarReflectance { get; set; }
        public double SolarReflectanceBack { get; set; }
        public double VisibleTransmittance { get; set; }
        public double VisibleReflectance { get; set; }
        public double VisibleReflectanceBack { get; set; }
        public double InfraredTransmittance { get; set; }
        public double Emissivity { get; set; }
        public double EmissivityBack { get; set; }
        public double Conductivity { get; set; }
        public double DirtCorrection { get; set; }
        public double SolarDiffusing { get; set; }

        public override object ToDragonfly()
        {
            return new EnergyWindowMaterialGlazing();
        }
    }

    public class EnergyWindowMaterialShade : MaterialBase
    {
        public override string Type
        {
            get { return GetType().Name; }
        }

        public override string Name { get; set; } = $"EnergyWindowMaterialShade_{Guid.NewGuid()}";
        public double SolarTransmittance { get; set; }
        public double SolarReflectance { get; set; }
        public double VisibleTransmittance { get; set; }
        public double VisibleReflectance { get; set; }
        public double Emissivity { get; set; }
        public double InfraredTransmittance { get; set; }
        public double Thickness { get; set; }
        public double Conductivity { get; set; }
        public double DistanceToGlass { get; set; }
        public double TopOpeningMultiplier { get; set; }
        public double BottomOpeningMultiplier { get; set; }
        public double LeftOpeningMultiplier { get; set; }
        public double RightOpeningMultiplier { get; set; }
        public double AirflowPermeability { get; set; }

        public override object ToDragonfly()
        {
            return new DF.EnergyWindowMaterialShade(Name, Type);
        }
    }

    public class EnergyWindowMaterialSimpleGlazSys : MaterialBase
    {
        public override string Type
        {
            get { return GetType().Name; }
        }

        public override string Name { get; set; } = $"EnergyWindowMaterialSimpleGlazSys_{Guid.NewGuid()}";
        public double UFactor { get; set; }
        public double Shgc { get; set; }
        public double Vt { get; set; }

        public override object ToDragonfly()
        {
            return new DF.EnergyWindowMaterialSimpleGlazSys(Name, UFactor, Shgc, Type);
        }
    }
}
