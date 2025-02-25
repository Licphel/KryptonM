﻿namespace KryptonM.Maths;

public class NoisePerlin : Noise
{

    private readonly int[] p = new int[512];
    private readonly int[] perm0 = new int[256];
    private Seed seed;

    public NoisePerlin(Seed seed)
    {
        this.seed = seed;

        for(var i = 0; i < 256; i++) perm0[i] = seed.NextInt(256);

        for(var i = 0; i < 256; i++) p[i + 256] = p[i] = perm0[i];
    }

    //From Ken Perlin.
    public float Generate(float x, float y, float z)
    {
        int X = floor(x) & 255,
            Y = floor(y) & 255,
            Z = floor(z) & 255;
        x -= floor(x);
        y -= floor(y);
        z -= floor(z);
        float u = fade(x),
            v = fade(y),
            w = fade(z);
        int A = p[X] + Y,
            AA = p[A] + Z,
            AB = p[A + 1] + Z,
            B = p[X + 1] + Y,
            BA = p[B] + Z,
            BB = p[B + 1] + Z;

        return lerp(w, lerp(v, lerp(u, grad(p[AA], x, y, z),
                    grad(p[BA], x - 1, y, z)
                ),
                lerp(u, grad(p[AB], x, y - 1, z),
                    grad(p[BB], x - 1, y - 1, z)
                )
            ),
            lerp(v, lerp(u, grad(p[AA + 1], x, y, z - 1),
                    grad(p[BA + 1], x - 1, y, z - 1)
                ),
                lerp(u, grad(p[AB + 1], x, y - 1, z - 1),
                    grad(p[BB + 1], x - 1, y - 1, z - 1)
                )
            )
        ) / 2 + 0.5f;
    }

    private static int floor(float v)
    {
        var i = (int)v;
        return v >= i ? i : i - 1;
    }

    private static float fade(float t)
    {
        return t * t * t * (t * (t * 6 - 15) + 10);
    }

    private static float lerp(float t, float a, float b)
    {
        return a + t * (b - a);
    }

    private static float grad(int hash, float x, float y, float z)
    {
        var h = hash & 15;
        float u = h < 8 ? x : y,
            v = h < 4 ? y : h == 12 || h == 14 ? x : z;
        return ((h & 1) == 0 ? u : -u) + ((h & 2) == 0 ? v : -v);
    }

}