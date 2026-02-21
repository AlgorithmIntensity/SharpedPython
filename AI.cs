using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SharpedPython
{
    internal class AI
    {
        private Random random;
        private Dictionary<string, object> model;
        private List<double[]> trainingData;
        private List<double[]> trainingLabels;

        public AI()
        {
            random = new Random();
            model = new Dictionary<string, object>();
            trainingData = new List<double[]>();
            trainingLabels = new List<double[]>();
        }

        // Линейная регрессия
        public Dictionary<string, double> LinearRegression(double[][] X, double[] y)
        {
            int n = X.Length;
            int m = X[0].Length;

            double[] weights = new double[m];
            double bias = 0;
            double learningRate = 0.01;
            int epochs = 1000;

            for (int epoch = 0; epoch < epochs; epoch++)
            {
                for (int i = 0; i < n; i++)
                {
                    double prediction = bias;
                    for (int j = 0; j < m; j++)
                    {
                        prediction += weights[j] * X[i][j];
                    }

                    double error = prediction - y[i];

                    for (int j = 0; j < m; j++)
                    {
                        weights[j] -= learningRate * error * X[i][j];
                    }
                    bias -= learningRate * error;
                }
            }

            return new Dictionary<string, double>
            {
                { "bias", bias },
                { "weights", weights.Length > 0 ? weights[0] : 0 }
            };
        }

        // K-ближайших соседей
        public int KNN(double[][] trainData, int[] trainLabels, double[] testPoint, int k = 3)
        {
            List<Tuple<double, int>> distances = new List<Tuple<double, int>>();

            for (int i = 0; i < trainData.Length; i++)
            {
                double dist = EuclideanDistance(trainData[i], testPoint);
                distances.Add(new Tuple<double, int>(dist, trainLabels[i]));
            }

            distances.Sort((a, b) => a.Item1.CompareTo(b.Item1));

            var nearest = distances.Take(k).Select(d => d.Item2).ToList();
            return nearest.GroupBy(x => x).OrderByDescending(g => g.Count()).First().Key;
        }

        private double EuclideanDistance(double[] a, double[] b)
        {
            double sum = 0;
            for (int i = 0; i < a.Length; i++)
            {
                sum += Math.Pow(a[i] - b[i], 2);
            }
            return Math.Sqrt(sum);
        }

        // Нейронная сеть (простейшая)
        public class NeuralNetwork
        {
            private int inputSize;
            private int hiddenSize;
            private int outputSize;
            private double[,] weightsInputHidden;
            private double[,] weightsHiddenOutput;
            private double[] hiddenBias;
            private double[] outputBias;
            private Random random;

            public NeuralNetwork(int input, int hidden, int output)
            {
                inputSize = input;
                hiddenSize = hidden;
                outputSize = output;
                random = new Random();

                weightsInputHidden = new double[input, hidden];
                weightsHiddenOutput = new double[hidden, output];
                hiddenBias = new double[hidden];
                outputBias = new double[output];

                InitializeWeights();
            }

            private void InitializeWeights()
            {
                for (int i = 0; i < inputSize; i++)
                    for (int j = 0; j < hiddenSize; j++)
                        weightsInputHidden[i, j] = random.NextDouble() - 0.5;

                for (int i = 0; i < hiddenSize; i++)
                    for (int j = 0; j < outputSize; j++)
                        weightsHiddenOutput[i, j] = random.NextDouble() - 0.5;
            }

            private double Sigmoid(double x)
            {
                return 1.0 / (1.0 + Math.Exp(-x));
            }

            public double[] Predict(double[] input)
            {
                // Прямое распространение
                double[] hidden = new double[hiddenSize];
                for (int i = 0; i < hiddenSize; i++)
                {
                    hidden[i] = hiddenBias[i];
                    for (int j = 0; j < inputSize; j++)
                    {
                        hidden[i] += input[j] * weightsInputHidden[j, i];
                    }
                    hidden[i] = Sigmoid(hidden[i]);
                }

                double[] output = new double[outputSize];
                for (int i = 0; i < outputSize; i++)
                {
                    output[i] = outputBias[i];
                    for (int j = 0; j < hiddenSize; j++)
                    {
                        output[i] += hidden[j] * weightsHiddenOutput[j, i];
                    }
                    output[i] = Sigmoid(output[i]);
                }

                return output;
            }
        }

        // Наивный Байес
        public class NaiveBayes
        {
            private Dictionary<int, List<double[]>> classFeatures;
            private Dictionary<int, double> classPriors;
            private Dictionary<int, double[]> classMeans;
            private Dictionary<int, double[]> classStdDevs;

            public void Fit(double[][] X, int[] y)
            {
                classFeatures = new Dictionary<int, List<double[]>>();

                for (int i = 0; i < X.Length; i++)
                {
                    if (!classFeatures.ContainsKey(y[i]))
                        classFeatures[y[i]] = new List<double[]>();
                    classFeatures[y[i]].Add(X[i]);
                }

                classPriors = new Dictionary<int, double>();
                classMeans = new Dictionary<int, double[]>();
                classStdDevs = new Dictionary<int, double[]>();

                foreach (var kvp in classFeatures)
                {
                    int classLabel = kvp.Key;
                    var features = kvp.Value;

                    classPriors[classLabel] = (double)features.Count / X.Length;

                    int nFeatures = features[0].Length;
                    double[] means = new double[nFeatures];
                    double[] stdDevs = new double[nFeatures];

                    for (int j = 0; j < nFeatures; j++)
                    {
                        means[j] = features.Average(f => f[j]);
                        stdDevs[j] = Math.Sqrt(features.Average(f => Math.Pow(f[j] - means[j], 2)));
                    }

                    classMeans[classLabel] = means;
                    classStdDevs[classLabel] = stdDevs;
                }
            }

            public int Predict(double[] x)
            {
                double bestScore = double.NegativeInfinity;
                int bestClass = -1;

                foreach (var kvp in classPriors)
                {
                    int classLabel = kvp.Key;
                    double logProb = Math.Log(kvp.Value);

                    for (int i = 0; i < x.Length; i++)
                    {
                        double mean = classMeans[classLabel][i];
                        double stdDev = classStdDevs[classLabel][i];

                        if (stdDev > 0)
                        {
                            double exponent = -Math.Pow(x[i] - mean, 2) / (2 * Math.Pow(stdDev, 2));
                            double coefficient = 1.0 / (stdDev * Math.Sqrt(2 * Math.PI));
                            logProb += Math.Log(coefficient) + exponent;
                        }
                    }

                    if (logProb > bestScore)
                    {
                        bestScore = logProb;
                        bestClass = classLabel;
                    }
                }

                return bestClass;
            }
        }

        // Генерация случайных данных
        public double[][] GenerateRandomData(int samples, int features, double min = 0, double max = 1)
        {
            double[][] data = new double[samples][];
            for (int i = 0; i < samples; i++)
            {
                data[i] = new double[features];
                for (int j = 0; j < features; j++)
                {
                    data[i][j] = random.NextDouble() * (max - min) + min;
                }
            }
            return data;
        }

        // Нормализация данных
        public double[][] Normalize(double[][] data)
        {
            int nSamples = data.Length;
            int nFeatures = data[0].Length;

            double[] mins = new double[nFeatures];
            double[] maxs = new double[nFeatures];

            for (int j = 0; j < nFeatures; j++)
            {
                mins[j] = data.Min(d => d[j]);
                maxs[j] = data.Max(d => d[j]);
            }

            double[][] normalized = new double[nSamples][];
            for (int i = 0; i < nSamples; i++)
            {
                normalized[i] = new double[nFeatures];
                for (int j = 0; j < nFeatures; j++)
                {
                    normalized[i][j] = (data[i][j] - mins[j]) / (maxs[j] - mins[j]);
                }
            }

            return normalized;
        }

        // Метрики качества
        public double Accuracy(int[] predictions, int[] actual)
        {
            int correct = 0;
            for (int i = 0; i < predictions.Length; i++)
            {
                if (predictions[i] == actual[i])
                    correct++;
            }
            return (double)correct / predictions.Length;
        }

        // Сохранение и загрузка модели
        public void SaveModel(string path)
        {
            // Простая сериализация
            string json = System.Text.Json.JsonSerializer.Serialize(model);
            File.WriteAllText(path, json);
        }

        public void LoadModel(string path)
        {
            string json = File.ReadAllText(path);
            model = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(json);
        }
    }
}