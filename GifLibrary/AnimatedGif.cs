using System;

namespace GifLibrary
{
    public static class AnimatedGif
    {
        /// <summary>
        /// Create a new Animated GIF
        /// </summary>
        /// <param name="repeat">Количество повторов GIF (0 означает бесконечно)</param>

        public static AnimatedGifCreator Create(string filePath, int repeat = 0)
        {
            return new AnimatedGifCreator(filePath, repeat);
        }
    }
}
