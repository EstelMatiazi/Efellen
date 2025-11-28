using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Network;

namespace Server.EffectsUtil
{
    public static class SlamVisuals
    {
        public static void SlamVisual(Mobile from, int radius, int effectID, int hue)
        {
            if (from == null || from.Deleted || from.Map == null)
                return;

            Map map = from.Map;
            Point3D center = from.Location;

            SlamRing(from, center, radius, effectID, hue);
        }

        public static void SlamRing(Mobile from, Point3D center, int radius, int effectID, int hue)
        {
            if (from == null || from.Deleted || from.Map == null)
                return;

            Map map = from.Map;

            int delayMS = 90;

            for (int r = 1; r <= radius; r++)
            {
                int ringRadius = r;
                Timer.DelayCall(TimeSpan.FromMilliseconds(r * delayMS), delegate
                {
                    DoRing(from, center, ringRadius, effectID, hue, map);
                });
            }
        }

        private static void DoRing(Mobile from, Point3D center, int radius, int effectID, int hue, Map map)
        {
            int x0 = center.X;
            int y0 = center.Y;

            for (int x = -radius; x <= radius; x++)
            {
                for (int y = -radius; y <= radius; y++)
                {
                    if (Math.Abs((Math.Abs(x) + Math.Abs(y)) - radius) <= 1)
                    {
                        int px = x0 + x;
                        int py = y0 + y;
                        int pz = map.GetAverageZ(px, py);

                        Point3D target = new Point3D(px, py, pz);

                        Effects.SendLocationEffect(target, map, effectID, 15, hue, 0);
                    }
                }
            }
        }
    }
}
