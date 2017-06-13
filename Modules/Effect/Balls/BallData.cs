﻿using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using Vixen.Module;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.Effect.Effect;
using ZedGraph;

namespace VixenModules.Effect.Balls
{
	[DataContract]
	public class BallData : EffectTypeModuleData
	{

		public BallData()
		{
			Colors = new List<ColorGradient> { new ColorGradient(Color.Red), new ColorGradient(Color.Lime), new ColorGradient(Color.Blue) };
			SpeedVariationCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 5.0, 5.0 }));
			CenterSpeedCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 30.0, 30.0 }));
			Filled = false;
			Wrap = false;
			Inverse = false;
			RandomRadius = false;
			Collide = false;
			BallType = BallType.Bounce;
			BallFill = BallFill.Fade;
			BackgroundColor = new ColorGradient(Color.Snow);
			Fade = true;
			ChangeCollideColor = true;
			RadiusCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 30.0, 30.0 }));
			BallCountCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 7.0, 7.0 }));
			LevelCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 100.0, 100.0 }));
			BallEdgeWidthCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 1.0, 1.0 }));
			Orientation = StringOrientation.Vertical;
		}

		[DataMember]
		public List<ColorGradient> Colors { get; set; }

		[DataMember]
		public Curve SpeedVariationCurve { get; set; }

		[DataMember]
		public BallFill BallFill { get; set; }

		[DataMember]
		public BallType BallType { get; set; }

		[DataMember]
		public bool Inverse { get; set; }

		[DataMember]
		public bool ChangeCollideColor { get; set; }

		[DataMember]
		public bool Collide { get; set; }

		[DataMember]
		public bool RandomRadius { get; set; }

		[DataMember]
		public ColorGradient BackgroundColor { get; set; }

		[DataMember]
		public Curve CenterSpeedCurve { get; set; }

		[DataMember]
		public Curve BallEdgeWidthCurve { get; set; }

		[DataMember]
		public bool Filled { get; set; }

		[DataMember]
		public bool Fade { get; set; }

		[DataMember]
		public bool Wrap { get; set; }

		[DataMember]
		public Curve LevelCurve { get; set; }

		[DataMember]
		public Curve RadiusCurve { get; set; }

		[DataMember]
		public Curve BallCountCurve { get; set; }

		[DataMember]
		public StringOrientation Orientation { get; set; }

		protected override EffectTypeModuleData CreateInstanceForClone()
		{
			BallData result = new BallData
			{
				Colors = Colors.ToList(),
				SpeedVariationCurve = new Curve(SpeedVariationCurve),
				CenterSpeedCurve = new Curve(CenterSpeedCurve),
				Orientation = Orientation,
				Inverse = Inverse,
				Filled = Filled,
				Wrap = Wrap,
				Collide = Collide,
				BallFill = BallFill,
				Fade = Fade,
				ChangeCollideColor = ChangeCollideColor,
				BallType = BallType,
				RandomRadius = RandomRadius,
				RadiusCurve = new Curve(RadiusCurve),
				BallCountCurve = new Curve(BallCountCurve),
				LevelCurve = new Curve(LevelCurve),
				BallEdgeWidthCurve = new Curve(BallEdgeWidthCurve)
			};
			return result;
		}
	}
}
