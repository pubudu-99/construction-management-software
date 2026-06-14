using System.Windows.Forms;

namespace ConstructionMS.Forms;

/// <summary>
/// Applies a consistent, professional style to every DataGridView
/// in the application. Call GridStyle.Apply(dgv) after InitializeComponent.
/// </summary>
public static class GridStyle
{
    // Colour palette — change here to update the whole app at once
    public static readonly Color HeaderBack    = Color.FromArgb(33, 64, 154);  // dark blue
    public static readonly Color HeaderFore    = Color.White;
    public static readonly Color RowEven       = Color.White;
    public static readonly Color RowOdd        = Color.FromArgb(240, 245, 255); // very light blue
    public static readonly Color SelectBack    = Color.FromArgb(173, 198, 255); // soft blue
    public static readonly Color SelectFore    = Color.FromArgb(10, 10, 10);
    public static readonly Color GridLine      = Color.FromArgb(210, 218, 235);
    public static readonly Color CellFore      = Color.FromArgb(30, 30, 30);

    // Status colours — used in CellFormatting events
    public static readonly Color StatusRed     = Color.FromArgb(220, 53, 69);
    public static readonly Color StatusAmber   = Color.FromArgb(255, 193, 7);
    public static readonly Color StatusGreen   = Color.FromArgb(40, 167, 69);
    public static readonly Color StatusWhite   = Color.White;
    public static readonly Color StatusBlack   = Color.FromArgb(30, 30, 30);

    // Darker selection variants — applied when a status row is selected/hovered
    public static readonly Color StatusRedSel   = Color.FromArgb(185, 28, 43);
    public static readonly Color StatusAmberSel = Color.FromArgb(214, 158, 0);
    public static readonly Color StatusGreenSel = Color.FromArgb(30, 126, 52);

    /// <summary>Applies the standard grid style. Call once per DataGridView.</summary>
    public static void Apply(DataGridView dgv)
    {
        // Header
        dgv.EnableHeadersVisualStyles = false;
        dgv.ColumnHeadersDefaultCellStyle.BackColor  = HeaderBack;
        dgv.ColumnHeadersDefaultCellStyle.ForeColor  = HeaderFore;
        dgv.ColumnHeadersDefaultCellStyle.Font       =
            new Font(dgv.Font.FontFamily, 9.5f, FontStyle.Bold);
        dgv.ColumnHeadersDefaultCellStyle.Padding    = new Padding(6, 4, 6, 4);
        dgv.ColumnHeadersHeight      = 36;
        dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;

        // Row heights and font
        dgv.RowTemplate.Height = 28;
        dgv.DefaultCellStyle.Font    = new Font(dgv.Font.FontFamily, 9f);
        dgv.DefaultCellStyle.Padding = new Padding(4, 2, 4, 2);

        // Alternating rows — "zebra stripe" pattern
        dgv.DefaultCellStyle.BackColor             = RowEven;
        dgv.DefaultCellStyle.ForeColor             = CellFore;
        dgv.AlternatingRowsDefaultCellStyle.BackColor = RowOdd;
        dgv.AlternatingRowsDefaultCellStyle.ForeColor = CellFore;

        // Selection colours
        dgv.DefaultCellStyle.SelectionBackColor    = SelectBack;
        dgv.DefaultCellStyle.SelectionForeColor    = SelectFore;
        dgv.AlternatingRowsDefaultCellStyle.SelectionBackColor = SelectBack;
        dgv.AlternatingRowsDefaultCellStyle.SelectionForeColor = SelectFore;

        // Grid lines
        dgv.GridColor           = GridLine;
        dgv.CellBorderStyle     = DataGridViewCellBorderStyle.SingleHorizontal;
        dgv.RowHeadersVisible   = false;
        dgv.BorderStyle         = BorderStyle.FixedSingle;
        dgv.BackgroundColor     = Color.White;

        // Behaviour
        dgv.SelectionMode         = DataGridViewSelectionMode.FullRowSelect;
        dgv.ReadOnly              = true;
        dgv.AllowUserToAddRows    = false;
        dgv.AllowUserToResizeRows = false;
        dgv.AutoSizeColumnsMode   = DataGridViewAutoSizeColumnsMode.Fill;
    }
}
