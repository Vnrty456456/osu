﻿// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Diagnostics;
using System.Linq;
using Humanizer;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Events;
using osu.Game.Graphics;
using osu.Game.Graphics.Containers;
using osu.Game.Graphics.Sprites;
using osu.Game.Graphics.UserInterface;
using osu.Game.Graphics.UserInterfaceV2;
using osuTK;
using osuTK.Graphics;
using osuTK.Input;

namespace osu.Game.Screens.Edit.Compose.Components
{
    public class BeatDivisorControl : CompositeDrawable
    {
        private readonly BindableBeatDivisor beatDivisor = new BindableBeatDivisor();

        public BeatDivisorControl(BindableBeatDivisor beatDivisor)
        {
            this.beatDivisor.BindTo(beatDivisor);
        }

        [BackgroundDependencyLoader]
        private void load(OsuColour colours)
        {
            Masking = true;
            CornerRadius = 5;

            InternalChildren = new Drawable[]
            {
                new Box
                {
                    Name = "Gray Background",
                    RelativeSizeAxes = Axes.Both,
                    Colour = colours.Gray4
                },
                new GridContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    Content = new[]
                    {
                        new Drawable[]
                        {
                            new Container
                            {
                                RelativeSizeAxes = Axes.Both,
                                Children = new Drawable[]
                                {
                                    new Box
                                    {
                                        Name = "Black Background",
                                        RelativeSizeAxes = Axes.Both,
                                        Colour = Color4.Black
                                    },
                                    new TickSliderBar(beatDivisor)
                                    {
                                        RelativeSizeAxes = Axes.Both,
                                    }
                                }
                            }
                        },
                        new Drawable[]
                        {
                            new Container
                            {
                                RelativeSizeAxes = Axes.Both,
                                Children = new Drawable[]
                                {
                                    new Box
                                    {
                                        RelativeSizeAxes = Axes.Both,
                                        Colour = colours.Gray4
                                    },
                                    new Container
                                    {
                                        RelativeSizeAxes = Axes.Both,
                                        Child = new GridContainer
                                        {
                                            RelativeSizeAxes = Axes.Both,
                                            Content = new[]
                                            {
                                                new Drawable[]
                                                {
                                                    new ChevronButton
                                                    {
                                                        Icon = FontAwesome.Solid.ChevronLeft,
                                                        Action = beatDivisor.Previous
                                                    },
                                                    new DivisorDisplay { BeatDivisor = { BindTarget = beatDivisor } },
                                                    new ChevronButton
                                                    {
                                                        Icon = FontAwesome.Solid.ChevronRight,
                                                        Action = beatDivisor.Next
                                                    }
                                                },
                                            },
                                            ColumnDimensions = new[]
                                            {
                                                new Dimension(GridSizeMode.Absolute, 20),
                                                new Dimension(),
                                                new Dimension(GridSizeMode.Absolute, 20)
                                            }
                                        }
                                    }
                                }
                            }
                        },
                        new Drawable[]
                        {
                            new TextFlowContainer(s => s.Font = s.Font.With(size: 14))
                            {
                                Padding = new MarginPadding { Horizontal = 15 },
                                Text = "beat snap",
                                RelativeSizeAxes = Axes.X,
                                TextAnchor = Anchor.TopCentre
                            },
                        },
                        new Drawable[]
                        {
                            new Container
                            {
                                RelativeSizeAxes = Axes.Both,
                                Children = new Drawable[]
                                {
                                    new Box
                                    {
                                        RelativeSizeAxes = Axes.Both,
                                        Colour = colours.Gray4
                                    },
                                    new Container
                                    {
                                        RelativeSizeAxes = Axes.Both,
                                        Child = new GridContainer
                                        {
                                            RelativeSizeAxes = Axes.Both,
                                            Content = new[]
                                            {
                                                new Drawable[]
                                                {
                                                    new ChevronButton
                                                    {
                                                        Icon = FontAwesome.Solid.ChevronLeft,
                                                        Action = () => cycleDivisorType(-1)
                                                    },
                                                    new DivisorTypeText { BeatDivisor = { BindTarget = beatDivisor } },
                                                    new ChevronButton
                                                    {
                                                        Icon = FontAwesome.Solid.ChevronRight,
                                                        Action = () => cycleDivisorType(1)
                                                    }
                                                },
                                            },
                                            ColumnDimensions = new[]
                                            {
                                                new Dimension(GridSizeMode.Absolute, 20),
                                                new Dimension(),
                                                new Dimension(GridSizeMode.Absolute, 20)
                                            }
                                        }
                                    }
                                }
                            }
                        },
                    },
                    RowDimensions = new[]
                    {
                        new Dimension(GridSizeMode.Absolute, 30),
                        new Dimension(GridSizeMode.Absolute, 20),
                        new Dimension(GridSizeMode.Absolute, 15)
                    }
                }
            };
        }

        private void cycleDivisorType(int direction)
        {
            Debug.Assert(Math.Abs(direction) == 1);
            int nextDivisorType = (int)beatDivisor.ValidDivisors.Value.Type + direction;
            if (nextDivisorType > (int)BeatDivisorType.Triplets)
                nextDivisorType = (int)BeatDivisorType.Common;
            else if (nextDivisorType < (int)BeatDivisorType.Common)
                nextDivisorType = (int)BeatDivisorType.Triplets;

            switch ((BeatDivisorType)nextDivisorType)
            {
                case BeatDivisorType.Common:
                    beatDivisor.ValidDivisors.Value = BeatDivisorPresetCollection.COMMON;
                    break;

                case BeatDivisorType.Triplets:
                    beatDivisor.ValidDivisors.Value = BeatDivisorPresetCollection.TRIPLETS;
                    break;

                case BeatDivisorType.Custom:
                    beatDivisor.ValidDivisors.Value = BeatDivisorPresetCollection.Custom(beatDivisor.ValidDivisors.Value.Presets.Max());
                    break;
            }
        }

        internal class DivisorDisplay : OsuAnimatedButton, IHasPopover
        {
            public BindableBeatDivisor BeatDivisor { get; } = new BindableBeatDivisor();

            private readonly OsuSpriteText divisorText;

            public DivisorDisplay()
            {
                Anchor = Anchor.Centre;
                Origin = Anchor.Centre;

                AutoSizeAxes = Axes.Both;

                Add(divisorText = new OsuSpriteText
                {
                    Font = OsuFont.Default.With(size: 20),
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Margin = new MarginPadding
                    {
                        Horizontal = 5
                    }
                });

                Action = this.ShowPopover;
            }

            [BackgroundDependencyLoader]
            private void load(OsuColour colours)
            {
                divisorText.Colour = colours.BlueLighter;
            }

            protected override void LoadComplete()
            {
                base.LoadComplete();
                updateState();
            }

            private void updateState()
            {
                BeatDivisor.BindValueChanged(val => divisorText.Text = $"1/{val.NewValue}", true);
            }

            public Popover GetPopover() => new CustomDivisorPopover
            {
                BeatDivisor = { BindTarget = BeatDivisor }
            };
        }

        internal class CustomDivisorPopover : OsuPopover
        {
            public BindableBeatDivisor BeatDivisor { get; } = new BindableBeatDivisor();

            private readonly OsuNumberBox divisorTextBox;

            public CustomDivisorPopover()
            {
                Child = new FillFlowContainer
                {
                    Width = 150,
                    AutoSizeAxes = Axes.Y,
                    Spacing = new Vector2(10),
                    Children = new Drawable[]
                    {
                        divisorTextBox = new OsuNumberBox
                        {
                            RelativeSizeAxes = Axes.X,
                            PlaceholderText = "Beat divisor"
                        },
                        new OsuTextFlowContainer
                        {
                            RelativeSizeAxes = Axes.X,
                            AutoSizeAxes = Axes.Y,
                            Text = "Related divisors will be added to the list of presets."
                        }
                    }
                };
            }

            protected override void LoadComplete()
            {
                base.LoadComplete();
                BeatDivisor.BindValueChanged(_ => updateState(), true);
                divisorTextBox.OnCommit += (_, __) => setPresets();

                Schedule(() => GetContainingInputManager().ChangeFocus(divisorTextBox));
            }

            private void setPresets()
            {
                if (!int.TryParse(divisorTextBox.Text, out int divisor) || divisor < 1 || divisor > 64)
                {
                    updateState();
                    return;
                }

                if (!BeatDivisor.ValidDivisors.Value.Presets.Contains(divisor))
                {
                    if (BeatDivisorPresetCollection.COMMON.Presets.Contains(divisor))
                        BeatDivisor.ValidDivisors.Value = BeatDivisorPresetCollection.COMMON;
                    else if (BeatDivisorPresetCollection.TRIPLETS.Presets.Contains(divisor))
                        BeatDivisor.ValidDivisors.Value = BeatDivisorPresetCollection.TRIPLETS;
                    else
                        BeatDivisor.ValidDivisors.Value = BeatDivisorPresetCollection.Custom(divisor);
                }

                BeatDivisor.Value = divisor;

                this.HidePopover();
            }

            private void updateState()
            {
                divisorTextBox.Text = BeatDivisor.Value.ToString();
            }
        }

        private class DivisorTypeText : OsuSpriteText
        {
            public BindableBeatDivisor BeatDivisor { get; } = new BindableBeatDivisor();

            public DivisorTypeText()
            {
                Anchor = Anchor.Centre;
                Origin = Anchor.Centre;

                Font = OsuFont.Default.With(size: 14);
            }

            protected override void LoadComplete()
            {
                base.LoadComplete();
                BeatDivisor.ValidDivisors.BindValueChanged(val => Text = val.NewValue.Type.Humanize(LetterCasing.LowerCase), true);
            }
        }

        internal class ChevronButton : IconButton
        {
            public ChevronButton()
            {
                Anchor = Anchor.Centre;
                Origin = Anchor.Centre;

                // Small offset to look a bit better centered along with the divisor text
                Y = 1;

                Size = new Vector2(20);
                IconScale = new Vector2(0.6f);
            }

            [BackgroundDependencyLoader]
            private void load(OsuColour colours)
            {
                IconColour = Color4.Black;
                HoverColour = colours.Gray7;
                FlashColour = colours.Gray9;
            }
        }

        private class TickSliderBar : SliderBar<int>
        {
            private Marker marker;

            [Resolved]
            private OsuColour colours { get; set; }

            private readonly BindableBeatDivisor beatDivisor;

            public TickSliderBar(BindableBeatDivisor beatDivisor)
            {
                CurrentNumber.BindTo(this.beatDivisor = beatDivisor);

                Padding = new MarginPadding { Horizontal = 5 };
            }

            protected override void LoadComplete()
            {
                base.LoadComplete();

                beatDivisor.ValidDivisors.BindValueChanged(_ => updateDivisors(), true);
            }

            private void updateDivisors()
            {
                ClearInternal();
                CurrentNumber.ValueChanged -= moveMarker;

                foreach (int t in beatDivisor.ValidDivisors.Value.Presets)
                {
                    AddInternal(new Tick
                    {
                        Anchor = Anchor.TopLeft,
                        Origin = Anchor.TopCentre,
                        RelativePositionAxes = Axes.X,
                        Colour = BindableBeatDivisor.GetColourFor(t, colours),
                        X = getMappedPosition(t)
                    });
                }

                AddInternal(marker = new Marker());
                CurrentNumber.ValueChanged += moveMarker;
                CurrentNumber.TriggerChange();
            }

            private void moveMarker(ValueChangedEvent<int> divisor)
            {
                marker.MoveToX(getMappedPosition(divisor.NewValue), 100, Easing.OutQuint);
                marker.Flash();
            }

            protected override void UpdateValue(float value)
            {
            }

            public override bool HandleNonPositionalInput => IsHovered && !CurrentNumber.Disabled;

            protected override bool OnKeyDown(KeyDownEvent e)
            {
                switch (e.Key)
                {
                    case Key.Right:
                        beatDivisor.Next();
                        OnUserChange(Current.Value);
                        return true;

                    case Key.Left:
                        beatDivisor.Previous();
                        OnUserChange(Current.Value);
                        return true;

                    default:
                        return false;
                }
            }

            protected override bool OnMouseDown(MouseDownEvent e)
            {
                marker.Active = true;
                return base.OnMouseDown(e);
            }

            protected override void OnMouseUp(MouseUpEvent e)
            {
                marker.Active = false;
                base.OnMouseUp(e);
            }

            protected override bool OnClick(ClickEvent e)
            {
                handleMouseInput(e.ScreenSpaceMousePosition);
                return true;
            }

            protected override void OnDrag(DragEvent e)
            {
                handleMouseInput(e.ScreenSpaceMousePosition);
            }

            protected override void OnDragEnd(DragEndEvent e)
            {
                handleMouseInput(e.ScreenSpaceMousePosition);
            }

            private void handleMouseInput(Vector2 screenSpaceMousePosition)
            {
                // copied from SliderBar so we can do custom spacing logic.
                float xPosition = (ToLocalSpace(screenSpaceMousePosition).X - RangePadding) / UsableWidth;

                CurrentNumber.Value = beatDivisor.ValidDivisors.Value.Presets.OrderBy(d => Math.Abs(getMappedPosition(d) - xPosition)).First();
                OnUserChange(Current.Value);
            }

            private float getMappedPosition(float divisor) => MathF.Pow((divisor - 1) / (beatDivisor.ValidDivisors.Value.Presets.Last() - 1), 0.90f);

            private class Tick : CompositeDrawable
            {
                public Tick()
                {
                    Size = new Vector2(2.5f, 10);

                    InternalChild = new Box { RelativeSizeAxes = Axes.Both };

                    CornerRadius = 0.5f;
                    Masking = true;
                }
            }

            private class Marker : CompositeDrawable
            {
                private Color4 defaultColour;

                private const float size = 7;

                [BackgroundDependencyLoader]
                private void load(OsuColour colours)
                {
                    Colour = defaultColour = colours.Gray4;
                    Anchor = Anchor.TopLeft;
                    Origin = Anchor.TopCentre;

                    Width = size;
                    RelativeSizeAxes = Axes.Y;
                    RelativePositionAxes = Axes.X;

                    InternalChildren = new Drawable[]
                    {
                        new Box
                        {
                            Width = 2,
                            RelativeSizeAxes = Axes.Y,
                            Origin = Anchor.BottomCentre,
                            Anchor = Anchor.BottomCentre,
                            Colour = ColourInfo.GradientVertical(Color4.White.Opacity(0.2f), Color4.White),
                            Blending = BlendingParameters.Additive,
                        },
                        new EquilateralTriangle
                        {
                            Origin = Anchor.BottomCentre,
                            Anchor = Anchor.BottomCentre,
                            Height = size,
                            EdgeSmoothness = new Vector2(1),
                            Colour = Color4.White,
                        }
                    };
                }

                private bool active;

                public bool Active
                {
                    get => active;
                    set
                    {
                        this.FadeColour(value ? Color4.White : defaultColour, 500, Easing.OutQuint);
                        active = value;
                    }
                }

                public void Flash()
                {
                    bool wasActive = active;

                    Active = true;

                    if (wasActive) return;

                    using (BeginDelayedSequence(50))
                        Active = false;
                }
            }
        }
    }
}
