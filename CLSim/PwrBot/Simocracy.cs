﻿using System.Collections.Generic;

namespace Simocracy.PwrBot
{
	public class Simocracy
	{
		/// <summary>
		/// Gibt das Flaggenkürzel des aktuellen Staates bzw. Nachfolgers zurück, die nicht (de)fusioniert wurden bzw. der Nachfolger eindeutig ist.
		/// </summary>
		public static Dictionary<string, string> Flags = new Dictionary<string, string>
		{
			{"ABR", "BSC"},
			{"BOS", "BSC"},
			{"DRB", "BSC"},
			{"ADE", "ORA"},
			{"AQU", "SNL"},
			{"AZO", "NZL"},
			{"FNZ", "NZL"},
			{"NHI", "NZL"},
			{"FRP", "NZL"},
			{"FNS", "NZL"},
			{"OKA", "NZL"},
			{"NZ", "NZL"},
			{"New Halma Islands", "NZL"},
			{"Neu Halmanesien", "NZL"},
			{"Neu-Halmanesien", "NZL"},
			{"Pacifica", "NZL"},
			{"ASR", "BTZ"},
			{"SUD", "PATA"},
			{"HYL", "HYA"},
			{"LAG", "RLQ"},
			{"UKSI", "GRSI"},
			{"SEV", "GRSI"},
			{"SR", "KLY"},
			{"STO", "KLY"},
			{"NGT", "KLY"},
			{"BOU", "KLY"},
			{"Vannekar", "GRA"},
			{"SHI", "KNN"},
			{"HIK", "KNN"},
			{"NVC", "KNN"},
			{"UIP", "RPP"},
			{"RPP44", "RPP"},
			{"RPA", "RPP"},
			{"Janinisches Reich", "RPP"},
			{"VIR", "TOR"},
			{"4VIR", "TOR"},
			{"RNL", "NLL"},
			{"AGM", "NLL"},
			{"RUQ", "KSW"},
			{"SOW", "KSW"},
			{"KUR", "CSVR"},
			{"RCF", "FRC"},
			{"NFRC", "FRC"},
			{"RCH", "FRC"},
			{"BRU", "FVS"},
			{"AST", "MAS"},
			{"BOL1", "BOL"},
			{"TRU", "MAC"},
			{"MAC1", "MAC"},
			{"EMM", "EMA"},
			{"HJH", "NDL"},
			{"SKV", "NDL"},
			{"KOR", "SPA"},
			{"Sijut", "GOA"},
			{"GOT", "GOA"},
			{"SIM", "NUS"},
			{"Nuestra Senora", "NUS"},
			{"Simultanien", "NUS"},
			{"Azoren", "KBAZ"},
			{"IBRU", "KBAB"},
			{"ORV", "NOR"}
		};

		/// <summary>
		/// Gibt den Staatsnamen des angegebenen Kürzels oder historischen Staates zurück
		/// </summary>
		public static Dictionary<string, string> StateNames = new Dictionary<string, string>
		{
			{"ADRM", "Ostmedirien"},
			{"ABR", "Boscoulis"},
			{"BOS", "Boscoulis"},
			{"BSC", "Boscoulis"},
			{"DRB", "Boscoulis"},
			{"Åbro", "Boscoulis"},
			{"Abro", "Boscoulis"},
			{"AKM", "Almoravidien"},
			{"ADE", "Oranienbund"},
			{"ORA", "Oranienbund"},
			{"AQ", "Oranienbund"},
			{"AKS", "Aksai"},
			{"AQU", "Neusimmanien"},
			{"SNL", "Neusimmanien"},
			{"SNL/ALT", "Neusimmanien"},
			{"Aquilon", "Neusimmanien"},
			{"AMI", "Aminier"},
			{"AZO", "Neuseeland"},
			{"FNZ", "Neuseeland"},
			{"NHI", "Neuseeland"},
			{"FRP", "Neuseeland"},
			{"FNS", "Neuseeland"},
			{"OKA", "Neuseeland"},
			{"NZL", "Neuseeland"},
			{"NZ", "Neuseeland"},
			{"New Halma Islands", "Neuseeland"},
			{"Neu Halmanesien", "Neuseeland"},
			{"Neu-Halmanesien", "Neuseeland"},
			{"Pacifica", "Neuseeland"},
			{"ASR", "Batazion"},
			{"BTZ", "Batazion"},
			{"Astraliana Royalem", "Batazion"},
			{"HEB", "Hebridan"},
			{"COA", "Australien"},
			{"UAK", "Australien"},
			{"AZM", "Azmodan"},
			{"SUD", "Patagonien"},
			{"PATA", "Patagonien"},
			{"Sudamérica", "Patagonien"},
			{"Sudamerica", "Patagonien"},
			{"HYL", "Hylalien"},
			{"HYA", "Hylalien"},
			{"LAG", "Lago"},
			{"RL", "Lago"},
			{"RLQ", "Lago"},
			{"SEV", "Sevi Island"},
			{"GRSI", "Sevi Island"},
			{"UKSI", "Sevi Island"},
			{"EDO", "Eldorado"},
			{"STO", "Kelyne"},
			{"NGT", "Kelyne"},
			{"MEY", "Kelyne"},
			{"KLY", "Kelyne"},
			{"BOU", "Kelyne"},
			{"Meyham", "Kelyne"},
			{"Nagato", "Kelyne"},
			{"Stormic", "Kelyne"},
			{"Storm Republic", "Kelyne"},
			{"GRA", "Grafenberg"},
			{"Vannekar", "Grafenberg"},
			{"BRI", "Barnien"},
			{"BRI-AN", "Anglia"},
			{"SHI", "Kanon"},
			{"HIK", "Kanon"},
			{"KNN", "Kanon"},
			{"NVC", "Kanon"},
			{"Shigoni", "Kanon"},
			{"Hikari", "Kanon"},
			{"UIP", "Papua"},
			{"RPA", "Papua"},
			{"RPP44", "Papua"},
			{"RPP", "Papua"},
			{"Janinisches Reich", "Papua"},
			{"VIR", "Toro"},
			{"4VIR", "Toro"},
			{"TOR", "Toro"},
			{"Virenien", "Toro"},
			{"RNL", "Neulettland"},
			{"AGM", "Neulettland"},
			{"NLL", "Neulettland"},
			{"Aggermond", "Neulettland"},
			{"RUQ", "Sowekien"},
			{"KSW", "Sowekien"},
			{"SOW", "Sowekien"},
			{"Ruquia", "Sowekien"},
			{"KUR", "Caltanien"},
			{"CSVR", "Caltanien"},
			{"Kurland", "Caltanien"},
			{"KPR", "Preußen"},
			{"KRM", "Medirien"},
			{"RCH", "Chryseum"},
			{"NFRC", "Chryseum"},
			{"FRC", "Chryseum"},
			{"BRU", "Polyessia"},
			{"FVS", "Polyessia"},
			{"Brûmiasta", "Polyessia"},
			{"Brumiasta", "Polyessia"},
			{"AST", "Astana"},
			{"MAS", "Astana"},
			{"BOL1", "Bolivarien"},
			{"BOL", "Bolivarien"},
			{"TRU", "Macronien"},
			{"MAC1", "Macronien"},
			{"MAC", "Macronien"},
			{"Trujilo", "Macronien"},
			{"EMM", "Emmeria"},
			{"EMA", "Emmeria"},
			{"FL", "Flugghingen"},
			{"FLU", "Flugghingen"},
			{"HJH", "Nordurland"},
			{"SKV", "Nordurland"},
			{"NDL", "Nordurland"},
			{"Jotunheim", "Nordurland"},
			{"Skørnvar", "Nordurland"},
			{"Skörnvar", "Nordurland"},
			{"KOR", "Spartan"},
			{"SPA", "Spartan"},
			{"GOA", "Goatanien"},
			{"GOT", "Goatanien"},
			{"Sijut", "Goatanien"},
			{"SSH", "Singa Shang"},
			{"SHIK", "Shikanojima"},
			{"NSI", "Shikanojima"},
			{"NUS", "Simultanien"},
			{"SIM", "Simultanien"},
			{"Nuestra Senora", "Simultanien"},
			{"RBS", "Südburgund"},
			{"Südburg.", "Südburgund"},
			{"Arancazuelaz", "URS"},
			{"IRBU", "Alm. Brumiasta"},
			{"KBAB", "Alm. Brumiasta"},
			{"MAMA", "Mamba Mamba"},
			{"MAU", "Mauritanien"},
			{"MEB", "Mitteleuropa"},
			{"MEX", "Mexicali"},
			{"MIR", "Mirabella"},
			{"PKY", "Kyiv"},
			{"RIV", "Rivero"},
			{"UDV", "Damas"},
			{"VRD", "Damas"},
			{"UIB", "Balearen"},
			{"USGB", "Grimbergen"},
			{"USRV", "Rivera"},
			{"WLJ", "Welanja"},
			{"YJB", "Yojahbalo"},
			{"ZUM", "Zumanien"},
			{"ZR", "Zumanien"},
			{"KBAZ", "Azoren"},
			{"ORV", "Norkanien"},
			{"NOR", "Norkanien"},
			{"UGD", "Ugandia"},
			{"ANT", "Ugantares"},
			{"VAL", "Valgerik"}
		};

		/// <summary>
		/// Gibt das Flaggenkürzel des aktuellen Staates bzw. Nachfolgers zurück, die nicht (de)fusioniert wurden bzw. der Nachfolger eindeutig ist.
		/// </summary>
		/// <param name="name">Kürzel oder Name des historischen Staates</param>
		/// <returns>Kürzel des aktuellen Staates</returns>
		public static string GetFlag(string name)
		{
			string current;
			return Flags.TryGetValue(name, out current) ? current : name;
		}

		/// <summary>
		/// Gibt den Staatsnamen des angegebenen Kürzels oder historischen Staates zurück
		/// </summary>
		/// <param name="flag">Historisches Kürzel oder Name</param>
		/// <returns>Aktueller Name</returns>
		public static string GetStateName(string flag)
		{
			string current;
			return StateNames.TryGetValue(flag, out current) ? current : flag;
		}
	}
}
