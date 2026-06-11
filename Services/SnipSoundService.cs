using System;
using System.IO;
using System.Media;
using System.Threading.Tasks;

namespace PresetSnip.Services;

public sealed class SnipSoundService
{
    private const int SampleRate = 44100;

    public void Play(double volume)
    {
        var clampedVolume = Math.Clamp(volume, 0, 1);
        if (clampedVolume <= 0)
        {
            return;
        }

        Task.Run(() =>
        {
            using var stream = BuildSnipWave(clampedVolume);
            using var player = new SoundPlayer(stream);
            player.Load();
            player.PlaySync();
        });
    }

    private static MemoryStream BuildSnipWave(double volume)
    {
        var sampleCount = (int)(SampleRate * 0.18);
        var samples = new short[sampleCount];

        for (var i = 0; i < sampleCount; i++)
        {
            var t = i / (double)SampleRate;
            var sliceEnvelope = Math.Exp(-t * 34);
            var clickEnvelope = Math.Exp(-Math.Pow((t - 0.055) * 92, 2));
            var tailEnvelope = Math.Exp(-Math.Pow((t - 0.105) * 58, 2));

            var slice = Math.Sin(2 * Math.PI * 2400 * t) * sliceEnvelope * 0.55;
            var click = Math.Sin(2 * Math.PI * 720 * t) * clickEnvelope * 0.35;
            var tail = Math.Sin(2 * Math.PI * 1280 * t) * tailEnvelope * 0.20;
            var sample = (slice + click + tail) * volume;

            samples[i] = (short)Math.Clamp(sample * short.MaxValue, short.MinValue, short.MaxValue);
        }

        var stream = new MemoryStream();
        using (var writer = new BinaryWriter(stream, System.Text.Encoding.ASCII, leaveOpen: true))
        {
            var dataLength = samples.Length * sizeof(short);

            writer.Write("RIFF".ToCharArray());
            writer.Write(36 + dataLength);
            writer.Write("WAVE".ToCharArray());
            writer.Write("fmt ".ToCharArray());
            writer.Write(16);
            writer.Write((short)1);
            writer.Write((short)1);
            writer.Write(SampleRate);
            writer.Write(SampleRate * sizeof(short));
            writer.Write((short)sizeof(short));
            writer.Write((short)16);
            writer.Write("data".ToCharArray());
            writer.Write(dataLength);

            foreach (var sample in samples)
            {
                writer.Write(sample);
            }
        }

        stream.Position = 0;
        return stream;
    }
}
