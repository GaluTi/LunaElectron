using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LunaElectron
{
    class Engine
    {
        public string name;
        public double J;
        public double F;
        public double m;
        public double p;

        public Engine(string name, double J, double F, double m, double p)
        {
            this.name = name;
            this.J = J;
            this.F = F;
            this.m = m;
            this.p = p;
        }
    }
}
