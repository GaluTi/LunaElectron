using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackNS
{
    public class Track
    {
        public double F, vc, mu, T_miss;

        public List<PhaseVector> track(double GM, double R0,
        double m0, double rF, double J, double Vgis, double dt0)
        {
            mu = GM;
            F = rF;
            vc = 9.81 * J;
            double t0 = 0;
            double V0 = Math.Sqrt(mu / R0);
            Vector r0 = new Vector(R0, 0, 0);
            Vector v0 = new Vector(0, V0, 0);
            PhaseVector Q0 = new PhaseVector(r0, v0, m0, t0);

            List<PhaseVector> Q = new List<PhaseVector>();
            Q.Add(Q0);

            double dt;
            double st;



            double k(PhaseVector q)
            {
                return vt(q.r, q.v, q.m).Abs() / q.v.Abs();
            }
            double k0 = k(Q0);

            int i = 0;

            double E2(PhaseVector q)
            {
                return q.v * q.v - 2 * mu / q.r.Abs();
            }


            while (true)
            {
                dt = k0 / k(Q[i]) * dt0;
                Q.Add(Q[i] + dq(Q[i], dt));
                if (E2(Q[i + 1]) > Vgis * Vgis)
                {
                    while (true)
                    {
                        st = dt * (E2(Q[i + 1]) - Vgis) / (E2(Q[i + 1]) - E2(Q[i]));
                        dt = dt - st;
                        Q[i + 1] = new PhaseVector(Q[i] + dq(Q[i], dt));
                        if (st < 0.001 * dt0)
                        {
                            break;
                        }
                    }
                    break;
                }
                i++;
            }
            T_miss = Q[Q.Count - 1].t - t0;
            return Q;
        }

        public double mfuelrelacc(double GM, double R0,
        double m0, double rF, double J, double Vgis, double dt0)
        {
            mu = GM;
            F = rF;
            vc = 9.81 * J;
            double t0 = 0;
            double V0 = Math.Sqrt(mu / R0);
            Vector r0 = new Vector(R0, 0, 0);
            Vector v0 = new Vector(0, V0, 0);
            PhaseVector Q0 = new PhaseVector(r0, v0, m0, t0);

            List<PhaseVector> Q = new List<PhaseVector>();
            Q.Add(Q0);

            double dt;
            double st;



            double k(PhaseVector q)
            {
                return vt(q.r, q.v, q.m).Abs() / q.v.Abs();
            }
            double k0 = k(Q0);

            int i = 0;

            double E2(PhaseVector q)
            {
                return q.v * q.v - 2 * mu / q.r.Abs();
            }


            while (true)
            {
                dt = k0 / k(Q[i]) * dt0;
                Q.Add(Q[i] + dq(Q[i], dt));
                if (E2(Q[i + 1]) > Vgis * Vgis)
                {
                    while (true)
                    {
                        st = dt * (E2(Q[i + 1]) - Vgis) / (E2(Q[i + 1]) - E2(Q[i]));
                        dt = dt - st;
                        Q[i + 1] = new PhaseVector(Q[i] + dq(Q[i], dt));
                        if (st < 0.001 * dt0)
                        {
                            break;
                        }
                    }
                    break;
                }
                i++;
            }
            T_miss = Q[Q.Count - 1].t - t0;
            return (m0 - Q[Q.Count - 1].m) / m0;
        }

        public double mfuelreldec(double GM, double R0_X, double R0_Y, double V0_X, double V0_Y,
        double m0, double rF, double J, double Vgis, double dt0)
        {
            mu = GM;
            F = rF;
            vc = 9.81 * J;
            double t0 = 0;
            //double V0 = Math.Sqrt(mu / R0);
            Vector r0 = new Vector(R0_X, R0_Y, 0);
            Vector v0 = new Vector(V0_X, V0_Y, 0);
            PhaseVector Q0 = new PhaseVector(r0, v0, m0, t0);

            List<PhaseVector> Q = new List<PhaseVector>();
            Q.Add(Q0);

            double dt;
            double st;



            double k(PhaseVector q)
            {
                return vt(q.r, q.v, q.m).Abs() / q.v.Abs();
            }
            double k0 = k(Q0);

            int i = 0;

            double E2(PhaseVector q)
            {
                return q.v * q.v - 2 * mu / q.r.Abs();
            }


            while (true)
            {
                dt = k0 / k(Q[i]) * dt0;
                Q.Add(Q[i] + dq(Q[i], dt));
                if (E2(Q[i + 1]) > Vgis * Vgis)
                {
                    while (true)
                    {
                        st = dt * (E2(Q[i + 1]) - Vgis) / (E2(Q[i + 1]) - E2(Q[i]));
                        dt = dt - st;
                        Q[i + 1] = new PhaseVector(Q[i] + dq(Q[i], dt));
                        if (st < 0.001 * dt0)
                        {
                            break;
                        }
                    }
                    break;
                }
                i++;
            }
            T_miss = Q[Q.Count - 1].t - t0;
            return (m0 - Q[Q.Count - 1].m) / m0;
        }

        private Vector rt(Vector v) { return v; }
        private Vector vt(Vector r, Vector v, double m) { return -mu / (r * r) * r.Dir() + F / m * v.Dir(); }
        private double mt() { return -Math.Abs(F) / vc; }
        private double tt() { return 1; }

        public PhaseVector qt1(Vector r, Vector v, double m, double t)
        { return new PhaseVector(rt(v), vt(r, v, m), mt(), tt()); }
        private PhaseVector qt(PhaseVector q)
        { return qt1(q.r, q.v, q.m, q.t); }

        private PhaseVector dq(PhaseVector x, double dt)
        {
            PhaseVector k1 = qt(x) * dt;
            PhaseVector k2 = qt(x + 0.5 * k1) * dt;
            PhaseVector k3 = qt(x + 0.5 * k2) * dt;
            PhaseVector k4 = qt(x + k3) * dt;
            return 1.0 / 6.0 * (k1 + 2.0 * k2 + 2.0 * k3 + k4);
        }
    }

    public class Vector
    {
        public double x = 0.0, y = 0.0, z = 0.0;
        public Vector(double X = 0.0, double Y = 0.0, double Z = 0.0)
        { x = X; y = Y; z = Z; }
        public static Vector operator +(Vector A, Vector B)
        { return new Vector(A.x + B.x, A.y + B.y, A.z + B.z); }
        public static Vector operator -(Vector A, Vector B)
        { return new Vector(A.x - B.x, A.y - B.y, A.z - B.z); }
        public static Vector operator *(double a, Vector A)
        { return new Vector(a * A.x, a * A.y, a * A.z); }
        public static Vector operator *(Vector A, double a)
        { return new Vector(a * A.x, a * A.y, a * A.z); }
        public static Vector operator /(Vector A, double a)
        { return new Vector(A.x / a, A.y / a, A.z / a); }
        public static double operator *(Vector A, Vector B)
        { return A.x * B.x + A.y * B.y + A.z * B.z; }
        public double Abs()
        { return Math.Sqrt(x * x + y * y + z * z); }
        public Vector Dir()
        { return this / this.Abs(); }
        public Vector Cross(Vector A, Vector B)
        { return new Vector(A.y * B.z - A.z * B.y, A.z * B.x - A.x * B.z, A.x * B.y - A.y * B.x); }
    }
    public class PhaseVector
    {
        public Vector r = new Vector(0.0, 0.0, 0.0);
        public Vector v = new Vector(0.0, 0.0, 0.0);
        public double m = 0.0;
        public double t = 0.0;
        public PhaseVector(Vector R, Vector V, double M, double T)
        { r = R; v = V; m = M; t = T; }
        public PhaseVector(PhaseVector Q)
        { r = Q.r; v = Q.v; m = Q.m; t = Q.t; }
        public static PhaseVector operator +(PhaseVector A, PhaseVector B)
        { return new PhaseVector(A.r + B.r, A.v + B.v, A.m + B.m, A.t + B.t); }
        public static PhaseVector operator -(PhaseVector A, PhaseVector B)
        { return new PhaseVector(A.r - B.r, A.v - B.v, A.m - B.m, A.t - B.t); }
        public static PhaseVector operator *(double a, PhaseVector A)
        { return new PhaseVector(a * A.r, a * A.v, a * A.m, a * A.t); }
        public static PhaseVector operator *(PhaseVector A, double a)
        { return new PhaseVector(a * A.r, a * A.v, a * A.m, a * A.t); }
        public static PhaseVector operator /(PhaseVector A, double a)
        { return new PhaseVector(A.r / a, A.v / a, A.m / a, A.t / a); }
    }
}
