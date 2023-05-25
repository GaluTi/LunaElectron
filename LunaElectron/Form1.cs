using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.ComponentModel.Design;
using System.Windows.Forms;
using System.Threading;
using TrackNS;

namespace LunaElectron
{
    public partial class Form1 : Form
    {
        Satellite sat;
        string path;

        public Form1()
        {
            InitializeComponent();

            path = @"D:\Programming\C#\LunaElectron\LunaElectron\Result_orbit.txt";
            UpdFile(DateTime.Now.ToString("yyyy-mm-dd HH:mm:ss"));
            UpdFile(GetSatMembersStr());

            sat = new Satellite();
            UpdForm();

            string[] members = GetSatMembersStr().Split('\t');

            var column1 = new DataGridViewColumn();
            column1.HeaderText = "Параметр";                        // текст в шапке
            column1.Width = 200;                                    // ширина колонки
            column1.ReadOnly = true;                                // значение в этой колонке нельзя править
            column1.Name = "param";                                 // текстовое имя колонки, его можно использовать вместо обращений по индексу
            column1.Frozen = true;                                  // флаг, что данная колонка всегда отображается на своем месте
            column1.CellTemplate = new DataGridViewTextBoxCell();   // тип нашей колонки
            dataGridView1.Columns.Add(column1);

            table_upd();
            UpdFile(GetSatValuesStr());

            comboBox1.Items.Add("Двигатель");
            for (int i = 0; i < sat.Engines.Count; i++)
            {
                comboBox1.Items.Add(sat.Engines[i].name);
            }
            comboBox1.SelectedIndex= 1;

            comboBox2.Items.Add("Платформа");
            for (int i = 0; i < sat.Platforms.Count; i++)
            {
                comboBox2.Items.Add(sat.Platforms[i].name);
            }
            comboBox2.SelectedIndex = 1;
        }

        
        public void UpdSat()
        {
            sat.N_eng = Convert.ToDouble(textBox1.Text);
            sat.k_pcs = Convert.ToDouble(textBox2.Text);
            sat.k_tank = Convert.ToDouble(textBox3.Text);
            sat.mu_sa = Convert.ToDouble(textBox4.Text);
            sat.k_extra = Convert.ToDouble(textBox5.Text);
            sat.eta_pcs = Convert.ToDouble(textBox6.Text);
            sat.k_sa = Convert.ToDouble(textBox7.Text);

            sat.p_platform = Convert.ToDouble(textBox20.Text);
            sat.p_eng = Convert.ToDouble(textBox21.Text);
            sat.p_engines = sat.N_eng * sat.p_eng;
            sat.p_burt_0 = Convert.ToDouble(textBox23.Text);
            sat.p_burt = sat.p_burt_0 * Convert.ToDouble(Math.Ceiling(Convert.ToDecimal(sat.N_eng / 2)));
            sat.p_fsts_0 = Convert.ToDouble(textBox25.Text);
            sat.p_fsts = sat.p_fsts_0 * Convert.ToDouble(Math.Ceiling(Convert.ToDecimal(sat.N_eng / 2)));
            sat.p_pcs = sat.eta_pcs * sat.p_engines;
            sat.p_saos = Convert.ToDouble(textBox28.Text);
            sat.p_eos = Convert.ToDouble(textBox29.Text);
            sat.Calc_P_sa();

            sat.RE = 1000 * Convert.ToDouble(textBox32.Text);
            sat.RL = 1000 * Convert.ToDouble(textBox33.Text);
            sat.mu0 = Convert.ToDouble(textBox34.Text);
            sat.mu1 = Convert.ToDouble(textBox35.Text);
            sat.h0 = 1000 * Convert.ToDouble(textBox36.Text);
            sat.h1 = 1000 * Convert.ToDouble(textBox37.Text);
            sat.R0 = sat.RE + sat.h0;
            sat.R1 = sat.RL + sat.h1;
            sat.J = Convert.ToDouble(textBox40.Text);
            sat.F = Convert.ToDouble(textBox41.Text) / 1000;
            sat.Vgis = Convert.ToDouble(textBox42.Text);
            sat.dt0 = Convert.ToDouble(textBox43.Text);

            sat.m_platform = Convert.ToDouble(textBox8.Text);
            sat.m_eng = Convert.ToDouble(textBox9.Text);
            sat.m_engines = sat.N_eng * sat.m_eng;
            sat.m_pcs = sat.k_pcs * sat.p_pcs;
            sat.m_sa = sat.mu_sa * sat.p_sa;
            sat.m_saos = Convert.ToDouble(textBox14.Text);
            sat.m_eos = Convert.ToDouble(textBox15.Text);
            sat.Calc();
            sat.m_f_main = sat.eta * sat.m0;
            sat.m_f_extra = sat.k_extra * sat.m_f_main;
            sat.m_fuel = sat.m_f_main + sat.m_f_extra;
            sat.m_fsts = sat.k_tank * sat.m_fuel;

            sat.acc = sat.N_eng * sat.F / sat.m0;
            sat.S_sa = sat.p_sa / 280;
            sat.T_miss = sat.track.T_miss;
        }
        
        
        public void UpdForm()
        {
            textBox1.Text = Convert.ToString(sat.N_eng);
            textBox2.Text = Convert.ToString(sat.k_pcs);
            textBox3.Text = Convert.ToString(sat.k_tank);
            textBox4.Text = Convert.ToString(sat.mu_sa);
            textBox5.Text = Convert.ToString(sat.k_extra);
            textBox6.Text = Convert.ToString(sat.eta_pcs);
            textBox7.Text = Convert.ToString(sat.k_sa);

            textBox8.Text = Convert.ToString(sat.m_platform);
            textBox9.Text = Convert.ToString(sat.m_eng);
            textBox10.Text = Convert.ToString(sat.m_engines);
            textBox11.Text = Convert.ToString(sat.m_fsts);
            textBox12.Text = Convert.ToString(sat.m_pcs);
            textBox13.Text = Convert.ToString(sat.m_sa);
            textBox14.Text = Convert.ToString(sat.m_saos);
            textBox15.Text = Convert.ToString(sat.m_eos);
            textBox16.Text = Convert.ToString(sat.m0);
            textBox17.Text = Convert.ToString(sat.m_f_main);
            textBox18.Text = Convert.ToString(sat.m_f_extra);
            textBox19.Text = Convert.ToString(sat.m_fuel);

            textBox20.Text = Convert.ToString(sat.p_platform);
            textBox21.Text = Convert.ToString(sat.p_eng);
            textBox22.Text = Convert.ToString(sat.p_engines);
            textBox23.Text = Convert.ToString(sat.p_burt_0);
            textBox24.Text = Convert.ToString(sat.p_burt);
            textBox25.Text = Convert.ToString(sat.p_fsts_0);
            textBox26.Text = Convert.ToString(sat.p_fsts);
            textBox27.Text = Convert.ToString(sat.p_pcs);
            textBox28.Text = Convert.ToString(sat.p_saos);
            textBox29.Text = Convert.ToString(sat.p_eos);
            textBox30.Text = Convert.ToString(sat.p_sa);
            textBox31.Text = Convert.ToString(sat.p0);

            textBox32.Text = Convert.ToString(sat.RE / 1000);
            textBox33.Text = Convert.ToString(sat.RL / 1000);
            textBox34.Text = Convert.ToString(sat.mu0);
            textBox35.Text = Convert.ToString(sat.mu1);
            textBox36.Text = Convert.ToString(sat.h0 / 1000);
            textBox37.Text = Convert.ToString(sat.h1 / 1000);
            textBox38.Text = Convert.ToString(sat.R0 / 1000);
            textBox39.Text = Convert.ToString(sat.R1 / 1000);
            textBox40.Text = Convert.ToString(sat.J);
            textBox41.Text = Convert.ToString(1000 * sat.F);
            textBox42.Text = Convert.ToString(sat.Vgis);
            textBox43.Text = Convert.ToString(sat.dt0);
            textBox44.Text = Convert.ToString(sat.m0_iter_cnt);
            textBox45.Text = Convert.ToString(sat.track.cnt_1);
        }

        public string GetSatMembersStr()
        {
            string s = "";
            System.Reflection.MemberInfo[] memberlist = typeof(Satellite).GetMembers();
            int ck = 0;
            for (int i = 0; i < memberlist.Length; ++i)
            {
                string s_tmp = Convert.ToString(memberlist[i]);
                if (s_tmp == "Double N_eng")
                {
                    ck = 1;
                    string[] subs = s_tmp.Split(' ');
                    s = s + subs[1];
                }
                else if (ck == 1)
                {
                    string[] subs = s_tmp.Split(' ');
                    s = s + '\t' + subs[1];
                }
            }
            return s;
        }

        public string GetSatValuesStr()
        {
           string s = "";
           string[] members = GetSatMembersStr().Split('\t');

            var typ = typeof(Satellite);
            var f = typ.GetField("N_eng");
            var val = f.GetValue(sat);
            s = s + Convert.ToString(val);
            for (int i = 1; i < members.Length; ++i)
            {
                string member = members[i];
                //Console.WriteLine(member);
                f = typ.GetField(member);
                val = f.GetValue(sat);
                s = s + '\t' + Convert.ToString(val);
            }
            return s;
        }

        public void UpdFile(string s)
        {
            using (StreamWriter sw = File.AppendText(path))
            {
                sw.WriteLine(s);
            }
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            UpdSat();
            UpdForm();
            UpdFile(GetSatValuesStr());
            table_upd();
        }

        private void button2_Click(object sender, EventArgs e)
        {

            for (int i = 500; i <= 36000; i+=500)
            {
                /*sat.N_eng = (double)i;*/
                sat.N_eng = (double)4;
                sat.h0 = (double)i * 1000;
                UpdForm();
                UpdSat();
                UpdForm();
                UpdFile(GetSatValuesStr());
                table_upd();
            }
        }

        void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if((string)comboBox1.SelectedItem != "Двигатель")
            {
                for(int i = 0; i < sat.Engines.Count; ++i)
                {
                    Engine eng = sat.Engines[i];
                    if(eng.name == (string)comboBox1.SelectedItem)
                    {
                        sat.eng_name = eng.name;
                        sat.J = eng.J;
                        sat.F = eng.F;
                        sat.m_eng = eng.m;
                        sat.p_eng = eng.p;
                        break;
                    }
                }
            }
            else
            {
                sat.eng_name = "Кастом";
            }
            UpdForm();
        }

        void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((string)comboBox2.SelectedItem != "Платформа")
            {
                for (int i = 0; i < sat.Platforms.Count; ++i)
                {
                    Platform plat = sat.Platforms[i];
                    if (plat.name == (string)comboBox2.SelectedItem)
                    {
                        sat.platform_name = plat.name;
                        sat.m_platform = plat.m;
                        sat.p_platform = plat.p;
                        break;
                    }
                }
            }
            else
            {
                sat.platform_name = "Кастом";
            }
            UpdForm();
        }

        private void table_upd()
        {
            //dataGridView1.Columns.Clear();
            //dataGridView1.Rows.Clear();
            /*
            string[] members = GetSatMembersStr().Split('\t');
        
            var column1 = new DataGridViewColumn();
            column1.HeaderText = "Параметр";                        // текст в шапке
            column1.Width = 200;                                    // ширина колонки
            column1.ReadOnly = true;                                // значение в этой колонке нельзя править
            column1.Name = "param";                                 // текстовое имя колонки, его можно использовать вместо обращений по индексу
            column1.Frozen = true;                                  // флаг, что данная колонка всегда отображается на своем месте
            column1.CellTemplate = new DataGridViewTextBoxCell();   // тип нашей колонки
            dataGridView1.Columns.Add(column1);*/

            var column2 = new DataGridViewColumn();
            column2.HeaderText = "Значение";                        // текст в шапке
            column2.ReadOnly = true;                                // значение в этой колонке нельзя править
            column2.Width = 125;                                    // ширина колонки
            column2.Name = "var";                                   // текстовое имя колонки, его можно использовать вместо обращений по индексу
            column2.Frozen = true;                                  // флаг, что данная колонка всегда отображается на своем месте
            column2.CellTemplate = new DataGridViewTextBoxCell();   // тип нашей колонки
            dataGridView1.Columns.Add(column2);
            //dataGridView1.Columns["var"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight; // сдвигаем данные направо

            dataGridView1.AllowUserToAddRows = false; //запрешаем пользователю самому добавлять строки

            // Важные
            dataGridView1.Rows.Add("Наименование платформы", sat.platform_name);
            dataGridView1.Rows.Add("Масса платформы, кг", sat.m_platform);
            dataGridView1.Rows.Add("Наименование двигателя", sat.eng_name);
            dataGridView1.Rows.Add("Удельный импульс, с", sat.J);
            dataGridView1.Rows.Add("Сила тяги двигателя, мН", 1000 * sat.F);
            dataGridView1.Rows.Add("Количество двигателей", sat.N_eng);
            dataGridView1.Rows.Add("Масса КА стартовая, кг", Math.Round(sat.m0, 1));
            dataGridView1.Rows.Add("Масса РТ, кг", Math.Round(sat.m_fuel), 2);
            dataGridView1.Rows.Add("Относительный расход РТ", Math.Round(sat.eta, 2));
            dataGridView1.Rows.Add("Масса ДУ, кг", sat.m_eng);
            dataGridView1.Rows.Add("Масса СОСБ, кг", sat.m_saos);
            dataGridView1.Rows.Add("Масса СОДУ, кг", sat.m_eos);
            dataGridView1.Rows.Add("Мощность СБ, Вт", Math.Round(sat.p_sa, 1));
            dataGridView1.Rows.Add("Площадь СБ, м2", Math.Round(sat.p_sa / 280, 1));
            dataGridView1.Rows.Add("Ускорение, мм/с2", Math.Round(1000 * sat.N_eng*sat.F / sat.m0, 2));
            //dataGridView1.Rows.Add("время перелёта, сут.", Math.Round(1.17 * sat.track.T_miss / (24*3600), 1));
            dataGridView1.Rows.Add("Время перелёта, сут.", Math.Round(sat.track.T_miss / (24 * 3600), 1));
            dataGridView1.Rows.Add("Высота околоземной орбиты, км", sat.h0 / 1000);
            dataGridView1.Rows.Add("Высота окололунной орбиты, км", sat.h1 / 1000);
            dataGridView1.Rows.Add("", "");

            /*
            dataGridView1.Rows.Add("коэффициент массы системы питания и управления", sat.k_pcs);
            dataGridView1.Rows.Add("коэффициент массы бака", sat.k_tank);
            dataGridView1.Rows.Add("коэффициент массы солнечных батарей", sat.k_sa);
            dataGridView1.Rows.Add("коэффициент массы дополниельного запаса рабочего тела", sat.k_extra);
            dataGridView1.Rows.Add("(1 - КПД) системы питания и управления", sat.eta_pcs);
            dataGridView1.Rows.Add("коэффициент запаса мощности солнечных батарей", sat.k_sp);

            dataGridView1.Rows.Add("потребляемая мощность платформы", sat.p_platform);
            dataGridView1.Rows.Add("потребляемая мощность двигателя", sat.p_engine);
            dataGridView1.Rows.Add("потребляемая мощность блока управления расходом топлива", sat.p_burt);
            dataGridView1.Rows.Add("потребляемая мощность системы хранения и передачи рабочего тела", sat.p_fsts);
            dataGridView1.Rows.Add("потребляемая мощность системы ориентации солнечных батарей", sat.p_saos);
            dataGridView1.Rows.Add("потребляемая мощность системы ориентации двигательной установки", sat.p_eos);
            dataGridView1.Rows.Add("энергопотребление бортовой аппаратуры КА", sat.p0);
            dataGridView1.Rows.Add("гиперболический избыток скорости", sat.Vgis);
            dataGridView1.Rows.Add("количество итераций в методе простых итераций", sat.m0_iter_cnt);*/
        }
    }
}
