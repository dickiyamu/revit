using System;
using System.Collections.Generic;
using DF = DragonflySchema;
using HB = HoneybeeSchema;
// ReSharper disable UnusedMember.Global

namespace Honeybee.Revit.Schemas
{
    public abstract class MaterialBase : IBaseObject, ISchema<object, object>
    {
        public abstract string Type { get; }
        public abstract object ToDragonfly();
        public abstract object ToHoneybee();
        public abstract string Identifier { get; set; }
        public abstract string DisplayName { get; set; }
    }

    public class EnergyMaterial : MaterialBase
    {
        public override string Type
        {
            get { return GetType().Name; }
        }

        public override string Identifier { get; set; } = $"EnergyMaterial_{Guid.NewGuid()}";
        public override string DisplayName { get; set; }
        public double Thickness { get; set; }
        public double Conductivity { get; set; }
        public double Density { get; set; }
        public double SpecificHeat { get; set; }
        public HB.Roughness? Roughness { get; set; } = HB.Roughness.MediumRough;
        public double ThermalAbsorptance { get; set; }
        public double SolarAbsorptance { get; set; }
        public double VisibleAbsorptance { get; set; }

        public override object ToDragonfly()
        {
            return new HB.EnergyMaterial(
                Identifier,
                Thickness,
                Conductivity,
                Density,
                SpecificHeat,
                DisplayName,
                Roughness,
                ThermalAbsorptance,
                SolarAbsorptance,
                VisibleAbsorptance
            );
        }

        public override object ToHoneybee()
        {
            throw new NotImplementedException();
        }
    }

    public class EnergyMaterialNoMass : MaterialBase
    {
        public override string Type
        {
            get { return GetType().Name; }
        }

        public override string Identifier { get; set; } = $"EnergyMaterialNoMass_{Guid.NewGuid()}";
        public override string DisplayName { get; set; }
        public double RValue { get; set; }

        public HB.Roughness? Roughness { get; set; } = HB.Roughness.MediumRough;
        public double ThermalAbsorptance { get; set; } = 0.9d;
        public double SolarAbsorptance { get; set; } = 0.7d;
        public double VisibleAbsorptance { get; set; } = 0.7d;

        public override object ToDragonfly()
        {
            return new HB.EnergyMaterialNoMass(
                Identifier,
                RValue,
                DisplayName,
                Roughness,
                ThermalAbsorptance,
                SolarAbsorptance,
                VisibleAbsorptance
            );
        }

        public override object ToHoneybee()
        {
            throw new NotImplementedException();
        }
    }

    public class EnergyWindowMaterialBlind : MaterialBase
    {
        public override string Type
        {
            get { return GetType().Name; }
        }

        public override string Identifier { get; set; } = $"EnergyWindowMaterialBlind_{Guid.NewGuid()}";
        public override string DisplayName { get; set; }

        public HB.SlatOrientation? SlatOrientation { get; set; } =
            HB.SlatOrientation.Horizontal;
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
            return new HB.EnergyWindowMaterialBlind(Identifier, DisplayName, SlatOrientation, SlatWidth, SlatSeparation,
                SlatThickness, SlatAngle, SlatConductivity, BeamSolarTransmittance, BeamSolarReflectance,
                BeamSolarReflectanceBack, DiffuseSolarTransmittance, DiffuseSolarReflectance,
                DiffuseSolarReflectanceBack, BeamVisibleTransmittance, BeamVisibleReflectance,
                BeamVisibleReflectanceBack, DiffuseVisibleTransmittance, DiffuseVisibleReflectance,
                DiffuseVisibleReflectanceBack, InfraredTransmittance, Emissivity, EmissivityBack, DistanceToGlass,
                TopOpeningMultiplier, BottomOpeningMultiplier, LeftOpeningMultiplier, RightOpeningMultiplier);
        }

        public override object ToHoneybee()
        {
            throw new NotImplementedException();
        }
    }

    public class EnergyWindowMaterialGas : MaterialBase
    {
        public override string Type
        {
            get { return GetType().Name; }
        }

        public override string Identifier { get; set; } = $"EnergyWindowMaterialGas_{Guid.NewGuid()}";
        public override string DisplayName { get; set; }
        public double Thickness { get; set; } = 0.0125;
        public HB.GasType? GasType { get; set; } = HB.GasType.Air;

        public override object ToDragonfly()
        {
            return new HB.EnergyWindowMaterialGas(Identifier, DisplayName, Thickness, GasType);
        }

        public override object ToHoneybee()
        {
            throw new NotImplementedException();
        }
    }

    public class EnergyWindowMaterialGasCustom : MaterialBase
    {
        public override string Type
        {
            get { return GetType().Name; }
        }

        public override string Identifier { get; set; } = $"EnergyWindowMaterialGasCustom_{Guid.NewGuid()}";
        public override string DisplayName { get; set; }
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
            return new HB.EnergyWindowMaterialGasCustom(
                Identifier,
                ConductivityCoeffA,
                ViscosityCoeffA,
                SpecificHeatCoeffA,
                SpecificHeatRatio,
                MolecularWeight, 
                DisplayName, 
                Thickness, 
                ConductivityCoeffB, 
                ConductivityCoeffC, 
                ViscosityCoeffB, 
                ViscosityCoeffC,
                SpecificHeatCoeffB, 
                SpecificHeatCoeffC
            );
        }

        public override object ToHoneybee()
        {
            throw new NotImplementedException();
        }
    }

    public class EnergyWindowMaterialGasMixture : MaterialBase
    {
        public override string Type
        {
            get { return GetType().Name; }
        }

        public override string Identifier { get; set; } = $"EnergyWindowMaterialGasMixture_{Guid.NewGuid()}";
        public override string DisplayName { get; set; }
        public List<HB.GasType> GasTypes { get; set; }
        public List<double> GasFractions { get; set; }
        public double Thickness { get; set; }

        public override object ToDragonfly()
        {
            return new HB.EnergyWindowMaterialGasMixture(Identifier, GasTypes, GasFractions, DisplayName, Thickness);
        }

        public override object ToHoneybee()
        {
            throw new NotImplementedException();
        }
    }

    public class EnergyWindowMaterialGlazing : MaterialBase
    {
        public override string Type
        {
            get { return GetType().Name; }
        }

        public override string Identifier { get; set; } = $"EnergyWindowMaterialGlazing_{Guid.NewGuid()}";
        public override string DisplayName { get; set; }
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
        public bool SolarDiffusing { get; set; }

        public override object ToDragonfly()
        {
            return new HB.EnergyWindowMaterialGlazing(Identifier, DisplayName, Thickness, SolarTransmittance, SolarReflectance,
                SolarReflectanceBack, VisibleTransmittance, VisibleReflectance, VisibleReflectanceBack,
                InfraredTransmittance, Emissivity, EmissivityBack, Conductivity, DirtCorrection, SolarDiffusing);
        }

        public override object ToHoneybee()
        {
            throw new NotImplementedException();
        }
    }

    public class EnergyWindowMaterialShade : MaterialBase
    {
        public override string Type
        {
            get { return GetType().Name; }
        }

        public override string Identifier { get; set; } = $"EnergyWindowMaterialShade_{Guid.NewGuid()}";
        public override string DisplayName { get; set; }
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
            return new HB.EnergyWindowMaterialShade(Identifier, DisplayName, SolarTransmittance, SolarReflectance,
                VisibleTransmittance, VisibleReflectance, Emissivity, InfraredTransmittance, Thickness, Conductivity,
                DistanceToGlass, TopOpeningMultiplier, BottomOpeningMultiplier, LeftOpeningMultiplier,
                RightOpeningMultiplier, AirflowPermeability);
        }

        public override object ToHoneybee()
        {
            throw new NotImplementedException();
        }
    }

    public class EnergyWindowMaterialSimpleGlazSys : MaterialBase
    {
        public override string Type
        {
            get { return GetType().Name; }
        }

        public override string Identifier { get; set; } = $"EnergyWindowMaterialSimpleGlazSys_{Guid.NewGuid()}";
        public override string DisplayName { get; set; }
        public double UFactor { get; set; }
        public double Shgc { get; set; }
        public double Vt { get; set; }

        public override object ToDragonfly()
        {
            return new HB.EnergyWindowMaterialSimpleGlazSys(Identifier, UFactor, Shgc, DisplayName, Vt);
        }

        public override object ToHoneybee()
        {
            throw new NotImplementedException();
        }
    }
}
