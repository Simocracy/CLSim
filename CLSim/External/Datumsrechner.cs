using System;
using System.ComponentModel.DataAnnotations;

namespace Simocracy
{
    /// <summary>
    /// Datumsrechner für Simocracy.
    /// 
    /// Berechnungen sind möglich ab dem 1. Januar 2008 Realzeit = 1. Januar 1930.
    /// Im Zeitraum von Januar 2008 bis September 2008 (= 1930 bis 2019) ist ein RL-Monat ein SY-Jahrzehnt, was jedoch nur als inoffizielle Angabe zu verstehen ist!
    /// Der 1. Januar 2009 Realzeit ist per Definition 1. Januar 2021 Simocracy-Zeit.
    /// Ab 1. Oktober 2008 = 1. Januar 2020 sind 3 RL-Monate gemäß Spielregeln 1 SY-Jahr.
    /// </summary>
    /// <seealso cref="https://simocracy.de/SY:Spielregeln#Zeit"/>
    /// <remarks>
    /// Optimiert und Übersetzt vom Simocracy PostWriter 2.0.5
    /// Basierend auf dem alten PHP-Datumsrechner von Fluggi
    /// </remarks>
    public static class Datumsrechner
    {

        #region Calculation Methods

        /// <summary>
        /// Gibt den aktuellen Zeitpunkt in Simocracy-Zeit zurück
        /// </summary>
        public static DateTime Now { get { return RealToSim(DateTime.Now); } }

        /// <summary>
        /// Rechnet zwischen Realzeit und Simocracy-Zeit um.
        /// Angaben vor dem 01.10.2008 Realzeit bzw. 01.01.2020 Spielzeit sind nur inoffiziell!
        /// </summary>
        /// <param name="date">Ausgangsdatum</param>
        /// <param name="direction">Berechnungsrichtung</param>
        /// <returns>Umgerechnetes Datum</returns>
        public static DateTime Calculate(DateTime date, EDateDirection direction)
        {
            switch(direction)
            {
                case EDateDirection.RealToSim:
                    return RealToSim(date);
                case EDateDirection.SimToReal:
                    return SimToReal(date);
                default:
                    throw new InvalidOperationException("Unknown Error");
            }
        }

        /// <summary>
        /// Rechnet das angegebene RL-Datum in ein SY-Datum um.
        /// Angaben vor dem 01.10.2008 Realzeit sind nur inoffiziell!
        /// </summary>
        /// <param name="realDate">RL-Datum</param>
        /// <returns>SY-Datum</returns>
        public static DateTime RealToSim(DateTime realDate)
        {
            if(realDate >= new DateTime(2008, 10, 1))
                return RealToSimPost2020(realDate);
            else if(realDate >= new DateTime(2008, 1, 1))
                return RealToSimPre2020(realDate);
            else
                throw new ArgumentOutOfRangeException("realDate", realDate, "Calculation RL to SY only possible after 2008-01-01");
        }

        /// <summary>
        /// Rechnet das angegebene RL-Datum zwischen Januar 2008 und September 2008 in ein SY-Datum zwischen 1. Januar 1930 und 31. Dezember 2019 um
        /// </summary>
        /// <param name="realDate">RL-Datum zwischen 1. Januar 2008 und 31. September 2008</param>
        /// <returns>SY-Datum zwischen 1. Januar 1930 und 31. Dezember 2019</returns>
        private static DateTime RealToSimPre2020(DateTime realDate)
        {
            if(realDate < new DateTime(2008, 1, 1) || realDate >= new DateTime(2008, 10, 1))
                throw new ArgumentOutOfRangeException("realDate", realDate, "Pre 2020 calculation RL to SY only possible from 2008-01-01 to 2008-09-31");

            // SY-Jahrzehnt berechnen
            var syDecade = GetSimocracyDecade(realDate);

            // Tage im RL-Monat
            var rlTotalDaysInMonth = DateTime.DaysInMonth(realDate.Year, realDate.Month);

            // Tage im SY-Jahrzehnt
            var syTotalDaysInDecade = GetTotalDaysInDecade(syDecade);

            // SY-Tage pro RL-Tag
            var syDaysPerRlDay = rlTotalDaysInMonth / syTotalDaysInDecade.TotalDays;

            // Vergangene RL-Tage im Monat
            var rlDayInMonth = realDate.TimeOfDay.TotalDays + realDate.Day - 1;

            // SY-Tag im Jahrzehnt
            var syDayInDecade = (rlDayInMonth / syDaysPerRlDay);

            // SY-Datum ermitteln
            var syDate = new DateTime(syDecade, 1, 1).AddDays(syDayInDecade);

            return syDate;
        }

        /// <summary>
        /// Rechnet das angegebene RL-Datum ab Oktober 2008 in ein SY-Datum ab 1. Januar 2020 um
        /// </summary>
        /// <param name="realDate">RL-Datum ab 1. Oktober 2008</param>
        /// <returns>SY-Datum ab 1. Januar 2020</returns>
        private static DateTime RealToSimPost2020(DateTime realDate)
        {
            if(realDate < new DateTime(2008, 10, 1))
                throw new ArgumentOutOfRangeException("realDate", realDate, "Calculation RL to SY only possible after 2008-10-01");

            // SY-Jahr berechnen
            var syYear = GetSimocracyYear(realDate);

            // Tage im RL-Quartal
            var rlTotalDaysQuarter = GetTotalDaysInQuarter(realDate);

            // Tage im SY-Jahr
            var syTotalDaysInYear = GetTotalDaysInYear(syYear);

            // SY-Tage pro RL-Tag
            var syDaysPerRlDay = rlTotalDaysQuarter.TotalDays / syTotalDaysInYear;

            // Vergangene RL-Tage im Quartal
            var rlDayInQuarter = GetDayInQuarter(realDate).TotalDays - 1;

            // SY-Tag im Jahr
            var syDayInYear = (rlDayInQuarter / syDaysPerRlDay);

            // SY-Datum ermitteln
            var syDate = new DateTime(syYear, 1, 1).AddDays(syDayInYear);

            return syDate;
        }

        /// <summary>
        /// Rechnet das angegebene SY-Datum in ein RL-Datum um
        /// Angaben vor dem 01.01.2020 Spielzeit sind nur inoffiziell!
        /// </summary>
        /// <param name="simDate">SY-Datum</param>
        /// <returns>RL-Datum</returns>
        public static DateTime SimToReal(DateTime simDate)
        {
            if(simDate >= new DateTime(2020, 10, 1))
                return SimToRealPost2020(simDate);
            else if(simDate >= new DateTime(1930, 1, 1))
                return SimToRealPre2020(simDate);
            else
                throw new ArgumentOutOfRangeException("simDate", simDate, "Calculation SY to RL only possible after 1930-01-01");
        }

        /// <summary>
        /// Rechnet das angegebene SY-Datum zwischen 1. Januar 1930 und 31. Dezember 2019 in ein RL-Datum zwischen 1. Januar 2008 und 31. September 2008 um
        /// </summary>
        /// <param name="simDate">SY-Datum zwischen 1. Januar 1930 und 31. Dezember 2019</param>
        /// <returns>RL-Datum</returns>
        private static DateTime SimToRealPre2020(DateTime simDate)
        {
            if(simDate < new DateTime(1930, 1, 1))
                throw new ArgumentOutOfRangeException("simDate", simDate, "Pre 2020 calculation SY to RL only possible from 1930-01-01 to 2019-12-31");

            // RL-Jahr berechnen
            var rlMonth = GetRealMonth(simDate);

            // Vergangene Tage im SY-Jahrzehnt
            var syDayInDecade = GetDayInDecade(simDate).TotalDays - 1;

            // Tage im RL-Monat
            var rlTotalDaysMont = DateTime.DaysInMonth(2008, rlMonth);

            // Tage im SY-Jahrzehnt
            var syTotalDaysInDecade = GetTotalDaysInDecade(simDate);

            // SY-Tage pro RL-Tag
            var syDaysPerRlDay = rlTotalDaysMont / syTotalDaysInDecade.TotalDays;

            // Vergangene Tage im RL-Monat
            var rlDayInMonth = syDayInDecade * syDaysPerRlDay;

            // RL-Datum ermitteln
            var date = new DateTime(2008, rlMonth, 1).AddDays(rlDayInMonth);

            return date;
        }


        /// <summary>
        /// Rechnet das angegebene SY-Datum ab 1. Januar 2020 in ein RL-Datum ab 1. Oktober 2008 um
        /// </summary>
        /// <param name="simDate">SY-Datum ab 1. Januar 2020</param>
        /// <returns>RL-Datum ab 1. Oktober 2008</returns>
        private static DateTime SimToRealPost2020(DateTime simDate)
        {
            if(simDate < new DateTime(2020, 1, 1))
                throw new ArgumentOutOfRangeException("simDate", simDate, "Calculation SY to RL only possible after 2020-01-01");

            // RL-Jahr berechnen
            var rlYear = GetRealYear(simDate);

            // RL-Quartal berechnen
            var rlQuarter = simDate.Year % 4;
            if(rlQuarter == 0)
                rlQuarter = 4;

            // Vergangene Tage im SY-Jahr
            var syDayInYear = simDate.TimeOfDay.Add(TimeSpan.FromDays(simDate.DayOfYear)).TotalDays - 1;

            // Tage im RL-Quartal
            var rlTotalDaysQuarter = GetTotalDaysInQuarter(rlYear, rlQuarter);

            // Tage im SY-Jahr
            var syTotalDaysInYear = GetTotalDaysInYear(simDate.Year);

            // SY-Tage pro RL-Tag
            var syDaysPerRlDay = rlTotalDaysQuarter.TotalDays / syTotalDaysInYear;

            // Vergangene Tage im RL-Quartal
            var rlDayInQuarter = syDayInYear * syDaysPerRlDay;

            // RL-Datum ermitteln
            var date = new DateTime(rlYear, (rlQuarter * 3) - 2, 1).AddDays(rlDayInQuarter);

            return date;
        }

        #endregion

        #region Calculation Utilities

        /// <summary>
        /// Rechnet das angegebene RL-Datum in ein Simocracy-Jahr um
        /// </summary>
        /// <param name="realDate">RL-Datum</param>
        /// <returns>Simocracy-Jahr</returns>
        public static int GetSimocracyYear(DateTime realDate)
        {
            if(realDate >= new DateTime(2008, 10, 1))
                return (realDate.Year - 2009) * 4 + 2020 + GetQuarter(realDate);
            else if(realDate >= new DateTime(2008, 1, 1))
                return RealToSim(realDate).Year;
            else
                throw new ArgumentOutOfRangeException("realDate", realDate, "Calculation of Simocracy year only possible after 2008-01-01");
        }

        /// <summary>
        /// Rechnet das angegebene Simocracy-Datum in ein RL-Jahr um
        /// </summary>
        /// <param name="simDate">Datum</param>
        /// <returns>RL-Jahr</returns>
        public static int GetRealYear(DateTime simDate)
        {
            if(simDate >= new DateTime(2020, 1, 1))
                return 2008 + (int)Math.Ceiling((simDate.Year - 2020) / 4D);
            else if(simDate >= new DateTime(1930, 1, 1))
                return 2008;
            else
                throw new ArgumentOutOfRangeException("simDate", simDate, "Calculation of real year only possible after 1930-01-01");
        }

        /// <summary>
        /// Rechnet das angegebene Simocracy-Datum in einen RL-Monat um
        /// </summary>
        /// <param name="simDate">Datum</param>
        /// <returns>RL-Monat</returns>
        public static int GetRealMonth(DateTime simDate)
        {
            if(simDate >= new DateTime(2020, 1, 1))
                return SimToRealPost2020(simDate).Month;
            else if(simDate >= new DateTime(1930, 1, 1))
                return (GetDecade(simDate) / 10) - 192;
            else
                throw new ArgumentOutOfRangeException("simDate", simDate, "Calculation of real year only possible after 1930-01-01");
        }

        /// <summary>
        /// Gibt die Anzahl der Tage im angegebenen Jahr zurück
        /// </summary>
        /// <param name="year">Jahr</param>
        /// <returns>Anzahl der Tage des Jahres</returns>
        private static int GetTotalDaysInYear(int year)
        {
            var thisYear = new DateTime(year, 1, 1);
            var nextYear = new DateTime(year + 1, 1, 1);

            return (nextYear - thisYear).Days;
        }

        #endregion

        #region Quarter Utilities


        /// <summary>
        /// Gibt den Tag im Quartal des angegebenen Datums zurück
        /// </summary>
        /// <param name="date">Datum</param>
        /// <returns>Tag im Quartal</returns>
        private static TimeSpan GetDayInQuarter(DateTime date)
        {
            return date - GetFirstDayInQuarter(date) + TimeSpan.FromDays(1);
        }

        /// <summary>
        /// Gibt die Tage im angegebenen Quartal des angegebenen Jahres zurück
        /// </summary>
        /// <param name="year">Jahr</param>
        /// <param name="quarter">Quartal</param>
        /// <returns>Anzahl Tage im Quartal</returns>
        private static TimeSpan GetTotalDaysInQuarter(int year, int quarter)
        {
            var date = new DateTime(year, quarter * 3, 1);
            return GetTotalDaysInQuarter(date);
        }

        /// <summary>
        /// Gibt die Anzahl der Tage im Quartal des angegebenen Datums zurück
        /// </summary>
        /// <param name="date">Datum</param>
        /// <returns>Anzahl Tage im Quartal</returns>
        private static TimeSpan GetTotalDaysInQuarter(DateTime date)
        {
            return GetLastDayInQuarter(date) - GetFirstDayInQuarter(date) + TimeSpan.FromDays(1);
        }

        /// <summary>
        /// Gibt den ersten Tag im Quartal des angegebenen Datums zurück
        /// </summary>
        /// <param name="date">Datum</param>
        /// <returns>Erster Tag im Quartal</returns>
        private static DateTime GetFirstDayInQuarter(DateTime date)
        {
            return new DateTime(date.Year, 3 * GetQuarter(date) - 2, 1);
        }

        /// <summary>
        /// Gibt den letzten Tag im Quartal des angegebenen Datums zurück
        /// </summary>
        /// <param name="date">Datum</param>
        /// <returns>Letzter Tag im Quartal</returns>
        private static DateTime GetLastDayInQuarter(DateTime date)
        {
            var endMonth = GetQuarter(date) * 3;
            return new DateTime(date.Year, endMonth, DateTime.DaysInMonth(date.Year, endMonth));
        }

        /// <summary>
        /// Gibt das Quartal des angegebenen Datums zurück
        /// </summary>
        /// <param name="date">Datum</param>
        /// <returns>Quartal</returns>
        private static int GetQuarter(DateTime date)
        {
            return (int)Math.Ceiling(date.Month / 3d);
        }

        #endregion

        #region Decade Utilites

        /// <summary>
        /// Rechnet das angegebene RL-Datum in ein Simocracy-Jahrzehnt um
        /// </summary>
        /// <param name="realDate">RL-Datum</param>
        /// <returns>Simocracy-Jahrzehnt</returns>
        private static int GetSimocracyDecade(DateTime realDate)
        {
            if(realDate < new DateTime(2008, 1, 1) || realDate >= new DateTime(2008, 10, 1))
                throw new ArgumentOutOfRangeException("realDate", realDate, "Calculation of Simocracy decade only possible from 2008-01-01 to 2008-09-31");
            return 1920 + 10 * realDate.Month;
        }

        /// <summary>
        /// Gibt den Tag im Jahrzehnt des angegebenen Datums zurück
        /// </summary>
        /// <param name="date">Datum</param>
        /// <returns>Tag im Jahrzehnt</returns>
        private static TimeSpan GetDayInDecade(DateTime date)
        {
            return date - GetFirstDayInDecade(date) + TimeSpan.FromDays(1);
        }

        /// <summary>
        /// Gibt die Anzahl der Tage im Jahrzehnt des angegebenen Jahres zurück
        /// </summary>
        /// <param name="year">Jahr</param>
        /// <returns>Anzahl der Tage</returns>
        private static TimeSpan GetTotalDaysInDecade(int year)
        {
            var dtYear = new DateTime(year, 1, 1);
            return GetLastDayInDecade(dtYear) - GetFirstDayInDecade(dtYear) + TimeSpan.FromDays(1);
        }

        /// <summary>
        /// Gibt die Anzahl der Tage im Jahrzehnt des angegebenen Datums zurück
        /// </summary>
        /// <param name="date">Datum</param>
        /// <returns>Anzahl der Tage</returns>
        private static TimeSpan GetTotalDaysInDecade(DateTime date)
        {
            return GetLastDayInDecade(date) - GetFirstDayInDecade(date) + TimeSpan.FromDays(1);
        }

        /// <summary>
        /// Gibt den ersten Tag im Jahrzehnt des angegebenen Datums zurück
        /// </summary>
        /// <param name="date">Datum</param>
        /// <returns>Erster Tag im Jahrzehnt</returns>
        private static DateTime GetFirstDayInDecade(DateTime date)
        {
            return new DateTime(GetDecade(date), 1, 1);
        }

        /// <summary>
        /// Gibt den letzten Tag im Jahrzehnt des angegebenen Datums zurück
        /// </summary>
        /// <param name="date">Datum</param>
        /// <returns>Letzter Tag im Jahrzehnt</returns>
        private static DateTime GetLastDayInDecade(DateTime date)
        {
            var firstDay = GetFirstDayInDecade(date);
            return new DateTime(firstDay.Year + 9, 12, 31);
        }

        /// <summary>
        /// Gibt das Jahrzehnt des angegebenen Datums zurück
        /// </summary>
        /// <param name="date">Datum</param>
        /// <returns>Jahrzehnt</returns>
        private static int GetDecade(DateTime date)
        {
            return date.Year - (date.Year % 10);
        }

        #endregion

    }

    public enum EDateDirection
    {
        [Display(Description = "Realzeit nach Simocracy-Zeit")]
        RealToSim = 0,

        [Display(Description = "Simocracy-Zeit ach Realzeit")]
        SimToReal = 1
    }
}
