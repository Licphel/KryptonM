namespace KryptonM.Maths;

public class NoiseOctave : Noise
{

    private readonly NoisePerlin[] noises;

    public NoiseOctave(Seed seed, int octave)
    {
        noises = new NoisePerlin[octave];

        for(var i = 0; i < octave; i++) noises[i] = new NoisePerlin(seed.Copyx(i));
    }

    public float Generate(float x, float y, float z)
    {
        float v = 0;
        var ampl = 1f / noises.Length;

        for(var i = 0; i < noises.Length; i++) v += noises[i].Generate(x, y, z) * ampl;
        return v;
    }

}