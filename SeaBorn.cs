using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SharpedPython
{
    public class DataFrame
    {
        public Dictionary<string, List<double>> NumericColumns { get; set; }
        public Dictionary<string, List<string>> CategoricalColumns { get; set; }

        public DataFrame()
        {
            NumericColumns = new Dictionary<string, List<double>>();
            CategoricalColumns = new Dictionary<string, List<string>>();
        }

        public void AddColumn(string name, List<double> data)
        {
            NumericColumns[name] = data;
        }

        public void AddColumn(string name, List<string> data)
        {
            CategoricalColumns[name] = data;
        }

        public List<double> GetNumeric(string colName)
        {
            return NumericColumns.ContainsKey(colName) ? NumericColumns[colName] : null;
        }

        public List<string> GetCategorical(string colName)
        {
            return CategoricalColumns.ContainsKey(colName) ? CategoricalColumns[colName] : null;
        }

        public int RowCount
        {
            get
            {
                if (NumericColumns.Count > 0)
                    return NumericColumns.First().Value.Count;
                if (CategoricalColumns.Count > 0)
                    return CategoricalColumns.First().Value.Count;
                return 0;
            }
        }
    }

    // Класс для графика
    public class Plot
    {
        public Bitmap Image { get; set; }
        public string Title { get; set; }
        public int Width { get; set; } = 800;
        public int Height { get; set; } = 600;
    }

    internal class SeaBorn
    {
        private Random random;
        private Dictionary<string, List<Color>> palettes;
        public string Style { get; set; } = "whitegrid";

        public SeaBorn()
        {
            random = new Random();
            InitializePalettes();
        }

        private void InitializePalettes()
        {
            palettes = new Dictionary<string, List<Color>>
            {
                ["deep"] = new List<Color>
                {
                    Color.FromArgb(77, 171, 197),   // голубой
                    Color.FromArgb(89, 161, 79),    // зеленый
                    Color.FromArgb(242, 142, 43),   // оранжевый
                    Color.FromArgb(225, 87, 89),    // красный
                    Color.FromArgb(162, 120, 157),  // фиолетовый
                    Color.FromArgb(184, 134, 78)    // коричневый
                },
                ["muted"] = new List<Color>
                {
                    Color.FromArgb(98, 179, 178),   // бирюзовый
                    Color.FromArgb(144, 186, 144),  // мятный
                    Color.FromArgb(242, 179, 125),  // персиковый
                    Color.FromArgb(219, 139, 139),  // розовый
                    Color.FromArgb(172, 149, 187),  // лавандовый
                    Color.FromArgb(182, 156, 128)   // бежевый
                },
                ["pastel"] = new List<Color>
                {
                    Color.FromArgb(179, 226, 226),  // светло-бирюзовый
                    Color.FromArgb(204, 236, 204),  // светло-зеленый
                    Color.FromArgb(255, 229, 204),  // светло-оранжевый
                    Color.FromArgb(255, 204, 204),  // светло-красный
                    Color.FromArgb(229, 204, 255),  // светло-фиолетовый
                    Color.FromArgb(242, 222, 187)   // светло-коричневый
                },
                ["bright"] = new List<Color>
                {
                    Color.FromArgb(0, 158, 255),    // ярко-синий
                    Color.FromArgb(0, 255, 0),      // ярко-зеленый
                    Color.FromArgb(255, 170, 0),    // ярко-оранжевый
                    Color.FromArgb(255, 0, 0),      // ярко-красный
                    Color.FromArgb(170, 0, 255),    // ярко-фиолетовый
                    Color.FromArgb(255, 255, 0)     // желтый
                },
                ["dark"] = new List<Color>
                {
                    Color.FromArgb(0, 79, 128),     // темно-синий
                    Color.FromArgb(0, 79, 0),       // темно-зеленый
                    Color.FromArgb(170, 85, 0),     // темно-оранжевый
                    Color.FromArgb(128, 0, 0),      // темно-красный
                    Color.FromArgb(79, 0, 128),     // темно-фиолетовый
                    Color.FromArgb(85, 43, 0)       // темно-коричневый
                }
            };
        }

        // Получить палитру
        public List<Color> GetPalette(string name, int nColors = 6)
        {
            if (palettes.ContainsKey(name.ToLower()))
            {
                var palette = palettes[name.ToLower()];
                return palette.Take(Math.Min(nColors, palette.Count)).ToList();
            }
            return palettes["deep"].Take(Math.Min(nColors, 6)).ToList();
        }

        // Установить стиль
        public void SetStyle(string style)
        {
            Style = style;
        }

        // Гистограмма
        public Plot Histogram(DataFrame data, string column, int bins = 10, string color = "blue", string title = "Гистограмма")
        {
            var plot = new Plot { Title = title };
            var values = data.GetNumeric(column);

            if (values == null || values.Count == 0)
                return plot;

            var bitmap = new Bitmap(plot.Width, plot.Height);
            using (var g = Graphics.FromImage(bitmap))
            {
                g.Clear(GetBackgroundColor());

                // Находим мин и макс
                double min = values.Min();
                double max = values.Max();
                double binWidth = (max - min) / bins;

                // Подсчет частот
                int[] counts = new int[bins];
                foreach (var v in values)
                {
                    int bin = (int)((v - min) / binWidth);
                    if (bin >= bins) bin = bins - 1;
                    counts[bin]++;
                }

                int maxCount = counts.Max();

                // Рисуем гистограмму
                int margin = 50;
                int chartWidth = plot.Width - 2 * margin;
                int chartHeight = plot.Height - 2 * margin;
                int barWidth = chartWidth / bins;

                for (int i = 0; i < bins; i++)
                {
                    int barHeight = (int)((double)counts[i] / maxCount * chartHeight);
                    int x = margin + i * barWidth;
                    int y = plot.Height - margin - barHeight;

                    Color barColor = GetColor(color, i);
                    using (var brush = new SolidBrush(barColor))
                    {
                        g.FillRectangle(brush, x, y, barWidth - 2, barHeight);
                    }

                    // Рамка
                    using (var pen = new Pen(Color.Black, 1))
                    {
                        g.DrawRectangle(pen, x, y, barWidth - 2, barHeight);
                    }
                }

                // Оси
                using (var pen = new Pen(Color.Black, 2))
                {
                    g.DrawLine(pen, margin, margin, margin, plot.Height - margin);
                    g.DrawLine(pen, margin, plot.Height - margin, plot.Width - margin, plot.Height - margin);
                }

                // Заголовок
                using (var font = new Font("Arial", 14, FontStyle.Bold))
                using (var brush = new SolidBrush(Color.Black))
                {
                    var titleSize = g.MeasureString(title, font);
                    g.DrawString(title, font, brush, (plot.Width - titleSize.Width) / 2, 10);
                }

                // Подписи осей
                using (var font = new Font("Arial", 10))
                using (var brush = new SolidBrush(Color.Black))
                {
                    g.DrawString(column, font, brush, plot.Width / 2 - 20, plot.Height - 20);
                    g.DrawString("Частота", font, brush, 10, plot.Height / 2);
                }
            }

            plot.Image = bitmap;
            return plot;
        }

        // Scatter plot (точечный график)
        public Plot ScatterPlot(DataFrame data, string xColumn, string yColumn, string hueColumn = null, string title = "Scatter Plot")
        {
            var plot = new Plot { Title = title };
            var xValues = data.GetNumeric(xColumn);
            var yValues = data.GetNumeric(yColumn);

            if (xValues == null || yValues == null || xValues.Count == 0)
                return plot;

            var bitmap = new Bitmap(plot.Width, plot.Height);
            using (var g = Graphics.FromImage(bitmap))
            {
                g.Clear(GetBackgroundColor());

                double xMin = xValues.Min();
                double xMax = xValues.Max();
                double yMin = yValues.Min();
                double yMax = yValues.Max();

                int margin = 50;
                int chartWidth = plot.Width - 2 * margin;
                int chartHeight = plot.Height - 2 * margin;

                // Определяем категории для цвета
                var categories = new List<string>();
                if (hueColumn != null && data.CategoricalColumns.ContainsKey(hueColumn))
                {
                    categories = data.GetCategorical(hueColumn).Distinct().ToList();
                }

                for (int i = 0; i < xValues.Count; i++)
                {
                    int x = margin + (int)((xValues[i] - xMin) / (xMax - xMin) * chartWidth);
                    int y = plot.Height - margin - (int)((yValues[i] - yMin) / (yMax - yMin) * chartHeight);

                    Color pointColor = Color.Blue;
                    if (hueColumn != null && categories.Count > 0)
                    {
                        string category = data.GetCategorical(hueColumn)[i];
                        int catIndex = categories.IndexOf(category);
                        pointColor = GetColor("deep", catIndex);
                    }

                    using (var brush = new SolidBrush(pointColor))
                    {
                        g.FillEllipse(brush, x - 4, y - 4, 8, 8);
                    }
                }

                // Оси
                using (var pen = new Pen(Color.Black, 2))
                {
                    g.DrawLine(pen, margin, margin, margin, plot.Height - margin);
                    g.DrawLine(pen, margin, plot.Height - margin, plot.Width - margin, plot.Height - margin);
                }

                // Заголовок и подписи
                using (var font = new Font("Arial", 14, FontStyle.Bold))
                using (var brush = new SolidBrush(Color.Black))
                {
                    var titleSize = g.MeasureString(title, font);
                    g.DrawString(title, font, brush, (plot.Width - titleSize.Width) / 2, 10);
                }

                using (var font = new Font("Arial", 10))
                using (var brush = new SolidBrush(Color.Black))
                {
                    g.DrawString(xColumn, font, brush, plot.Width / 2, plot.Height - 25);
                    g.DrawString(yColumn, font, brush, 10, plot.Height / 2);
                }
            }

            plot.Image = bitmap;
            return plot;
        }

        // Линейный график
        public Plot LinePlot(DataFrame data, string xColumn, string yColumn, string title = "Line Plot")
        {
            var plot = new Plot { Title = title };
            var xValues = data.GetNumeric(xColumn);
            var yValues = data.GetNumeric(yColumn);

            if (xValues == null || yValues == null || xValues.Count == 0)
                return plot;

            var bitmap = new Bitmap(plot.Width, plot.Height);
            using (var g = Graphics.FromImage(bitmap))
            {
                g.Clear(GetBackgroundColor());

                double xMin = xValues.Min();
                double xMax = xValues.Max();
                double yMin = yValues.Min();
                double yMax = yValues.Max();

                int margin = 50;
                int chartWidth = plot.Width - 2 * margin;
                int chartHeight = plot.Height - 2 * margin;

                var points = new List<Point>();
                for (int i = 0; i < xValues.Count; i++)
                {
                    int x = margin + (int)((xValues[i] - xMin) / (xMax - xMin) * chartWidth);
                    int y = plot.Height - margin - (int)((yValues[i] - yMin) / (yMax - yMin) * chartHeight);
                    points.Add(new Point(x, y));
                }

                // Рисуем линию
                if (points.Count > 1)
                {
                    using (var pen = new Pen(Color.Blue, 2))
                    {
                        g.DrawLines(pen, points.ToArray());
                    }
                }

                // Рисуем точки
                foreach (var p in points)
                {
                    using (var brush = new SolidBrush(Color.Red))
                    {
                        g.FillEllipse(brush, p.X - 4, p.Y - 4, 8, 8);
                    }
                }

                // Оси
                using (var pen = new Pen(Color.Black, 2))
                {
                    g.DrawLine(pen, margin, margin, margin, plot.Height - margin);
                    g.DrawLine(pen, margin, plot.Height - margin, plot.Width - margin, plot.Height - margin);
                }

                // Заголовок и подписи
                using (var font = new Font("Arial", 14, FontStyle.Bold))
                using (var brush = new SolidBrush(Color.Black))
                {
                    var titleSize = g.MeasureString(title, font);
                    g.DrawString(title, font, brush, (plot.Width - titleSize.Width) / 2, 10);
                }
            }

            plot.Image = bitmap;
            return plot;
        }

        // Box plot (ящик с усами)
        public Plot BoxPlot(DataFrame data, string column, string title = "Box Plot")
        {
            var plot = new Plot { Title = title };
            var values = data.GetNumeric(column);

            if (values == null || values.Count == 0)
                return plot;

            var sorted = values.OrderBy(x => x).ToList();
            double q1 = sorted[values.Count / 4];
            double median = sorted[values.Count / 2];
            double q3 = sorted[3 * values.Count / 4];
            double iqr = q3 - q1;
            double lowerWhisker = q1 - 1.5 * iqr;
            double upperWhisker = q3 + 1.5 * iqr;

            var bitmap = new Bitmap(plot.Width, plot.Height);
            using (var g = Graphics.FromImage(bitmap))
            {
                g.Clear(GetBackgroundColor());

                int margin = 80;
                int boxWidth = 100;
                int centerX = plot.Width / 2;
                int yMin = margin;
                int yMax = plot.Height - margin;

                double yScale = (yMax - yMin) / (values.Max() - values.Min());

                int yQ1 = yMax - (int)((q1 - values.Min()) * yScale);
                int yMed = yMax - (int)((median - values.Min()) * yScale);
                int yQ3 = yMax - (int)((q3 - values.Min()) * yScale);
                int yLower = yMax - (int)((lowerWhisker - values.Min()) * yScale);
                int yUpper = yMax - (int)((upperWhisker - values.Min()) * yScale);

                // Ящик
                using (var brush = new SolidBrush(Color.FromArgb(100, 100, 200, 255)))
                {
                    g.FillRectangle(brush, centerX - boxWidth / 2, yQ3, boxWidth, yQ1 - yQ3);
                }

                // Медиана
                using (var pen = new Pen(Color.Red, 2))
                {
                    g.DrawLine(pen, centerX - boxWidth / 2, yMed, centerX + boxWidth / 2, yMed);
                }

                // Усы
                using (var pen = new Pen(Color.Black, 2))
                {
                    g.DrawLine(pen, centerX, yQ3, centerX, yUpper);
                    g.DrawLine(pen, centerX, yQ1, centerX, yLower);
                    g.DrawLine(pen, centerX - 20, yUpper, centerX + 20, yUpper);
                    g.DrawLine(pen, centerX - 20, yLower, centerX + 20, yLower);
                }

                // Заголовок
                using (var font = new Font("Arial", 14, FontStyle.Bold))
                using (var brush = new SolidBrush(Color.Black))
                {
                    var titleSize = g.MeasureString(title, font);
                    g.DrawString(title, font, brush, (plot.Width - titleSize.Width) / 2, 10);
                }

                // Подпись
                using (var font = new Font("Arial", 10))
                using (var brush = new SolidBrush(Color.Black))
                {
                    g.DrawString(column, font, brush, centerX - 20, yMax + 20);
                }
            }

            plot.Image = bitmap;
            return plot;
        }

        // Тепловая карта
        public Plot HeatMap(double[,] data, string title = "Heat Map")
        {
            int rows = data.GetLength(0);
            int cols = data.GetLength(1);
            var plot = new Plot { Title = title };

            var bitmap = new Bitmap(plot.Width, plot.Height);
            using (var g = Graphics.FromImage(bitmap))
            {
                g.Clear(GetBackgroundColor());

                double maxVal = double.MinValue;
                double minVal = double.MaxValue;

                for (int i = 0; i < rows; i++)
                    for (int j = 0; j < cols; j++)
                    {
                        maxVal = Math.Max(maxVal, data[i, j]);
                        minVal = Math.Min(minVal, data[i, j]);
                    }

                int margin = 50;
                int cellWidth = (plot.Width - 2 * margin) / cols;
                int cellHeight = (plot.Height - 2 * margin) / rows;

                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        int x = margin + j * cellWidth;
                        int y = margin + i * cellHeight;

                        double normalized = (data[i, j] - minVal) / (maxVal - minVal);
                        Color cellColor = GetHeatColor(normalized);

                        using (var brush = new SolidBrush(cellColor))
                        {
                            g.FillRectangle(brush, x, y, cellWidth - 1, cellHeight - 1);
                        }

                        // Рамка
                        using (var pen = new Pen(Color.Gray, 1))
                        {
                            g.DrawRectangle(pen, x, y, cellWidth - 1, cellHeight - 1);
                        }

                        // Значение
                        using (var font = new Font("Arial", 8))
                        using (var brush = new SolidBrush(GetTextColor(normalized)))
                        {
                            string val = data[i, j].ToString("F2");
                            var size = g.MeasureString(val, font);
                            g.DrawString(val, font, brush,
                                x + (cellWidth - size.Width) / 2,
                                y + (cellHeight - size.Height) / 2);
                        }
                    }
                }
            }

            plot.Image = bitmap;
            return plot;
        }

        // Показать график в форме
        public void Show(Plot plot)
        {
            var form = new Form
            {
                Text = plot.Title,
                Width = plot.Width + 20,
                Height = plot.Height + 50,
                StartPosition = FormStartPosition.CenterScreen
            };

            var pictureBox = new PictureBox
            {
                Image = plot.Image,
                SizeMode = PictureBoxSizeMode.AutoSize,
                Location = new Point(10, 10)
            };

            form.Controls.Add(pictureBox);
            form.ShowDialog();
        }

        // Сохранить график
        public void Save(Plot plot, string filename)
        {
            plot.Image.Save(filename);
        }

        // Вспомогательные методы
        private Color GetBackgroundColor()
        {
            switch (Style)
            {
                case "darkgrid":
                    return Color.FromArgb(40, 40, 40);
                case "whitegrid":
                    return Color.White;
                case "dark":
                    return Color.FromArgb(30, 30, 30);
                case "ticks":
                    return Color.WhiteSmoke;
                default:
                    return Color.White;
            }
        }

        private Color GetColor(string palette, int index)
        {
            var pal = GetPalette(palette);
            return pal[index % pal.Count];
        }

        private Color GetHeatColor(double value)
        {
            // От синего (холодный) к красному (горячий)
            int r = (int)(255 * value);
            int b = (int)(255 * (1 - value));
            return Color.FromArgb(r, 0, b);
        }

        private Color GetTextColor(double normalized)
        {
            return normalized > 0.5 ? Color.Black : Color.White;
        }

        // Генерация примеров данных
        public DataFrame LoadDataset(string name)
        {
            var data = new DataFrame();

            switch (name.ToLower())
            {
                case "iris":
                    // Данные ирисов (первые несколько)
                    data.AddColumn("sepal_length", new List<double> { 5.1, 4.9, 4.7, 4.6, 5.0, 5.4, 4.6, 5.0 });
                    data.AddColumn("sepal_width", new List<double> { 3.5, 3.0, 3.2, 3.1, 3.6, 3.9, 3.4, 3.4 });
                    data.AddColumn("petal_length", new List<double> { 1.4, 1.4, 1.3, 1.5, 1.4, 1.7, 1.4, 1.5 });
                    data.AddColumn("petal_width", new List<double> { 0.2, 0.2, 0.2, 0.2, 0.2, 0.4, 0.3, 0.2 });
                    data.AddColumn("species", new List<string> { "setosa", "setosa", "setosa", "setosa", "setosa", "setosa", "setosa", "setosa" });
                    break;

                case "tips":
                    // Данные о чаевых
                    data.AddColumn("total_bill", new List<double> { 16.99, 10.34, 21.01, 23.68, 24.59 });
                    data.AddColumn("tip", new List<double> { 1.01, 1.66, 3.50, 3.31, 3.61 });
                    data.AddColumn("size", new List<double> { 2, 3, 3, 2, 4 });
                    data.AddColumn("day", new List<string> { "Sun", "Sun", "Sun", "Sun", "Sun" });
                    break;

                default:
                    // Случайные данные
                    var rand = new Random();
                    var x = new List<double>();
                    var y = new List<double>();
                    for (int i = 0; i < 50; i++)
                    {
                        x.Add(i);
                        y.Add(rand.NextDouble() * 100);
                    }
                    data.AddColumn("x", x);
                    data.AddColumn("y", y);
                    break;
            }

            return data;
        }
    }
}