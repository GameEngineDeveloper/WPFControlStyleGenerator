
using System;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Forms;

using ColorConverter = System.Windows.Media.ColorConverter;
using MediaColor = System.Windows.Media.Color;
using DrawingColor = System.Drawing.Color;
using WinFormsColorDialog = System.Windows.Forms.ColorDialog;
using WinFormsDialogResult = System.Windows.Forms.DialogResult;
using TextBox = System.Windows.Controls.TextBox;

namespace ButtonStyleGenerator;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    // ==========================================
    // Button Style Generator
    // ==========================================

    private void GenerateXaml_Click(object sender, RoutedEventArgs e)
    {
        // Normal colors
        string normalBgStart = ToColor(NormalBackgroundStartBox.Text);
        string normalBgEnd = ToColor(NormalBackgroundEndBox.Text);
        string normalFg = ToColor(NormalForegroundBox.Text);

        // MouseOver colors
        string hoverBgStart = ToColor(HoverBackgroundStartBox.Text);
        string hoverBgEnd = ToColor(HoverBackgroundEndBox.Text);
        string hoverFg = ToColor(HoverForegroundBox.Text);

        // Pressed colors
        string pressedBgStart = ToColor(PressedBackgroundStartBox.Text);
        string pressedBgEnd = ToColor(PressedBackgroundEndBox.Text);
        string pressedFg = ToColor(PressedForegroundBox.Text);

        // Animation duration
        double transitionSeconds = TransitionDurationSlider.Value;
        string animDuration = FormatDuration(transitionSeconds);

        StringBuilder sb = new StringBuilder();

        sb.AppendLine("<Style TargetType=\"Button\">");

        sb.AppendLine($"  <Setter Property=\"Foreground\" Value=\"{normalFg}\"/>");

        sb.AppendLine("  <Setter Property=\"Template\">");
        sb.AppendLine("    <Setter.Value>");
        sb.AppendLine("      <ControlTemplate TargetType=\"Button\">");

        // Border
        sb.AppendLine("        <Border x:Name=\"border\" CornerRadius=\"4\" BorderThickness=\"0\">");

        // Resources
        sb.AppendLine("          <Border.Resources>");

        sb.AppendLine("            <LinearGradientBrush x:Key=\"bgBrush\" StartPoint=\"0,0\" EndPoint=\"0,1\">");
        sb.AppendLine($"              <GradientStop Color=\"{normalBgStart}\" Offset=\"0.0\"/>");
        sb.AppendLine($"              <GradientStop Color=\"{normalBgEnd}\" Offset=\"1.0\"/>");
        sb.AppendLine("            </LinearGradientBrush>");

        sb.AppendLine($"            <SolidColorBrush x:Key=\"fgBrush\" Color=\"{normalFg}\"/>");

        sb.AppendLine("          </Border.Resources>");

        // Visual States
        sb.AppendLine("          <VisualStateManager.VisualStateGroups>");
        sb.AppendLine("            <VisualStateGroup x:Name=\"CommonStates\">");

        // ==========================================
        // Normal State (MouseLeave)
        // ==========================================

        sb.AppendLine("              <VisualState x:Name=\"Normal\">");
        sb.AppendLine("                <Storyboard>");

        AppendColorAnimation(
            sb, "border",
            "(Border.Background).(LinearGradientBrush.GradientStops)[0].(GradientStop.Color)",
            normalBgStart, animDuration);

        AppendColorAnimation(
            sb, "border",
            "(Border.Background).(LinearGradientBrush.GradientStops)[1].(GradientStop.Color)",
            normalBgEnd, animDuration);

        AppendColorAnimation(
            sb, "contentPresenter",
            "(TextElement.Foreground).(SolidColorBrush.Color)",
            normalFg, animDuration);

        sb.AppendLine("                </Storyboard>");
        sb.AppendLine("              </VisualState>");

        // ==========================================
        // MouseOver State
        // ==========================================

        sb.AppendLine("              <VisualState x:Name=\"MouseOver\">");
        sb.AppendLine("                <Storyboard>");

        AppendColorAnimation(
            sb, "border",
            "(Border.Background).(LinearGradientBrush.GradientStops)[0].(GradientStop.Color)",
            hoverBgStart, animDuration);

        AppendColorAnimation(
            sb, "border",
            "(Border.Background).(LinearGradientBrush.GradientStops)[1].(GradientStop.Color)",
            hoverBgEnd, animDuration);

        AppendColorAnimation(
            sb, "contentPresenter",
            "(TextElement.Foreground).(SolidColorBrush.Color)",
            hoverFg, animDuration);

        sb.AppendLine("                </Storyboard>");
        sb.AppendLine("              </VisualState>");

        // ==========================================
        // Pressed State
        // ==========================================

        sb.AppendLine("              <VisualState x:Name=\"Pressed\">");
        sb.AppendLine("                <Storyboard>");

        AppendColorAnimation(
            sb, "border",
            "(Border.Background).(LinearGradientBrush.GradientStops)[0].(GradientStop.Color)",
            pressedBgStart, animDuration);

        AppendColorAnimation(
            sb, "border",
            "(Border.Background).(LinearGradientBrush.GradientStops)[1].(GradientStop.Color)",
            pressedBgEnd, animDuration);

        AppendColorAnimation(
            sb, "contentPresenter",
            "(TextElement.Foreground).(SolidColorBrush.Color)",
            pressedFg, animDuration);

        sb.AppendLine("                </Storyboard>");
        sb.AppendLine("              </VisualState>");

        sb.AppendLine("            </VisualStateGroup>");
        sb.AppendLine("          </VisualStateManager.VisualStateGroups>");

        // Background
        sb.AppendLine("          <Border.Background>");
        sb.AppendLine("            <StaticResource ResourceKey=\"bgBrush\"/>");
        sb.AppendLine("          </Border.Background>");

        // ContentPresenter
        sb.AppendLine("          <ContentPresenter x:Name=\"contentPresenter\"");
        sb.AppendLine("              HorizontalAlignment=\"Center\"");
        sb.AppendLine("              VerticalAlignment=\"Center\"");
        sb.AppendLine("              Content=\"{TemplateBinding Content}\"");
        sb.AppendLine("              ContentTemplate=\"{TemplateBinding ContentTemplate}\"");
        sb.AppendLine("              RecognizesAccessKey=\"True\"");
        sb.AppendLine("              TextElement.Foreground=\"{StaticResource fgBrush}\"/>");

        sb.AppendLine("        </Border>");
        sb.AppendLine("      </ControlTemplate>");
        sb.AppendLine("    </Setter.Value>");
        sb.AppendLine("  </Setter>");
        sb.AppendLine("</Style>");

        // Output
        XamlOutputBox.Text = sb.ToString();
    }

    // ==========================================
    // Color Animation Generator
    // ==========================================

    private void AppendColorAnimation(
        StringBuilder sb,
        string targetName,
        string targetProperty,
        string color,
        string duration)
    {
        sb.AppendLine(
            $"                  <ColorAnimation " +
            $"Storyboard.TargetName=\"{targetName}\" " +
            $"Storyboard.TargetProperty=\"{targetProperty}\" " +
            $"To=\"{color}\" " +
            $"Duration=\"{duration}\"/>");
    }

    // ==========================================
    // Duration Formatter
    // ==========================================

    private string FormatDuration(double seconds)
    {
        return $"0:0:{seconds.ToString("0.00", CultureInfo.InvariantCulture)}";
    }

    // ==========================================
    // Color Converter
    // ==========================================

    private string ToColor(string hex)
    {
        try
        {
            MediaColor color =
                (MediaColor)ColorConverter.ConvertFromString(hex);

            return color.ToString();
        }
        catch
        {
            return "#FFFFFFFF";
        }
    }

    // ==========================================
    // Darker Color
    // ==========================================

    private string ToDarker(string hex)
    {
        try
        {
            MediaColor color =
                (MediaColor)ColorConverter.ConvertFromString(hex);

            byte r = (byte)(color.R * 0.8);
            byte g = (byte)(color.G * 0.8);
            byte b = (byte)(color.B * 0.8);

            return MediaColor.FromArgb(
                color.A, r, g, b).ToString();
        }
        catch
        {
            return "#FFAAAAAA";
        }
    }

    // ==========================================
    // Color Picker
    // ==========================================

    private void ColorPicker_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not System.Windows.Controls.Button button ||
            button.Tag == null)
        {
            return;
        }

        string targetName = button.Tag.ToString();

        if (FindName(targetName) is not TextBox textBox)
            return;

        DrawingColor defaultColor = DrawingColor.White;

        try
        {
            MediaColor mediaColor =
                (MediaColor)ColorConverter.ConvertFromString(textBox.Text);

            defaultColor = DrawingColor.FromArgb(
                mediaColor.A,
                mediaColor.R,
                mediaColor.G,
                mediaColor.B);
        }
        catch
        {
        }

        using WinFormsColorDialog dialog = new WinFormsColorDialog
        {
            Color = defaultColor,
            FullOpen = true
        };

        if (dialog.ShowDialog() == WinFormsDialogResult.OK)
        {
            DrawingColor color = dialog.Color;

            string hex =
                $"#{color.A:X2}{color.R:X2}{color.G:X2}{color.B:X2}";

            textBox.Text = hex;
        }
    }
}
