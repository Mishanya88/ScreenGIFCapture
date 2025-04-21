using System;

namespace GifLibrary
{
    public static class AnimatedGif
    {
        /// <summary>
        /// Create a new Animated GIF
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="delay"></param>
        /// <param name="repeat">Количество повторов GIF (0 означает бесконечно)</param>
        /// 
        public static AnimatedGifCreator Create(string filePath, int delay, int repeat = 0)
        {
            return new AnimatedGifCreator(filePath, delay, repeat);
        }
    }
}
