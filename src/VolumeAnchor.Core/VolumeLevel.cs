namespace VolumeAnchor.Core;

public readonly record struct VolumeLevel
{
    public float Value { get; }

    public VolumeLevel(float value)
    {
        if (!float.IsFinite(value) || value < 0f || value > 1f)
            throw new ArgumentOutOfRangeException(nameof(value));

        Value = value;
    }
}
