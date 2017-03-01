using System;
using System.ComponentModel;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Xamarin.Forms.Internals;
using GlobalResource = Android.Resource;
using AView = Android.Views.View;
using AMotionEvent = Android.Views.MotionEvent;
using AMotionEventActions = Android.Views.MotionEventActions;
using static System.String;
using Object = Java.Lang.Object;

namespace Xamarin.Forms.Platform.Android.FastRenderers
{
	public class ButtonRenderer : AppCompatButton, IVisualElementRenderer, AView.IOnAttachStateChangeListener,
		AView.IOnFocusChangeListener, IEffectControlProvider
	{
		string _defaultContentDescription;
		bool? _defaultFocusable;
		float _defaultFontSize;
		string _defaultHint;
		int? _defaultLabelFor;
		Typeface _defaultTypeface;
		int _imageHeight = -1;
		bool _isDisposed;
		TextColorSwitcher _textColorSwitcher;

		public event EventHandler<VisualElementChangedEventArgs> ElementChanged;
		public event EventHandler<PropertyChangedEventArgs> ElementPropertyChanged;

		public ButtonRenderer() : base(Forms.Context)
		{
			System.Diagnostics.Debug.WriteLine("Fast Button!");
			Initialize();
		}

		public VisualElement Element => Button;
		Button Button { get; set; }

		void IEffectControlProvider.RegisterEffect(Effect effect)
		{
			var platformEffect = effect as PlatformEffect;
			if (platformEffect != null)
			{
				OnRegisterEffect(platformEffect);
			}
		}

		void IOnAttachStateChangeListener.OnViewAttachedToWindow(AView attachedView)
		{
			UpdateText();
		}

		void IOnAttachStateChangeListener.OnViewDetachedFromWindow(AView detachedView)
		{
		}

		void IOnFocusChangeListener.OnFocusChange(AView v, bool hasFocus)
		{
			OnNativeFocusChanged(hasFocus);

			((IElementController)Button).SetValueFromRenderer(VisualElement.IsFocusedPropertyKey, hasFocus);
		}

		SizeRequest IVisualElementRenderer.GetDesiredSize(int widthConstraint, int heightConstraint)
		{
			UpdateText();

			AView view = this;
			view.Measure(widthConstraint, heightConstraint);

			return new SizeRequest(new Size(MeasuredWidth, MeasuredHeight), MinimumSize());
		}

		void IVisualElementRenderer.SetElement(VisualElement element)
		{
			if (element == null)
			{
				throw new ArgumentNullException(nameof(element));
			}

			if (!(element is Button))
			{
				throw new ArgumentException($"{nameof(element)} must be of type {nameof(Button)}");
			}

			VisualElement oldElement = Button;
			Button = (Button)element;

			Performance.Start();

			if (oldElement != null)
			{
				oldElement.PropertyChanged -= OnElementPropertyChanged;
			}

			Color currentColor = oldElement?.BackgroundColor ?? Color.Default;
			if (element.BackgroundColor != currentColor)
			{
				UpdateBackgroundColor();
			}

			element.PropertyChanged += OnElementPropertyChanged;

			// Todo hartez InputTransparent comes from FormsViewGroup, do we need it here?
			//InputTransparent = Element.InputTransparent;

			OnElementChanged(new ElementChangedEventArgs<Button>(oldElement as Button, Button));

			if (Tracker == null)
			{
				SetTracker(new VisualElementTracker(this));
			}

			SendVisualElementInitialized(element, this);

			EffectUtilities.RegisterEffectControlProvider(this, oldElement, element);

			if (!IsNullOrEmpty(element.AutomationId))
			{
				SetAutomationId(element.AutomationId);
			}

			SetContentDescription();
			SetFocusable();

			Performance.Stop();
		}

		void IVisualElementRenderer.SetLabelFor(int? id)
		{
			if (_defaultLabelFor == null)
			{
				_defaultLabelFor = LabelFor;
			}

			LabelFor = (int)(id ?? _defaultLabelFor);
		}

		public VisualElementTracker Tracker { get; private set; }

		void IVisualElementRenderer.UpdateLayout()
		{
			Performance.Start();
			Tracker?.UpdateLayout();
			Performance.Stop();
		}

		AView IVisualElementRenderer.View => this;

		ViewGroup IVisualElementRenderer.ViewGroup => null;

		protected override void Dispose(bool disposing)
		{
			if (_isDisposed)
			{
				return;
			}

			_isDisposed = true;

			if (disposing)
			{
				SetOnClickListener(null);
				SetOnTouchListener(null);
				RemoveOnAttachStateChangeListener(this);
				Tag = null;
				_textColorSwitcher = null;

				if (Tracker != null)
				{
					Tracker.Dispose();
					Tracker = null;
				}

				if (Element != null)
				{
					Element.PropertyChanged -= OnElementPropertyChanged;
				}
			}

			base.Dispose(disposing);
		}

		protected virtual Size MinimumSize()
		{
			return new Size();
		}

		protected virtual void OnElementChanged(ElementChangedEventArgs<Button> e)
		{
			if (e.OldElement != null)
			{
			}

			if (e.NewElement != null)
			{
				SetLabeledBy();
				UpdateAll();
				UpdateBackgroundColor();
			}

			ElementChanged?.Invoke(this, new VisualElementChangedEventArgs(e.OldElement, e.NewElement));
		}

		protected void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == Button.TextProperty.PropertyName)
			{
				UpdateText();
			}
			else if (e.PropertyName == Button.TextColorProperty.PropertyName)
			{
				UpdateTextColor();
			}
			else if (e.PropertyName == VisualElement.IsEnabledProperty.PropertyName)
			{
				UpdateIsEnabled();
			}
			else if (e.PropertyName == Button.FontProperty.PropertyName)
			{
				UpdateFont();
			}
			else if (e.PropertyName == Button.ImageProperty.PropertyName)
			{
				UpdateBitmap();
			}
			else if (e.PropertyName == VisualElement.IsVisibleProperty.PropertyName)
			{
				UpdateText();
			}

			if (e.PropertyName == VisualElement.IsEnabledProperty.PropertyName)
			{
				UpdateIsEnabled();
			}
			else if (e.PropertyName == Accessibility.LabeledByProperty.PropertyName)
			{
				SetLabeledBy();
			}

			if (e.PropertyName == VisualElement.BackgroundColorProperty.PropertyName)
			{
				UpdateBackgroundColor();
			}
			// TODO hartez 2017/03/01 15:06:25 InputTransparent again, do we need to worry about this on a button?	
			//else if (e.PropertyName == VisualElement.InputTransparentProperty.PropertyName)
			//	InputTransparent = Element.InputTransparent;
			else if (e.PropertyName == Accessibility.HintProperty.PropertyName)
			{
				SetContentDescription();
			}
			else if (e.PropertyName == Accessibility.NameProperty.PropertyName)
			{
				SetContentDescription();
			}
			else if (e.PropertyName == Accessibility.IsInAccessibleTreeProperty.PropertyName)
			{
				SetFocusable();
			}

			ElementPropertyChanged?.Invoke(this, e);
		}

		protected override void OnLayout(bool changed, int l, int t, int r, int b)
		{
			if (Element == null)
			{
				return;
			}

			if (_imageHeight > -1)
			{
				// We've got an image (and no text); it's already centered horizontally,
				// we just need to adjust the padding so it centers vertically
				int diff = (b - t - _imageHeight) / 2;
				diff = Math.Max(diff, 0);
				SetPadding(0, diff, 0, -diff);
			}

			base.OnLayout(changed, l, t, r, b);
		}

		protected virtual void OnRegisterEffect(PlatformEffect effect)
		{
			effect.Container = this;
			effect.Control = this;
		}

		protected void SetAutomationId(string id)
		{
			ContentDescription = id;
		}

		protected void SetContentDescription()
		{
			if (Element == null)
			{
				return;
			}

			if (SetHint())
			{
				return;
			}

			if (_defaultContentDescription == null)
			{
				_defaultContentDescription = ContentDescription;
			}

			string elemValue = Join(" ", (string)Element.GetValue(Accessibility.NameProperty),
				(string)Element.GetValue(Accessibility.HintProperty));

			ContentDescription = !IsNullOrWhiteSpace(elemValue) ? elemValue : _defaultContentDescription;
		}

		protected void SetFocusable()
		{
			if (Element == null)
			{
				return;
			}

			if (!_defaultFocusable.HasValue)
			{
				_defaultFocusable = Focusable;
			}

			Focusable = (bool)((bool?)Element.GetValue(Accessibility.IsInAccessibleTreeProperty) ?? _defaultFocusable);
		}

		protected bool SetHint()
		{
			if (Element == null)
			{
				return false;
			}

			if (_defaultHint == null)
			{
				_defaultHint = Hint;
			}

			string elemValue = Join(". ", (string)Element.GetValue(Accessibility.NameProperty),
				(string)Element.GetValue(Accessibility.HintProperty));

			Hint = !IsNullOrWhiteSpace(elemValue) ? elemValue : _defaultHint;

			return true;
		}

		protected void SetTracker(VisualElementTracker tracker)
		{
			Tracker = tracker;
		}

		protected void UpdateBackgroundColor()
		{
			if (Element == null)
			{
				return;
			}

			Color backgroundColor = Element.BackgroundColor;
			if (backgroundColor.IsDefault)
			{
				if (SupportBackgroundTintList != null)
				{
					Context context = Context;
					int id = GlobalResource.Attribute.ButtonTint;
					unchecked
					{
						using (var value = new TypedValue())
						{
							try
							{
								Resources.Theme theme = context.Theme;
								if (theme != null && theme.ResolveAttribute(id, value, true))
#pragma warning disable 618
								{
									SupportBackgroundTintList = Resources.GetColorStateList(value.Data);
								}
#pragma warning restore 618
								else
								{
									SupportBackgroundTintList = new ColorStateList(ColorExtensions.States,
										new[] { (int)0xffd7d6d6, 0x7fd7d6d6 });
								}
							}
							catch (Exception ex)
							{
								Log.Warning("Xamarin.Forms.Platform.Android.ButtonRenderer",
									"Could not retrieve button background resource: {0}", ex);
								SupportBackgroundTintList = new ColorStateList(ColorExtensions.States,
									new[] { (int)0xffd7d6d6, 0x7fd7d6d6 });
							}
						}
					}
				}
			}
			else
			{
				int intColor = backgroundColor.ToAndroid().ToArgb();
				int disableColor = backgroundColor.MultiplyAlpha(0.5).ToAndroid().ToArgb();
				SupportBackgroundTintList = new ColorStateList(ColorExtensions.States, new[] { intColor, disableColor });
			}
		}

		internal virtual void OnNativeFocusChanged(bool hasFocus)
		{
		}

		internal virtual void SendVisualElementInitialized(VisualElement element, AView nativeView)
		{
			element.SendViewInitialized(nativeView);
		}

		void Initialize()
		{
			SoundEffectsEnabled = false;
			SetOnClickListener(ButtonClickListener.Instance.Value);
			SetOnTouchListener(ButtonTouchListener.Instance.Value);
			AddOnAttachStateChangeListener(this);
			OnFocusChangeListener = this;

			Tag = this;
				// TODO hartez The button used to be tagged with its renderer, probably for cell reuse or something? Does that still make sense? (probably yes)

			_textColorSwitcher = new TextColorSwitcher(TextColors);
				// TODO hartez 2017/03/01 10:04:24 Possibly this shouldn't be initialized until a text color update is made	
		}

		void SetLabeledBy()
		{
			var elemValue = (VisualElement)Element?.GetValue(Accessibility.LabeledByProperty);

			if (elemValue == null)
			{
				return;
			}

			int id = Id;
			if (id == -1)
			{
				id = Id = FormsAppCompatActivity.GetUniqueId();
			}

			IVisualElementRenderer renderer = elemValue.GetRenderer();
			renderer?.SetLabelFor(id);
		}

		void UpdateAll()
		{
			UpdateFont();
			UpdateText();
			UpdateBitmap();
			UpdateTextColor();
			UpdateIsEnabled();
		}

		void UpdateBitmap()
		{
			if (Element == null)
			{
				return;
			}

			FileImageSource elementImage = Button.Image;
			string imageFile = elementImage?.File;
			_imageHeight = -1;

			if (elementImage == null || IsNullOrEmpty(imageFile))
			{
				SetCompoundDrawablesWithIntrinsicBounds(null, null, null, null);
				return;
			}

			Drawable image = Context.Resources.GetDrawable(imageFile);

			if (IsNullOrEmpty(Button.Text))
			{
				// No text, so no need for relative position; just center the image
				// There's no option for just plain-old centering, so we'll use Top 
				// (which handles the horizontal centering) and some tricksy padding (in OnLayout)
				// to handle the vertical centering 

				// Clear any previous padding and set the image as top/center
				SetPadding(0, 0, 0, 0);
				SetCompoundDrawablesWithIntrinsicBounds(null, image, null, null);

				// Keep track of the image height so we can use it in OnLayout
				_imageHeight = image.IntrinsicHeight;

				image.Dispose();
				return;
			}

			Button.ButtonContentLayout layout = Button.ContentLayout;

			CompoundDrawablePadding = (int)layout.Spacing;

			switch (layout.Position)
			{
				case Button.ButtonContentLayout.ImagePosition.Top:
					SetCompoundDrawablesWithIntrinsicBounds(null, image, null, null);
					break;
				case Button.ButtonContentLayout.ImagePosition.Bottom:
					SetCompoundDrawablesWithIntrinsicBounds(null, null, null, image);
					break;
				case Button.ButtonContentLayout.ImagePosition.Right:
					SetCompoundDrawablesWithIntrinsicBounds(null, null, image, null);
					break;
				default:
					// Defaults to image on the left
					SetCompoundDrawablesWithIntrinsicBounds(image, null, null, null);
					break;
			}

			image?.Dispose();
		}

		void UpdateFont()
		{
			if (Element == null)
			{
				return;
			}

			Font font = Button.Font;

			if (font == Font.Default && _defaultFontSize == 0f)
			{
				return;
			}

			if (_defaultFontSize == 0f)
			{
				_defaultTypeface = Typeface;
				_defaultFontSize = TextSize;
			}

			if (font == Font.Default)
			{
				Typeface = _defaultTypeface;
				SetTextSize(ComplexUnitType.Px, _defaultFontSize);
			}
			else
			{
				Typeface = font.ToTypeface();
				SetTextSize(ComplexUnitType.Sp, font.ToScaledPixel());
			}
		}

		void UpdateIsEnabled()
		{
			Enabled = Element.IsEnabled;
		}

		void UpdateText()
		{
			if (Element == null)
			{
				return;
			}

			string oldText = Text;
			Text = Button.Text;

			// If we went from or to having no text, we need to update the image position
			if (IsNullOrEmpty(oldText) != IsNullOrEmpty(Text))
			{
				UpdateBitmap();
			}
		}

		void UpdateTextColor()
		{
			if (Element == null)
			{
				return;
			}

			_textColorSwitcher?.UpdateTextColor(this, Button.TextColor);
		}

		// TODO hartez 2017/03/01 13:56:46 Just implement these directly on the class, no point in pushing this to another object	
		class ButtonClickListener : Object, IOnClickListener
		{
			#region Statics

			public static readonly Lazy<ButtonClickListener> Instance =
				new Lazy<ButtonClickListener>(() => new ButtonClickListener());

			#endregion

			public void OnClick(AView v)
			{
				var renderer = v.Tag as ButtonRenderer;
				((IButtonController)renderer?.Element)?.SendClicked();
			}
		}

		class ButtonTouchListener : Object, IOnTouchListener
		{
			public static readonly Lazy<ButtonTouchListener> Instance =
				new Lazy<ButtonTouchListener>(() => new ButtonTouchListener());

			public bool OnTouch(AView v, AMotionEvent e)
			{
				var renderer = v.Tag as ButtonRenderer;
				if (renderer != null)
				{
					var buttonController = renderer.Element as IButtonController;
					if (e.Action == AMotionEventActions.Down)
					{
						buttonController?.SendPressed();
					}
					else if (e.Action == AMotionEventActions.Up)
					{
						buttonController?.SendReleased();
					}
				}
				return false;
			}
		}
	}
}