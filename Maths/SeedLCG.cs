namespace KryptonM.Maths;

public class SeedLCG : Seed
{

    private static readonly int A = 4097;
    private static readonly int C = 123435311;
    private static readonly int M = (int)Math.Pow(2, 32);

    public long InitialSeed;
    public long NowSeed;

    public SeedLCG(long initialSeed)
    {
        SetSeed(initialSeed);
    }

    public SeedLCG()
    {
        SetSeed(new Random().Next());
    }

    private long NewSeed()
    {
        NowSeed = (NowSeed * A + C) % M;
        return NowSeed;
    }

    public override float NextFloat()
    {
        return (float)NewSeed() / (M - 1);
    }

    public override int NextInt(int bound)
    {
        return (int)(bound * NextFloat());
    }

    public override void SetSeed(long seed)
    {
        InitialSeed = seed;
        NowSeed = seed;
    }

    public override long GetISeed()
    {
        return InitialSeed;
    }

    public override long GetCSeed()
    {
        return NowSeed;
    }

    public override Seed Copy()
    {
        return new SeedLCG(InitialSeed);
    }

    public override Seed Copyx(int offset)
    {
        return new SeedLCG(InitialSeed + offset);
    }

}