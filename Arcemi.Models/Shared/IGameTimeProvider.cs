using System;

namespace Arcemi.Models
{
    public interface IGameTimeProvider
    {
        TimeSpan Get();
    }
}