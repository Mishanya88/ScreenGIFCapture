namespace GifLibrary
{
    using System.Collections;
    using System.Drawing;
    using System.Drawing.Imaging;

    /// <summary>
    ///     Квантайзер на основе палитры (PaletteQuantizer).
    /// </summary>
    public class PaletteQuantizer : Quantizer
    {
        /// <summary>
        ///     Таблица соответствия цветов
        /// </summary>
        private readonly Hashtable _colorMap;

        /// <summary>
        ///     Список всех цветов палитры
        /// </summary>
        protected Color[] Colors;

        /// <summary>
        ///     Конструктор палитрового квантайзера
        /// </summary>
        /// <param name="palette">Цветовая палитра, к которой нужно квантовать</param>
        /// <remarks>
        ///     Палитровая квантование требует только одного прохода
        /// </remarks>
        public PaletteQuantizer(ArrayList palette)
            : base(true)
        {
            _colorMap = new Hashtable();

            Colors = new Color[palette.Count];
            palette.CopyTo(Colors);
        }

        /// <summary>
        ///     Переопределить этот метод для обработки пикселя во втором проходе алгоритма
        /// </summary>
        /// <param name="pixel">Пиксель, который нужно квантовать</param>
        /// <returns>Значение после квантования</returns>
        protected override byte QuantizePixel(Color32 pixel)
        {
            byte colorIndex = 0;
            int colorHash = pixel.ARGB;

            // Проверка, есть ли цвет в таблице
            if (_colorMap.ContainsKey(colorHash))
            {
                colorIndex = (byte)_colorMap[colorHash];
            }
            else
            {
                // Не найден — ищем наиболее близкое совпадение в палитре.
                // Сначала проверим альфа-канал — если 0, ищем прозрачный цвет
                if (pixel.Alpha == 0)
                {
                    // Прозрачный. Ищем первый цвет с альфа 0
                    for (int index = 0; index < Colors.Length; index++)
                        if (Colors[index].A == 0)
                        {
                            colorIndex = (byte)index;
                            break;
                        }
                }
                else
                {
                    // Непрозрачный...
                    int leastDistance = int.MaxValue;
                    int red = pixel.Red;
                    int green = pixel.Green;
                    int blue = pixel.Blue;

                    // Ищем наиболее близкий цвет в палитре
                    for (int index = 0; index < Colors.Length; index++)
                    {
                        var paletteColor = Colors[index];

                        int redDistance = paletteColor.R - red;
                        int greenDistance = paletteColor.G - green;
                        int blueDistance = paletteColor.B - blue;

                        int distance = redDistance * redDistance +
                                       greenDistance * greenDistance +
                                       blueDistance * blueDistance;

                        if (distance < leastDistance)
                        {
                            colorIndex = (byte)index;
                            leastDistance = distance;

                            // Если найдено точное совпадение, выходим из цикла
                            if (distance == 0)
                                break;
                        }
                    }
                }

                // Сохраняем результат в таблицу для ускорения следующего поиска
                _colorMap.Add(colorHash, colorIndex);
            }

            return colorIndex;
        }

        /// <summary>
        ///     Получить палитру для квантованного изображения
        /// </summary>
        /// <param name="palette">Произвольная палитра, будет перезаписана</param>
        /// <returns>Новая цветовая палитра</returns>
        protected override ColorPalette GetPalette(ColorPalette palette)
        {
            for (int index = 0; index < Colors.Length; index++)
                palette.Entries[index] = Colors[index];

            return palette;
        }
    }
}
