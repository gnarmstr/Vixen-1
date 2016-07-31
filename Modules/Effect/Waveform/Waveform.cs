using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Linq;
using Vixen.Sys;
using Vixen.Sys.Attribute;
using VixenModules.Effect.AudioHelp;
using VixenModules.EffectEditor.EffectDescriptorAttributes;
using Vixen.Attributes;
using VixenModules.App.ColorGradients;
using VixenModules.Property.Color;


namespace VixenModules.Effect.Waveform
{
    public class Waveform : AudioPluginBase
	{
		private const int Spacing = 30;

		[Value]
        [ProviderCategory(@"Color",3)]
        [PropertyOrder(6)]
        [ProviderDisplayName(@"Reverse")]
        public bool Inverted
        {
            get { return ((WaveformData)_data).Inverted; }
            set
            {
                ((WaveformData)_data).Inverted = value;
                IsDirty = true;
				OnPropertyChanged();
			}
        }

        [Value]
        [ProviderCategory(@"Response Speed",2)]
        [ProviderDisplayName(@"Scroll Speed")]
        [ProviderDescription(@"How fast the effect goes. Lower is faster")]
        [PropertyEditor("SliderEditor")]
        [NumberRange(0, 50, 1)]
        [PropertyOrder(0)]
        public int ScrollSpeed
        {
            get { return 50 - ((WaveformData)_data).ScrollSpeed; }
            set
            {
                ((WaveformData)_data).ScrollSpeed = 50 - value;
                IsDirty = true;
				OnPropertyChanged();
			}
        }

		public Waveform()
        {
            _audioHelper = new AudioHelper(this);
        }

		protected override void TargetNodesChanged()
		{
			CheckForInvalidColorData();
			if (DepthOfEffect > TargetNodes.FirstOrDefault().GetMaxChildDepth() - 1)
			{
				DepthOfEffect = 0;
			}
		}

		//Validate that the we are using valid colors and set appropriate defaults if not.
		private void CheckForInvalidColorData()
		{
			var validColors = GetValidColors();
			if (validColors.Any())
			{
				if (!_data.MeterColorGradient.GetColorsInGradient().IsSubsetOf(validColors))
				{
					//Our color is not valid for any elements we have.
					//Try to set a default color gradient from our available colors
					if (validColors.Count > 1)
					{
						//Try to make some kind of transitions
						List<float> positions = new List<float>(validColors.Count);
						positions.AddRange(validColors.Select((t, i) => i/(float) (validColors.Count - 1)));

						ColorBlend linearBlend = new ColorBlend();
						linearBlend.Colors = validColors.ToArray();
						linearBlend.Positions = positions.ToArray();

						MeterColorGradient = new ColorGradient(linearBlend);

					}
					else
					{
						MeterColorGradient = new ColorGradient(validColors.First());
					}
				}

				//ensure we are using Custom and we can't change it. We are limited in discrete color mode
				MeterColorStyle = MeterColorTypes.Custom;
				EnableColorTypesSelection(false);
			}
			else
			{
				EnableColorTypesSelection(true);
			}

		}

		// renders the given node to the internal ElementData dictionary. If the given node is
		// not a element, will recursively descend until we render its elements.
		protected override void RenderNode(ElementNode node)
		{
            if (!_audioHelper.AudioLoaded)
                return;

            int currentElement = 0;

			foreach (ElementNode elementNode in node.GetLeafEnumerator()) {
				// this is probably always going to be a single element for the given node, as
				// we have iterated down to leaf nodes in RenderNode() above. May as well do
				// it this way, though, in case something changes in future.
				if (elementNode == null || elementNode.Element == null)
					continue;
				bool discreteColors = ColorModule.isElementNodeDiscreteColored(elementNode);

				for (int i = 1;i<(int)(TimeSpan.TotalMilliseconds/Spacing);i++)
                {
                    int startAudioTime;
                    int endAudioTime;
                    if (((WaveformData)_data).Inverted)
                    {
                        startAudioTime = i * Spacing - (node.GetLeafEnumerator().Count()-currentElement) * ((WaveformData)_data).ScrollSpeed + 1;
                        endAudioTime = (i + 1) * Spacing - (node.GetLeafEnumerator().Count()-currentElement) * ((WaveformData)_data).ScrollSpeed;
                    }
                    else
                    {
                        startAudioTime = i * Spacing - currentElement * ((WaveformData)_data).ScrollSpeed + 1;
                        endAudioTime = (i + 1) * Spacing - currentElement * ((WaveformData)_data).ScrollSpeed;
                    }
                    TimeSpan startTime = TimeSpan.FromMilliseconds(i * Spacing);

                    if (startAudioTime > 0 && startAudioTime < TimeSpan.TotalMilliseconds && endAudioTime > 0 && endAudioTime < TimeSpan.TotalMilliseconds)
                    {

                        double gradientPosition1 = (_audioHelper.VolumeAtTime(startAudioTime) + _data.Range) / _data.Range;
                        double gradientPosition2 = (_audioHelper.VolumeAtTime(endAudioTime) + _data.Range) / _data.Range;
						
						//Some odd corner cases
						if (gradientPosition1 <= 0)
							gradientPosition1 = 0;
						if (gradientPosition1 >= 1)
							gradientPosition1 = 1;

						//Some odd corner cases
						if (gradientPosition2 <= 0)
							gradientPosition2 = 0;
						if (gradientPosition2 >= 1)
							gradientPosition2 = 1;

						_elementData.Add(GenerateEffectIntents(elementNode, WorkingGradient, MeterIntensityCurve, gradientPosition1, gradientPosition2, TimeSpan.FromMilliseconds(Spacing), startTime, discreteColors));

					}
                }
                currentElement++;
            }

		}

	    
	}
}
