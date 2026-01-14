namespace Waves
{
    public interface IWaveGenerator
    {
        GeneratedWaveData GenerateWave(int waveNumber, WaveGenerationProfile profile);
        GeneratedWaveData PreviewWave(int waveNumber, WaveGenerationProfile profile);
    }
}

