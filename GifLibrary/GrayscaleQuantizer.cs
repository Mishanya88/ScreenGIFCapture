namespace GifLibrary
{
    using System;
    using System.Collections;
    using System.Drawing;

    /// <summary>
    ///     Класс для квантования палитры в градациях серого.
    /// </summary>
    public class GrayscaleQuantizer : PaletteQuantizer
    {
        /// <summary>
        ///     Конструктор квантовщика палитры
        /// </summary>
        /// <remarks>
        ///     Для квантования палитры требуется только один шаг квантования
        /// </remarks>
        public GrayscaleQuantizer()
            : base(new ArrayList())
        {
            Colors = new Color[256];

            const int nColors = 256;

            // Инициализация новой таблицы цветов с записями, которые определяются
            // оптимизированным алгоритмом поиска палитры; для демонстрации используется градация серого.
            for (uint i = 0; i < nColors; i++)
            {
                const uint alpha = 0xFF; // Цвета непрозрачные.
                uint intensity = Convert.ToUInt32(i * 0xFF / (nColors - 1)); // Равномерное распределение.

                // В GIF-энкодере первый элемент в палитре с нулевым альфа-каналом
                // становится прозрачным цветом в GIF.
                // Для демонстрации выберем первый произвольный цвет.

                // Создание градации серого для демонстрации.
                // В другом случае используйте алгоритм уменьшения количества цветов,
                // и сгенерируйте оптимальную палитру для этого алгоритма.
                // Например, цветовой гистограммой или палитрой медианного разделения.
                Colors[i] = Color.FromArgb((int)alpha,
                    (int)intensity,
                    (int)intensity,
                    (int)intensity);
            }
        }

        /// <summary>
        ///     Переопределите этот метод для обработки пикселя на втором проходе алгоритма
        /// </summary>
        /// <param name="pixel">Пиксель для квантования</param>
        /// <returns>Квантованное значение</returns>
        protected override byte QuantizePixel(Color32 pixel)
        {
            double luminance = pixel.Red * 0.299 + pixel.Green * 0.587 + pixel.Blue * 0.114;

            // Градации серого представляют собой карту интенсивности от черного до белого.
            // Вычисляем индекс в таблице градаций серого, который
            // приближает интенсивность, и затем округляем индекс.
            // Также ограничиваем выбор индекса количеством цветов, которые нужно создать,
            // и затем задаем индекс этого пикселя в качестве байтового значения.
            byte colorIndex = (byte)(luminance + 0.5);

            return colorIndex;
        }
    }
}