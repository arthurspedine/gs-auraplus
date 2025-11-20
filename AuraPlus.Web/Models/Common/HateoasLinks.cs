namespace AuraPlus.Web.Models.Common;

public class HateoasLinks
{
    public List<Link> Links { get; set; } = new List<Link>();

    public void AddLink(string href, string rel, string method = "GET")
    {
        Links.Add(new Link { Href = href, Rel = rel, Method = method });
    }

    public void AddSelfLink(string href)
    {
        AddLink(href, "self", "GET");
    }

    public void AddPaginationLinks(string baseUrl, int currentPage, int totalPages, int pageSize)
    {
        // Self
        AddLink($"{baseUrl}?page={currentPage}&pageSize={pageSize}", "self", "GET");

        // First
        if (currentPage > 1)
        {
            AddLink($"{baseUrl}?page=1&pageSize={pageSize}", "first", "GET");
        }

        // Previous
        if (currentPage > 1)
        {
            AddLink($"{baseUrl}?page={currentPage - 1}&pageSize={pageSize}", "previous", "GET");
        }

        // Next
        if (currentPage < totalPages)
        {
            AddLink($"{baseUrl}?page={currentPage + 1}&pageSize={pageSize}", "next", "GET");
        }

        // Last
        if (currentPage < totalPages)
        {
            AddLink($"{baseUrl}?page={totalPages}&pageSize={pageSize}", "last", "GET");
        }
    }
}
