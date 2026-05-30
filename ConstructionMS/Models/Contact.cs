namespace ConstructionMS.Models;

/// <summary>
/// Represents a client or supplier in the contacts register.
/// </summary>
public class Contact
{
    public int     ContactId     { get; set; }
    public string  Type          { get; set; } = "";
    public string  Name          { get; set; } = "";
    public string? ContactPerson { get; set; }
    public string? Phone         { get; set; }
    public string? Email         { get; set; }
    public string? Address       { get; set; }
    public string? Notes         { get; set; }
}
