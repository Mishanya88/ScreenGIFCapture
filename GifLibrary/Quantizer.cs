namespace GifLibrary
{
    using System.Drawing.Imaging;
    using System.Drawing;
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Абстрактный базовый класс, реализующий алгоритм квантования изображений.
    /// </summary>
    public abstract class Quantizer
    {
        private readonly int _pixelSize;

        /// <summary>
        ///     Флаг, указывающий, требуется ли один проход или два прохода для квантования.
        /// </summary>
        private readonly bool _singlePass;

        /// <summary>
        ///     Конструктор квантователя
        /// </summary>
        /// <param name="singlePass">Если true, то квантование будет выполняться за один проход по исходным пикселям</param>
        /// <remarks>
        ///     Если вы создаёте этот класс с параметром singlePass = true, то при квантовании изображения
        ///     будет вызываться только функция 'QuantizeImage'. Если требуются два прохода, то сначала
        ///     будет вызвана 'InitialQuantizeImage', а затем 'QuantizeImage'.
        /// </remarks>
        protected Quantizer(bool singlePass)
        {
            _singlePass = singlePass;
            _pixelSize = Marshal.SizeOf(typeof(Color32));
        }

        /// <summary>
        ///     Квантовать изображение и вернуть результирующее растровое изображение
        /// </summary>
        /// <param name="source">Изображение для квантования</param>
        /// <returns>Квантованная версия изображения</returns>
        public Bitmap Quantize(Image source)
        {
            // Получить размеры исходного изображения
            int height = source.Height;
            int width = source.Width;

            // Создать прямоугольник из этих размеров
            var bounds = new Rectangle(0, 0, width, height);

            // Сначала сделать копию изображения в формате 32bpp
            var copy = new Bitmap(width, height, PixelFormat.Format32bppArgb);

            // Создать изображение в формате 8bpp
            var output = new Bitmap(width, height, PixelFormat.Format8bppIndexed);

            // Заблокировать изображение в памяти
            using (var g = Graphics.FromImage(copy))
            {
                g.PageUnit = GraphicsUnit.Pixel;

                // Нарисовать исходное изображение на копии,
                // это также обеспечит преобразование формата.
                g.DrawImage(source, bounds);
            }

            // Указатель на данные изображения
            BitmapData sourceData = null;

            try
            {
                // Получить биты исходного изображения и заблокировать их в памяти
                sourceData = copy.LockBits(bounds, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

                // Вызвать функцию первого прохода, если алгоритм не одношаговый.
                // Например, для Octree-квантователя, этот шаг проходит по всем пикселям изображения,
                // строит структуру данных и создаёт палитру.
                if (!_singlePass)
                    FirstPass(sourceData, width, height);

                // Установить палитру цветов в выходное изображение. Я передаю текущую палитру,
                // так как нет способа создать новую пустую палитру.
                output.Palette = GetPalette(output.Palette);

                // Затем вызвать второй проход, который выполняет само преобразование
                SecondPass(sourceData, output, width, height, bounds);
            }
            finally
            {
                // Разблокировать биты изображения
                copy.UnlockBits(sourceData);
            }

            // Вернуть готовое квантованное изображение
            return output;
        }

        /// <summary>
        ///     Выполнить первый проход по пикселям изображения
        /// </summary>
        /// <param name="sourceData">Исходные данные изображения</param>
        /// <param name="width">Ширина изображения в пикселях</param>
        /// <param name="height">Высота изображения в пикселях</param>
        protected virtual void FirstPass(BitmapData sourceData, int width, int height)
        {
            // Указатель на строку исходных данных. Используется byte для удобства добавления смещения (stride)
            var pSourceRow = sourceData.Scan0;

            // Проход по каждой строке
            for (int row = 0; row < height; row++)
            {
                // Указатель на первый пиксель в строке
                var pSourcePixel = pSourceRow;

                // Проход по каждому столбцу
                for (int col = 0; col < width; col++)
                {
                    InitialQuantizePixel(new Color32(pSourcePixel));
                    pSourcePixel = (IntPtr)((long)pSourcePixel + _pixelSize);
                }

                // Добавить смещение для перехода к следующей строке
                pSourceRow = (IntPtr)((long)pSourceRow + sourceData.Stride);
            }
        }

        /// <summary>
        ///     Выполнить второй проход по изображению
        /// </summary>
        /// <param name="sourceData">Исходное изображение, заблокированное в памяти</param>
        /// <param name="output">Выходное изображение</param>
        /// <param name="width">Ширина изображения в пикселях</param>
        /// <param name="height">Высота изображения в пикселях</param>
        /// <param name="bounds">Ограничивающий прямоугольник</param>
        protected virtual void SecondPass(BitmapData sourceData, Bitmap output, int width, int height,
            Rectangle bounds)
        {
            BitmapData outputData = null;

            try
            {
                // Заблокировать выходное изображение в памяти
                outputData = output.LockBits(bounds, ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);

                // Указатели на данные
                var pSourceRow = sourceData.Scan0;
                var pSourcePixel = pSourceRow;
                var pPreviousPixel = pSourcePixel;

                var pDestinationRow = outputData.Scan0;
                var pDestinationPixel = pDestinationRow;

                // Преобразовать первый пиксель
                byte pixelValue = QuantizePixel(new Color32(pSourcePixel));

                // Записать значение первого пикселя
                Marshal.WriteByte(pDestinationPixel, pixelValue);

                // Проход по каждой строке
                for (int row = 0; row < height; row++)
                {
                    pSourcePixel = pSourceRow;
                    pDestinationPixel = pDestinationRow;

                    // Проход по каждому пикселю в строке
                    for (int col = 0; col < width; col++)
                    {
                        // Если пиксель отличается от предыдущего, преобразовать его
                        if (Marshal.ReadInt32(pPreviousPixel) != Marshal.ReadInt32(pSourcePixel))
                        {
                            pixelValue = QuantizePixel(new Color32(pSourcePixel));
                            pPreviousPixel = pSourcePixel;
                        }

                        // Записать преобразованное значение
                        Marshal.WriteByte(pDestinationPixel, pixelValue);

                        pSourcePixel = (IntPtr)((long)pSourcePixel + _pixelSize);
                        pDestinationPixel = (IntPtr)((long)pDestinationPixel + 1);
                    }

                    // Перейти к следующей строке
                    pSourceRow = (IntPtr)((long)pSourceRow + sourceData.Stride);
                    pDestinationRow = (IntPtr)((long)pDestinationRow + outputData.Stride);
                }
            }
            finally
            {
                // Разблокировать выходные данные
                output.UnlockBits(outputData);
            }
        }

        /// <summary>
        ///     Переопределите этот метод, чтобы обработать пиксель в первом проходе алгоритма
        /// </summary>
        /// <param name="pixel">Пиксель для квантования</param>
        /// <remarks>
        ///     Этот метод необходимо переопределять только в случае, если алгоритму требуется два прохода,
        ///     как, например, в Octree-квантовании.
        /// </remarks>
        protected virtual void InitialQuantizePixel(Color32 pixel)
        {
        }

        /// <summary>
        ///     Переопределите этот метод для обработки пикселя во втором проходе алгоритма
        /// </summary>
        /// <param name="pixel">Пиксель для квантования</param>
        /// <returns>Квантованное значение</returns>
        protected abstract byte QuantizePixel(Color32 pixel);

        /// <summary>
        ///     Получить палитру для квантованного изображения
        /// </summary>
        /// <param name="original">Исходная палитра (перезаписывается)</param>
        /// <returns>Новая цветовая палитра</returns>
        protected abstract ColorPalette GetPalette(ColorPalette original);
    }



    /// <summary>
    ///     Структура, определяющая цвет в формате 32 бита на пиксель (32 bpp)
    /// </summary>
    /// <remarks>
    ///     Эта структура используется для чтения данных из изображения с глубиной цвета 32 бита на пиксель,
    ///     и поля расположены именно в таком порядке, так как именно так данные хранятся в памяти.
    /// </remarks>
    [StructLayout(LayoutKind.Explicit)]
        public struct Color32
        {
            public Color32(IntPtr pSourcePixel)
            {
                this = (Color32)Marshal.PtrToStructure(pSourcePixel, typeof(Color32));
            }

            /// <summary>
            ///     Содержит синий компонент цвета
            /// </summary>
            [FieldOffset(0)] public byte Blue;

            /// <summary>
            ///     Содержит зелёный компонент цвета
            /// </summary>
            [FieldOffset(1)] public byte Green;

            /// <summary>
            ///     Содержит красный компонент цвета
            /// </summary>
            [FieldOffset(2)] public byte Red;

            /// <summary>
            ///     Содержит альфа-компонент (прозрачность) цвета
            /// </summary>
            [FieldOffset(3)] public byte Alpha;

            /// <summary>
            ///     Позволяет трактовать Color32 как целое число (int32)
            /// </summary>
            [FieldOffset(0)] public int ARGB;

            /// <summary>
            ///     Возвращает объект Color для данного Color32
            /// </summary>
            public Color Color => Color.FromArgb(Alpha, Red, Green, Blue);
        }
}
