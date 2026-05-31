using System.Runtime.CompilerServices;

namespace ConstructionMS.Forms;

/// <summary>
/// Central design system for the whole application — colour palette, fonts,
/// button roles, and reusable card/badge components. This is the single source
/// of truth for visual styling outside of <see cref="GridStyle"/> (which styles
/// data grids). Update colours or fonts here to change the entire app at once.
/// </summary>
public static class Theme
{
    // ── Palette (refined current blue) ──────────────────────────────────────
    public static readonly Color Brand        = Color.FromArgb(30, 90, 160);   // header / chrome
    public static readonly Color Primary       = Color.FromArgb(33, 64, 154);  // primary action
    public static readonly Color Success       = Color.FromArgb(40, 167, 69);  // positive / create
    public static readonly Color Caution       = Color.FromArgb(217, 119, 6);  // deliberate state change
    public static readonly Color Danger        = Color.FromArgb(220, 53, 69);  // destructive
    public static readonly Color Surface       = Color.FromArgb(248, 249, 251);// page background
    public static readonly Color Card          = Color.White;                  // card background
    public static readonly Color Border         = Color.FromArgb(222, 226, 232);
    public static readonly Color TextPrimary    = Color.FromArgb(33, 37, 41);
    public static readonly Color TextMuted       = Color.FromArgb(108, 117, 125);
    public static readonly Color Disabled        = Color.FromArgb(206, 212, 218);
    public static readonly Color DisabledText    = Color.FromArgb(134, 142, 150);

    // ── Fonts ───────────────────────────────────────────────────────────────
    public static Font Title()     => new("Segoe UI", 15F, FontStyle.Bold);
    public static Font Heading()    => new("Segoe UI", 11F, FontStyle.Bold);
    public static Font CardTitle()  => new("Segoe UI", 9.5F, FontStyle.Bold);
    public static Font CardValue()  => new("Segoe UI", 16F, FontStyle.Bold);
    public static Font LabelFont()  => new("Segoe UI", 9F, FontStyle.Bold);
    public static Font Body()       => new("Segoe UI", 9.5F);
    public static Font Small()      => new("Segoe UI", 8.5F);
    public static Font ButtonFont() => new("Segoe UI", 9.5F, FontStyle.Bold);

    /// <summary>Button intent — drives colour and disabled appearance.</summary>
    public enum ButtonRole { Primary, Success, Caution, Danger, Secondary }

    // Track controls already themed so the recursive Apply() and explicit calls
    // do not double-subscribe handlers.
    private static readonly ConditionalWeakTable<Control, object> _themed = new();
    private static bool MarkThemed(Control c)
    {
        if (_themed.TryGetValue(c, out _)) return false;
        _themed.Add(c, _marker);
        return true;
    }
    private static readonly object _marker = new();

    // ── Buttons ─────────────────────────────────────────────────────────────

    /// <summary>
    /// Styles a button for its role and keeps a real greyed-out look when it is
    /// disabled (WinForms flat buttons keep their custom colour otherwise).
    /// </summary>
    public static void StyleButton(Button b, ButtonRole role)
    {
        bool first = MarkThemed(b);

        Color bg = role switch
        {
            ButtonRole.Primary   => Primary,
            ButtonRole.Success   => Success,
            ButtonRole.Caution   => Caution,
            ButtonRole.Danger    => Danger,
            _                    => Card,           // Secondary = outlined
        };
        bool secondary = role == ButtonRole.Secondary;
        Color fg    = secondary ? Primary : Color.White;
        Color hover = secondary ? Surface : Darken(bg, 0.12);

        b.FlatStyle                 = FlatStyle.Flat;
        b.Font                      = ButtonFont();
        b.FlatAppearance.BorderSize = secondary ? 1 : 0;

        void Repaint()
        {
            if (b.Enabled)
            {
                b.BackColor = bg;
                b.ForeColor = fg;
                b.Cursor    = Cursors.Hand;
                b.FlatAppearance.BorderColor        = secondary ? Border : bg;
                b.FlatAppearance.MouseOverBackColor = hover;
            }
            else
            {
                b.BackColor = secondary ? Surface : Disabled;
                b.ForeColor = DisabledText;
                b.Cursor    = Cursors.Default;
                b.FlatAppearance.BorderColor        = Disabled;
                b.FlatAppearance.MouseOverBackColor = secondary ? Surface : Disabled;
            }
        }

        if (first) b.EnabledChanged += (_, _) => Repaint();
        Repaint();
    }

    // ── Label helpers ───────────────────────────────────────────────────────
    public static void StyleTitle(Label l)   { l.Font = Title();   l.ForeColor = Brand; }
    public static void StyleHeading(Label l)  { l.Font = Heading();  l.ForeColor = TextPrimary; }
    public static void StyleFieldLabel(Label l){ l.Font = LabelFont(); l.ForeColor = TextPrimary; }
    public static void StyleMuted(Label l)    { l.Font = Small();    l.ForeColor = TextMuted; }

    // ── Badge ────────────────────────────────────────────────────────────────

    /// <summary>Creates a small pill-style count/label badge.</summary>
    public static Label Badge(string text, Color bg)
    {
        var lbl = new Label
        {
            Text      = text,
            AutoSize  = true,
            Font      = new Font("Segoe UI", 8.5F, FontStyle.Bold),
            ForeColor = Color.White,
            BackColor = bg,
            Padding   = new Padding(8, 3, 8, 3),
            TextAlign = ContentAlignment.MiddleCenter,
        };
        return lbl;
    }

    // ── Colour utilities ───────────────────────────────────────────────────
    public static Color Darken(Color c, double amount) =>
        Color.FromArgb(c.A,
            (int)Math.Max(0, c.R * (1 - amount)),
            (int)Math.Max(0, c.G * (1 - amount)),
            (int)Math.Max(0, c.B * (1 - amount)));

    public static Color Lighten(Color c, double amount) =>
        Color.FromArgb(c.A,
            (int)Math.Min(255, c.R + (255 - c.R) * amount),
            (int)Math.Min(255, c.G + (255 - c.G) * amount),
            (int)Math.Min(255, c.B + (255 - c.B) * amount));

    // ── Recursive light pass ─────────────────────────────────────────────────

    /// <summary>
    /// Walks a control tree and applies consistent role colours to buttons and
    /// title/heading fonts to bold labels — by recognising the app's existing
    /// colours. Only changes colours/fonts; never moves or resizes anything.
    /// Call once at the end of a form constructor.
    /// </summary>
    public static void Apply(Control root)
    {
        foreach (Control c in root.Controls)
        {
            switch (c)
            {
                case Button b when !_themed.TryGetValue(b, out _):
                    StyleButton(b, RoleFromColor(b.BackColor));
                    break;

                case Label l:
                    if (l.Font.Bold && l.Font.Size >= 13f)      StyleTitle(l);
                    else if (l.Font.Bold && l.Font.Size >= 11f) StyleHeading(l);
                    break;

                case DataGridView:
                    break; // styled by GridStyle
            }

            if (c.HasChildren) Apply(c);
        }
    }

    /// <summary>Maps an existing button background colour to the closest role.</summary>
    private static ButtonRole RoleFromColor(Color c)
    {
        // Greens → Success
        if (Near(c, Color.FromArgb(40, 167, 69)) || Near(c, Color.LightGreen))
            return ButtonRole.Success;
        // Blues → Primary
        if (Near(c, Color.FromArgb(30, 90, 160)) || Near(c, Color.FromArgb(33, 64, 154)))
            return ButtonRole.Primary;
        // Reds → Danger
        if (Near(c, Color.FromArgb(220, 53, 69)) || Near(c, Color.FromArgb(180, 30, 30)))
            return ButtonRole.Danger;
        // Everything else (greys, whites, light) → Secondary outline
        return ButtonRole.Secondary;
    }

    private static bool Near(Color a, Color b, int tol = 24) =>
        Math.Abs(a.R - b.R) <= tol && Math.Abs(a.G - b.G) <= tol && Math.Abs(a.B - b.B) <= tol;
}

/// <summary>
/// A flat card panel with a 1px border, a coloured top accent strip, a bold
/// title, and a <see cref="Content"/> area to host child controls. Used for the
/// dashboard KPI and alert cards.
/// </summary>
public sealed class CardPanel : Panel
{
    /// <summary>The fill area where card body controls should be added.</summary>
    public Panel Content { get; } = new Panel();

    /// <summary>The card's title label (docked at the top).</summary>
    public Label TitleLabel { get; } = new Label();

    private readonly Color _accent;

    public CardPanel(string title, Color accent)
    {
        _accent   = accent;
        BackColor = Theme.Card;
        Padding   = new Padding(1, 6, 1, 1);   // room for border + accent strip

        Content.Dock      = DockStyle.Fill;
        Content.BackColor = Theme.Card;
        Content.Padding   = new Padding(14, 6, 14, 10);

        TitleLabel.Dock      = DockStyle.Top;
        TitleLabel.AutoSize  = true;          // grows with the font at any DPI — never clips
        TitleLabel.Text      = title;
        TitleLabel.Font      = Theme.CardTitle();
        TitleLabel.ForeColor = Theme.TextPrimary;
        TitleLabel.Padding   = new Padding(14, 8, 8, 4);

        Controls.Add(Content);
        Controls.Add(TitleLabel);

        SetStyle(ControlStyles.ResizeRedraw, true);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        using var border = new Pen(Theme.Border);
        e.Graphics.DrawRectangle(border, 0, 0, Width - 1, Height - 1);
        using var strip = new SolidBrush(_accent);
        e.Graphics.FillRectangle(strip, 1, 1, Width - 2, 4);
    }
}

/// <summary>
/// A thin horizontal progress bar that paints its fill as a percentage of its
/// own width — so it resizes automatically with its container (responsive).
/// </summary>
public sealed class MiniBar : Panel
{
    public double Percent { get; set; }
    public Color  FillColor { get; set; } = Theme.Primary;

    public MiniBar()
    {
        Height    = 8;
        BackColor = Theme.Lighten(Theme.Border, 0.3);
        SetStyle(ControlStyles.ResizeRedraw, true);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        int w = (int)(Width * Math.Min(Math.Max(Percent, 0), 100) / 100.0);
        if (w <= 0) return;
        using var fill = new SolidBrush(FillColor);
        e.Graphics.FillRectangle(fill, 0, 0, w, Height);
    }
}
