/// <summary>
/// Represents a single page in the game's ending sequence.
/// </summary>
public class EndingPage
{
    /// <summary>
    /// The title of the ending page.
    /// </summary>
    public string Title;

    /// <summary>
    /// The main body text of the ending page.
    /// </summary>
    public string Body;

    /// <summary>
    /// Creates a new ending page with the given title and body.
    /// </summary>
    /// <param name="title">The title of the page.</param>
    /// <param name="body">The text content of the page.</param>
    public EndingPage(string title, string body)
    {
        this.Title = title;
        this.Body = body;
    }
}
