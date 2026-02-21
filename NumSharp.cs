using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpedPython
{
    internal class NumSharp
    {
        public class ndarray
        {
            private double[] data;
            private int[] shape;
            private int[] strides;

            // Конструкторы
            public ndarray(int size)
            {
                data = new double[size];
                shape = new int[] { size };
                CalculateStrides();
            }

            public ndarray(int rows, int cols)
            {
                data = new double[rows * cols];
                shape = new int[] { rows, cols };
                CalculateStrides();
            }

            public ndarray(int[] dimensions)
            {
                int totalSize = 1;
                foreach (int dim in dimensions)
                    totalSize *= dim;

                data = new double[totalSize];
                shape = dimensions;
                CalculateStrides();
            }

            public ndarray(double[] array)
            {
                data = array;
                shape = new int[] { array.Length };
                CalculateStrides();
            }

            public ndarray(double[,] array)
            {
                int rows = array.GetLength(0);
                int cols = array.GetLength(1);
                data = new double[rows * cols];
                shape = new int[] { rows, cols };

                for (int i = 0; i < rows; i++)
                    for (int j = 0; j < cols; j++)
                        data[i * cols + j] = array[i, j];

                CalculateStrides();
            }

            private void CalculateStrides()
            {
                strides = new int[shape.Length];
                int stride = 1;
                for (int i = shape.Length - 1; i >= 0; i--)
                {
                    strides[i] = stride;
                    stride *= shape[i];
                }
            }

            // Индексатор
            public double this[params int[] indices]
            {
                get
                {
                    int index = GetIndex(indices);
                    return data[index];
                }
                set
                {
                    int index = GetIndex(indices);
                    data[index] = value;
                }
            }

            private int GetIndex(int[] indices)
            {
                if (indices.Length != shape.Length)
                    throw new ArgumentException("Number of indices must match array dimensions");

                int index = 0;
                for (int i = 0; i < indices.Length; i++)
                {
                    if (indices[i] < 0 || indices[i] >= shape[i])
                        throw new IndexOutOfRangeException($"Index {indices[i]} out of range for dimension {i}");

                    index += indices[i] * strides[i];
                }
                return index;
            }

            // Публичные методы доступа к данным
            public double[] GetData()
            {
                return data;
            }

            public void SetData(double[] newData)
            {
                if (newData.Length != data.Length)
                    throw new ArgumentException("New data must have the same length");
                data = newData;
            }

            public double[] GetDataCopy()
            {
                double[] copy = new double[data.Length];
                Array.Copy(data, copy, data.Length);
                return copy;
            }

            // Свойства
            public int[] Shape => shape;
            public int Size => data.Length;
            public int Dimensions => shape.Length;

            // Преобразование в массив
            public double[] ToArray() => GetDataCopy();

            public double[,] To2DArray()
            {
                if (shape.Length != 2)
                    throw new InvalidOperationException("Array is not 2-dimensional");

                double[,] result = new double[shape[0], shape[1]];
                for (int i = 0; i < shape[0]; i++)
                    for (int j = 0; j < shape[1]; j++)
                        result[i, j] = data[i * shape[1] + j];

                return result;
            }

            // Статистика
            public double Sum()
            {
                return data.Sum();
            }

            public double Mean()
            {
                return data.Average();
            }

            public double Min()
            {
                return data.Min();
            }

            public double Max()
            {
                return data.Max();
            }

            public double Std()
            {
                double mean = Mean();
                double sum = 0;
                foreach (double val in data)
                    sum += Math.Pow(val - mean, 2);
                return Math.Sqrt(sum / data.Length);
            }

            // Операторы
            public static ndarray operator +(ndarray a, ndarray b)
            {
                if (a.shape.SequenceEqual(b.shape))
                {
                    ndarray result = new ndarray(a.shape);
                    for (int i = 0; i < a.data.Length; i++)
                        result.data[i] = a.data[i] + b.data[i];
                    return result;
                }
                throw new ArgumentException("Arrays must have the same shape");
            }

            public static ndarray operator +(ndarray a, double b)
            {
                ndarray result = new ndarray(a.shape);
                for (int i = 0; i < a.data.Length; i++)
                    result.data[i] = a.data[i] + b;
                return result;
            }

            public static ndarray operator -(ndarray a, ndarray b)
            {
                if (a.shape.SequenceEqual(b.shape))
                {
                    ndarray result = new ndarray(a.shape);
                    for (int i = 0; i < a.data.Length; i++)
                        result.data[i] = a.data[i] - b.data[i];
                    return result;
                }
                throw new ArgumentException("Arrays must have the same shape");
            }

            public static ndarray operator *(ndarray a, ndarray b)
            {
                if (a.shape.SequenceEqual(b.shape))
                {
                    ndarray result = new ndarray(a.shape);
                    for (int i = 0; i < a.data.Length; i++)
                        result.data[i] = a.data[i] * b.data[i];
                    return result;
                }
                throw new ArgumentException("Arrays must have the same shape");
            }

            public static ndarray operator /(ndarray a, ndarray b)
            {
                if (a.shape.SequenceEqual(b.shape))
                {
                    ndarray result = new ndarray(a.shape);
                    for (int i = 0; i < a.data.Length; i++)
                        result.data[i] = a.data[i] / b.data[i];
                    return result;
                }
                throw new ArgumentException("Arrays must have the same shape");
            }

            // Математические функции
            public ndarray Sin()
            {
                ndarray result = new ndarray(shape);
                for (int i = 0; i < data.Length; i++)
                    result.data[i] = Math.Sin(data[i]);
                return result;
            }

            public ndarray Cos()
            {
                ndarray result = new ndarray(shape);
                for (int i = 0; i < data.Length; i++)
                    result.data[i] = Math.Cos(data[i]);
                return result;
            }

            public ndarray Exp()
            {
                ndarray result = new ndarray(shape);
                for (int i = 0; i < data.Length; i++)
                    result.data[i] = Math.Exp(data[i]);
                return result;
            }

            public ndarray Log()
            {
                ndarray result = new ndarray(shape);
                for (int i = 0; i < data.Length; i++)
                    result.data[i] = Math.Log(data[i]);
                return result;
            }

            public ndarray Sqrt()
            {
                ndarray result = new ndarray(shape);
                for (int i = 0; i < data.Length; i++)
                    result.data[i] = Math.Sqrt(data[i]);
                return result;
            }

            public ndarray Abs()
            {
                ndarray result = new ndarray(shape);
                for (int i = 0; i < data.Length; i++)
                    result.data[i] = Math.Abs(data[i]);
                return result;
            }

            // Транспонирование
            public ndarray T()
            {
                if (shape.Length != 2)
                    throw new InvalidOperationException("Transpose only supported for 2D arrays");

                ndarray result = new ndarray(shape[1], shape[0]);
                for (int i = 0; i < shape[0]; i++)
                    for (int j = 0; j < shape[1]; j++)
                        result[j, i] = this[i, j];

                return result;
            }

            // Изменение формы
            public ndarray Reshape(params int[] newShape)
            {
                int totalSize = 1;
                foreach (int dim in newShape)
                    totalSize *= dim;

                if (totalSize != data.Length)
                    throw new ArgumentException("New shape must have the same total size");

                ndarray result = new ndarray(newShape);
                Array.Copy(data, result.data, data.Length);
                return result;
            }

            // Срезы
            public ndarray GetSlice(int dim, int start, int end)
            {
                if (dim < 0 || dim >= shape.Length)
                    throw new ArgumentException("Invalid dimension");

                int[] newShape = (int[])shape.Clone();
                newShape[dim] = end - start;

                ndarray result = new ndarray(newShape);

                int sliceSize = 1;
                for (int i = dim + 1; i < shape.Length; i++)
                    sliceSize *= shape[i];

                int sourceOffset = start * strides[dim];
                int destOffset = 0;

                for (int i = 0; i < (end - start); i++)
                {
                    Array.Copy(data, sourceOffset + i * strides[dim],
                               result.data, destOffset + i * sliceSize, sliceSize);
                }

                return result;
            }

            // Матричное умножение
            public static ndarray Dot(ndarray a, ndarray b)
            {
                if (a.shape.Length != 2 || b.shape.Length != 2)
                    throw new ArgumentException("Dot product only supported for 2D arrays");

                if (a.shape[1] != b.shape[0])
                    throw new ArgumentException("Incompatible dimensions for matrix multiplication");

                ndarray result = new ndarray(a.shape[0], b.shape[1]);

                for (int i = 0; i < a.shape[0]; i++)
                {
                    for (int j = 0; j < b.shape[1]; j++)
                    {
                        double sum = 0;
                        for (int k = 0; k < a.shape[1]; k++)
                        {
                            sum += a[i, k] * b[k, j];
                        }
                        result[i, j] = sum;
                    }
                }

                return result;
            }
        }

        // Функции создания массивов
        public static ndarray array(double[] data)
        {
            return new ndarray(data);
        }

        public static ndarray array(double[,] data)
        {
            return new ndarray(data);
        }

        public static ndarray zeros(int size)
        {
            ndarray arr = new ndarray(size);
            return arr;
        }

        public static ndarray zeros(int rows, int cols)
        {
            ndarray arr = new ndarray(rows, cols);
            return arr;
        }

        public static ndarray zeros(int[] shape)
        {
            return new ndarray(shape);
        }

        public static ndarray ones(int size)
        {
            ndarray arr = new ndarray(size);
            double[] data = arr.GetData();
            for (int i = 0; i < data.Length; i++)
                data[i] = 1;
            arr.SetData(data);
            return arr;
        }

        public static ndarray ones(int rows, int cols)
        {
            ndarray arr = new ndarray(rows, cols);
            double[] data = arr.GetData();
            for (int i = 0; i < data.Length; i++)
                data[i] = 1;
            arr.SetData(data);
            return arr;
        }

        public static ndarray full(int size, double value)
        {
            ndarray arr = new ndarray(size);
            double[] data = arr.GetData();
            for (int i = 0; i < data.Length; i++)
                data[i] = value;
            arr.SetData(data);
            return arr;
        }

        public static ndarray arange(int start, int stop, int step = 1)
        {
            int size = (stop - start) / step;
            ndarray arr = new ndarray(size);
            double[] data = arr.GetData();

            for (int i = 0; i < size; i++)
                data[i] = start + i * step;

            arr.SetData(data);
            return arr;
        }

        public static ndarray linspace(double start, double stop, int num)
        {
            ndarray arr = new ndarray(num);
            double[] data = arr.GetData();
            double step = (stop - start) / (num - 1);

            for (int i = 0; i < num; i++)
                data[i] = start + i * step;

            arr.SetData(data);
            return arr;
        }

        public static ndarray eye(int n)
        {
            ndarray arr = new ndarray(n, n);
            double[] data = arr.GetData();
            for (int i = 0; i < n; i++)
                data[i * n + i] = 1;
            arr.SetData(data);
            return arr;
        }

        public static ndarray random(int size, double min = 0, double max = 1)
        {
            Random rand = new Random();
            ndarray arr = new ndarray(size);
            double[] data = arr.GetData();

            for (int i = 0; i < size; i++)
                data[i] = rand.NextDouble() * (max - min) + min;

            arr.SetData(data);
            return arr;
        }

        public static ndarray random(int rows, int cols, double min = 0, double max = 1)
        {
            Random rand = new Random();
            ndarray arr = new ndarray(rows, cols);
            double[] data = arr.GetData();

            for (int i = 0; i < data.Length; i++)
                data[i] = rand.NextDouble() * (max - min) + min;

            arr.SetData(data);
            return arr;
        }

        // Статистические функции
        public static double sum(ndarray arr)
        {
            return arr.Sum();
        }

        public static double mean(ndarray arr)
        {
            return arr.Mean();
        }

        public static double min(ndarray arr)
        {
            return arr.Min();
        }

        public static double max(ndarray arr)
        {
            return arr.Max();
        }

        public static double std(ndarray arr)
        {
            return arr.Std();
        }

        // Математические функции
        public static ndarray sin(ndarray arr)
        {
            return arr.Sin();
        }

        public static ndarray cos(ndarray arr)
        {
            return arr.Cos();
        }

        public static ndarray exp(ndarray arr)
        {
            return arr.Exp();
        }

        public static ndarray log(ndarray arr)
        {
            return arr.Log();
        }

        public static ndarray sqrt(ndarray arr)
        {
            return arr.Sqrt();
        }

        public static ndarray abs(ndarray arr)
        {
            return arr.Abs();
        }

        // Операции с массивами
        public static ndarray dot(ndarray a, ndarray b)
        {
            return ndarray.Dot(a, b);
        }

        public static ndarray transpose(ndarray arr)
        {
            return arr.T();
        }

        public static ndarray reshape(ndarray arr, params int[] newShape)
        {
            return arr.Reshape(newShape);
        }

        // Конкатенация
        public static ndarray concatenate(ndarray[] arrays, int axis = 0)
        {
            int totalSize = arrays.Sum(arr => arr.Size);
            ndarray result = new ndarray(totalSize);
            double[] resultData = result.GetData();

            int offset = 0;
            foreach (var arr in arrays)
            {
                double[] arrData = arr.GetData();
                Array.Copy(arrData, 0, resultData, offset, arr.Size);
                offset += arr.Size;
            }

            result.SetData(resultData);
            return result;
        }
    }
}