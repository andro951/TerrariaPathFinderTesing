using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerrariaPathFinderTesting;

namespace TerrariaPathFinderTesing {
	public static class ElementsEnergyMath {
		public const float SpeedOfLight = 299792458f;
		public const float SpeedOfLightSquared = SpeedOfLight * SpeedOfLight;
		public const float JoulesPerKG = SpeedOfLightSquared;
		public const float KGPerAMU = 1.66053906660e-27f;
		public const float JoulesPerAMU = JoulesPerKG * KGPerAMU;
		//1 watt = 1 joule per second
		//Joules = Watts * Seconds
		public static void Testing() {
			ElementInfo iron = Element.Iron.ToElementInfo();
			$"{iron.Name}; JoulesPerCM3: {iron.JoulesPerCM3}, AtomicMass: {iron.AtomicMass}, Density: {iron.Density}".Log();

			ElementInfo gold = Element.Gold.ToElementInfo();
			$"{gold.Name}; JoulesPerCM3: {gold.JoulesPerCM3}, AtomicMass: {gold.AtomicMass}, Density: {gold.Density}".Log();
		}
		public static ElementInfo ToElementInfo(this Element element) => ToElementInfo((int)element);
		public static ElementInfo ToElementInfo(this int atomicNumber) => Elements[atomicNumber - 1];
		private static ElementInfo[] Elements = new ElementInfo[] {
			new(Element.Hydrogen, 1.008f, 0.00008988f, "H", Phase.Gas, ElementCategory.Nonmetal, 14.01f, 20.28f),
			new(Element.Helium, 4.0026f, 0.0001785f, "He", Phase.Gas, ElementCategory.NobleGas, 0.95f, 4.22f),
			new(Element.Lithium, 6.94f, 0.534f, "Li", Phase.Solid, ElementCategory.AlkaliMetal, 453.69f, 1615f),
			new(Element.Beryllium, 9.0122f, 1.85f, "Be", Phase.Solid, ElementCategory.AlkalineEarthMetal, 1560f, 2742f),
			new(Element.Boron, 10.81f, 2.34f, "B", Phase.Solid, ElementCategory.Metalloid, 2349f, 4200f),
			new(Element.Carbon, 12.011f, 2.267f, "C", Phase.Solid, ElementCategory.Nonmetal, 3800f, 4300f),
			new(Element.Nitrogen, 14.007f, 0.0012506f, "N", Phase.Gas, ElementCategory.Nonmetal, 63.15f, 77.36f),
			new(Element.Oxygen, 15.999f, 0.001429f, "O", Phase.Gas, ElementCategory.Nonmetal, 54.36f, 90.20f),
			new(Element.Fluorine, 18.998f, 0.001696f, "F", Phase.Gas, ElementCategory.Nonmetal, 53.48f, 85.03f),
			new(Element.Neon, 20.180f, 0.0008999f, "Ne", Phase.Gas, ElementCategory.NobleGas, 24.56f, 27.07f),
			new(Element.Sodium, 22.990f, 0.971f, "Na", Phase.Solid, ElementCategory.AlkaliMetal, 370.87f, 1156f),
			new(Element.Magnesium, 24.305f, 1.738f, "Mg", Phase.Solid, ElementCategory.AlkalineEarthMetal, 923f, 1380f),
			new(Element.Aluminum, 26.982f, 2.7f, "Al", Phase.Solid, ElementCategory.OtherMetal, 933.47f, 2792f),
			new(Element.Silicon, 28.085f, 2.3296f, "Si", Phase.Solid, ElementCategory.Metalloid, 1687f, 3538f),
			new(Element.Phosphorus, 30.974f, 1.82f, "P", Phase.Solid, ElementCategory.Nonmetal, 317.30f, 550f),
			new(Element.Sulfur, 32.06f, 2.067f, "S", Phase.Solid, ElementCategory.Nonmetal, 388.36f, 717.8f),
			new(Element.Chlorine, 35.45f, 0.003214f, "Cl", Phase.Gas, ElementCategory.Nonmetal, 171.6f, 239.11f),
			new(Element.Argon, 39.948f, 0.0017837f, "Ar", Phase.Gas, ElementCategory.NobleGas, 83.80f, 87.30f),
			new(Element.Potassium, 39.098f, 0.862f, "K", Phase.Solid, ElementCategory.AlkaliMetal, 336.53f, 1032f),
			new(Element.Calcium, 40.078f, 1.54f, "Ca", Phase.Solid, ElementCategory.AlkalineEarthMetal, 1115f, 1757f),
			new(Element.Scandium, 44.956f, 2.985f, "Sc", Phase.Solid, ElementCategory.TransitionMetal, 1814f, 3109f),
			new(Element.Titanium, 47.867f, 4.506f, "Ti", Phase.Solid, ElementCategory.TransitionMetal, 1941f, 3560f),
			new(Element.Vanadium, 50.942f, 6.11f, "V", Phase.Solid, ElementCategory.TransitionMetal, 2183f, 3680f),
			new(Element.Chromium, 51.996f, 7.15f, "Cr", Phase.Solid, ElementCategory.TransitionMetal, 2180f, 2944f),
			new(Element.Manganese, 54.938f, 7.47f, "Mn", Phase.Solid, ElementCategory.TransitionMetal, 1519f, 2334f),
			new(Element.Iron, 55.845f, 7.874f, "Fe", Phase.Solid, ElementCategory.TransitionMetal, 1811f, 3134f),
			new(Element.Cobalt, 58.933f, 8.86f, "Co", Phase.Solid, ElementCategory.TransitionMetal, 1768f, 3200f),
			new(Element.Nickel, 58.693f, 8.912f, "Ni", Phase.Solid, ElementCategory.TransitionMetal, 1728f, 3186f),
			new(Element.Copper, 63.546f, 8.96f, "Cu", Phase.Solid, ElementCategory.TransitionMetal, 1357.77f, 2835f),
			new(Element.Zinc, 65.38f, 7.14f, "Zn", Phase.Solid, ElementCategory.TransitionMetal, 692.88f, 1180f),
			new(Element.Gallium, 69.723f, 5.91f, "Ga", Phase.Solid, ElementCategory.OtherMetal, 302.91f, 2673f),
			new(Element.Germanium, 72.63f, 5.323f, "Ge", Phase.Solid, ElementCategory.Metalloid, 1211.4f, 3106f),
			new(Element.Arsenic, 74.922f, 5.776f, "As", Phase.Solid, ElementCategory.Metalloid, 1090f, 887f),
			new(Element.Selenium, 78.971f, 4.809f, "Se", Phase.Solid, ElementCategory.Nonmetal, 494f, 958f),
			new(Element.Bromine, 79.904f, 3.122f, "Br", Phase.Liquid, ElementCategory.Nonmetal, 265.8f, 332f),
			new(Element.Krypton, 83.798f, 0.003733f, "Kr", Phase.Gas, ElementCategory.NobleGas, 115.79f, 119.93f),
			new(Element.Rubidium, 85.468f, 1.532f, "Rb", Phase.Solid, ElementCategory.AlkaliMetal, 312.46f, 961f),
			new(Element.Strontium, 87.62f, 2.64f, "Sr", Phase.Solid, ElementCategory.AlkalineEarthMetal, 1050f, 1655f),
			new(Element.Yttrium, 88.906f, 4.469f, "Y", Phase.Solid, ElementCategory.TransitionMetal, 1799f, 3609f),
			new(Element.Zirconium, 91.224f, 6.506f, "Zr", Phase.Solid, ElementCategory.TransitionMetal, 2128f, 4682f),
			new(Element.Niobium, 92.906f, 8.57f, "Nb", Phase.Solid, ElementCategory.TransitionMetal, 2750f, 5017f),
			new(Element.Molybdenum, 95.95f, 10.22f, "Mo", Phase.Solid, ElementCategory.TransitionMetal, 2896f, 4912f),
			new(Element.Technetium, 98f, 11.5f, "Tc", Phase.Solid, ElementCategory.TransitionMetal, 2430f, 4538f),
			new(Element.Ruthenium, 101.07f, 12.37f, "Ru", Phase.Solid, ElementCategory.TransitionMetal, 2607f, 4423f),
			new(Element.Rhodium, 102.91f, 12.41f, "Rh", Phase.Solid, ElementCategory.TransitionMetal, 2237f, 3968f),
			new(Element.Palladium, 106.42f, 12.02f, "Pd", Phase.Solid, ElementCategory.TransitionMetal, 1828.05f, 3236f),
			new(Element.Silver, 107.87f, 10.501f, "Ag", Phase.Solid, ElementCategory.TransitionMetal, 1234.93f, 2435f),
			new(Element.Cadmium, 112.41f, 8.65f, "Cd", Phase.Solid, ElementCategory.TransitionMetal, 594.22f, 1040f),
			new(Element.Indium, 114.82f, 7.31f, "In", Phase.Solid, ElementCategory.OtherMetal, 429.75f, 2345f),
			new(Element.Tin, 118.71f, 7.31f, "Sn", Phase.Solid, ElementCategory.OtherMetal, 505.08f, 2875f),
			new(Element.Antimony, 121.76f, 6.697f, "Sb", Phase.Solid, ElementCategory.Metalloid, 903.78f, 1860f),
			new(Element.Tellurium, 127.60f, 6.24f, "Te", Phase.Solid, ElementCategory.Metalloid, 722.66f, 1261f),
			new(Element.Iodine, 126.90f, 4.93f, "I", Phase.Solid, ElementCategory.Nonmetal, 386.85f, 457.4f),
			new(Element.Xenon, 131.29f, 0.005887f, "Xe", Phase.Gas, ElementCategory.NobleGas, 161.4f, 165.03f),
			new(Element.Cesium, 132.91f, 1.93f, "Cs", Phase.Solid, ElementCategory.AlkaliMetal, 301.59f, 944f),
			new(Element.Barium, 137.33f, 3.62f, "Ba", Phase.Solid, ElementCategory.AlkalineEarthMetal, 1000f, 2170f),
			new(Element.Lanthanum, 138.91f, 6.145f, "La", Phase.Solid, ElementCategory.Lanthanide, 1193f, 3737f),
			new(Element.Cerium, 140.12f, 6.77f, "Ce", Phase.Solid, ElementCategory.Lanthanide, 1068f, 3716f),
			new(Element.Praseodymium, 140.91f, 6.77f, "Pr", Phase.Solid, ElementCategory.Lanthanide, 1208f, 3793f),
			new(Element.Neodymium, 144.24f, 7.01f, "Nd", Phase.Solid, ElementCategory.Lanthanide, 1297f, 3347f),
			new(Element.Promethium, 145f, 7.26f, "Pm", Phase.Solid, ElementCategory.Lanthanide, 1315f, 3273f),
			new(Element.Samarium, 150.36f, 7.52f, "Sm", Phase.Solid, ElementCategory.Lanthanide, 1345f, 2067f),
			new(Element.Europium, 151.96f, 5.243f, "Eu", Phase.Solid, ElementCategory.Lanthanide, 1099f, 1802f),
			new(Element.Gadolinium, 157.25f, 7.895f, "Gd", Phase.Solid, ElementCategory.Lanthanide, 1585f, 3546f),
			new(Element.Terbium, 158.93f, 8.23f, "Tb", Phase.Solid, ElementCategory.Lanthanide, 1629f, 3503f),
			new(Element.Dysprosium, 162.50f, 8.55f, "Dy", Phase.Solid, ElementCategory.Lanthanide, 1680f, 2840f),
			new(Element.Holmium, 164.93f, 8.795f, "Ho", Phase.Solid, ElementCategory.Lanthanide, 1734f, 2993f),
			new(Element.Erbium, 167.26f, 9.066f, "Er", Phase.Solid, ElementCategory.Lanthanide, 1802f, 3141f),
			new(Element.Thulium, 168.93f, 9.321f, "Tm", Phase.Solid, ElementCategory.Lanthanide, 1818f, 2223f),
			new(Element.Ytterbium, 173.04f, 6.965f, "Yb", Phase.Solid, ElementCategory.Lanthanide, 1097f, 1469f),
			new(Element.Lutetium, 174.97f, 9.841f, "Lu", Phase.Solid, ElementCategory.Lanthanide, 1925f, 3675f),
			new(Element.Hafnium, 178.49f, 13.31f, "Hf", Phase.Solid, ElementCategory.TransitionMetal, 2506f, 4876f),
			new(Element.Tantalum, 180.95f, 16.69f, "Ta", Phase.Solid, ElementCategory.TransitionMetal, 3290f, 5731f),
			new(Element.Tungsten, 183.84f, 19.25f, "W", Phase.Solid, ElementCategory.TransitionMetal, 3695f, 5828f),
			new(Element.Rhenium, 186.21f, 21.02f, "Re", Phase.Solid, ElementCategory.TransitionMetal, 3459f, 5869f),
			new(Element.Osmium, 190.23f, 22.59f, "Os", Phase.Solid, ElementCategory.TransitionMetal, 3306f, 5285f),
			new(Element.Iridium, 192.22f, 22.56f, "Ir", Phase.Solid, ElementCategory.TransitionMetal, 2719f, 4701f),
			new(Element.Platinum, 195.08f, 21.45f, "Pt", Phase.Solid, ElementCategory.TransitionMetal, 2041.4f, 4098f),
			new(Element.Gold, 196.97f, 19.32f, "Au", Phase.Solid, ElementCategory.TransitionMetal, 1337.33f, 3129f),
			new(Element.Mercury, 200.59f, 13.5336f, "Hg", Phase.Liquid, ElementCategory.TransitionMetal, 234.32f, 629.88f),
			new(Element.Thallium, 204.38f, 11.85f, "Tl", Phase.Solid, ElementCategory.OtherMetal, 577f, 1746f),
			new(Element.Lead, 207.2f, 11.342f, "Pb", Phase.Solid, ElementCategory.OtherMetal, 600.61f, 2022f),
			new(Element.Bismuth, 208.98f, 9.78f, "Bi", Phase.Solid, ElementCategory.OtherMetal, 544.7f, 1837f),
			new(Element.Polonium, 209f, 9.196f, "Po", Phase.Solid, ElementCategory.Metalloid, 527f, 1235f),
			new(Element.Astatine, 210f, 7f, "At", Phase.Solid, ElementCategory.Metalloid, 575f, 610f),
			new(Element.Radon, 222f, 0.00973f, "Rn", Phase.Gas, ElementCategory.NobleGas, 202f, 211.3f),
			new(Element.Francium, 223f, 1.87f, "Fr", Phase.Solid, ElementCategory.AlkaliMetal, 300f, 950f),
			new(Element.Radium, 226f, 5.5f, "Ra", Phase.Solid, ElementCategory.AlkalineEarthMetal, 973f, 2010f),
			new(Element.Actinium, 227f, 10.07f, "Ac", Phase.Solid, ElementCategory.Actinide, 1323f, 3471f),
			new(Element.Thorium, 232.04f, 11.72f, "Th", Phase.Solid, ElementCategory.Actinide, 2023f, 5061f),
			new(Element.Protactinium, 231.04f, 15.37f, "Pa", Phase.Solid, ElementCategory.Actinide, 1841f, 4300f),
			new(Element.Uranium, 238.03f, 19.05f, "U", Phase.Solid, ElementCategory.Actinide, 1405.3f, 4404f),
			new(Element.Neptunium, 237f, 20.45f, "Np", Phase.Solid, ElementCategory.Actinide, 917f, 4273f),
			new(Element.Plutonium, 244f, 19.84f, "Pu", Phase.Solid, ElementCategory.Actinide, 913f, 3505f),
			new(Element.Americium, 243f, 13.69f, "Am", Phase.Solid, ElementCategory.Actinide, 1449f, 2880f),
			new(Element.Curium, 247f, 13.51f, "Cm", Phase.Solid, ElementCategory.Actinide, 1613f, 3383f),
			new(Element.Berkelium, 247f, 14.79f, "Bk", Phase.Solid, ElementCategory.Actinide, 1259f, 2900f),
			new(Element.Californium, 251f, 15.1f, "Cf", Phase.Solid, ElementCategory.Actinide, 1173f, 1743f),
			new(Element.Einsteinium, 252f, 8.84f, "Es", Phase.Solid, ElementCategory.Actinide, 1133f, 1269f),
			new(Element.Fermium, 257f, 9.7f, "Fm", Phase.Solid, ElementCategory.Actinide, 1800f, 3500f),
			new(Element.Mendelevium, 258f, 10.3f, "Md", Phase.Solid, ElementCategory.Actinide, 1100f, -1),
			new(Element.Nobelium, 259f, 9.9f, "No", Phase.Solid, ElementCategory.Actinide, 1100f, -1),
			new(Element.Lawrencium, 266f, 15.6f, "Lr", Phase.Solid, ElementCategory.TransitionMetal, 1900f, -1),
			new(Element.Rutherfordium, 267f, 23.2f, "Rf", Phase.Solid, ElementCategory.TransitionMetal, -1, -1),
			new(Element.Dubnium, 268f, 29.3f, "Db", Phase.Solid, ElementCategory.TransitionMetal, -1, -1),
			new(Element.Seaborgium, 269f, 35f, "Sg", Phase.Solid, ElementCategory.TransitionMetal, -1, -1),
			new(Element.Bohrium, 270f, 37.1f, "Bh", Phase.Solid, ElementCategory.TransitionMetal, -1, -1),
			new(Element.Hassium, 277f, 40.7f, "Hs", Phase.Solid, ElementCategory.TransitionMetal, -1, -1),
			new(Element.Meitnerium, 278f, 37.4f, "Mt", Phase.Solid, ElementCategory.TransitionMetal, -1, -1),
			new(Element.Darmstadtium, 281f, 34.8f, "Ds", Phase.Solid, ElementCategory.TransitionMetal, -1, -1),
			new(Element.Roentgenium, 282f, 28.7f, "Rg", Phase.Solid, ElementCategory.TransitionMetal, -1, -1),
			new(Element.Copernicium, 285f, 23.7f, "Cn", Phase.Solid, ElementCategory.TransitionMetal, -1, -1),
			new(Element.Nihonium, 286f, 16f, "Nh", Phase.Solid, ElementCategory.PostTransitionMetal, -1, -1),
			new(Element.Flerovium, 289f, 14f, "Fl", Phase.Solid, ElementCategory.PostTransitionMetal, -1, -1),
			new(Element.Moscovium, 290f, 13.5f, "Mc", Phase.Solid, ElementCategory.PostTransitionMetal, -1, -1),
			new(Element.Livermorium, 293f, 12.9f, "Lv", Phase.Solid, ElementCategory.PostTransitionMetal, -1, -1),
			new(Element.Tennessine, 294f, 7.2f, "Ts", Phase.Solid, ElementCategory.PostTransitionMetal, -1, -1),
			new(Element.Oganesson, 294f, 4.9f, "Og", Phase.Solid, ElementCategory.NobleGas, -1, -1),
		};
	}
	public struct ElementInfo {
		public Element AtomicNumber;
		public float AtomicMass;
		public float Density;//g/cm^3
		public string Symbol;
		public Phase PhaseAtRoomTemperature;
		public ElementCategory ElementCategory;
		private float MeltingPoint;
		private float BoilingPoint;
		public float JoulesPerCM3;
		public ElementInfo(Element atomicNumber, float atomicMass, float density, string symbol, Phase phaseAtRoomTemperature, ElementCategory elementCategory, float meltingPoint, float boilingPoint) {
			AtomicNumber = atomicNumber;
			AtomicMass = atomicMass;
			Density = density;
			Symbol = symbol;
			PhaseAtRoomTemperature = phaseAtRoomTemperature;
			ElementCategory = elementCategory;
			MeltingPoint = meltingPoint;
			BoilingPoint = boilingPoint;
			JoulesPerCM3 = CalculateJoulesPerCM3(density);
		}
		private static float CalculateJoulesPerCM3(float density) => ElementsEnergyMath.SpeedOfLightSquared * density / 1000f;
		public bool TryGetMeltingPoint(out float meltingPoint) {
			meltingPoint = MeltingPoint;
			return MeltingPoint != -1;
		}
		public bool TryGetBoilingPoint(out float boilingPoint) {
			boilingPoint = BoilingPoint;
			return BoilingPoint != -1;
		}
		public string Name => AtomicNumber.ToString();
	}
	public enum Phase {
		Solid,
		Liquid,
		Gas,
		Plasma
	}
	public enum Element {
		Hydrogen = 1,
		Helium = 2,
		Lithium = 3,
		Beryllium = 4,
		Boron = 5,
		Carbon = 6,
		Nitrogen = 7,
		Oxygen = 8,
		Fluorine = 9,
		Neon = 10,
		Sodium = 11,
		Magnesium = 12,
		Aluminum = 13,
		Silicon = 14,
		Phosphorus = 15,
		Sulfur = 16,
		Chlorine = 17,
		Argon = 18,
		Potassium = 19,
		Calcium = 20,
		Scandium = 21,
		Titanium = 22,
		Vanadium = 23,
		Chromium = 24,
		Manganese = 25,
		Iron = 26,
		Cobalt = 27,
		Nickel = 28,
		Copper = 29,
		Zinc = 30,
		Gallium = 31,
		Germanium = 32,
		Arsenic = 33,
		Selenium = 34,
		Bromine = 35,
		Krypton = 36,
		Rubidium = 37,
		Strontium = 38,
		Yttrium = 39,
		Zirconium = 40,
		Niobium = 41,
		Molybdenum = 42,
		Technetium = 43,
		Ruthenium = 44,
		Rhodium = 45,
		Palladium = 46,
		Silver = 47,
		Cadmium = 48,
		Indium = 49,
		Tin = 50,
		Antimony = 51,
		Tellurium = 52,
		Iodine = 53,
		Xenon = 54,
		Cesium = 55,
		Barium = 56,
		Lanthanum = 57,
		Cerium = 58,
		Praseodymium = 59,
		Neodymium = 60,
		Promethium = 61,
		Samarium = 62,
		Europium = 63,
		Gadolinium = 64,
		Terbium = 65,
		Dysprosium = 66,
		Holmium = 67,
		Erbium = 68,
		Thulium = 69,
		Ytterbium = 70,
		Lutetium = 71,
		Hafnium = 72,
		Tantalum = 73,
		Tungsten = 74,
		Rhenium = 75,
		Osmium = 76,
		Iridium = 77,
		Platinum = 78,
		Gold = 79,
		Mercury = 80,
		Thallium = 81,
		Lead = 82,
		Bismuth = 83,
		Polonium = 84,
		Astatine = 85,
		Radon = 86,
		Francium = 87,
		Radium = 88,
		Actinium = 89,
		Thorium = 90,
		Protactinium = 91,
		Uranium = 92,
		Neptunium = 93,
		Plutonium = 94,
		Americium = 95,
		Curium = 96,
		Berkelium = 97,
		Californium = 98,
		Einsteinium = 99,
		Fermium = 100,
		Mendelevium = 101,
		Nobelium = 102,
		Lawrencium = 103,
		Rutherfordium = 104,
		Dubnium = 105,
		Seaborgium = 106,
		Bohrium = 107,
		Hassium = 108,
		Meitnerium = 109,
		Darmstadtium = 110,
		Roentgenium = 111,
		Copernicium = 112,
		Nihonium = 113,
		Flerovium = 114,
		Moscovium = 115,
		Livermorium = 116,
		Tennessine = 117,
		Oganesson = 118
	}
	[Flags]
	public enum ElementCategory : short {
		None = 0,
		Metal = 1 << 0,             // 1
		Nonmetal = 1 << 1,          // 2
		Metalloid = 1 << 2,         // 4
		NobleGas = 1 << 3,          // 8
		AlkaliMetal = 1 << 4,       // 16
		AlkalineEarthMetal = 1 << 5,// 32
		TransitionMetal = 1 << 6,   // 64
		Actinide = 1 << 7,          // 128
		Lanthanide = 1 << 8,        // 256
		Synthetic = 1 << 9,         // 512

		OtherMetal = Metal | AlkaliMetal | AlkalineEarthMetal,
		PostTransitionMetal = Metal | Metalloid,
	}
}
