﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms.DataVisualization.Charting;

namespace TestMySpline
{
	class Program
	{
		static void Main(string[] args)
		{
			TestTdm();
            //TestSplineOnWikipediaExample();
            //TestSpline();
            //TestPerf();
            //TestFitParametric();
        }

        private static void TestFitParametric()
		{
			// Create the data to be fitted
			float[] x = { 0.5f, 2.0f, 3.0f, 4.5f, 3.0f, 2.0f };
			float[] y = { 4.0f, 2.0f, 6.0f, 4.0f, 3.0f, 5.0f };

			float[] xs, ys;
			CubicSpline.FitParametric(x, y, 100, out xs, out ys);
			PlotSplineSolution("Cubic Spline Interpolation - Parametric Fit", x, y, xs, ys, @"..\..\testSplineFitParametric.png");

            // Specify start slope
            CubicSpline.FitParametric(x, y, 100, out xs, out ys, 1, 0);
            PlotSplineSolution("Cubic Spline Interpolation - Parametric Fit - Start Slope Specified", x, y, xs, ys, @"..\..\testSplineFitParametric_start_1.png");

            CubicSpline.FitParametric(x, y, 100, out xs, out ys, 0, 1);
            PlotSplineSolution("Cubic Spline Interpolation - Parametric Fit - Start Slope Specified", x, y, xs, ys, @"..\..\testSplineFitParametric_start_2.png");

            CubicSpline.FitParametric(x, y, 100, out xs, out ys, 1, -1);
            PlotSplineSolution("Cubic Spline Interpolation - Parametric Fit - Start Slope Specified", x, y, xs, ys, @"..\..\testSplineFitParametric_start_3.png");

            CubicSpline.FitParametric(x, y, 100, out xs, out ys, -1, -1);
            PlotSplineSolution("Cubic Spline Interpolation - Parametric Fit - Start Slope Specified", x, y, xs, ys, @"..\..\testSplineFitParametric_start_4.png");

            CubicSpline.FitParametric(x, y, 100, out xs, out ys, -1, 1);
            PlotSplineSolution("Cubic Spline Interpolation - Parametric Fit - Start Slope Specified", x, y, xs, ys, @"..\..\testSplineFitParametric_start_5.png");

            CubicSpline.FitParametric(x, y, 100, out xs, out ys, 1, -1, 1, 1);
            PlotSplineSolution("Cubic Spline Interpolation - Parametric Fit - Boundary Slope Specified", x, y, xs, ys, @"..\..\testSplineFitParametric_both_1.png");

            CubicSpline.FitParametric(x, y, 100, out xs, out ys, 1, -1, 1, -1);
            PlotSplineSolution("Cubic Spline Interpolation - Parametric Fit - Boundary Slope Specified", x, y, xs, ys, @"..\..\testSplineFitParametric_both_2.png");
        }

        private static void TestSpline()
		{
			int n = 6;

			// Create the data to be fitted
			float[] x = new float[n];
			float[] y = new float[n];
			Random rand = new Random(1);

			for (int i = 0; i < n; i++)
			{
				x[i] = i;
				y[i] = (float)rand.NextDouble() * 10;
			}

			// Compute the x values at which we will evaluate the spline.
			// Upsample the original data by a const factor.
			int upsampleFactor = 10;
			int nInterpolated = n * upsampleFactor;
			float[] xs = new float[nInterpolated];

			for (int i = 0; i < nInterpolated; i++)
			{
				xs[i] = (float)i * (n - 1) / (float)(nInterpolated - 1);
			}

			float[] ys = CubicSpline.Compute(x, y, xs, 0.0f, Single.NaN, true);

			string path = @"..\..\testSpline.png";
			PlotSplineSolution("Cubic Spline Interpolation - Random Data", x, y, xs, ys, path);
		}

		private static void TestPerf()
		{
			int n = 10000;

			// Create the data to be fitted
			float[] x = new float[n];
			float[] y = new float[n];
			Random rand = new Random(1);

			for (int i = 0; i < n; i++)
			{
				x[i] = i;
				y[i] = (float)rand.NextDouble() * 10;
			}

			// Compute the x values that we will evaluate the spline at.
			// Upsample the original data by a const factor.
			int upsampleFactor = 10;
			int nInterpolate = n * upsampleFactor;
			float[] xs = new float[nInterpolate];

			for (int i = 0; i < nInterpolate; i++)
			{
				xs[i] = (float)i / upsampleFactor;
			}

			// For perf, test multiple reps
			int reps = 100;
			DateTime start = DateTime.Now;

			for (int i = 0; i < reps; i++)
			{
				float[] ys = CubicSpline.Compute(x, y, xs);
			}

			TimeSpan duration = DateTime.Now - start;
			Console.WriteLine("CubicSpline upsample from {0:n0} to {1:n0} points took {2:0.00} ms for {3} iterations ({2:0.000} ms per iteration)", 
				n, nInterpolate, duration.TotalMilliseconds, reps, duration.TotalMilliseconds / reps);

			// Compare to NRinC
			//float[] y2 = new float[n];
			//float[] ys2 = new float[nInterpolate];
			//start = DateTime.Now;

			//for (int i = 0; i < reps; i++)
			//{
			//	CubicSplineNR.Spline(x, y, y2);
			//	CubicSplineNR.EvalSpline(x, y, y2, xs, ys2);
			//}

			//duration = DateTime.Now - start;
			//Console.WriteLine("CubicSplineNR upsample from {0:n0} to {1:n0} points took {2:0.00} ms for {3} iterations ({2:0.000} ms per iteration)",
			//	n, nInterpolate, duration.TotalMilliseconds, reps, duration.TotalMilliseconds / reps);
		}

		/// <summary>
		/// This is the Wikipedia "Spline Interpolation" article example.
		/// </summary>
		private static void TestSplineOnWikipediaExample()
		{
			// Create the test points.
			float[] x = new float[] { -1.0f, 0.0f, 3.0f };
			float[] y = new float[] { 0.5f, 0.0f, 3.0f };

			Console.WriteLine("x: {0}", ArrayUtil.ToString(x));
			Console.WriteLine("y: {0}", ArrayUtil.ToString(y));

			// Create the upsampled X values to interpolate
			int n = 20;
			float[] xs = new float[n];
			float stepSize = (x[x.Length - 1] - x[0]) / (n - 1);

			for (int i = 0; i < n; i++)
			{
				xs[i] = x[0] + i * stepSize;
			}

			// Fit and eval
			CubicSpline spline = new CubicSpline();
			float[] ys = spline.FitAndEval(x, y, xs);

			Console.WriteLine("xs: {0}", ArrayUtil.ToString(xs));
			Console.WriteLine("ys: {0}", ArrayUtil.ToString(ys));

			// Plot
			string path = @"..\..\spline-wikipedia.png";
			PlotSplineSolution("Cubic Spline Interpolation - Wikipedia Example", x, y, xs, ys, path);

			// Try slope, spline is already computed at this point
			float[] slope = spline.EvalSlope(xs);
			string slopePath = @"..\..\spline-wikipedia-slope.png";
			PlotSplineSolution("Cubic Spline Interpolation - Wikipedia Example - Slope", x, y, xs, ys, slopePath, slope);
		}

		private static TriDiagonalMatrixF TestTdm()
		{
			TriDiagonalMatrixF m = new TriDiagonalMatrixF(10);

			for (int i = 0; i < m.N; i++)
			{
				m.A[i] = 1.111111f;
				m.B[i] = 2.222222f;
				m.C[i] = 3.333333f;
			}

			Console.WriteLine("Matrix:\n{0}", m.ToDisplayString(",4:0.00", "    "));

			for (int i = 0; i < m.N; i++)
			{
				m[i, i] = 4.4444f;
			}

			Console.WriteLine("Matrix:\n{0}", m.ToDisplayString(",4:0.00", "    "));

			// Solve
			Random rand = new Random(1);
			float[] d = new float[m.N];

			for (int i = 0; i < d.Length; i++)
			{
				d[i] = (float)rand.NextDouble();
			}

			float[] x = m.Solve(d);

			Console.WriteLine("Solve returned: ");

			for (int i = 0; i < x.Length; i++)
			{
				Console.Write("{0:0.0000}, ", x[i]);
			}

			Console.WriteLine();
			return m;
		}

		#region PlotSplineSolution

		private static void PlotSplineSolution(string title, float[] x, float[] y, float[] xs, float[] ys, string path, float[] qPrime = null)
		{
			var chart = new Chart();
			chart.Size = new Size(600, 400);
			chart.Titles.Add(title);
			chart.Legends.Add(new Legend("Legend"));

			ChartArea ca = new ChartArea("DefaultChartArea");
			ca.AxisX.Title = "X";
			ca.AxisY.Title = "Y";
			chart.ChartAreas.Add(ca);

			Series s1 = CreateSeries(chart, "Spline", CreateDataPoints(xs, ys), Color.Blue, MarkerStyle.None);
			Series s2 = CreateSeries(chart, "Original", CreateDataPoints(x, y), Color.Green, MarkerStyle.Diamond);

			chart.Series.Add(s2);
			chart.Series.Add(s1);

			if (qPrime != null)
			{
				Series s3 = CreateSeries(chart, "Slope", CreateDataPoints(xs, qPrime), Color.Red, MarkerStyle.None);
				chart.Series.Add(s3);
			}

			ca.RecalculateAxesScale();
			ca.AxisX.Minimum = Math.Floor(ca.AxisX.Minimum);
			ca.AxisX.Maximum = Math.Ceiling(ca.AxisX.Maximum);
			int nIntervals = (x.Length - 1);
			nIntervals = Math.Max(4, nIntervals);
			ca.AxisX.Interval = (ca.AxisX.Maximum - ca.AxisX.Minimum) / nIntervals;

			// Save
			if (File.Exists(path))
			{
				File.Delete(path);
			}

			using (FileStream fs = new FileStream(path, FileMode.CreateNew))
			{
				chart.SaveImage(fs, ChartImageFormat.Png);
			}
		}

		private static List<DataPoint> CreateDataPoints(float[] x, float[] y)
		{
			Debug.Assert(x.Length == y.Length);
			List<DataPoint> points = new List<DataPoint>();

			for (int i = 0; i < x.Length; i++)
			{
				points.Add(new DataPoint(x[i], y[i]));
			}

			return points;
		}

		private static Series CreateSeries(Chart chart, string seriesName, IEnumerable<DataPoint> points, Color color, MarkerStyle markerStyle = MarkerStyle.None)
		{
			var s = new Series()
				{
					XValueType = ChartValueType.Double,
					YValueType = ChartValueType.Double,
					Legend = chart.Legends[0].Name,
					IsVisibleInLegend = true,
					ChartType = SeriesChartType.Line,
					Name = seriesName,
					ChartArea = chart.ChartAreas[0].Name,
					MarkerStyle = markerStyle,
					Color = color,
					MarkerSize = 8
				};

			foreach (var p in points)
			{
				s.Points.Add(p);
			}

			return s;
		}

		#endregion
	}
}
