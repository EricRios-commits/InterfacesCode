namespace Gaze
{
    public interface IGazeProgressProvider
    {
        public float GazeProgress { get; }
        public bool IsGazing { get; }
        
    }
}
