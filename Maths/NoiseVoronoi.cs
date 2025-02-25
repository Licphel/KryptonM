namespace KryptonM.Maths;

public class NoiseVoronoi : Noise
{

    private readonly long seed;

    public NoiseVoronoi(Seed seed)
    {
        this.seed = seed.GetCSeed();
    }

    public float Generate(float x, float y, float z)
    {
        var X = floor(x);
        var Y = floor(y);
        var Z = floor(z);

        float xc = 0;
        float yc = 0;
        float zc = 0;
        float md = int.MaxValue;

        for(var k = Z - 2; k <= Z + 2; k++)
        for(var j = Y - 2; j <= Y + 2; j++)
        for(var i = X - 2; i <= X + 2; i++)
        {
            var xp = i + seedl(i, j, k, seed);
            var yp = j + seedl(i, j, k, seed + 1);
            var zp = k + seedl(i, j, k, seed + 2);
            var xd = xp - x;
            var yd = yp - y;
            var zd = zp - z;
            var d = xd * xd + yd * yd + zd * zd;

            if(d < md)
            {
                md = d;
                xc = xp;
                yc = yp;
                zc = zp;
            }
        }

        float v;

        return seedl(floor(xc), floor(yc), floor(zc), 0);
    }

    private static int floor(float v)
    {
        var i = (int)v;
        return v >= i ? i : i - 1;
    }

    private static float seedl(int x, int y, int z, long seed)
    {
        var v1 = (x + 2687 * y + 433 * z + 941 * seed) & int.MaxValue;
        var v2 = (v1 * (v1 * v1 * 113 + 653) + 2819) & int.MaxValue;
        return 1 - (float)v2 / int.MaxValue;
    }

}