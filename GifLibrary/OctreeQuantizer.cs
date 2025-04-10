namespace GifLibrary
{
    using System;
    using System.Collections;
    using System.Drawing;
    using System.Drawing.Imaging;
    /// <summary>
    ///     Квантизация с использованием октанового дерева
    /// </summary>
    public class OctreeQuantizer : Quantizer
    {
        /// <summary>
        ///     Максимально допустимая глубина цвета
        /// </summary>
        private readonly int _maxColors;

        /// <summary>
        ///     Хранит дерево
        /// </summary>
        private readonly Octree _octree;

        /// <summary>
        ///     Конструктор октанового квантайзера
        /// </summary>
        /// <remarks>
        ///     Алгоритм октанового квантайзера состоит из двух проходов. Первый проход строит октановое дерево,
        ///     второй проход выполняет квантизацию цвета на основе узлов дерева
        /// </remarks>
        /// <param name="maxColors">Максимальное количество цветов для возвращаемого палитры</param>
        /// <param name="maxColorBits">Количество значимых бит</param>
        public OctreeQuantizer(int maxColors, int maxColorBits)
            : base(false)
        {
            if (maxColors > 255)
                throw new ArgumentOutOfRangeException(nameof(maxColors), maxColors,
                    "Количество цветов должно быть меньше 256");

            if ((maxColorBits < 1) | (maxColorBits > 8))
                throw new ArgumentOutOfRangeException(nameof(maxColorBits), maxColorBits,
                    "Значение должно быть от 1 до 8");

            // Создание октанового дерева
            _octree = new Octree(maxColorBits);
            _maxColors = maxColors;
        }

        /// <summary>
        ///     Обработать пиксель в первом проходе алгоритма
        /// </summary>
        /// <param name="pixel">Пиксель для квантизации</param>
        /// <remarks>
        ///     Эта функция переопределяется, только если ваш алгоритм квантизации требует двух проходов,
        ///     как, например, алгоритм октанового дерева.
        /// </remarks>
        protected override void InitialQuantizePixel(Color32 pixel)
        {
            // Добавить цвет в октановое дерево
            _octree.AddColor(pixel);
        }

        /// <summary>
        ///     Переопределите эту функцию для обработки пикселя во втором проходе алгоритма
        /// </summary>
        /// <param name="pixel">Пиксель для квантизации</param>
        /// <returns>Квантизированный цвет</returns>
        protected override byte QuantizePixel(Color32 pixel)
        {
            byte paletteIndex = (byte)_maxColors; // Цвет в [_maxColors] устанавливается как прозрачный

            // Получить индекс палитры, если это не прозрачный пиксель
            if (pixel.Alpha > 0)
                paletteIndex = (byte)_octree.GetPaletteIndex(pixel);

            return paletteIndex;
        }

        /// <summary>
        ///     Получить палитру для квантизированного изображения
        /// </summary>
        /// <param name="original">Старая палитра, которая будет перезаписана</param>
        /// <returns>Новая палитра цветов</returns>
        protected override ColorPalette GetPalette(ColorPalette original)
        {
            // Сначала преобразуем октановое дерево в палитру из _maxColors цветов
            var palette = _octree.Palletize(_maxColors - 1);

            // Затем преобразуем палитру в соответствии с этими цветами
            for (int index = 0; index < palette.Count; index++)
                original.Entries[index] = (Color)palette[index];

            // Добавляем прозрачный цвет
            original.Entries[_maxColors] = Color.FromArgb(0, 0, 0, 0);

            return original;
        }

        /// <summary>
        ///     Класс, который выполняет фактическую квантизацию
        /// </summary>
        private class Octree
        {
            /// <summary>
            ///     Маска, используемая для получения соответствующих пикселей для данного узла
            /// </summary>
            private static readonly int[] Mask = { 0x80, 0x40, 0x20, 0x10, 0x08, 0x04, 0x02, 0x01 };

            /// <summary>
            ///     Максимальное количество значимых бит в изображении
            /// </summary>
            private readonly int _maxColorBits;

            /// <summary>
            ///     Корень октанового дерева
            /// </summary>
            private readonly OctreeNode _root;

            /// <summary>
            ///     Кэширование последнего квантизированного цвета
            /// </summary>
            private int _previousColor;

            /// <summary>
            ///     Хранение последнего квантизированного узла
            /// </summary>
            private OctreeNode _previousNode;

            /// <summary>
            ///     Создание октанового дерева
            /// </summary>
            /// <param name="maxColorBits">Максимальное количество значимых бит в изображении</param>
            public Octree(int maxColorBits)
            {
                _maxColorBits = maxColorBits;
                Leaves = 0;
                ReducibleNodes = new OctreeNode[9];
                _root = new OctreeNode(0, _maxColorBits, this);
                _previousColor = 0;
                _previousNode = null;
            }

            private int Leaves { get; set; }

            protected OctreeNode[] ReducibleNodes { get; }

            /// <summary>
            ///     Добавить заданный цвет в октановое дерево
            /// </summary>
            /// <param name="pixel"></param>
            public void AddColor(Color32 pixel)
            {
                // Проверяем, был ли запрашиваемый цвет таким же, как последний
                if (_previousColor == pixel.ARGB)
                {
                    // Если да, проверим, есть ли у нас предыдущий узел
                    if (null == _previousNode)
                    {
                        _previousColor = pixel.ARGB;
                        _root.AddColor(pixel, _maxColorBits, 0, this);
                    }
                    else
                    // Просто обновляем предыдущий узел
                    {
                        _previousNode.Increment(pixel);
                    }
                }
                else
                {
                    _previousColor = pixel.ARGB;
                    _root.AddColor(pixel, _maxColorBits, 0, this);
                }
            }

            /// <summary>
            ///     Уменьшить глубину дерева
            /// </summary>
            private void Reduce()
            {
                int index;

                // Найти самый глубокий уровень, содержащий хотя бы один сокращаемый узел
                for (index = _maxColorBits - 1; index > 0 && null == ReducibleNodes[index]; index--) { }

                // Уменьшить узел, недавно добавленный в список на уровне 'index'
                var node = ReducibleNodes[index];
                ReducibleNodes[index] = node.NextReducible;

                // Уменьшаем количество листьев после сокращения узла
                Leaves -= node.Reduce();

                // На всякий случай, если я уменьшил последний добавленный цвет и следующий цвет
                // будет таким же, инвалидация предыдущего узла...
                _previousNode = null;
            }

            /// <summary>
            ///     Отслеживать последний узел, который был квантизирован
            /// </summary>
            /// <param name="node">Последний квантизированный узел</param>
            protected void TrackPrevious(OctreeNode node)
            {
                _previousNode = node;
            }

            /// <summary>
            ///     Преобразовать узлы в октановом дереве в палитру с максимальным количеством цветов
            /// </summary>
            /// <param name="colorCount">Максимальное количество цветов</param>
            /// <returns>Массив с палитризованными цветами</returns>
            public ArrayList Palletize(int colorCount)
            {
                while (Leaves > colorCount)
                    Reduce();

                // Теперь палетизуем узлы
                var palette = new ArrayList(Leaves);
                int paletteIndex = 0;
                _root.ConstructPalette(palette, ref paletteIndex);

                // И возвращаем палитру
                return palette;
            }

            /// <summary>
            ///     Получить индекс палитры для переданного цвета
            /// </summary>
            /// <param name="pixel"></param>
            /// <returns></returns>
            public int GetPaletteIndex(Color32 pixel)
            {
                return _root.GetPaletteIndex(pixel, 0);
            }

            /// <summary>
            ///     Класс, инкапсулирующий каждый узел дерева
            /// </summary>
            protected class OctreeNode
            {
                /// <summary>
                ///     Составляющая синего цвета
                /// </summary>
                private int _blue;

                /// <summary>
                ///     Составляющая зеленого цвета
                /// </summary>
                private int _green;

                /// <summary>
                ///     Флаг, указывающий, что это листовой узел
                /// </summary>
                private bool _leaf;

                /// <summary>
                ///     Индекс этого узла в палитре
                /// </summary>
                private int _paletteIndex;

                /// <summary>
                ///     Количество пикселей в этом узле
                /// </summary>
                private int _pixelCount;

                /// <summary>
                ///     Составляющая красного цвета
                /// </summary>
                private int _red;

                /// <summary>
                ///     Конструктор узла
                /// </summary>
                /// <param name="level">Уровень в дереве = 0 - 7</param>
                /// <param name="colorBits">Количество значимых бит в цвете изображения</param>
                /// <param name="octree">Дерево, к которому принадлежит этот узел</param>
                public OctreeNode(int level, int colorBits, Octree octree)
                {
                    // Конструируем новый узел
                    _leaf = level == colorBits;

                    _red = _green = _blue = 0;
                    _pixelCount = 0;

                    // Если узел является листом, увеличиваем количество листьев
                    if (_leaf)
                    {
                        octree.Leaves++;
                        NextReducible = null;
                        Children = null;
                    }
                    else
                    {
                        // Иначе добавляем этот узел к списку редуцируемых узлов
                        NextReducible = octree.ReducibleNodes[level];
                        octree.ReducibleNodes[level] = this;
                        Children = new OctreeNode[8];
                    }
                }

                /// <summary>
                ///     Получить/Установить следующий редуцируемый узел
                /// </summary>
                public OctreeNode NextReducible { get; }

                /// <summary>
                ///     Возвращает дочерние узлы
                /// </summary>
                private OctreeNode[] Children { get; }

                /// <summary>
                ///     Добавить цвет в дерево
                /// </summary>
                /// <param name="pixel">Цвет</param>
                /// <param name="colorBits">Количество значимых битов цвета</param>
                /// <param name="level">Уровень в дереве</param>
                /// <param name="octree">Дерево, к которому принадлежит этот узел</param>
                public void AddColor(Color32 pixel, int colorBits, int level, Octree octree)
                {
                    // Обновляем информацию о цвете, если это лист
                    if (_leaf)
                    {
                        Increment(pixel);
                        // Настроить предыдущий узел
                        octree.TrackPrevious(this);
                    }
                    else
                    {
                        // Переходим на следующий уровень в дереве
                        int shift = 7 - level;
                        int index = ((pixel.Red & Mask[level]) >> (shift - 2)) |
                                    ((pixel.Green & Mask[level]) >> (shift - 1)) |
                                    ((pixel.Blue & Mask[level]) >> shift);

                        var child = Children[index];

                        if (child == null)
                        {
                            // Создаем новый дочерний узел и сохраняем в массиве
                            child = new OctreeNode(level + 1, colorBits, octree);
                            Children[index] = child;
                        }

                        // Добавляем цвет в дочерний узел
                        child.AddColor(pixel, colorBits, level + 1, octree);
                    }
                }

                /// <summary>
                ///     Уменьшить узел, удалив всех его детей
                /// </summary>
                /// <returns>Количество удаленных листьев</returns>
                public int Reduce()
                {
                    _red = _green = _blue = 0;
                    int children = 0;

                    // Проходим по всем детям и добавляем их информацию в этот узел
                    for (int index = 0; index < 8; index++)
                        if (Children[index] != null)
                        {
                            _red += Children[index]._red;
                            _green += Children[index]._green;
                            _blue += Children[index]._blue;
                            _pixelCount += Children[index]._pixelCount;
                            ++children;
                            Children[index] = null;
                        }

                    // Теперь меняем этот узел на лист
                    _leaf = true;

                    // Возвращаем количество удаленных узлов для уменьшения количества листьев
                    return children - 1;
                }

                /// <summary>
                ///     Обход дерева для формирования палитры цветов
                /// </summary>
                /// <param name="palette">Палитра</param>
                /// <param name="paletteIndex">Текущий индекс палитры</param>
                public void ConstructPalette(ArrayList palette, ref int paletteIndex)
                {
                    if (_leaf)
                    {
                        // Используем следующий индекс палитры
                        _paletteIndex = paletteIndex++;

                        // Устанавливаем цвет для текущего индекса палитры
                        palette.Add(Color.FromArgb(_red / _pixelCount, _green / _pixelCount, _blue / _pixelCount));
                    }
                    else
                    {
                        // Проходим по детям, ищем листья
                        for (int index = 0; index < 8; index++)
                            if (Children[index] != null)
                                Children[index].ConstructPalette(palette, ref paletteIndex);
                    }
                }

                /// <summary>
                ///     Получить индекс палитры для переданного цвета
                /// </summary>
                public int GetPaletteIndex(Color32 pixel, int level)
                {
                    int paletteIndex = _paletteIndex;

                    if (!_leaf)
                    {
                        int shift = 7 - level;
                        int index = ((pixel.Red & Mask[level]) >> (shift - 2)) |
                                    ((pixel.Green & Mask[level]) >> (shift - 1)) |
                                    ((pixel.Blue & Mask[level]) >> shift);

                        if (Children[index] != null)
                            paletteIndex = Children[index].GetPaletteIndex(pixel, level + 1);
                        else
                            throw new Exception("Не ожидали этого!");
                    }

                    return paletteIndex;
                }

                /// <summary>
                ///     Увеличить счетчик пикселей и добавить информацию о цвете
                /// </summary>
                public void Increment(Color32 pixel)
                {
                    _pixelCount++;
                    _red += pixel.Red;
                    _green += pixel.Green;
                    _blue += pixel.Blue;
                }
            }
        }
    }
}
