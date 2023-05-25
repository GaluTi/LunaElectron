using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.ComponentModel.Design;
using System.Threading.Tasks;
using System.Threading;
using TrackNS;

namespace LunaElectron
{
    class Satellite
    {

        public List<Engine> Engines;// варианты двигателей
        public List<Platform> Platforms;   // варианты платформ

        public double N_eng;        // количество двигателей
        public double k_pcs;        // коэффициент массы системы питания и управления
        public double k_tank;       // коэффициент массы бака
        public double mu_sa;        // коэффициент массы солнечных батарей
        public double k_extra;      // коэффициент массы дополниельного запаса рабочего тела
        public double eta_pcs;      // (1 - КПД) системы питания и управления
        public double k_sa;         // коэффициент запаса мощности солнечных батарей

        public double m_platform;   // масса платформы
        public double m_eng;        // масса двигателя
        public double m_engines;    // масса двигательной установки
        public double m_fsts;       // масса системы хранения и передачи рабочего тела
        public double m_pcs;        // масса системы питания и управления
        public double m_sa;         // масса солнечных батарей
        public double m_saos;       // масса сисетма ориентации солнечных батарей
        public double m_eos;        // масса системы ориентации двигательной установки
        public double m0;           // стартовая масса КА 
        public double m_f_main;     // масса основного запаса рабочего тела
        public double m_f_extra;    // масса дополнительного запаса пабочего тела
        public double m_fuel;       // масса рабочего тела

        public double p_platform;   // потребляемая мощность платформы
        public double p_eng;        // потребляемая мощность двигателя
        public double p_engines;    // потребляемая мощность двигательной установки
        public double p_burt_0;     // потребляемая мощность блока управления расходом топлива на 2 двигателя
        public double p_burt;       // потребляемая мощность блока управления расходом топлива
        public double p_fsts_0;     // потребляемая мощность системы хранения и передачи рабочего тела на 2 двигателя
        public double p_fsts;       // потребляемая мощность системы хранения и передачи рабочего тела
        public double p_pcs;        // потребляемая мощность системы питания и управления
        public double p_saos;       // потребляемая мощность системы ориентации солнечных батарей
        public double p_eos;        // потребляемая мощность системы ориентации двигательной установки
        public double p_sa;         // производимая мощность солнечных батарей
        public double p0;           // полная потребляемая мощность частей и приборов КА

        public double RE;           // радиус Земли
        public double RL;           // радиус Луны
        public double mu0;          // гравитационный параметр Земли
        public double mu1;          // гравитационный параметр Луны
        public double h0;           // высота околоземной орбиты
        public double h1;           // высота окололунной орбиты
        public double R0;           // радиус околоземной орбиты
        public double R1;           // радиус окололунной орбиты

        public double J;            // удельный импульс
        public double Vgis;         // гиперболический избыток скорости
        public double F;            // сила тяги в мН
        public double S_sa;         // площадь солнечных батарей
        public double T_miss;       // время перелёта
        public double acc;          // ускорение


        public Track track;         // объект типа траектория
        public double dt0;          // колличество шагов интегрирования
        public double eta;          // относительный расход рабочего тела

        public int m0_iter_cnt;     // количество итераций в методе простых итераций
        public string eng_name;     // наименование двигателя
        public string platform_name;// наименование платформы

        public Satellite()
        {
            Engines = new List<Engine>();
            Engines.Add(new Engine("ВЧИД-8",     3800, 0.009,  4,    1350));
            Engines.Add(new Engine("СПД-50",     860,  0.014,  1.23, 225));
            Engines.Add(new Engine("СПД-50М(1)", 930,  0.0148, 1.32, 225));
            Engines.Add(new Engine("СПД-50М(2)", 1200, 0.018,  1.32, 300));
            Engines.Add(new Engine("СПД-70",     1470, 0.039,  1.5,  670));
            Engines.Add(new Engine("СПД-70М(1)", 1430, 0.036,  2.6,  600));
            Engines.Add(new Engine("СПД-70М(2)", 1530, 0.048,  2.6,  800));
            Engines.Add(new Engine("СПД-70М(3)", 1600, 0.059,  2.6,  1000));
            Engines.Add(new Engine("СПД-100В",   1540, 0.083,  3.5,  1350));
            Engines.Add(new Engine("СПД-100ВМ",  1600, 0.090,  4.2,  1350));
            Engines.Add(new Engine("СПД-140Д",   1750, 0.290,  8.5,  4500));
            Engines.Add(new Engine("ПлаС-34",    1300, 0.022,  0.97, 360));
            Engines.Add(new Engine("ПлаС-40",    1880, 0.040,  1.2,  650));
            Engines.Add(new Engine("ПлаС-55",    1950, 0.072,  2.5,  1200));

            Platforms = new List<Platform>();
            Platforms.Add(new Platform("Карат",     140,  250));
            Platforms.Add(new Platform("Навигатор", 980,  1000));
            Platforms.Add(new Platform("Ямал",      1000, 3000));
            Platforms.Add(new Platform("АТОМ",      800,  2500));
            Platforms.Add(new Platform("DX",        50,   28));

            N_eng = 100;
            k_pcs = 0.007;
            k_tank = 0.1;
            mu_sa = 0.014;
            k_extra = 0.05;
            eta_pcs = 0.1;
            k_sa = 1.1;

            RE = 6.371e6;
            RL = 1.737e6;
            mu0 = 3.973e14;
            mu1 = 4.904e12;
            h0 = 500e3;
            h1 = 200e3;
            R0 = RE + h0;
            R1 = RL + h1;

            //J = 3800;
            J = Engines[0].J;
            Vgis = 0;
            //F = 0.009;
            F = Engines[0].F;

            dt0 = 100;
            eta = 0.1;

            //p_platform = 250;
            p_platform = Platforms[0].p;
            //p_eng = 1350;
            p_eng = Engines[0].p;
            p_engines = N_eng * p_eng;
            p_burt_0 = 10;
            p_burt = p_burt_0 * Convert.ToDouble(Math.Ceiling(Convert.ToDecimal(N_eng / 2)));
            p_fsts_0 = 20;
            p_fsts = p_fsts_0 * Convert.ToDouble(Math.Ceiling(Convert.ToDecimal(N_eng / 2)));
            p_pcs = eta_pcs * p_engines;
            p_saos = 10;
            p_eos = 10;
            p_sa = k_sa * (p_platform + N_eng * p_eng * (1 + eta_pcs) + Convert.ToDouble(Math.Ceiling(Convert.ToDecimal(N_eng / 2))) * (p_burt_0 + p_fsts_0) + p_saos + p_eos);
            p0 = p_sa / k_sa;

            //m_platform = 140;
            m_platform = Platforms[0].m;
            //m_eng = 4;
            m_eng = Engines[0].m;
            m_engines = N_eng * m_eng;
            m_pcs = k_pcs * p_pcs;
            m_sa = mu_sa * p_sa;
            m_saos = 10;
            m_eos = 10;
            m0 = 100;
            m0_iter_cnt = 0;
            track = new Track(m0);
            Calc();
            m_f_main = eta * m0;
            m_f_extra = k_extra * m_f_main;
            m_fuel = m_f_main + m_f_extra;
            m_fsts = k_tank * m_fuel;

            acc = N_eng * F / m0;
            S_sa = p_sa / 280;
            T_miss = track.T_miss;

            eng_name = "ВЧИД-8";
            platform_name = "Карат";
        }

        public void Calc_Eta()
        {
            //eta = 1.17 * track.mfuelrelacc(mu0, R0, m0, N_eng * F, J, Vgis, dt0);
            eta = track.mfuelrel(mu0, mu1, R0, R1, m0, N_eng*F, J, Vgis, dt0);
            //m0_iter_cnt = track.cnt_1;
        }

        // Вычисляем p0 из уравнения баланса энергии
        public void Calc_P_sa()
        {
            p_sa = k_sa * (p_platform + N_eng * p_eng * (1 + eta_pcs) + Convert.ToDouble(Math.Ceiling(Convert.ToDecimal(N_eng / 2))) * (p_burt_0 + p_fsts_0) + p_saos + p_eos);
            p0 = p_sa / k_sa;
        }

        // Вычисляем m0 из уравнения баланса массы
        public void Calc_M0()
        {   
            m0 = (m_platform + N_eng * m_eng + k_pcs * eta_pcs * N_eng * p_eng + mu_sa * p_sa + m_saos + m_eos) / (1 - eta * (1 + k_extra) * (1 + k_tank));
        }

        // Метод простых итераций для вычисления m0
        public void Calc()
        {
            double tol = 1e-3;
            double m_prev = -1;
            m0_iter_cnt = 0;
            track.cnt_1 = 0;
            while (Math.Abs(m0 - m_prev) > tol)
            {
                m0_iter_cnt++;
                m_prev = m0;
                Calc_P_sa();
                Calc_M0();
                Calc_Eta();
            }

            /*
            for (int i = 0; i < 5; ++i)
            {
                Calc_P_sa();
                Calc_M0();
                Calc_Eta();
            }*/
        }

        public string GetMembersStr()
        {
            string s = "";
            System.Reflection.MemberInfo[] memberlist = typeof(Satellite).GetMembers();
            int ck = 0;
            for (int i = 0; i < memberlist.Length; ++i)
            {
                //Console.WriteLine(Convert.ToString(memberlist[i]));
                if (Convert.ToString(memberlist[i]) == "N_eng")
                {
                    ck = 1;
                    s += Convert.ToString(memberlist[i]);
                }
                else if(ck == 1)
                {
                    s = s+ '\t' + Convert.ToString(memberlist[i]);
                } 
            }
            return s;
        }
    }
}
