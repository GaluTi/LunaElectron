using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LunaElectron
{
    class Platform
    {
        public string name;
        public double m;
        public double p;

        public Platform(string name, double m, double p)
        {
            this.name = name;
            this.m = m;
            this.p = p;
        }
    }
}
